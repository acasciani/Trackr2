using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;

namespace Trackr
{
    public partial class WebUsersController : OpenAccessBaseApiController<TrackrModels.WebUser, TrackrModels.UserManagement>, IScopable<WebUser, int>
    {
        [Route("api/WebUsers/GetScoped/{UserID}/{permission}")]
        [HttpGet]
        public List<WebUser> GetScopedEntities(int UserID, string permission, FetchStrategy fetchStrategy = null)
        {
            ScopeController sc = new ScopeController();
            var assignments = sc.GetScopeAssignments(UserID, permission);

            List<WebUser> users = new List<WebUser>();
            foreach (ScopeAssignment assignment in assignments)
            {
                users.AddRange(GetScopedEntities(assignment, fetchStrategy));
            }

            return users;
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

        public WebUser GetScopedEntity(int UserID, string permission, int entityID, FetchStrategy fetchStrategy = null)
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
                        return GetWhere(i => i.UserID == entityID, fetchStrategy).First();
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
            List<int> userIDs = new List<int>();

            switch (scopeAssignment.Scope.ScopeName)
            {
                case "Club": // highest level
                    userIDs.AddRange(Get().Select(i => i.UserID));
                    break;

                default: break;
            }

            return userIDs;
        }

        private IQueryable<WebUser> GetScopedEntities(ScopeAssignment scopeAssignment, FetchStrategy fetchStrategy = null)
        {
            List<int> scopedIDs = GetScopedIDs(scopeAssignment);

            if (fetchStrategy == null)
            {
                return GetWhere(i => scopedIDs.Contains(i.UserID));
            }
            else
            {
                return GetWhere(i => scopedIDs.Contains(i.UserID), fetchStrategy);
            }
        }

        /// <summary>
        /// If the user has any instance of deny for this permissin, then this returns false
        /// </summary>
        public bool IsAllowed(int userID, string permission)
        {
            ScopeController sc = new ScopeController();
            var assignments = sc.GetScopeAssignments(userID, permission);
            var denyCount = assignments.Count(i => i.IsDeny);
            var allowCount = assignments.Count(i => !i.IsDeny);

            return denyCount == 0 && allowCount > 0;
        }
    }
}