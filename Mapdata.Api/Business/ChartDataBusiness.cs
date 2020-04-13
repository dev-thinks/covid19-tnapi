using Mapdata.Api.DbContexts;
using Mapdata.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Mapdata.Api.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class ChartDataBusiness
    {
        private readonly TnDistrictContext _context;
        private readonly int[] emptyList = new[] { 0 };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public ChartDataBusiness(TnDistrictContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtName"></param>
        /// <returns></returns>
        public async Task<ChartDataResult> GetChartCasesDataAsync(string dtName)
        {
            var result = new ChartDataResult();

            result.Chart.Type = "spline";
            result.Subtitle.UseHtml = true;
            result.Subtitle.Text = "<span>Source: <a target=\"_blank\" href=\"https://stopcorona.tn.gov.in/daily-bulletin/\"><u>Govt. of Tamil Nadu, Health & Family Welfare Department</u></a></span>";

            result.YAxis.Title.Text = "Count";
            result.Tooltip.ValueSuffix = " count(s)";
            result.Legend.Enabled = false;

            if (string.IsNullOrEmpty(dtName))
            {
                result.Title.Text = "Tamil Nadu - Daily cases";

                var stateData = await _context.StateCumulative
                    .OrderByDescending(s => s.Id)
                    .Take(30)
                    .ToListAsync();

                stateData = stateData.OrderBy(s => s.Id).ToList();

                var dates = stateData.Select(s => s.Date).ToList();

                result.XAxis.Categories = dates;

                var series = new Series
                {
                    Name = "Cases",
                    Data = stateData.Select(s => (int)s.Cases).Count() > 0 ? stateData.Select(s => (int)s.Cases).ToList() : emptyList.ToList(),
                    Color = "Orange"
                };

                result.Series.Add(series);
            }
            else
            {
                result.Title.Text = $"{dtName} - Total cases";

                var districtData = await _context.DailyData.Include("District")
                    .Where(s => s.District.Name == dtName)
                    .OrderByDescending(s => s.Id)
                    .Take(30)
                    .ToListAsync();

                districtData = districtData.OrderBy(s => s.Id).ToList();

                var dates = districtData.Select(s => s.Date).ToList();

                result.XAxis.Categories = dates;

                var series = new Series
                {
                    Name = "Cases",
                    Data = districtData.Select(s => (int)s.Cases).Count() > 0 ? districtData.Select(s => (int)s.Cases).ToList() : emptyList.ToList(),
                    Color = "Orange"
                };

                result.Series.Add(series);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ChartDataResult> GetChartDeathDataAsync(string dtName)
        {
            var result = new ChartDataResult();

            result.Chart.Type = "spline";
            result.Subtitle.UseHtml = true;
            result.Subtitle.Text = "<span>Source: <a target=\"_blank\" href=\"https://stopcorona.tn.gov.in/daily-bulletin/\"><u>Govt. of Tamil Nadu, Health & Family Welfare Department</u></a></span>";

            result.YAxis.Title.Text = "Count";
            result.Tooltip.ValueSuffix = " count(s)";
            result.Legend.Enabled = false;

            if (string.IsNullOrEmpty(dtName))
            {
                result.Title.Text = "Tamil Nadu - Reported Death cases";

                var stateData = await _context.StateCumulative
                    .OrderByDescending(s => s.Id)
                    .Take(30)
                    .ToListAsync();

                stateData = stateData.OrderBy(s => s.Id).ToList();

                var dates = stateData.Select(s => s.Date).ToList();

                result.XAxis.Categories = dates;

                var series = new Series
                {
                    Name = "Death cases",
                    Data = stateData.Select(s => (int)s.Death).Count() > 0 ? stateData.Select(s => (int)s.Death).ToList() : emptyList.ToList(),
                    Color = "#E10000"
                };

                result.Series.Add(series);
            }
            else
            {
                result.Title.Text = $"{dtName} - Total cases";

                var districtData = await _context.DailyData.Include("District")
                    .Where(s => s.District.Name == dtName)
                    .OrderByDescending(s => s.Id)
                    .Take(30)
                    .ToListAsync();

                districtData = districtData.OrderBy(s => s.Id).ToList();

                var dates = districtData.Select(s => s.Date).ToList();

                result.XAxis.Categories = dates;

                var series = new Series
                {
                    Name = "Death Cases",
                    Data = districtData.Select(s => s.Death).Count() > 0 ? districtData.Select(s => (int)s.Death).ToList() : emptyList.ToList(),
                    Color = "#E10000"
                };

                result.Series.Add(series);
            }

            return result;
        }

    }
}
