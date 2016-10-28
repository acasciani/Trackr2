using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;
using Trackr.Utils;

namespace Trackr.Source.Wizards
{
    public partial class TeamManagement : WizardBase<int>
    {
        public class TeamSavedEventArgs : EventArgs
        {
            public int? TeamID { get; set; }
        }

        public string CreatePermission { get; set; }
        public string EditPermission { get; set; }

        public event EventHandler TeamSavedSuccess;
        public event EventHandler TeamSavedError;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                using (TeamsController tc = new TeamsController())
                using (WebUsersController wuc = new WebUsersController())
                {
                    if (IsNew)
                    {
                        if (!wuc.IsAllowed(CurrentUser.UserID, string.IsNullOrWhiteSpace(CreatePermission) ? Permissions.TeamManagement.CreateTeam : CreatePermission))
                        {
                            throw new UserUnauthorizedException("You do not have permission to create a new team.");
                        }
                    }
                    else
                    {
                        if (tc.GetScopedEntity(CurrentUser.UserID, string.IsNullOrWhiteSpace(EditPermission) ? Permissions.TeamManagement.EditTeam : EditPermission, PrimaryKey.Value) == null)
                        {
                            throw new UserNotScopedException("You are not allowed to edit the selected team.");
                        }
                    }
                }
            }

            AlertBox.HideStatus();

            // set permission on program picker
            if (WasNew)
            {
                ptpPicker.Permission = string.IsNullOrWhiteSpace(CreatePermission) ? Permissions.TeamManagement.CreateTeam : CreatePermission;
            }
            else
            {
                ptpPicker.Permission = string.IsNullOrWhiteSpace(EditPermission) ? Permissions.TeamManagement.EditTeam : EditPermission;
            }

            if (IsPostBack)
            {
                if (TeamManager.Team == null)
                {
                    Response.Redirect(Request.Url.AbsoluteUri, true);
                }

                return;
            }

            // Anything that needs to get run on page load (not postback), place in the Reload method
            Reload();
        }


        public void Reload()
        {
            AlertBox.HideStatus();

            WasNew = IsNew;

            if (IsNew)
            {
                Populate_Create();
            }
            else
            {
                Populate_Edit();
            }
        }

        private void Populate_Create()
        {
            TeamManager.CreateTeam();

            Team team = TeamManager.Team;

            ptpPicker.SelectedProgramID = null;
            TeamWizard.ActiveStepIndex = 0;            
        }

        private void Populate_Edit()
        {
            using (TeamsController tc = new TeamsController())
            {
                TeamManager.EditTeam(tc.GetScopedEntity(CurrentUser.UserID, (WasNew ? (string.IsNullOrWhiteSpace(CreatePermission) ? Permissions.TeamManagement.CreateTeam : CreatePermission) : (string.IsNullOrWhiteSpace(EditPermission) ? Permissions.TeamManagement.EditTeam : EditPermission)), PrimaryKey.Value).TeamID);

                Team team = TeamManager.Team;

                ptpPicker.SelectedProgramID = team.ProgramID;
                ptpPicker.Populate();

                txtTeamName.Text = team.TeamName;
                txtActiveFrom.Text = team.StartYear.ToString("yyyy-MM-dd");
                txtActiveTo.Text = team.EndYear.ToString("yyyy-MM-dd");
                txtMinRosterSize.Text = team.MinRosterSize.ToString();
                txtMaxRosterSize.Text = team.MaxRosterSize.ToString();
                txtMaxDOB.Text = team.AgeCutoff.ToString("yyyy-MM-dd");

                // Reset views
                TeamWizard.ActiveStepIndex = 0;
            }
        }

        private void Save_Step1()
        {
            TeamManager.UpdateInfo(ptpPicker.SelectedProgramID.Value, txtTeamName.Text, DateTime.Parse(txtActiveFrom.Text), DateTime.Parse(txtActiveTo.Text), short.Parse(txtMinRosterSize.Text), short.Parse(txtMaxRosterSize.Text), DateTime.Parse(txtMaxDOB.Text));
        }

        protected void TeamWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }

            switch (e.CurrentStepIndex)
            {
                case 1:

                    break;

                default: break;
            }
        }

        protected void validatorDateTimeParses_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime date;
            args.IsValid = DateTime.TryParse(args.Value, out date) && new DateTime(1900, 1, 1) <= date && date <= new DateTime(2200, 1, 1);
        }

        protected void TeamWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }

            try
            {
                int teamID = TeamManager.SaveData(CurrentUser.UserID);

                if (TeamSavedSuccess != null)
                {
                    TeamSavedSuccess(null, new TeamSavedEventArgs() { TeamID = teamID });
                }
                else
                {
                    AlertBox.AddAlert("Successfully saved changes.");
                }
            }
            catch (TeamModifiedByAnotherProcessException ex)
            {
                AlertBox.AddAlert("Unable to save changes. This team was modified by someone else before you committed your changes. Please reload the page and try again.", false, UI.AlertBoxType.Error);

                if (TeamSavedError != null)
                {
                    TeamSavedError(null, new TeamSavedEventArgs() { TeamID = TeamManager.Team.TeamID == 0 ? (int?)null : TeamManager.Team.TeamID });
                }
            }
            catch (Exception ex)
            {
                Page.Master.HandleException(ex);

                if (TeamSavedError != null)
                {
                    TeamSavedError(null, null);
                }
            }
        }




        #region Registration Rule Administration
        public IQueryable gvRegRules_GetData()
        {

            return TeamManager.Team.Guardians.Where(i => i.Active).Select(i => new
            {
                Guardian = i.Person.FName + " " + i.Person.LName,
                IsRemovable = true,
                GuardianID = i.GuardianID,
                EditToken = i.EditToken,
                PersonEditToken = i.Person.EditToken
            }).OrderByDescending(i => i.Guardian).AsQueryable();
        }

        private void ClearRegistrationRuleForm()
        {
            txtGuardianFirstName.Text = null;
            txtGuardianMiddleInitial.Text = null;
            txtGuardianLastName.Text = null;
            AddressBook.Reset();
            EmailBook.Reset();
            PhoneNumberBook.Reset();
            mvGuardianTabs.ActiveViewIndex = 0;
            pnlAddGuardian.Visible = false;
            lnkAddGuardian.Visible = true;
            UpdateGuardianTabs();
        }

        private Guid SaveRegistrationRule(Guid? editToken)
        {
            if (!editToken.HasValue)
            {
                editToken = PlayerManager.AddGuardian(ClubID);
            }

            Guid playerEditToken = PlayerManager.Player.Guardians.First(i => i.EditToken == editToken.Value).Person.EditToken;

            char? mInitial = string.IsNullOrWhiteSpace(txtGuardianMiddleInitial.Text) ? (char?)null : txtGuardianMiddleInitial.Text[0];
            PlayerManager.UpdatePerson(playerEditToken, txtGuardianFirstName.Text, txtGuardianLastName.Text, mInitial, null);

            return editToken.Value;
        }

        private void Populate_RegistrationRuleEdit(Guid guardianEditToken)
        {
            ClearGuardianForm();

            // populate
            Guardian guardian = PlayerManager.Player.Guardians.First(i => i.EditToken == guardianEditToken);
            txtGuardianFirstName.Text = guardian.Person.FName;
            txtGuardianMiddleInitial.Text = guardian.Person.MInitial.HasValue ? guardian.Person.MInitial.Value.ToString() : "";
            txtGuardianLastName.Text = guardian.Person.LName;

            pnlAddGuardian.Visible = true;
            lnkAddGuardian.Visible = false;
        }

        protected void gvRegRules_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid editToken = (Guid)gvGuardians.DataKeys[e.NewEditIndex].Value;
            Populate_GuardianEdit(editToken);
            gvGuardians.EditIndex = e.NewEditIndex;
            gvGuardians.DataBind();
            UpdateGuardianTabs();
        }

        protected void gvRegRules_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvGuardians.EditIndex = -1;
            gvGuardians.DataBind();
            UpdateGuardianTabs();
        }

        public void gvRegRules_DeleteItem(Guid editToken)
        {
            ClearGuardianForm();
            PlayerManager.DeleteGuardian(editToken);
            gvGuardians.DataBind();
        }

        protected void lnkAddRegistrationRule_Click(object sender, EventArgs e)
        {
            ClearGuardianForm();
            pnlAddGuardian.Visible = true;
            lnkAddGuardian.Visible = false;
            gvGuardians.EditIndex = -1;
            gvGuardians.DataBind();
            UpdateGuardianTabs();
        }

        protected void lnkSaveRegistrationRule_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid || HasGuardianMatches())
            {
                return;
            }

            SaveGuardian_Step1();
        }

        private void SaveRegistrationRule_Step1()
        {
            Guid? editToken = gvGuardians.EditIndex != -1 ? (Guid)gvGuardians.DataKeys[gvGuardians.EditIndex].Value : (Guid?)null;
            editToken = SaveGuardian(editToken);
            gvGuardians.DataBind();

            for (int i = 0; i < gvGuardians.DataKeys.Count; i++)
            {
                if (editToken.Value == (Guid)gvGuardians.DataKeys[i].Value)
                {
                    gvGuardians.EditIndex = i;
                    break;
                }
            }

            if (!editToken.HasValue)
            {
                ClearGuardianForm();
            }
            else
            {
                UpdateGuardianTabs();
            }

            lnkGuardianTab_Click(lnkGuardianAddress, new EventArgs() { });
        }

        protected void lnkAddEditRegistrationRuleClose_Click(object sender, EventArgs e)
        {
            AddressBook.Reset();
            mvGuardianTabs.ActiveViewIndex = 0;
            pnlAddGuardian.Visible = false;
            lnkAddGuardian.Visible = true;
            gvGuardians.EditIndex = -1;
            gvGuardians.DataBind();
            UpdateGuardianTabs();
        }

        protected void lnkSelectRegistrationRule_Click(object sender, EventArgs e)
        {
            LinkButton button = (LinkButton)sender;
            int personID = int.Parse(button.CommandArgument);

            PlayerManager.AddGuardians(new List<int>() { personID });

            pnlPossibleGuardianMatches.Visible = false;
            gvGuardians.EditIndex = -1;
            gvGuardians.DataBind();
            ClearGuardianForm();
        }
        #endregion

        // The return type can be changed to IEnumerable, however to support
        // paging and sorting, the following parameters must be added:
        //     int maximumRows
        //     int startRowIndex
        //     out int totalRowCount
        //     string sortByExpression
        public IQueryable gvRegRules_GetData1()
        {
            return null;
        }
    }
}