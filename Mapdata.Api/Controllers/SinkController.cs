using System;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Mapdata.Api.Controllers
{
    /// <summary>
    /// Sink green box
    /// </summary>
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    [Route("api/sink")]
    [EnableCors]
    public class SinkController : ControllerBase
    {
        private readonly ILogger<SinkController> _logger;

        public SinkController(ILogger<SinkController> logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Gets current server machine time zone (supports both windows/non-windows)
        /// </summary>
        /// <returns></returns>
        [ApiVersion("1.0")]
        [HttpGet("gettime")]
        public IActionResult GetTime()
        {
            _logger.LogDebug("{machine}", new
            {
                RuntimeInformation.FrameworkDescription,
                RuntimeInformation.ProcessArchitecture,
                RuntimeInformation.OSArchitecture,
                RuntimeInformation.OSDescription
            });
            
            var timeZone = TimeZoneInfo.GetSystemTimeZones().Any(x => x.Id == "Eastern Standard Time") ? 
                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time") : 
                TimeZoneInfo.FindSystemTimeZoneById("America/New_York");

            return Ok(timeZone);
        }
    }
}