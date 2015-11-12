using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackrModels;
using Telerik.OpenAccess;

namespace Trackr
{
    public class ScopeController
    {
        public List<ScopeAssignment> GetScopeAssignments(int userID, string permission)
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

                var allRoleAssignments = allScopeAssignments.Where(i => !i.IsDeny && i.RoleID.HasValue && i.Role.Permissions.Contains(permObject));
                var allPermissionAssignments = allScopeAssignments.Where(i => !i.IsDeny && i.Permission == permObject);

                return allRoleAssignments.Union(allPermissionAssignments).ToList();
            }
        }
    }
}