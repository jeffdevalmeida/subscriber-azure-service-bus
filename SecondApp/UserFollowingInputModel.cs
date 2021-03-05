using System;
using System.Collections.Generic;
using System.Text;

namespace SecondApp
{
    public class UserFollowingInputModel
    {
        public int IdUserFollower { get; set; }
        public int IdUserFollowee { get; set; }
        public string FollowedAt { get; set; }
        public string Email { get; set; }
    }
}
