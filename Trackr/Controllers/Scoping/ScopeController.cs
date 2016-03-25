using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackrModels;
using Telerik.OpenAccess;
using System.Linq.Expressions;

namespace Trackr
{
    public static class ScopeController<T, U, K>
        where T : IScopable<U, K>, new()
        where U : class, new()
        where K : struct
    {
        public static List<ScopeAssignment> GetScopeAssignments(int userID, string permission, bool isDeny)
        {
            using (UserManagement um = new UserManagement())
            {
                Permission permObject = um.Permissions.Where(i => i.PermissionName == permission).FirstOrDefault();

                if (permObject == null)
                {
                    throw new Exception("Permission does not exist in the database.");
                }

                var allScopeAssignments = um.ScopeAssignments
                    .Include(i => i.Permission)
                    .Include(i => i.Role)
                    .Where(i => i.UserID == userID);

                var allRoleAssignments = allScopeAssignments.Where(i => isDeny == i.IsDeny && i.RoleID.HasValue && i.Role.Permissions.Contains(permObject));
                var allPermissionAssignments = allScopeAssignments.Where(i => isDeny == i.IsDeny && i.Permission == permObject);

                return allRoleAssignments.Union(allPermissionAssignments).ToList();
            }
        }

        public static List<ScopeAssignment> GetScopeAssignments(int userID, bool isDeny)
        {
            using (UserManagement um = new UserManagement())
            {
                var allScopeAssignments = um.ScopeAssignments.Where(i => i.UserID == userID && i.IsDeny == isDeny);
                return allScopeAssignments.ToList();
            }
        }




        public static List<K> GetScopedIDList(int id, string permissionName, Expression<Func<U, bool>> preFilter)
        {
            List<ScopeAssignment> allowedScopes = string.IsNullOrWhiteSpace(permissionName) ? GetScopeAssignments(id, false) : GetScopeAssignments(id, permissionName, false);
            List<ScopeAssignment> deniedScopes = string.IsNullOrWhiteSpace(permissionName) ? GetScopeAssignments(id, true) : GetScopeAssignments(id, permissionName, true);

            List<K> scopedIDs = new List<K>();

            T processor = new T();

            foreach (ScopeAssignment allowedScope in allowedScopes)
            {
                scopedIDs.AddRange(processor.SelectIDListByScopeAssignment(allowedScope, preFilter));
            }

            foreach (ScopeAssignment deniedScope in deniedScopes)
            {
                scopedIDs = scopedIDs.Except(processor.SelectIDListByScopeAssignment(deniedScope, i => true == true)).ToList();
            }

            return scopedIDs;
        }

        public static bool IsUserScoped(int UserID, string permission, K entityID)
        {
            return GetScopedIDList(UserID, permission, i => true == true).Contains(entityID);
        }
    }
}