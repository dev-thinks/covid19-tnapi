using System;
using System.Collections.Generic;

namespace Mapdata.Api.Entities
{
    public partial class Comment
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Feedback { get; set; }
    }
}
