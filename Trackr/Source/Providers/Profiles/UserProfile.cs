using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Profile;
using System.Web.Security;

namespace Trackr.Providers.Profiles
{
    public class UserProfile : ProfileBase
    {
        public static UserProfile GetUserProfile(string userID)
        {
            var test = Create(userID) as UserProfile;

            return test;
        }

        public static UserProfile GetUserProfile()
        {
            throw new NotImplementedException("Pass in a userid");
        }

        public TeamManagement TeamManagement
        {
            get { return base["TeamManagement"] as TeamManagement; }
            set { base["TeamManagement"] = value; }
        }
    }
}