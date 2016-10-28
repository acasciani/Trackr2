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

            if (IsPostBack)
            {
                /*
                if (PlayerManager.Player == null)
                {
                    Response.Redirect(Request.Url.AbsoluteUri, true);
                }
                 */

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
                UpdateTabs();
            }
            else
            {
                Populate_Edit();
                UpdateTabs();
            }
        }

        private void Populate_Create()
        {
            TeamManager.CreateTeam();

            Team team = TeamManager.Team;

            /*
            txtFirstName.Text = player.Person.FName;
            txtLastName.Text = player.Person.LName;
            txtMiddleInitial.Text = player.Person.MInitial.HasValue ? player.Person.MInitial.Value.ToString() : "";

            txtDateOfBirth.Text = player.Person.DateOfBirth.HasValue ? player.Person.DateOfBirth.Value.ToString("yyyy-MM-dd") : "";

            divPreview.Visible = false;

            OldPlayerPicture = null;
            NewPlayerPicture = null;
            pnlPossiblePlayerMatches.Visible = false;
            PlayerWizard.ActiveStepIndex = 0;

            // add default guardians
            if (AddThesePeopleAsDefaultGuardians != null && AddThesePeopleAsDefaultGuardians.Count() > 0)
            {
                PlayerManager.AddGuardians(AddThesePeopleAsDefaultGuardians);
            }
             */
            
        }

        private void Populate_Edit()
        {
            using (TeamsController tc = new TeamsController())
            {
                TeamManager.EditTeam(tc.GetScopedEntity(CurrentUser.UserID, (WasNew ? (string.IsNullOrWhiteSpace(CreatePermission) ? Permissions.TeamManagement.CreateTeam : CreatePermission) : (string.IsNullOrWhiteSpace(EditPermission) ? Permissions.TeamManagement.EditTeam : EditPermission)), PrimaryKey.Value).TeamID);

                Player player = PlayerManager.Player;

                /*
                txtFirstName.Text = player.Person.FName;
                txtLastName.Text = player.Person.LName;
                txtMiddleInitial.Text = player.Person.MInitial.HasValue ? player.Person.MInitial.Value.ToString() : "";

                txtDateOfBirth.Text = player.Person.DateOfBirth.HasValue ? player.Person.DateOfBirth.Value.ToString("yyyy-MM-dd") : "";

                divPreview.Visible = false;
                pnlPossiblePlayerMatches.Visible = false;

                // Reset views
                PlayerWizard.ActiveStepIndex = 0;*/
            }
        }

        private void Save_Step1()
        {
            /*
            char? mInitial = string.IsNullOrWhiteSpace(txtMiddleInitial.Text) ? (char?)null : txtMiddleInitial.Text[0];
            PlayerManager.UpdatePerson(PlayerManager.Player.Person.EditToken, txtFirstName.Text, txtLastName.Text, mInitial, DateTime.Parse(txtDateOfBirth.Text));
             */
        }

        protected void lnkTeamTab_Click(object sender, EventArgs e)
        {
            /*
            LinkButton btn = (LinkButton)sender;

            int selectedTabIndex;
            if (int.TryParse(btn.CommandArgument, out selectedTabIndex))
            {
                mvPlayerInfoTabs.ActiveViewIndex = selectedTabIndex;
                UpdatePlayerTabs();
            }
             * */
        }

        private void UpdateTabs()
        {/*
            Player player = PlayerManager.Player;

            bool canAdvance = player.Person.FName != null && player.Person.LName != null && player.Person.DateOfBirth.HasValue;

            lnkPlayerGeneral.Enabled = mvPlayerInfoTabs.ActiveViewIndex != 0;
            lnkPlayerAddress.Enabled = canAdvance && mvPlayerInfoTabs.ActiveViewIndex != 1;
            lnkPlayerEmails.Enabled = canAdvance && mvPlayerInfoTabs.ActiveViewIndex != 2;
            lnkPlayerPhones.Enabled = canAdvance && mvPlayerInfoTabs.ActiveViewIndex != 3;
          * */
        }

        protected void Step1_Info_Activate(object sender, EventArgs e)
        {
            /*
            AddressBook_Player.HideForm();
            EmailAddressBook_Player.HideForm();
            PhoneNumberBook_Player.HideForm();
            mvPlayerInfoTabs.ActiveViewIndex = 0;
            UpdatePlayerTabs();
             * */
        }

        protected void lnkSaveTeam_Click(object sender, EventArgs e)
        {
            /*
            if (!Page.IsValid)
            {
                return;
            }

            Save_Step1();
            UpdatePlayerTabs();
            lnkPlayerTab_Click(lnkPlayerAddress, null);
             * */
        }

        protected void lnkContinueAnywaysTeam_Click(object sender, EventArgs e)
        {
            /*
            if (!Page.IsValid)
            {
                return;
            }

            Save_Step1();
            UpdateTabs();
             * */
        }

        protected void TeamWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {/*
            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }

            switch (e.CurrentStepIndex)
            {
                case 0:
                    if (HasPlayerMatches())
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        // iterate through the player info tabs
                        switch (mvPlayerInfoTabs.ActiveViewIndex)
                        {
                            case 0:
                                lnkSavePlayer_Click(sender, null);
                                lnkPlayerTab_Click(lnkPlayerAddress, null);
                                e.Cancel = true;
                                break;

                            case 1:
                                lnkPlayerTab_Click(lnkPlayerEmails, null);
                                e.Cancel = true;
                                break;

                            case 2:
                                lnkPlayerTab_Click(lnkPlayerPhones, null);
                                e.Cancel = true;
                                break;

                            default: break;
                        }
                    }
                    break;

                default: break;
            }*/
        }

        protected void validatorDateTimeParses_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime date;
            args.IsValid = DateTime.TryParse(args.Value, out date) && new DateTime(1900, 1, 1) <= date && date <= new DateTime(2200, 1, 1);
        }

        protected void TeamWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            /*
            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }

            try
            {
                bool updateTeamPasses;
                if (WasNew)
                {
                    updateTeamPasses = string.IsNullOrWhiteSpace(CreatePermission) || CreatePermission == Permissions.PlayerManagement.CreatePlayer;
                }
                else
                {
                    updateTeamPasses = string.IsNullOrWhiteSpace(EditPermission) || EditPermission == Permissions.PlayerManagement.EditPlayer;
                }

                int playerID = PlayerManager.SaveData(CurrentUser.UserID, updateTeamPasses);

                if (PlayerSavedSuccess != null)
                {
                    PlayerSavedSuccess(null, new PlayerSavedEventArgs() { PlayerID = playerID });
                }
                else
                {
                    AlertBox.AddAlert("Successfully saved changes.");
                }
            }
            catch (PlayerModifiedByAnotherProcessException ex)
            {
                AlertBox.AddAlert("Unable to save changes. This player was modified by someone else before you committed your changes. Please reload the page and try again.", false, UI.AlertBoxType.Error);

                if (PlayerSavedError != null)
                {
                    PlayerSavedError(null, new PlayerSavedEventArgs() { PlayerID = PlayerManager.Player.PlayerID == 0 ? (int?)null : PlayerManager.Player.PlayerID });
                }
            }
            catch (Exception ex)
            {
                Page.Master.HandleException(ex);

                if (PlayerSavedError != null)
                {
                    PlayerSavedError(null, null);
                }
            }*/
        }






    }
}