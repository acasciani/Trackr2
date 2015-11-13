using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;
using Trackr.Utils;
using System.Web.Security;

namespace Trackr.Source.Wizards
{
    public partial class UserManagement : WizardBase<int>
    {
        [Serializable]
        private class ScopeAssignmentResult
        {
            public string RoleName { get; set; }
            public string PermissionName { get; set; }
            public string ScopeType { get; set; }
            public string ScopeValue { get; set; }
            public bool IsDeny { get; set; }
            public int? ScopeAssignmentID { get; set; }
            public int ScopeID { get; set; }
            public int ResourceID { get; set; }
            public int? RoleID { get; set; }
            public int? PermissionID { get; set; }
        }

        private List<ScopeAssignmentResult> ScopeAssignmentResults
        {
            get { return ViewState["ScopeAssignments"] as List<ScopeAssignmentResult>; }
            set { ViewState["ScopeAssignments"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            UserWizard.DisplaySideBar = UserWizard.ActiveStepIndex > 0;

            if (IsPostBack)
            {
                return;
            }

            using (WebUsersController wuc = new WebUsersController())
            {
                if (IsNew)
                {
                    if (wuc.IsAllowed(CurrentUser.UserID, Permissions.UserManagement.CreateUser))
                    {
                        // remove the edit stuff
                        UserWizard.ActiveStepIndex = 0;
                    }
                    else
                    {
                        throw new UserUnauthorizedException("You do not have permission to create a new user.");
                    }
                }
                else
                {
                    if (wuc.GetScopedEntity(CurrentUser.UserID, Permissions.UserManagement.EditUser, PrimaryKey.Value) != null)
                    {
                        // remove create stuff
                        UserWizard.ActiveStepIndex = 1;
                    }
                    else
                    {
                        throw new UserNotScopedException("You are not allowed to edit the selected user.");
                    }
                }
            }

            UserWizard.DisplaySideBar = UserWizard.ActiveStepIndex > 0;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AlertBox.HideStatus();

            if (IsPostBack)
            {
                return;
            }

            if (IsNew)
            {
                Populate_Create();
            }
            else
            {
                Populate_Edit();
            }

            // update dropdownlists
            ddlRole.Populate(DropDownType.Role);
            ddlScopeType.Populate(DropDownType.ScopeType);
            ddlPermission.Populate(DropDownType.Permission);
        }

        
        private void Populate_Create()
        {

        }

        private void Populate_Edit()
        {
            using (WebUsersController wuc = new WebUsersController())
            using(WebUserInfosController wuic = new WebUserInfosController())
            {
                WebUser wu = wuc.GetScopedEntity(CurrentUser.UserID, Permissions.UserManagement.EditUser, PrimaryKey.Value);
                txtEmail.Text = wu.Email;

                WebUserInfo info = wuic.GetWhere(i=>i.UserID == PrimaryKey.Value).FirstOrDefault();
                if(info != null){
                    txtFirstName.Text = info.FName;
                    txtMiddleInitial.Text = info.MInitial.HasValue ? info.MInitial.ToString() : "";
                    txtLastName.Text = info.LName;
                }
            }
        }

        // The return type can be changed to IEnumerable, however to support
        // paging and sorting, the following parameters must be added:
        //     int maximumRows
        //     int startRowIndex
        //     out int totalRowCount
        //     string sortByExpression
        public IQueryable gvRoleAssignments_GetData()
        {
            if (ScopeAssignmentResults == null)
            {
                if (IsNew)
                {
                    ScopeAssignmentResults = new List<ScopeAssignmentResult>();
                }
                else
                {
                    using (ScopeAssignmentsController sac = new ScopeAssignmentsController())
                    {
                        FetchStrategy fetch = new FetchStrategy();
                        fetch.LoadWith<ScopeAssignment>(i => i.Role);
                        fetch.LoadWith<ScopeAssignment>(i => i.Scope);
                        fetch.LoadWith<ScopeAssignment>(i => i.Permission);

                        ScopeAssignmentResults = sac.GetWhere(i => i.UserID == PrimaryKey.Value, fetch).Select(i => new ScopeAssignmentResult()
                        {
                            RoleName = i.RoleID.HasValue ? i.Role.RoleName : "",
                            ScopeAssignmentID = i.ScopeAssignmentID,
                            ScopeType = i.Scope.ScopeName,
                            ScopeValue = sac.GetScopeValueDisplay(i.ScopeID, i.ResourceID),
                             ResourceID = i.ResourceID, 
                             RoleID = i.RoleID,
                             ScopeID = i.ScopeID,
                              PermissionName = i.PermissionID.HasValue ? i.Permission.PermissionName : "",
                               PermissionID = i.PermissionID,
                                IsDeny = i.IsDeny
                        }).ToList();
                    }
                }
            }

            return ScopeAssignmentResults.AsQueryable();
        }
       
        // The id parameter name should match the DataKeyNames value set on the control
        public void gvRoleAssignments_DeleteItem(int? ScopeAssignmentID, int? ScopeID, int? ResourceID, int? RoleID)
        {
            if (ScopeAssignmentID.HasValue)
            {
                using (ScopeAssignmentsController sac = new ScopeAssignmentsController())
                {
                    HttpResponseMessage response = sac.Delete(ScopeAssignmentID.Value);
                    if (response.IsSuccessStatusCode)
                    {
                        //YES
                        ScopeAssignmentResults.RemoveAll(i => i.ScopeAssignmentID == ScopeAssignmentID.Value);
                    }
                    else
                    {
                        //no
                    }
                }
            }
            else
            {
                ScopeAssignmentResults.RemoveAll(i => i.ScopeID == ScopeID && i.ResourceID == ResourceID && i.RoleID == RoleID);
            }
        }

        protected void lnkAddRoleAssignment_Click(object sender, EventArgs e)
        {
            pnlAddRoleAssignment.Visible = true;
        }

        protected void ddlScopeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int scopeID;
            if (int.TryParse(ddlScopeType.SelectedValue, out scopeID))
            {
                ddlScopeValue.Populate_ScopeValues(scopeID, retainFirst: true);
                ddlScopeValue.Focus();
            }
        }

        protected void btnAddAssignment_Click(object sender, EventArgs e)
        {
            int roleID, permissionID, scopeID, resourceID;

            if ((int.TryParse(ddlRole.SelectedValue, out roleID) || int.TryParse(ddlPermission.SelectedValue, out permissionID)) && int.TryParse(ddlScopeType.SelectedValue, out scopeID) && int.TryParse(ddlScopeValue.SelectedValue, out resourceID))
            {
                // check that it isn't already in the list
                if (ScopeAssignmentResults.Count(i => i.ScopeID == scopeID && i.ResourceID == resourceID && ((int.TryParse(ddlRole.SelectedValue, out roleID) && i.RoleID == roleID) || (int.TryParse(ddlPermission.SelectedValue, out permissionID) && i.PermissionID == permissionID))) > 0)
                {
                    return;
                }

                ScopeAssignmentResults.Add(new ScopeAssignmentResult()
                {
                    ResourceID = resourceID,
                    RoleID = !int.TryParse(ddlRole.SelectedValue, out roleID) ? (int?)null : roleID,
                    PermissionID = !int.TryParse(ddlPermission.SelectedValue, out permissionID) ? (int?)null : permissionID,
                    ScopeID = scopeID,
                    RoleName = ddlRole.SelectedIndex == 0 ? "" : ddlRole.SelectedItem.Text,
                    ScopeType = ddlScopeType.SelectedItem.Text,
                    ScopeValue = ddlScopeValue.SelectedItem.Text,
                    IsDeny = chkDenyFlag.Checked,
                    PermissionName = ddlPermission.SelectedIndex == 0 ? "" : ddlPermission.SelectedItem.Text
                });

                gvRoleAssignments.DataBind();
            }
        }

        protected void Step1_Edit_Deactivate(object sender, EventArgs e)
        {
            MembershipUser user = Membership.GetUser(PrimaryKey.Value);

            if (user.UserName != txtEmail.Text && Membership.GetUser(txtEmail.Text) == null)
            {
                using (WebUsersController wuc = new WebUsersController())
                {
                    WebUser webUser = wuc.Get(PrimaryKey.Value);
                    webUser.Email = txtEmail.Text;
                    wuc.Update(webUser);
                }
            }
        }

        protected void Step2_Personal_Deactivate(object sender, EventArgs e)
        {
            using (WebUserInfosController wuic = new WebUserInfosController())
            {
                WebUserInfo info = wuic.GetWhere(i => i.UserID == PrimaryKey.Value).FirstOrDefault();

                if (info == null)
                {
                    // no info, so add one
                    info = new WebUserInfo()
                    {
                        FName = txtFirstName.Text,
                        LName = txtLastName.Text,
                        MInitial = string.IsNullOrWhiteSpace(txtMiddleInitial.Text) ? (char?)null : (char)txtMiddleInitial.Text.ToCharArray(0,1)[0],
                        UserID = PrimaryKey.Value
                    };
                    wuic.AddNew(info);
                }
                else
                {
                    info.FName = txtFirstName.Text;
                    info.LName = txtLastName.Text;
                    info.MInitial = string.IsNullOrWhiteSpace(txtMiddleInitial.Text) ? (char?)null : (char)txtMiddleInitial.Text.ToCharArray(0, 1)[0];
                    wuic.Update(info);
                }
            }
        }

        protected void UserWizard_CreatedUser(object sender, EventArgs e)
        {
            MembershipUser user = Membership.GetUser(((CreateUserWizard)sender).UserName);
            if (user != null)
            {
                PrimaryKey = (int)user.ProviderUserKey;
            }

            Populate_Edit();
            UserWizard.ActiveStepIndex = 2;
            UserWizard.DisplaySideBar = true;

            AlertBox.SetStatus("Successfully created user. Enter user's personal information below.");
        }

        protected void Step3_RoleAssignments_Deactivate(object sender, EventArgs e)
        {
            using (ScopeAssignmentsController sac = new ScopeAssignmentsController())
            {
                foreach (ScopeAssignmentResult sar in ScopeAssignmentResults.Where(i=>!i.ScopeAssignmentID.HasValue))
                {
                    ScopeAssignment assignment = new ScopeAssignment()
                    {
                        IsDeny = false,
                        ResourceID = sar.ResourceID,
                        RoleID = sar.RoleID.HasValue ? sar.RoleID.Value : (int?)null,
                        ScopeID = sar.ScopeID,
                        UserID = PrimaryKey.Value,
                        PermissionID = sar.PermissionID.HasValue ? sar.PermissionID.Value : (int?)null
                    };
                    sac.AddNew(assignment);
                }
            }
        }

        protected void UserWizard_CreateUserError(object sender, CreateUserErrorEventArgs e)
        {
            string plainEnglishMsg = "An error occurred while creating the user.";

            switch (e.CreateUserError)
            {
                case MembershipCreateStatus.DuplicateEmail: plainEnglishMsg = "This email address is already in use and cannot be used again."; break;
                case MembershipCreateStatus.DuplicateProviderUserKey: plainEnglishMsg = "The provider key specified is already in use and cannot be used again."; break;
                case MembershipCreateStatus.DuplicateUserName: plainEnglishMsg = "This email address is already in use and cannot be used again."; break;
                case MembershipCreateStatus.InvalidEmail: plainEnglishMsg = "This is an invalid email address."; break;
                default: break;
            }

            AlertBox.SetStatus(plainEnglishMsg, UI.AlertBoxType.Error);
        }

        protected void validatorEmailExists_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = Membership.FindUsersByName(args.Value).Count == 0;
        }

        protected void validatorPermissionNotAlsoEntered_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (ddlRole.SelectedIndex > 0 && ddlPermission.SelectedIndex > 0)
            {
                args.IsValid= false;
            }
            else
            {
                args.IsValid=true;
            }
        }

        protected void validatorSelectItem_ServerValidate(object source, ServerValidateEventArgs args)
        {
            int tryParse;
            args.IsValid = int.TryParse(args.Value, out tryParse);
        }

        protected void validatorRolePermissionRequired_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ddlPermission.SelectedIndex > 0 || ddlRole.SelectedIndex > 0;
        }

        protected void lnkEditAgain_Click(object sender, EventArgs e)
        {
            UserWizard.ActiveStepIndex = 1;
        }
    }
}