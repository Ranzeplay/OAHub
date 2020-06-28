﻿using OAHub.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Status.Models
{
    public class StatusUser : OAuthUser
    {
        public IEnumerable<Track> Tracks { get; set; }
    }
}
