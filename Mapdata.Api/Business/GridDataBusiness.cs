using System;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtName"></param>
        /// <returns></returns>
        public async Task<GridSummaryResult> GetGridSummaryAsync(string dtName)
        {
            var result = new GridSummaryResult { DistrictName = dtName };

            var dt = await _context.District.Where(s => s.Name == dtName).FirstOrDefaultAsync();

            var districtId = dt.Id;

            var dailyDtResult = await _context.DailyData
                    .Where(s => s.DistrictId == districtId).ToListAsync();

            if (dailyDtResult != null && dailyDtResult.Count > 0)
            {
                result.TotalCases = (int)dailyDtResult.Sum(a => a.Cases);
                result.TotalRecovered = (int)dailyDtResult.Sum(a => a.Recovered);
                result.TotalDeath = (int)dailyDtResult.Sum(a => a.Death);
            }

            var todayDate = DateTime.Now.ToString("dd/MM");

            var todayDtResult = await _context.DailyData
                                .Where(s => s.DistrictId == districtId && s.Date == todayDate)
                                .FirstOrDefaultAsync();

            if (todayDtResult != null)
            {
                result.NewCases = (int)todayDtResult.Cases;
                result.NewRecovered = (int)todayDtResult.Recovered;
                result.NewDeath = (int)todayDtResult.Death;
            }

            return result;
        }
    }
}
