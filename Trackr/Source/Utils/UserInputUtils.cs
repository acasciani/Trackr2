using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace Trackr.Utils
{
    public enum DropDownType { Role, ScopeType, Permission }

    public static class UserInputUtils
    {
        public static void Populate(this DropDownList ddl, DropDownType type, string defaultSelectionValue = null, bool retainFirst = false)
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

            Reset(ddl, retainFirst);
            ddl.DataValueField = "Value";
            ddl.DataTextField = "Label";
            ddl.DataBind();

            SelectDefault(ddl, defaultSelectionValue);
        }


        public static void Populate_ScopeValues(this DropDownList ddl, int scopeTypeID, string defaultSelectionValue = null, bool retainFirst = false)
        {
            Reset(ddl, retainFirst);

            using(ScopeAssignmentsController sac = new ScopeAssignmentsController()){
                var scopeValues = sac.GetScopeValueDisplay(scopeTypeID);
                ddl.DataSource = scopeValues.Select(i => new { Label = i.Value, Value = i.Key }).OrderBy(i => i.Label);
                ddl.DataTextField = "Label";
                ddl.DataValueField = "Value";
                ddl.DataBind();

                SelectDefault(ddl, defaultSelectionValue);
            }
        }

        public static void Reset(this DropDownList ddl, bool keepFirst)
        {
            ListItem first = keepFirst && ddl.Items.Count > 0 ? ddl.Items[0] : null;

            ddl.Items.Clear();
            
            if (first != null)
            {
                ddl.Items.Add(first);
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

        public static string GetTenDigitNumber(string input)
        {
            // ##########
            MatchCollection matches = Regex.Matches(input, @"[0-9]");

            if (matches.Count != 10)
            {
                throw new Exception("The phone number does not have ten digits. Unable to get the ten digit phone number.");
            }

            return string.Join("", matches.Cast<Match>().Select(m => m.Value));
        }

        public static string FormatTenDigitNumber(string input)
        {
            // (###) ###-####
            char[] ten = GetTenDigitNumber(input).ToCharArray();
            return string.Format("({0}{1}{2}) {3}{4}{5}-{6}{7}{8}{9}", ten[0], ten[1], ten[2], ten[3], ten[4], ten[5], ten[6], ten[7], ten[8], ten[9]);
        }
    }
}