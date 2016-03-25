using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Http;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;

namespace Trackr
{
    public partial class WebUsersController : OpenAccessBaseApiController<TrackrModels.WebUser, TrackrModels.UserManagement>, IScopableController<int>
    {/*
        [Route("api/WebUsers/GetScoped/{UserID}/{permission}")]
        [HttpGet]
        public List<WebUser> GetScopedEntities(int UserID, string permission, FetchStrategy fetchStrategy = null)
        {
            return null;
        }

        public List<int> GetScopedIDs(int UserID, string permission)
        {
            return null;
        }

        public WebUser GetScopedEntity(int UserID, string permission, int entityID, FetchStrategy fetchStrategy = null)
        {
            return null;
        }
        s
        public bool IsUserScoped(int UserID, string permission, int entityID)
        {
            return null;
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

        */

        private class WebUsersScopeController : IScopable<WebUser, int>
        {
            public bool IsUserScoped(int UserID, string permission, int entityID)
            {
                return ScopeController<WebUsersScopeController, WebUser, int>.IsUserScoped(UserID, permission, entityID);
            }

            public List<int> SelectIDListByScopeAssignment(ScopeAssignment scopeAssignment, Expression<Func<WebUser, bool>> preFilter)
            {
                using (UserManagement um = new UserManagement())
                {
                    List<int> IDs = new List<int>();

                    switch (scopeAssignment.Scope.ScopeName)
                    {
                        case "Club": // highest level
                            IDs.AddRange(um.WebUsers.Select(i => i.UserID));
                            break;
                    }

                    return IDs;
                }
            }
        }



        public WebUser GetScopedEntity(int UserID, string permission, int primaryKey)
        {
            return GetScopedEntity(UserID, permission, primaryKey, new FetchStrategy());
        }

        public WebUser GetScopedEntity(int UserID, string permission, int primaryKey, FetchStrategy fetch)
        {
            List<int> userIDs = ScopeController<WebUsersScopeController, WebUser, int>.GetScopedIDList(UserID, permission, i => i.UserID == primaryKey);
            return GetWhere(i => userIDs.Contains(i.UserID), fetch).FirstOrDefault();
        }

        public IEnumerable<WebUser> GetScopedEntities(int UserID, string permission)
        {
            List<int> webUserIDs = ScopeController<WebUsersScopeController, WebUser, int>.GetScopedIDList(UserID, permission, i => true == true);
            return GetWhere(i => webUserIDs.Contains(i.UserID));
        }

        public bool IsUserScoped(int UserID, string permission, int entityID)
        {
            return ScopeController<WebUsersScopeController, WebUser, int>.IsUserScoped(UserID, permission, entityID);
        }

        public List<int> GetScopedIDs(int UserID, string permission)
        {
            return ScopeController<WebUsersScopeController, WebUser, int>.GetScopedIDList(UserID, permission, i => true == true);
        }

        /// <summary>
        /// If the user has any instance of deny for this permissin, then this returns false
        /// </summary>
        public bool IsAllowed(int userID, string permission)
        {
            var denyCount = ScopeController<WebUsersScopeController, WebUser, int>.GetScopeAssignments(userID, permission, true);
            var allowCount = ScopeController<WebUsersScopeController, WebUser, int>.GetScopeAssignments(userID, permission, false);

            return /*denyCount.Count() == 0 && */allowCount.Count() > 0;
        }
    }
}