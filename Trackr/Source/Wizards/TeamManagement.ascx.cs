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

                // now try to save registration rules
                try
                {
                    TeamManager.SaveRegistrationRules(teamID);

                    if (TeamSavedSuccess != null)
                    {
                        TeamSavedSuccess(null, new TeamSavedEventArgs() { TeamID = teamID });
                    }
                    else
                    {
                        AlertBox.AddAlert("Successfully saved changes.");
                    }
                }
                catch (Exception ex)
                {
                    AlertBox.AddAlert("Successfully saved basic information but failed to save registration rules.", false, UI.AlertBoxType.Error);

                    if (TeamSavedError != null)
                    {
                        TeamSavedError(null, new TeamSavedEventArgs() { TeamID = teamID });
                    }

                    Page.Master.HandleException(ex);
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

            return TeamManager.RegistrationRules.Where(i => i.Active).Select(i => new
            {
                TeamName = i.OldTeam == null ? "" : i.OldTeam.TeamName,
                IsRemovable = true,
                RegistrationRuleID = i.RegistrationRuleID,
                EditToken = i.EditToken
            }).OrderByDescending(i => i.TeamName).AsQueryable();
        }

        private void ClearRegistrationRuleForm()
        {
            ptpPicker_PreviousTeam.SelectedProgramID = null;
            ptpPicker_PreviousTeam.SelectedTeamID = null;
            ptpPicker_PreviousTeam.Populate();
            txtRegistrationOpenFrom.Text = "";
            txtRegistrationOpenTo.Text = "";
            txtAgeCutoff.Text = "";
        }

        private Guid SaveRegistrationRule(Guid? editToken)
        {
            if (!editToken.HasValue)
            {
                editToken = TeamManager.AddRegistrationRule();
            }

            TeamManager.UpdateRegistrationRule(editToken.Value, ptpPicker_PreviousTeam.SelectedTeamID, DateTime.Parse(txtRegistrationOpenFrom.Text), DateTime.Parse(txtRegistrationOpenTo.Text), string.IsNullOrWhiteSpace(txtAgeCutoff.Text) ? (DateTime?)null : DateTime.Parse(txtAgeCutoff.Text));

            return editToken.Value;
        }

        private void Populate_RegistrationRuleEdit(Guid registrationRuleEditToken)
        {
            ClearRegistrationRuleForm();

            // populate
            RegistrationRule rule = TeamManager.RegistrationRules.First(i => i.EditToken == registrationRuleEditToken);

            txtRegistrationOpenFrom.Text = rule.RegistrationOpens.ToString("yyyy-MM-dd");
            txtRegistrationOpenTo.Text = rule.RegistrationCloses.ToString("yyyy-MM-dd");
            txtAgeCutoff.Text = rule.DateOfBirthCutoff.HasValue ? rule.DateOfBirthCutoff.Value.ToString("yyyy-MM-dd") : "";

            // set picker
            if (rule.OldTeamID.HasValue)
            {
                ptpPicker_PreviousTeam.SelectedProgramID = rule.OldTeam.ProgramID;
                ptpPicker_PreviousTeam.SelectedTeamID = rule.OldTeamID;
                ptpPicker_PreviousTeam.Populate();
            }

            pnlAddRegistrationRule.Visible = true;
            lnkAddRegistrationRule.Visible = false;
        }

        protected void gvRegRules_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid editToken = (Guid)gvRegRules.DataKeys[e.NewEditIndex].Value;
            Populate_RegistrationRuleEdit(editToken);
            gvRegRules.EditIndex = e.NewEditIndex;
            gvRegRules.DataBind();
        }

        protected void gvRegRules_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvRegRules.EditIndex = -1;
            gvRegRules.DataBind();
        }

        public void gvRegRules_DeleteItem(Guid editToken)
        {
            ClearRegistrationRuleForm();
            TeamManager.DeleteRegistrationRule(editToken);
            gvRegRules.DataBind();
            pnlAddRegistrationRule.Visible = false;
            lnkAddRegistrationRule.Visible = true;
        }

        protected void lnkAddRegistrationRule_Click(object sender, EventArgs e)
        {
            ClearRegistrationRuleForm();
            pnlAddRegistrationRule.Visible = true;
            lnkAddRegistrationRule.Visible = false;
            gvRegRules.EditIndex = -1;
            gvRegRules.DataBind();
        }

        protected void lnkSaveRegistrationRule_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            SaveRegistrationRule();
            pnlAddRegistrationRule.Visible = false;
            lnkAddRegistrationRule.Visible = true;
        }

        private void SaveRegistrationRule()
        {
            Guid? editToken = gvRegRules.EditIndex != -1 ? (Guid)gvRegRules.DataKeys[gvRegRules.EditIndex].Value : (Guid?)null;
            editToken = SaveRegistrationRule(editToken);
            gvRegRules.DataBind();

            for (int i = 0; i < gvRegRules.DataKeys.Count; i++)
            {
                if (editToken.Value == (Guid)gvRegRules.DataKeys[i].Value)
                {
                    gvRegRules.EditIndex = i;
                    break;
                }
            }

            if (!editToken.HasValue)
            {
                ClearRegistrationRuleForm();
            }
        }

        protected void lnkAddEditRegistrationRuleClose_Click(object sender, EventArgs e)
        {
            pnlAddRegistrationRule.Visible = false;
            lnkAddRegistrationRule.Visible = true;
            gvRegRules.EditIndex = -1;
            gvRegRules.DataBind();
        }
        #endregion
    }
}