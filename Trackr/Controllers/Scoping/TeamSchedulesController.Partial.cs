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
    public partial class TeamSchedulesController : OpenAccessBaseApiController<TrackrModels.TeamSchedule, TrackrModels.ClubManagement>, IScopable<TeamSchedule, int>
    {
        public List<TeamSchedule> GetScopedEntities(int UserID, string permission, FetchStrategy fetchStrategy = null)
        {
            ScopeController sc = new ScopeController();
            var assignments = sc.GetScopeAssignments(UserID, permission);

            List<TeamSchedule> teamSchedules = new List<TeamSchedule>();
            foreach (ScopeAssignment assignment in assignments)
            {
                teamSchedules.AddRange(GetScopedEntities(assignment, fetchStrategy));
            }

            return teamSchedules;
        }

        public List<int> GetScopedIDs(int UserID, string permission)
        {
            ScopeController sc = new ScopeController();
            var assignments = sc.GetScopeAssignments(UserID, permission);

            List<int> ids = new List<int>();
            foreach (ScopeAssignment assignment in assignments)
            {
                ids.AddRange(GetScopedIDs(assignment));
            }

            return ids;
        }

        public TeamSchedule GetScopedEntity(int UserID, string permission, int entityID, FetchStrategy fetchStrategy = null)
        {
            ScopeController sc = new ScopeController();
            var assignments = sc.GetScopeAssignments(UserID, permission);

            foreach (ScopeAssignment assignment in assignments)
            {
                if (GetScopedIDs(assignment).Contains(entityID))
                {
                    if (fetchStrategy == null)
                    {
                        return Get(entityID);
                    }
                    else
                    {
                        return GetWhere(i => i.TeamScheduleID == entityID, fetchStrategy).First();
                    }
                }
            }

            return null;
        }

        public bool IsUserScoped(int UserID, string permission, int entityID)
        {
            ScopeController sc = new ScopeController();
            var assignments = sc.GetScopeAssignments(UserID, permission);

            foreach (ScopeAssignment assignment in assignments)
            {
                if (GetScopedIDs(assignment).Contains(entityID))
                {
                    return true;
                }
            }

            return false;
        }

        private List<int> GetScopedIDs(ScopeAssignment scopeAssignment)
        {
            List<int> teamScheduleIDs = new List<int>();

            switch (scopeAssignment.Scope.ScopeName)
            {
                case "Club": // highest level
                    teamScheduleIDs.AddRange(Get().Select(i => i.TeamScheduleID));
                    break;

                default: break;
            }

            return teamScheduleIDs;
        }

        private IQueryable<TeamSchedule> GetScopedEntities(ScopeAssignment scopeAssignment, FetchStrategy fetchStrategy = null)
        {
            List<int> scopedIDs = GetScopedIDs(scopeAssignment);

            if (fetchStrategy == null)
            {
                return GetWhere(i => scopedIDs.Contains(i.TeamScheduleID));
            }
            else
            {
                return GetWhere(i => scopedIDs.Contains(i.TeamScheduleID), fetchStrategy);
            }
        }
    }
}