using Mapdata.Api.DbContexts;
using Mapdata.Api.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mapdata.Api.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class GeoJsonBusiness
    {
        private readonly TnDistrictContext _context;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="context"></param>
        public GeoJsonBusiness(TnDistrictContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<dynamic> GetDataAsync()
        {
            using (StreamReader r = new StreamReader("Data/map.geojson"))
            {
                string json = r.ReadToEnd();

                var deserialized = JToken.Parse(json);

                var features = deserialized["features"].Value<JArray>();

                foreach (var sub_obj in features.Children())
                {
                    var properties = sub_obj["properties"];

                    if (properties != null)
                    {
                        var district = properties?.FirstOrDefault().Value<JProperty>();

                        System.Diagnostics.Debug.WriteLine($"Name: {district.Name}\t Value: {district.Value}");

                        var districtData = await GetDataForDistrictAsync(district.Value.ToString());

                        properties["colorCode"] = "Blue";
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

                return deserialized;
            }
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

        private async Task<(List<Entities.StateCumulative> data, List<string> dates)> GetStateCumulativeAsync()
        {
            var stateData = await _context.StateCumulative
                .OrderByDescending(s => s.Id)
                .Take(10)
                .ToListAsync();

            stateData = stateData.OrderBy(s => s.Id).ToList();

            var dates = stateData.Select(s => s.Date).ToList();

            return (stateData, dates);
        }

    }
}
