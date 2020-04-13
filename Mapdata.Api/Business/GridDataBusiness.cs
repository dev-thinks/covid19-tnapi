using Mapdata.Api.DbContexts;
using Mapdata.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mapdata.Api.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class GridDataBusiness
    {
        private readonly TnDistrictContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public GridDataBusiness(TnDistrictContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<StateWideResult> GetStateWideAsync()
        {
            var result = new StateWideResult();

            var data = await _context.StateCumulative
                .ToListAsync();

            result.TotalCases = (int)data.Select(s => s.Cases).Sum();
            result.TotalDeath = (int)data.Select(s => s.Death).Sum();
            result.Recovered = (int)data.Select(s => s.Recovered).Sum();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<GridDataResult>> GetGridDataAsync()
        {
            var result = new List<GridDataResult>();

            var data = await _context.StateCumulative
                .ToListAsync();

            result.Add(new GridDataResult
            {
                Name = "Tamil Nadu (Overall)",
                TotalCases = (int)data.Select(s => s.Cases).Sum(),
                Death = (int)data.Select(s => s.Death).Sum(),
                Recovered = (int)data.Select(s => s.Recovered).Sum()
            });

            var dailyDtResult = _context.DailyData
                    .GroupBy(s => s.DistrictId)
                    .Select(s => new
                    {
                        DistrictId = s.Key,
                        TotalCases = s.Sum(a => a.Cases),
                        Death = s.Sum(a => a.Death),
                        Recovered = s.Sum(a => a.Recovered)
                    });

            var districtData = await (from dt in _context.District
                                      join dtData in dailyDtResult on dt.Id equals dtData.DistrictId into leftjoinData
                                      from distData in leftjoinData.DefaultIfEmpty()
                                      select new
                                      {
                                          Name = dt.Name,
                                          TotalCases = distData.TotalCases,
                                          Death = distData.Death,
                                          Recovered = distData.Recovered
                                      }).ToListAsync();

            var groupByData = districtData
                .GroupBy(s => s.Name)
                .Select(s => new GridDataResult
                {
                    Name = s.Key,
                    TotalCases = (int)s.Sum(a => a.TotalCases),
                    Death = (int)s.Sum(a => a.Death),
                    Recovered = (int)s.Sum(a => a.Recovered)
                })
                .OrderBy(s => s.Name);

            result.AddRange(groupByData);

            return result;
        }
    }
}
