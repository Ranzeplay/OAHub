using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Status.Models.ViewModels.Manage
{
    public class TrackIndexModel
    {
        public TrackIndexModel(Models.Track track)
        {
            Id = track.Id.ToString();
            Name = track.Name;

            if (track.Posts != null)
            {
                LastStatusAnnounced = track.Posts.Where(p => p.ShowOnHeader)
                                .OrderByDescending(p => p.PublishTime.Year)                     // Make the last post on top by publish time
                                .ThenByDescending(p => p.PublishTime.Month)
                                .ThenByDescending(p => p.PublishTime.Day)
                                .ThenByDescending(p => p.PublishTime.Hour)
                                .ThenByDescending(p => p.PublishTime.Minute)
                                .ThenByDescending(p => p.PublishTime.Second)
                                .ThenByDescending(p => p.PublishTime.Millisecond)
                                .First().Title;                                                 // Get the title on header
            }
            else
            {
                LastStatusAnnounced = "None";
            }


        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string LastStatusAnnounced { get; set; }
    }
}
