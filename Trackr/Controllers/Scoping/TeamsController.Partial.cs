using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;
using Telerik.OpenAccess;
using System.Linq.Expressions;

namespace Trackr
{
    public partial class TeamsController : OpenAccessBaseApiController<TrackrModels.Team, TrackrModels.ClubManagement>, IScopableController<int>
    {
        private class TeamsScopeController : IScopable<Team, int>
        {
            public bool IsUserScoped(int UserID, string permission, int entityID)
            {
                return ScopeController<TeamsScopeController, Team, int>.IsUserScoped(UserID, permission, entityID);
            }

            public List<int> SelectIDListByScopeAssignment(ScopeAssignment scopeAssignment, Expression<Func<Team, bool>> preFilter)
            {
                using (ClubManagement cm = new ClubManagement())
                {
                    List<int> IDs = new List<int>();

                    switch (scopeAssignment.Scope.ScopeName)
                    {
                        case "Club": // highest level
                            List<int> clubTeamIDs = cm.Programs.Where(i => i.ClubID == scopeAssignment.ResourceID).Include<Program>(i => i.Teams).SelectMany(i => i.Teams).Select(i => i.TeamID).Distinct().ToList();
                            IDs.AddRange(cm.Teams.Where(i => clubTeamIDs.Contains(i.TeamID)).Select(i => i.TeamID));
                            break;

                        case "Program":
                            List<int> programTeamIDs = cm.Teams.Where(i => i.ProgramID == scopeAssignment.ResourceID).Select(i => i.TeamID).Distinct().ToList();
                            IDs.AddRange(cm.Teams.Where(i => programTeamIDs.Contains(i.TeamID)).Select(i => i.TeamID).Distinct());
                            break;

                        case "Team":
                            IDs.AddRange(cm.Teams.Where(i => i.TeamID == scopeAssignment.ResourceID).Select(i => i.TeamID).Distinct());
                            break;
                    }

                    return IDs;
                }
            }
        }



        public IEnumerable<Team> GetScopedEntities(int UserID, string permission)
        {
            List<int> teamIDs = ScopeController<TeamsScopeController, Team, int>.GetScopedIDList(UserID, permission, i => true == true);
            return GetWhere(i => teamIDs.Contains(i.TeamID));
        }

        public bool IsUserScoped(int UserID, string permission, int entityID)
        {
            return ScopeController<TeamsScopeController, Team, int>.IsUserScoped(UserID, permission, entityID);
        }

        public List<int> GetScopedIDs(int UserID, string permission)
        {
            return ScopeController<TeamsScopeController, Team, int>.GetScopedIDList(UserID, permission, i => true == true);
        }
    }
}