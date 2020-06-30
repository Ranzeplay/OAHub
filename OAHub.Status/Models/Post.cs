using OAHub.Base.Models.StatusModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Status.Models
{
    public class Post : PostBase
    {
        public Track ForTrack { get; set; }

        public static Post BlankPost()
        {
            return new Post
            {
                Title = "Unknown",
                Description = "Not set",
                PublishTime = DateTime.Now,
                Color = PostColor.Dark,
                ShowOnHeader = true,
            };
        }
    }
}
