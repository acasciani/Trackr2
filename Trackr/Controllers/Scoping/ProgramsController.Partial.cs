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
    public partial class ProgramsController : OpenAccessBaseApiController<TrackrModels.Program, TrackrModels.ClubManagement>, IScopableController<int>
    {
        public class ProgramsScopeController : IScopable<Program, int>
        {
            public List<int> SelectIDListByScopeAssignment(ScopeAssignment scopeAssignment, Expression<Func<Program, bool>> preFilter)
            {
                using (ClubManagement cm = new ClubManagement())
                {
                    List<int> IDs = new List<int>();

                    switch (scopeAssignment.Scope.ScopeName)
                    {
                        case "Club": // highest level
                            IDs.AddRange(cm.Programs.Where(i => i.ClubID == scopeAssignment.ResourceID).Select(i => i.ProgramID));
                            break;

                        case "Program":
                            IDs.AddRange(cm.Programs.Where(i => i.ProgramID == scopeAssignment.ResourceID).Select(i => i.ProgramID));
                            break;

                        case "Team":
                            IDs.AddRange(cm.Teams.Where(i => i.TeamID == scopeAssignment.ResourceID).Select(i => i.ProgramID));
                            break;

                        case "Player":
                            List<int> programIDs = cm.TeamPlayers.Where(i => i.PlayerID == scopeAssignment.ResourceID && i.Active).Select(j => j.Team.ProgramID)
                                .Union(cm.PlayerPasses.Where(i => i.PlayerID == scopeAssignment.ResourceID && i.Active).SelectMany(j => j.TeamPlayers).Select(j => j.Team.ProgramID))
                                .Distinct().ToList();

                            IDs.AddRange(programIDs);
                            break;

                        default: break;
                    }

                    return IDs;
                }
            }
        }

        public bool IsUserScoped(int UserID, string permission, int entityID)
        {
            return ScopeController<ProgramsScopeController, Program, int>.IsUserScoped(UserID, permission, entityID);
        }

        public IEnumerable<Program> GetScopedEntities(int UserID, string permission)
        {
            return GetScopedEntities(UserID, permission, new FetchStrategy());
        }

        public IEnumerable<Program> GetScopedEntities(int UserID, string permission, FetchStrategy fetch)
        {
            List<int> programIDs = ScopeController<ProgramsScopeController, Program, int>.GetScopedIDList(UserID, permission, i => true == true);
            return GetWhere(i => programIDs.Contains(i.ProgramID), fetch);
        }

        public List<int> GetScopedIDs(int UserID, string permission)
        {
            return ScopeController<ProgramsScopeController, Program, int>.GetScopedIDList(UserID, permission, i => true == true);
        }
    }
}