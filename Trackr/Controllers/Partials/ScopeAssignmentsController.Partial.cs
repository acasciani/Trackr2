using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TrackrModels;

namespace Trackr
{
    public partial class ScopeAssignmentsController : OpenAccessBaseApiController<TrackrModels.ScopeAssignment, TrackrModels.UserManagement>
    {
        public string GetScopeValueDisplay(int scopeID, int resourceID)
        {
            switch (scopeID)
            {
                case 1:
                    using (ClubsController cc = new ClubsController())
                    {
                        return cc.Get(resourceID).ClubName;
                    }

                default: return "Unknown";
            }
        }

        public List<KeyValuePair<int, string>> GetScopeValueDisplay(int scopeID)
        {
            switch (scopeID)
            {
                case 1:
                    using (ClubsController cc = new ClubsController())
                    {
                        return cc.Get().Select(i => new KeyValuePair<int, string>(i.ClubID, i.ClubName)).ToList();
                    }

                default: return Enumerable.Empty<KeyValuePair<int,string>>().ToList();
            }
        }
    }
}