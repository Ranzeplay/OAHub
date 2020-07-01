using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Status.Models.ViewModels.Track
{
    public class DeleteModel
    {
        public bool Confirm { get; set; }

        public string TrackName { get; set; }
    }
}
