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
    public partial class ProgramsController : OpenAccessBaseApiController<TrackrModels.Program, TrackrModels.ClubManagement>, IScopable<Program, int>
    {
        public List<Program> GetScopedEntities(int UserID, string permission, FetchStrategy fetchStrategy = null)
        {
            ScopeController sc = new ScopeController();
            var assignments = sc.GetScopeAssignments(UserID, permission);

            List<Program> programs = new List<Program>();
            foreach (ScopeAssignment assignment in assignments)
            {
                programs.AddRange(GetScopedEntities(assignment, fetchStrategy));
            }

            return programs;
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

        public Program GetScopedEntity(int UserID, string permission, int entityID, FetchStrategy fetchStrategy = null)
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
                        return GetWhere(i => i.ProgramID == entityID, fetchStrategy).First();
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
            List<int> programIDs = new List<int>();

            switch (scopeAssignment.Scope.ScopeName)
            {
                case "Club": // highest level
                    programIDs.AddRange(Get().Select(i => i.ProgramID));
                    break;

                default: break;
            }

            return programIDs;
        }

        private IQueryable<Program> GetScopedEntities(ScopeAssignment scopeAssignment, FetchStrategy fetchStrategy = null)
        {
            List<int> scopedIDs = GetScopedIDs(scopeAssignment);

            if (fetchStrategy == null)
            {
                return GetWhere(i => scopedIDs.Contains(i.ProgramID));
            }
            else
            {
                return GetWhere(i => scopedIDs.Contains(i.ProgramID), fetchStrategy);
            }
        }
    }
}