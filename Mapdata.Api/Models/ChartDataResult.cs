using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mapdata.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ChartDataResult
    {
        /// <summary>
        /// 
        /// </summary>
        public ChartDataResult()
        {
            Chart = new Chart();
            Title = new Title();
            Subtitle = new Subtitle();
            XAxis = new XAxis();
            YAxis = new YAxis();
            Tooltip = new Tooltip();
            Series = new List<Series>();
            Legend = new Legend();
            Credits = new Credits();
            PlotOptions = new PlotOptions();
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("legend")]
        public Legend Legend { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("credits")]
        public Credits Credits { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("chart")]
        public Chart Chart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("title")]
        public Title Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("subtitle")]
        public Subtitle Subtitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("xAxis")]
        public XAxis XAxis { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("yAxis")]
        public YAxis YAxis { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("tooltip")]
        public Tooltip Tooltip { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("series")]
        public List<Series> Series { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("plotOptions")]
        public PlotOptions PlotOptions { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Chart
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Series
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("color")]
        public string Color { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("data")]
        public List<int> Data { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Subtitle
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("useHTML")]
        public bool UseHtml { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Title
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Tooltip
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("valueSuffix")]
        public string ValueSuffix { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class XAxis
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("categories")]
        public List<string> Categories { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class YAxis
    {
        /// <summary>
        /// 
        /// </summary>
        public YAxis()
        {
            Title = new Title();
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("allowDecimals")]
        public bool AllowDecimal { get; } = false;

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("title")]
        public Title Title { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Legend
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Credits
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; } = false;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class PlotOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public PlotOptions()
        {
            LineOptions = new LineOptions();
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("spline")]
        public LineOptions LineOptions { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LineOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public LineOptions()
        {
            DataLabels = new DataLabels();
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("dataLabels")]
        public DataLabels DataLabels { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataLabels
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; } = true;
    }

}
