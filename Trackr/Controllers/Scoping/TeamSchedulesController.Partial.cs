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
using Telerik.OpenAccess;

namespace Trackr
{
    public partial class TeamSchedulesController : OpenAccessBaseApiController<TrackrModels.TeamSchedule, TrackrModels.ClubManagement>, IScopableController<int>
    {
        private class TeamSchedulesScopeController : IScopable<TeamSchedule, int>
        {
            public bool IsUserScoped(int UserID, string permission, int entityID)
            {
                return ScopeController<TeamSchedulesScopeController, TeamSchedule, int>.IsUserScoped(UserID, permission, entityID);
            }

            public List<int> SelectIDListByScopeAssignment(ScopeAssignment scopeAssignment, Expression<Func<TeamSchedule, bool>> preFilter)
            {
                using (ClubManagement cm = new ClubManagement())
                {
                    List<int> IDs = new List<int>();

                    switch (scopeAssignment.Scope.ScopeName)
                    {
                        case "Club": // highest level
                            List<int> clubTeamIDs = cm.Programs.Where(i => i.ClubID == scopeAssignment.ResourceID).Include<Program>(i => i.Teams).SelectMany(i => i.Teams).Select(i => i.TeamID).Distinct().ToList();
                            IDs.AddRange(cm.TeamSchedules.Where(i => clubTeamIDs.Contains(i.TeamID)).Select(i => i.TeamScheduleID));
                            break;

                        case "Program":
                            List<int> programTeamIDs = cm.Teams.Where(i => i.ProgramID == scopeAssignment.ResourceID).Select(i => i.TeamID).Distinct().ToList();
                            IDs.AddRange(cm.TeamSchedules.Where(i => programTeamIDs.Contains(i.TeamID)).Select(i => i.TeamScheduleID).Distinct());
                            break;

                        case "Team":
                            IDs.AddRange(cm.TeamSchedules.Where(i => i.TeamID == scopeAssignment.ResourceID).Select(i => i.TeamScheduleID).Distinct());
                            break;
                    }

                    return IDs;
                }
            }
        }



        public IEnumerable<TeamSchedule> GetScopedEntities(int UserID, string permission)
        {
            List<int> teamScheduleIDs = ScopeController<TeamSchedulesScopeController, TeamSchedule, int>.GetScopedIDList(UserID, permission, i => true == true);
            return GetWhere(i => teamScheduleIDs.Contains(i.TeamScheduleID));
        }

        public bool IsUserScoped(int UserID, string permission, int entityID)
        {
            return ScopeController<TeamSchedulesScopeController, TeamSchedule, int>.IsUserScoped(UserID, permission, entityID);
        }

        public List<int> GetScopedIDs(int UserID, string permission)
        {
            return ScopeController<TeamSchedulesScopeController, TeamSchedule, int>.GetScopedIDList(UserID, permission, i => true == true);
        }
    }
}