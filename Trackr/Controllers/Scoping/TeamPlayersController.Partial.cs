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
    public partial class TeamPlayersController : OpenAccessBaseApiController<TrackrModels.TeamPlayer, TrackrModels.ClubManagement>, IScopable<TeamPlayer, int>
    {
        public List<TeamPlayer> GetScopedEntities(int UserID, string permission, FetchStrategy fetchStrategy = null)
        {
            ScopeController sc = new ScopeController();
            var assignments = sc.GetScopeAssignments(UserID, permission);

            List<TeamPlayer> teamPlayers = new List<TeamPlayer>();
            foreach (ScopeAssignment assignment in assignments)
            {
                teamPlayers.AddRange(GetScopedEntities(assignment, fetchStrategy));
            }

            return teamPlayers;
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

        public TeamPlayer GetScopedEntity(int UserID, string permission, int entityID, FetchStrategy fetchStrategy = null)
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
                        return GetWhere(i => i.TeamPlayerID == entityID, fetchStrategy).First();
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
            List<int> teamPlayerIDs = new List<int>();

            switch (scopeAssignment.Scope.ScopeName)
            {
                case "Club": // highest level
                    teamPlayerIDs.AddRange(Get().Select(i => i.TeamPlayerID));
                    break;

                default: break;
            }

            return teamPlayerIDs;
        }

        private IQueryable<TeamPlayer> GetScopedEntities(ScopeAssignment scopeAssignment, FetchStrategy fetchStrategy = null)
        {
            List<int> scopedIDs = GetScopedIDs(scopeAssignment);

            if (fetchStrategy == null)
            {
                return GetWhere(i => scopedIDs.Contains(i.TeamPlayerID));
            }
            else
            {
                return GetWhere(i => scopedIDs.Contains(i.TeamPlayerID), fetchStrategy);
            }
        }
    }
}