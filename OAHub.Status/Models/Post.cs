using OAHub.Base.Models.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Status.Models
{
    public class Post : PostBase
    {
        public Track ForTrack { get; set; }
    }
}
