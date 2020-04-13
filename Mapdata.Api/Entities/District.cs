using System;
using System.Collections.Generic;

namespace Mapdata.Api.Entities
{
    public partial class District
    {
        public District()
        {
            DailyData = new HashSet<DailyData>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DailyData> DailyData { get; set; }
    }
}
