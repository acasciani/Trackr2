using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackrModels;

namespace Trackr.Source.Controls
{
    public partial class LeftNavigation : UserControl
    {
        public bool IsMobile { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {



        }

        public bool IsCurrentModule(string moduleName)
        {
            return Request.Url.LocalPath.ToUpper().StartsWith("/MODULES/" + moduleName.ToUpper());
        }


        private void GetLinks()
        {
            using (WebUsersController wuc = new WebUsersController())
            using(LinksController lc = new LinksController())
            {
                IEnumerable<ScopeAssignment> assignedScopes = wuc.Get(CurrentUser.UserID).ScopeAssignments;

                // if user has even one allowed permission, then we need to add it. need to do it by permission then by role
                var allowedByPermission = assignedScopes.Where(i => i.PermissionID.HasValue && !i.IsDeny).Select(i => i.PermissionID.Value);
                var allowedByRole = assignedScopes.Where(i => i.RoleID.HasValue && !i.IsDeny).Select(i => i.Role).SelectMany(i => i.Permissions).Select(i => i.PermissionID);

                List<int> allowed = allowedByPermission.Union(allowedByRole).Distinct().ToList();
                
                // now get the links
                var links = lc.GetWhere(i => i.LinkPermissions.Where(j => !j.IsDeny && allowed.Contains(j.PermissionID)).Count() > 0);

                var noGroups = links.Where(i=>!i.LinkGroupID.HasValue);

                if (noGroups.Count() > 0)
                {
                    rptNoGroup.DataSource = noGroups;
                    rptNoGroup.DataBind();
                    ulNoGroups.Visible = true;
                }
                else
                {
                    ulNoGroups.Visible = false;
                }
            }

        }


    }
}