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
    public partial class TeamPlayersController : OpenAccessBaseApiController<TrackrModels.TeamPlayer, TrackrModels.ClubManagement>, IScopableController<int>
    {
        private class TeamPlayersScopeController : IScopable<TeamPlayer, int>
        {
            public bool IsUserScoped(int UserID, string permission, int entityID)
            {
                return ScopeController<TeamPlayersScopeController, TeamPlayer, int>.IsUserScoped(UserID, permission, entityID);
            }

            public List<int> SelectIDListByScopeAssignment(ScopeAssignment scopeAssignment, Expression<Func<TeamPlayer, bool>> preFilter)
            {
                using (ClubManagement cm = new ClubManagement())
                {
                    List<int> IDs = new List<int>();

                    switch (scopeAssignment.Scope.ScopeName)
                    {
                        case "Club": // highest level
                                List<int> clubTeamIDs = cm.Programs.Where(i => i.ClubID == scopeAssignment.ResourceID).Include<Program>(i => i.Teams).SelectMany(i => i.Teams).Select(i => i.TeamID).Distinct().ToList();
                                IDs.AddRange(cm.TeamPlayers.Where(i=>clubTeamIDs.Contains(i.TeamID)).Select(i => i.TeamPlayerID));
                            break;

                        case "Program":
                                List<int> programTeamIDs = cm.Teams.Where(i => i.ProgramID == scopeAssignment.ResourceID).Select(i => i.TeamID).Distinct().ToList();
                                IDs.AddRange(cm.TeamPlayers.Where(i => programTeamIDs.Contains(i.TeamID)).Select(i => i.TeamPlayerID).Distinct());
                            break;

                        case "Team":
                            IDs.AddRange(cm.TeamPlayers.Where(i => i.TeamID == scopeAssignment.ResourceID).Select(i => i.TeamPlayerID).Distinct());
                            break;
                    }

                    return IDs;
                }
            }
        }



        public IEnumerable<TeamPlayer> GetScopedEntities(int UserID, string permission, FetchStrategy fetch)
        {
            List<int> teamPlayerIDs = ScopeController<TeamPlayersScopeController, TeamPlayer, int>.GetScopedIDList(UserID, permission, i => true == true);
            return GetWhere(i => teamPlayerIDs.Contains(i.TeamPlayerID), fetch);
        }

        public IEnumerable<TeamPlayer> GetScopedEntities(int UserID, string permission)
        {
            return GetScopedEntities(UserID, permission, new FetchStrategy());
        }

        public bool IsUserScoped(int UserID, string permission, int entityID)
        {
            return ScopeController<TeamPlayersScopeController, TeamPlayer, int>.IsUserScoped(UserID, permission, entityID);
        }

        public List<int> GetScopedIDs(int UserID, string permission)
        {
            return ScopeController<TeamPlayersScopeController, TeamPlayer, int>.GetScopedIDList(UserID, permission, i => true == true);
        }
    }
}