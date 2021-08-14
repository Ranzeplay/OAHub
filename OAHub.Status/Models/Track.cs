using OAHub.Base.Models.StatusModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Status.Models
{
    public class Track : TrackBase
    {
        public StatusUser CreatedBy { get; set; }

        public IEnumerable<Post> Posts { get; set; }
    }
}
