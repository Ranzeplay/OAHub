using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Base.Models
{
    public class Log
    {
        public long Id { get; set; }

        public string Content { get; set; }

        public DateTime CreateTime { get; set; }

        public LogLevel LogLevel { get; set; }
    }
}
