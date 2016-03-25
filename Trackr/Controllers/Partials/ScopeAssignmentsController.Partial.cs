using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Telerik.OpenAccess.FetchOptimization;
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

                case 2:
                    using (ProgramsController pc = new ProgramsController())
                    {
                        FetchStrategy fetch = new FetchStrategy();
                        fetch.LoadWith<Program>(i => i.Club);

                        Program program = pc.GetWhere(i => i.ProgramID == resourceID, fetch).First();
                        return string.Format("{0} - {1}", program.ProgramName, program.Club.ClubName);
                    }

                case 3:
                    using (TeamsController tc = new TeamsController())
                    {
                        FetchStrategy fetch = new FetchStrategy();
                        fetch.LoadWith<Team>(i => i.Program);
                        fetch.LoadWith<Program>(i => i.Club);

                        Team team = tc.GetWhere(i => i.TeamID == resourceID, fetch).First();
                        return string.Format("{0} - {1} - {2}", team.TeamName, team.Program.ProgramName, team.Program.Club.ClubName);
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

                case 2:
                    using (ProgramsController pc = new ProgramsController())
                    {
                        FetchStrategy fetch = new FetchStrategy();
                        fetch.LoadWith<Program>(i => i.Club);

                        return pc.GetWhere(i => true == true, fetch).Select(i => new KeyValuePair<int, string>(i.ProgramID, string.Format("{0} - {1}", i.ProgramName, i.Club.ClubName))).ToList();
                    }

                case 3:
                    using (TeamsController tc = new TeamsController())
                    {
                        FetchStrategy fetch = new FetchStrategy();
                        fetch.LoadWith<Team>(i => i.Program);
                        fetch.LoadWith<Program>(i => i.Club);

                        return tc.GetWhere(i => true == true, fetch).Select(i => new KeyValuePair<int, string>(i.TeamID, string.Format("{0} - {1} - {2}", i.TeamName, i.Program.ProgramName, i.Program.Club.ClubName))).ToList();
                    }

                default: return Enumerable.Empty<KeyValuePair<int,string>>().ToList();
            }
        }
    }
}