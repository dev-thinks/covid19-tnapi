using System;
using System.Collections.Generic;

namespace Mapdata.Api.Entities
{
    public partial class StateCumulative
    {
        public long Id { get; set; }
        public string Date { get; set; }
        public long? Cases { get; set; }
        public long? Death { get; set; }
        public long? Recovered { get; set; }
    }
}
