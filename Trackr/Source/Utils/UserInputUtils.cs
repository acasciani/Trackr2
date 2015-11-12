using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Trackr.Utils
{
    public enum DropDownType { Role, ScopeType, Permission }

    public static class UserInputUtils
    {
        public static void Populate(this DropDownList ddl, DropDownType type, string defaultSelectionValue = null, int[] retainIndices = null)
        {
            switch (type)
            {
                case DropDownType.Role:
                    using (RolesController rc = new RolesController())
                    {
                        var roles = rc.Get().Select(i => new { Label = i.RoleName, Value = i.RoleID }).OrderBy(i => i.Label).ToList();
                        ddl.DataSource = roles;
                    }
                    break;

                case DropDownType.ScopeType:
                    using (ScopesController sc = new ScopesController())
                    {
                        var scopes = sc.Get().Select(i => new { Label = i.ScopeName, Value = i.ScopeID }).OrderBy(i => i.Label).ToList();
                        ddl.DataSource = scopes;
                    }
                    break;

                case DropDownType.Permission:
                    using (PermissionsController pc = new PermissionsController())
                    {
                        var permissions = pc.Get().Select(i => new { Label = i.PermissionName, Value = i.PermissionID }).OrderBy(i => i.Label).ToList();
                        ddl.DataSource = permissions;
                    }
                    break;

                default: break;
            }

            ddl.DataValueField = "Value";
            ddl.DataTextField = "Label";
            ddl.DataBind();

            SelectDefault(ddl, defaultSelectionValue);
        }


        public static void Populate_ScopeValues(this DropDownList ddl, int scopeTypeID, string defaultSelectionValue = null, int[] retainIndices = null)
        {
            using(ScopeAssignmentsController sac = new ScopeAssignmentsController()){
                var scopeValues = sac.GetScopeValueDisplay(scopeTypeID);
                ddl.DataSource = scopeValues.Select(i => new { Label = i.Value, Value = i.Key }).OrderBy(i => i.Label);
                ddl.DataTextField = "Label";
                ddl.DataValueField = "Value";
                ddl.DataBind();

                SelectDefault(ddl, defaultSelectionValue);
            }
        }

        private static void SelectDefault(DropDownList ddl, string defaultSelectionValue = null)
        {
            if (!string.IsNullOrWhiteSpace(defaultSelectionValue))
            {
                foreach (ListItem item in ddl.Items)
                {
                    if (item.Value == defaultSelectionValue)
                    {
                        item.Selected = true;
                        return;
                    }
                }
            }
        }
    }
}