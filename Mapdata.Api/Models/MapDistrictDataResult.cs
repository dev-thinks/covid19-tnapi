namespace Mapdata.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class MapDistrictDataResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int DistrictId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DistrictName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalCases { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Death { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Recovered { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TodayCases { get; set; }
    }
}
