using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mapdata.Api.DbContexts;
using Mapdata.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

namespace Mapdata.Api.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class GeoJsonBusiness
    {
        private readonly TnDistrictContext _context;
        private readonly IMemoryCache _cache;

        private readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6)
        };
        
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cache"></param>
        public GeoJsonBusiness(TnDistrictContext context, IMemoryCache cache)
        {
            _cache = cache;
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<dynamic> GetDataAsync()
        {
            if (!_cache.TryGetValue("geojsondata", out JToken deserialized))
            {
                if (!_cache.TryGetValue("geojsoncache", out string json))
                {
                    using (StreamReader r = new StreamReader("Data/map.geojson"))
                    {
                        json = r.ReadToEnd();
                    }

                    _cache.Set("geojsoncache", json, _cacheOptions);
                }

                deserialized = JToken.Parse(json);

                var features = deserialized["features"].Value<JArray>();

                foreach (var subObj in features.Children())
                {
                    var properties = subObj["properties"];

                    if (properties != null)
                    {
                        var district = properties?.FirstOrDefault().Value<JProperty>();

                        var districtData = await GetDataForDistrictAsync(district.Value.ToString());

                        if (districtData != null && districtData.TotalCases > 0)
                        {
                            properties["totalCases"] = districtData.TotalCases.ToString();
                        }
                        else
                        {
                            properties["totalCases"] = "--";
                        }

                        if (districtData != null && districtData.TodayCases > 0)
                        {
                            properties["newCases"] = districtData.TodayCases.ToString();
                        }
                        else
                        {
                            properties["newCases"] = "--";
                        }
                    }
                }

                _cache.Set("geojsondata", deserialized, _cacheOptions);
            }

            return deserialized;
        }

        private async Task<MapDistrictDataResult> GetDataForDistrictAsync(string dName)
        {
            var result = new MapDistrictDataResult();

            var district = await _context.District.Where(s => s.Name == dName).FirstOrDefaultAsync();

            if (district != null)
            {
                result.DistrictId = (int)district.Id;
                result.DistrictName = dName;

                var dailyDtResult = await _context.DailyData
                    .Where(s => s.DistrictId == result.DistrictId)
                    .ToListAsync();

                var cumulativeResult = dailyDtResult
                    .GroupBy(s => s.DistrictId)
                    .Select(s => new
                    {
                        TotalCases = s.Sum(a => a.Cases),
                        Death = s.Sum(a => a.Death),
                        Recovered = s.Sum(a => a.Recovered)
                    }).FirstOrDefault();

                if (cumulativeResult != null)
                {
                    result.TotalCases = (int)cumulativeResult.TotalCases;
                    result.Death = (int)cumulativeResult.Death;
                    result.Recovered = (int)cumulativeResult.Recovered;
                }

                var todayDate = DateTime.Today.ToString("dd/MM");

                var dailyResult = await _context.DailyData
                    .Where(s => s.DistrictId == result.DistrictId && s.Date == todayDate)
                    .FirstOrDefaultAsync();

                if (dailyResult != null)
                {
                    result.TodayCases = (int)dailyResult.Cases;
                }

                return result;
            }

            return null;
        }
    }
}