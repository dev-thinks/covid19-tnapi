using Mapdata.Api.Business;
using Mapdata.Api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Mapdata.Api.Controllers
{
    /// <summary>
    /// Service hour api controllers
    /// </summary>
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("api/map")]
    [EnableCors]
    public class MapApiServiceController : ControllerBase
    {
        private readonly ILogger<MapApiServiceController> _logger;
        private readonly GeoJsonBusiness _geoJsonBusiness;
        private readonly ChartDataBusiness _chartData;
        private readonly GridDataBusiness _gridData;
        private readonly CommentBusiness _comment;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="geoJsonBusiness"></param>
        /// <param name="chartData"></param>
        /// <param name="gridData"></param>
        /// <param name="comment"></param>
        public MapApiServiceController(ILogger<MapApiServiceController> logger, GeoJsonBusiness geoJsonBusiness, 
            ChartDataBusiness chartData, GridDataBusiness gridData, CommentBusiness comment)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _geoJsonBusiness = geoJsonBusiness;
            _chartData = chartData;
            _gridData = gridData;
            _comment = comment;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ApiVersion("1.0")]
        [HttpGet("getgeojson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDataAsync()
        {
            var data = await _geoJsonBusiness.GetDataAsync();

            return Ok(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtName"></param>
        /// <returns></returns>
        [ApiVersion("1.0")]
        [HttpGet("chartdata/cases")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChartDataResult))]
        public async Task<IActionResult> GetChartDataAsync(string dtName = "")
        {
            var data = await _chartData.GetChartCasesDataAsync(dtName);

            return Ok(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtName"></param>
        /// <returns></returns>
        [ApiVersion("1.0")]
        [HttpGet("chartdata/death")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChartDataResult))]
        public async Task<IActionResult> GetChartDeathDataAsync(string dtName = "")
        {
            var data = await _chartData.GetChartDeathDataAsync(dtName);

            return Ok(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ApiVersion("1.0")]
        [HttpGet("statedata")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StateWideResult))]
        public async Task<IActionResult> GetStateDataAsync()
        {
            var data = await _gridData.GetStateWideAsync();

            return Ok(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ApiVersion("1.0")]
        [HttpGet("griddata")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GridDataResult>))]
        public async Task<IActionResult> GetGridDataAsync()
        {
            var data = await _gridData.GetGridDataAsync();

            return Ok(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ApiVersion("1.0")]
        [HttpPost("addcomment")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<IActionResult> AddComment(CommentRequest model)
        {
            if (model != null && !string.IsNullOrEmpty(model.Name) && !string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Comments))
            {
                var returnValue = await _comment.SaveComment(model);

                return Ok(returnValue);
            }

            return BadRequest("Bad parameter");
        }

    }
}
