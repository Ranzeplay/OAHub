using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAHub.Status.Models.ViewModels.Track
{
    public class SummaryModel
    {
        public string Name { get; set; }

        public Post HeadingPost { get; set; }

        public List<Post> RecentPosts { get; set; }
    }
}
