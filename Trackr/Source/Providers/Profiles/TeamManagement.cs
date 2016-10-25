using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trackr.Providers.Profiles
{
    [Serializable]
    public class TeamManagement
    {
        public List<int> RegistrationProgress_TeamIDsHide { get; set; }
        public bool RegistrationProgress_ShowHidden { get; set; }
    }
}