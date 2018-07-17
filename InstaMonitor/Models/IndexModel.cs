using System.Collections.Generic;
using InstaSharper.Classes.Models;

namespace InstaMonitor.Models
{
    public class IndexModel
    {
        public IndexModel() { }

        public IndexModel(List<InstaUserShort> followers, List<InstaUserShort> following)
        {
            Followers = followers;
            Following = following;
        }

        public string Username { get; set; }

        public List<InstaUserShort> Followers { get; set; }

        public List<InstaUserShort> Following { get; set; }

        public List<InstaUserShort> FollowersNotFollowed { get; set; }

        public List<InstaUserShort> FollowingNotFollowers { get; set; }

        public bool SearchedWithSuccess { get; set; }

        public string Message { get; set; }
    }
}