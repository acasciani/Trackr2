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
    public partial class PlayerManagement : WizardBase<int>
    {
        private int ClubID = 1;

        public string CreatePermission { get; set; }
        public string EditPermission { get; set; }

        private byte[] OldPlayerPicture
        {
            get { return Session["OldPlayerPicture"] as byte[]; }
            set { Session["OldPlayerPicture"] = value; }
        }
        private byte[] NewPlayerPicture
        {
            get { return Session["NewPlayerPicture"] as byte[]; }
            set { Session["NewPlayerPicture"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            using (PlayersController pc = new PlayersController())
            using (WebUsersController wuc = new WebUsersController())
            {
                if (IsNew)
                {
                    if (!wuc.IsAllowed(CurrentUser.UserID, string.IsNullOrWhiteSpace(CreatePermission) ? Permissions.PlayerManagement.CreatePlayer : CreatePermission))
                    {
                        throw new UserUnauthorizedException("You do not have permission to create a new player.");
                    }
                }
                else
                {
                    if (pc.GetScopedEntity(CurrentUser.UserID, string.IsNullOrWhiteSpace(EditPermission) ? Permissions.PlayerManagement.EditPlayer : EditPermission, PrimaryKey.Value) == null)
                    {
                        throw new UserNotScopedException("You are not allowed to edit the selected player.");
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AlertBox.HideStatus();

            AddressBook_Player.GetData = GetPlayerAddresses;
            AddressBook.GetData = GetGuardianAddresses;

            PhoneNumberBook_Player.GetData = GetPlayerPhoneNumberes;
            PhoneNumberBook.GetData = GetGuardianPhoneNumberes;

            EmailAddressBook_Player.GetData = GetPlayerEmailAddresses;
            EmailBook.GetData = GetGuardianEmailAddresses;

            if (IsPostBack)
            {
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
                UpdatePlayerTabs();
            }
            else
            {
                Populate_Edit();
                UpdatePlayerTabs();
            }

            Player player = PlayerManager.Player;

            // populate player's contact books
            AddressBook_Player.Reset();
            AddressBook_Player.PersonEditToken = player.Person.EditToken;

            EmailAddressBook_Player.Reset();
            EmailAddressBook_Player.PersonEditToken = player.Person.EditToken;

            PhoneNumberBook_Player.Reset();
            PhoneNumberBook_Player.PersonEditToken = player.Person.EditToken;
        }

        private void Populate_Create()
        {
            PlayerManager.CreatePlayer(ClubID);

            OldPlayerPicture = null;
            NewPlayerPicture = null;
            pnlPossiblePlayerMatches.Visible = false;
            PlayerWizard.ActiveStepIndex = 0;
        }

        private void Populate_Edit()
        {
            using (PlayersController pc = new PlayersController())
            {
                PlayerManager.EditPlayer(pc.GetScopedEntity(CurrentUser.UserID, (WasNew ? (string.IsNullOrWhiteSpace(CreatePermission) ? Permissions.PlayerManagement.CreatePlayer : CreatePermission) : (string.IsNullOrWhiteSpace(EditPermission) ? Permissions.PlayerManagement.EditPlayer : EditPermission)), PrimaryKey.Value).PlayerID);

                Player player = PlayerManager.Player;

                txtFirstName.Text = player.Person.FName;
                txtLastName.Text = player.Person.LName;
                txtMiddleInitial.Text = player.Person.MInitial.HasValue ? player.Person.MInitial.Value.ToString() : "";

                txtDateOfBirth.Text = player.Person.DateOfBirth.HasValue ? player.Person.DateOfBirth.Value.ToString("yyyy-MM-dd") : "";

                divPreview.Visible = false;
                pnlPossiblePlayerMatches.Visible = false;

                // Reset views
                PlayerWizard.ActiveStepIndex = 0;
            }
        }

        protected void lnkEditAgain_Click(object sender, EventArgs e)
        {
            PlayerWizard.ActiveStepIndex = 0;
        }

        private void Save_Step1()
        {
            PlayerManager.UpdatePerson(PlayerManager.Player.Person.EditToken, txtFirstName.Text, txtLastName.Text, DateTime.Parse(txtDateOfBirth.Text));
        }

        protected void lnkPlayerTab_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            int selectedTabIndex;
            if (int.TryParse(btn.CommandArgument, out selectedTabIndex))
            {
                mvPlayerInfoTabs.ActiveViewIndex = selectedTabIndex;
                UpdatePlayerTabs();
            }
        }

        private void UpdatePlayerTabs()
        {
            Player player = PlayerManager.Player;

            bool canAdvance = player.Person.FName != null && player.Person.LName != null && player.Person.DateOfBirth.HasValue;

            lnkPlayerGeneral.Enabled = mvPlayerInfoTabs.ActiveViewIndex != 0;
            lnkPlayerAddress.Enabled = canAdvance && mvPlayerInfoTabs.ActiveViewIndex != 1;
            lnkPlayerEmails.Enabled = canAdvance && mvPlayerInfoTabs.ActiveViewIndex != 2;
            lnkPlayerPhones.Enabled = canAdvance && mvPlayerInfoTabs.ActiveViewIndex != 3;
        }

        protected void Step1_Info_Activate(object sender, EventArgs e)
        {
            AddressBook_Player.HideForm();
            EmailAddressBook_Player.HideForm();
            PhoneNumberBook_Player.HideForm();
            mvPlayerInfoTabs.ActiveViewIndex = 0;
            UpdatePlayerTabs();
        }

        protected void lnkSavePlayer_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid || HasPlayerMatches())
            {
                return;
            }

            Save_Step1();
            UpdatePlayerTabs();
            lnkPlayerTab_Click(lnkPlayerAddress, null);
        }

        protected void lnkContinueAnywaysPlayer_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            Save_Step1();
            UpdatePlayerTabs();
        }

        protected void PlayerWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
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
                        Save_Step1();
                    }
                    break;

                default: break;
            }
        }

        protected void validatorDateTimeParses_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime date;
            args.IsValid = DateTime.TryParse(args.Value, out date) && new DateTime(1900, 1, 1) <= date && date <= new DateTime(2200, 1, 1);
        }

        protected void PlayerWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }

            try
            {
                PlayerManager.SaveData(CurrentUser.UserID);
                AlertBox.AddAlert("Successfully saved changes.");
            }
            catch (PlayerModifiedByAnotherProcessException)
            {
                AlertBox.AddAlert("Unable to save changes. This player was modified by someone else before you committed your changes. Please reload the page and try again.", false, UI.AlertBoxType.Error);
            }
            catch (Exception ex)
            {
                Page.Master.HandleException(ex);
            }
        }

        private bool HasPlayerMatches()
        {
            // now check for any possible duplicate players (only when creating a new player)
            if (IsNew)
            {
                using (PlayersController pc = new PlayersController())
                {
                    string fName = txtFirstName.Text.Trim();
                    string lName = txtLastName.Text.Trim();
                    char? mInitial = string.IsNullOrWhiteSpace(txtMiddleInitial.Text) ? (char?)null : txtMiddleInitial.Text.ToCharArray()[0];

                    var matches = pc.GetPossibleMatches(1, fName, lName, mInitial, DateTime.Parse(txtDateOfBirth.Text))
                        .GroupBy(i => new { i.PlayerID, i.DOB_Distance, i.FirstName_Distance, i.LastName_Distance })
                        .OrderBy(i => i.Key.DOB_Distance).ThenBy(i => i.Key.LastName_Distance).ThenBy(i => i.Key.FirstName_Distance)
                        .Select(i => new
                        {
                            PlayerID = i.Key.PlayerID,
                            FirstName = i.First().FirstName,
                            LastName = i.First().LastName,
                            DateOfBirth = i.First().DateOfBirth,
                            Teams = i.Select(j => new { Name = j.TeamName, Year = j.TeamStart.HasValue ? j.TeamStart.Value.Year : DateTime.MinValue.Year }).OrderByDescending(j => j.Year).ThenBy(j => j.Name)
                        });

                    if (matches.Count() > 0)
                    {
                        gvPossiblePlayerMatches.DataSource = matches;
                        gvPossiblePlayerMatches.DataBind();
                        pnlPossiblePlayerMatches.Visible = true;
                        return true;
                    }
                }
            }

            return false;
        }




        #region Team Administration
        public IQueryable gvTeamAssignments_GetData()
        {
            /*
            using (TeamPlayersController tpc = new TeamPlayersController())
            {
                FetchStrategy fetch = new FetchStrategy();
                fetch.LoadWith<TeamPlayer>(i => i.PlayerPass);
                fetch.LoadWith<TeamPlayer>(i => i.Team);
                fetch.LoadWith<Team>(i => i.Program);

                TeamPlayers = tpc.GetWhere(i => (i.PlayerPassID.HasValue && i.PlayerPass.PlayerID == PrimaryKey.Value) || (!i.PlayerPassID.HasValue && i.PlayerID == PrimaryKey.Value), fetch).ToList();

                return TeamPlayers.Select(i => new
                {
                    TeamName = i.Team.TeamName,
                    ProgramName = i.Team.Program.ProgramName,
                    Season = string.Format("{0:yyyy} - {1:yy}", i.Team.StartYear, i.Team.EndYear),
                    IsSecondary = i.IsSecondary,
                    StartYear = i.Team.StartYear,
                    IsRemovable = DateTime.Now.ToUniversalTime() < i.Team.EndYear.ToUniversalTime(),
                    PlayerPassNumber = i.PlayerPassID.HasValue && !string.IsNullOrWhiteSpace(i.PlayerPass.PassNumber) ? i.PlayerPass.PassNumber : "",
                    TeamPlayerID = i.TeamPlayerID
                }).OrderByDescending(i => i.StartYear).ThenBy(i => i.ProgramName).ThenBy(i => i.TeamName).AsQueryable();
            }*/
            return null;
        }

        private void ClearTeamPlayerForm()
        {
            ptpPicker.SelectedProgramID = null;
            ptpPicker.SelectedTeamID = null;
            ptpPicker.SelectedPlayerID = null;
            ptpPicker.Populate();
            pnlAddEditTeamPlayer.Visible = false;
            ddlPlayerPassForTeam.Enabled = true;
        }

        protected void gvTeamAssignments_RowEditing(object sender, GridViewEditEventArgs e)
        {
           // Populate_TeamPlayerEdit(TeamPlayers[e.NewEditIndex].TeamPlayerID);
            gvTeamAssignments.EditIndex = e.NewEditIndex;
            gvTeamAssignments.DataBind();
        }

        protected void gvTeamAssignments_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvTeamAssignments.EditIndex = -1;
            gvTeamAssignments.DataBind();
        }

        private void SaveTeamPlayer(int? teamPlayerID)
        {
            using (TeamPlayersController tpc = new TeamPlayersController())
            {
                TeamPlayer teamPlayer = teamPlayerID.HasValue ? tpc.Get(teamPlayerID.Value) : new TeamPlayer();

                teamPlayer.IsSecondary = chkIsSecondary.Checked;
                teamPlayer.TeamID = ptpPicker.SelectedTeamID.Value;

                int playerPassID;
                // if there is a valid player pass id selected, then use that otherwise use player id. there should never be both specified
                if (int.TryParse(ddlPlayerPassForTeam.SelectedValue, out playerPassID))
                {
                    teamPlayer.PlayerPassID = playerPassID;
                    teamPlayer.PlayerID = null;
                }
                else
                {
                    teamPlayer.PlayerPassID = null;
                    teamPlayer.PlayerID = PrimaryKey.Value;
                }

                if (!teamPlayerID.HasValue)
                {
                    tpc.AddNew(teamPlayer);
                    AlertBox.SetStatus("Successfully saved new team assignment.");
                }
                else
                {
                    tpc.Update(teamPlayer);
                    AlertBox.SetStatus("Successfully saved existing team assignment.");
                }

                pnlAddEditTeamPlayer.Visible = false;
                gvTeamAssignments.EditIndex = -1;
                gvTeamAssignments.DataBind();
            }
        }

        private void Populate_TeamPlayerEdit(int teamPlayerID)
        {
            using (TeamPlayersController tpc = new TeamPlayersController())
            {
                FetchStrategy fetch = new FetchStrategy();
                fetch.LoadWith<TeamPlayer>(i => i.Team);

                TeamPlayer player = tpc.GetWhere(i => i.TeamPlayerID == teamPlayerID, fetch).First();
                ClearTeamPlayerForm();

                ddlPlayerPassForTeam.DataBind();

                // populate
                ptpPicker.SelectedProgramID = player.Team.ProgramID;
                ptpPicker.SelectedTeamID = player.TeamID;
                ptpPicker.Populate();

                chkIsSecondary.Checked = player.IsSecondary;

                if (player.PlayerPassID.HasValue)
                {
                    ddlPlayerPassForTeam.SelectedValue = player.PlayerPassID.Value.ToString();
                }

                pnlAddEditTeamPlayer.Visible = true;
            }
        }

        protected void lnkSaveTeamPlayer_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

           // int? teamPlayerID = gvTeamAssignments.EditIndex != -1 ? TeamPlayers[gvTeamAssignments.EditIndex].TeamPlayerID : (int?)null;
           // SaveTeamPlayer(teamPlayerID);
        }

        protected void lnkAddTeamPlayer_Click(object sender, EventArgs e)
        {
            ClearTeamPlayerForm();
            pnlAddEditTeamPlayer.Visible = true;
            gvTeamAssignments.EditIndex = -1;
            gvTeamAssignments.DataBind();
        }

        public void gvTeamAssignmentss_DeleteItem(int? TeamPlayerID)
        {
            ClearTeamPlayerForm();

            if (!TeamPlayerID.HasValue)
            {
                return;
            }
            using (TeamPlayersController tpc = new TeamPlayersController())
            {
                tpc.Delete(TeamPlayerID.Value);
                AlertBox.SetStatus("Successfully deleted team assignment.");
            }

            gvTeamAssignments.DataBind();
        }

        protected void validatorIsTeamSelected_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ptpPicker.SelectedTeamID.HasValue;
        }

        public IQueryable ddlPlayerPassForTeam_GetData()
        {
            // get any player passes
            using (PlayerPassesController ppc = new PlayerPassesController())
            {
                return ppc.GetWhere(i => i.PlayerID == PrimaryKey.Value && i.PassNumber != null && i.PassNumber.Trim() != "").Select(i => new
                {
                    Label = string.Format("{0} - Expires: {1:MM/dd/yyyy}", i.PassNumber, i.Expires),
                    Value = i.PlayerPassID,
                    Expires = i.Expires
                }).OrderByDescending(i => i.Expires).AsQueryable();
            }
        }
        #endregion


        #region Pass Administration
        private void ClearPlayerPassForm()
        {
            OldPlayerPicture = null;
            NewPlayerPicture = null;
            txtPassExpires.Text = "";
            txtPassNumber.Text = "";
            divPreview.Visible = false;
            pnlAddEditPass.Visible = false;
            txtPassExpires.ReadOnly = false;
            txtPassNumber.ReadOnly = false;
            pnlPhotoUpload.Visible = true;
            validatorPlayerPassExpiresDuplicate.Enabled = true;
            validatorPlayerPassExpiresRequired.Enabled = true;
            validatorPlayerPassExpiresValid.Enabled = true;
        }

        private void SetPreviewImage(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                imgUploadPreview.ImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String(data);
            }
            divPreview.Visible = data != null && data.Length > 0;
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            byte[] array = uploadPlayerPass.FileBytes;

            if (array != null)
            {
                NewPlayerPicture = array;
                SetPreviewImage(array);
            }
        }

        protected void lnkReloadImage_Click(object sender, EventArgs e)
        {
            NewPlayerPicture = OldPlayerPicture;
            SetPreviewImage(NewPlayerPicture);
        }

        public IQueryable gvPlayerPasses_GetData()
        {
            return PlayerManager.Player.PlayerPasses.Where(i => i.Active).Select(i => new
            {
                Expiration = i.Expires,
                PassNumber = i.PassNumber,
                PlayerPassID = i.PlayerPassID,
                Editable = DateTime.Today.ToUniversalTime() < i.Expires,
            }).OrderByDescending(i => i.Expiration).AsQueryable();
        }

        protected void gvPlayerPasses_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //Populate_PlayerPassEdit(PlayerPasses[e.NewEditIndex].PlayerPassID);
            gvPlayerPasses.EditIndex = e.NewEditIndex;
            gvPlayerPasses.DataBind();
        }

        protected void gvPlayerPasses_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPlayerPasses.EditIndex = -1;
            gvPlayerPasses.DataBind();
        }

        private Guid SavePlayerPass(Guid? editToken)
        {
            if (!editToken.HasValue)
            {
                editToken = PlayerManager.AddPlayerPass();
            }

            PlayerManager.UpdatePlayerPass(editToken.Value, (string.IsNullOrWhiteSpace(txtPassNumber.Text) ? null : txtPassNumber.Text), DateTime.Parse(txtPassExpires.Text));
            PlayerManager.UpdatePlayerPassPhoto(editToken.Value, (NewPlayerPicture == null || NewPlayerPicture.Length == 0 ? null : NewPlayerPicture));

            NewPlayerPicture = null;
            OldPlayerPicture = null;
            pnlAddEditPass.Visible = false;
            gvPlayerPasses.EditIndex = -1;
            gvPlayerPasses.DataBind();

            return editToken.Value;
        }

        private void Populate_PlayerPassEdit(Guid editToken)
        {
            ClearPlayerPassForm();

            // populate
            PlayerPass playerPass = PlayerManager.Player.PlayerPasses.First(i => i.EditToken == editToken);

            txtPassExpires.Text = playerPass.Expires.ToString("yyyy-MM-dd");
            txtPassNumber.Text = playerPass.PassNumber;
            OldPlayerPicture = playerPass.Photo;
            NewPlayerPicture = playerPass.Photo;

            if (playerPass.Photo != null && playerPass.Photo.Count() > 0)
            {
                SetPreviewImage(playerPass.Photo);
            }

            pnlAddEditPass.Visible = true;
        }

        protected void lnkSavePlayerPass_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            Guid? editToken = gvPlayerPasses.EditIndex != -1 ? (Guid)gvPlayerPasses.DataKeys[gvPlayerPasses.EditIndex].Value : (Guid?)null;
            editToken = SavePlayerPass(editToken);
            gvPlayerPasses.DataBind();
        }

        protected void lnkAddPlayerPass_Click(object sender, EventArgs e)
        {
            ClearPlayerPassForm();
            pnlAddEditPass.Visible = true;
            gvPlayerPasses.EditIndex = -1;
            gvPlayerPasses.DataBind();
        }

        protected void lnkViewPlayerPass_Click(object sender, EventArgs e)
        {
            Guid playerPassEditToken;
            if (Guid.TryParse(((LinkButton)sender).CommandArgument, out playerPassEditToken))
            {
                Populate_PlayerPassEdit(playerPassEditToken);
                txtPassExpires.ReadOnly = true;
                txtPassNumber.ReadOnly = true;
                pnlPhotoUpload.Visible = false;
                validatorPlayerPassExpiresDuplicate.Enabled = false;
                validatorPlayerPassExpiresRequired.Enabled = false;
                validatorPlayerPassExpiresValid.Enabled = false;
            }
        }

        protected void validatorPlayerPassExpiresDuplicate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime exp;
            if (DateTime.TryParse(args.Value, out exp))
            {
                Guid? editToken = gvPlayerPasses.EditIndex != -1 ? (Guid)gvPlayerPasses.DataKeys[gvPlayerPasses.EditIndex].Value : (Guid?)null;

                if (!editToken.HasValue)
                {
                    args.IsValid = PlayerManager.Player.PlayerPasses.Count(i => i.Expires == exp) == 0;
                }
                else
                {
                    args.IsValid = PlayerManager.Player.PlayerPasses.Count(i => i.Expires == exp && i.EditToken != editToken.Value) == 0;
                }
            }
        }

        // The id parameter name should match the DataKeyNames value set on the control
        public void gvPlayerPasses_DeleteItem(Guid editToken)
        {
            ClearPlayerPassForm();
            PlayerManager.DeletePlayerPass(editToken);
            gvPlayerPasses.DataBind();
        }
        #endregion


        #region Guardian Administration
        public IQueryable gvGuardians_GetData()
        {
            return PlayerManager.Player.Guardians.Where(i => i.Active).Select(i => new
            {
                Guardian = i.Person.FName + " " + i.Person.LName,
                IsRemovable = true,
                GuardianID = i.GuardianID,
                EditToken = i.EditToken,
                PersonEditToken = i.Person.EditToken
            }).OrderByDescending(i => i.Guardian).AsQueryable();
        }

        private void ClearGuardianForm()
        {
            txtGuardianFirstName.Text = null;
            txtGuardianMiddleInitial.Text = null;
            txtGuardianLastName.Text = null;
            AddressBook.Reset();
            EmailBook.Reset();
            PhoneNumberBook.Reset();
            mvGuardianTabs.ActiveViewIndex = 0;
            pnlAddGuardian.Visible = false;
            UpdateGuardianTabs();
        }

        private Guid SaveGuardian(Guid? editToken)
        {
            if (!editToken.HasValue)
            {
                editToken = PlayerManager.AddGuardian(ClubID);
            }

            Guid playerEditToken = PlayerManager.Player.Guardians.First(i => i.EditToken == editToken.Value).Person.EditToken;

            PlayerManager.UpdatePerson(playerEditToken, txtGuardianFirstName.Text, txtGuardianLastName.Text, null);

            lnkAddGuardian.Visible = true;

            return editToken.Value;
        }

        private void Populate_GuardianEdit(Guid guardianEditToken)
        {
            ClearGuardianForm();

            // populate
            Guardian guardian = PlayerManager.Player.Guardians.First(i => i.EditToken == guardianEditToken);
            txtGuardianFirstName.Text = guardian.Person.FName;
            txtGuardianMiddleInitial.Text = guardian.Person.MInitial.HasValue ? guardian.Person.MInitial.Value.ToString() : "";
            txtGuardianLastName.Text = guardian.Person.LName;

            pnlAddGuardian.Visible = true;
        }

        private void Populate_AddressBook(Guid personEditToken)
        {
            AddressBook.Reset();
            AddressBook.PersonEditToken = personEditToken;
            AddressBook.DataBind();
        }

        private void Populate_PhoneNumberBook(Guid personEditToken)
        {
            PhoneNumberBook.Reset();
            PhoneNumberBook.PersonEditToken = personEditToken;
            PhoneNumberBook.DataBind();
        }

        private void Populate_EmailBook(Guid personEditToken)
        {
            EmailBook.Reset();
            EmailBook.PersonEditToken = personEditToken;
            EmailBook.DataBind();
        }

        protected void gvGuardians_RowEditing(object sender, GridViewEditEventArgs e)
        {
            lnkAddGuardian.Visible = false;
            Guid editToken = (Guid)gvGuardians.DataKeys[e.NewEditIndex].Value;
            Populate_GuardianEdit(editToken);
            gvGuardians.EditIndex = e.NewEditIndex;
            gvGuardians.DataBind();
            UpdateGuardianTabs();
        }

        protected void gvGuardians_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            lnkAddGuardian.Visible = true;
            gvGuardians.EditIndex = -1;
            gvGuardians.DataBind();
            UpdateGuardianTabs();
        }

        public void gvGuardians_DeleteItem(Guid editToken)
        {
            ClearGuardianForm();
            PlayerManager.DeleteGuardian(editToken);
            gvGuardians.DataBind();
        }

        protected void lnkAddGuardian_Click(object sender, EventArgs e)
        {
            ClearGuardianForm();
            pnlAddGuardian.Visible = true;
            lnkAddGuardian.Visible = false;
            gvGuardians.EditIndex = -1;
            gvGuardians.DataBind();
            UpdateGuardianTabs();
        }

        protected void lnkSaveGuardian_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

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

        protected void lnkGuardianTab_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            int selectedTabIndex;
            if (int.TryParse(btn.CommandArgument, out selectedTabIndex))
            {
                mvGuardianTabs.ActiveViewIndex = selectedTabIndex;
                UpdateGuardianTabs();
                AddressBook.Reset();

                Guardian guardian = PlayerManager.Player.Guardians.First(i => i.EditToken == (Guid)gvGuardians.DataKeys[gvGuardians.EditIndex].Value);

                switch(selectedTabIndex)
                {
                    case 1: // populate address book
                        Populate_AddressBook(guardian.Person.EditToken);
                        break;

                    case 2: // populate email book
                        Populate_EmailBook(guardian.Person.EditToken);
                        break;

                    case 3: // populate phone book
                        Populate_PhoneNumberBook(guardian.Person.EditToken);
                        break;

                    default: break;
                }
            }
        }

        private void UpdateGuardianTabs()
        {
            lnkGuardianGeneral.Enabled = mvGuardianTabs.ActiveViewIndex != 0;
            lnkGuardianAddress.Enabled = gvGuardians.EditIndex > -1 && mvGuardianTabs.ActiveViewIndex != 1;
            lnkGuardianEmails.Enabled = gvGuardians.EditIndex > -1 && mvGuardianTabs.ActiveViewIndex != 2;
            lnkGuardianPhones.Enabled = gvGuardians.EditIndex > -1 && mvGuardianTabs.ActiveViewIndex != 3;
        }

        protected void lnkAddEditGuardianClose_Click(object sender, EventArgs e)
        {
            AddressBook.Reset();
            mvGuardianTabs.ActiveViewIndex = 0;
            pnlAddGuardian.Visible = false;
            gvGuardians.EditIndex = -1;
            gvGuardians.DataBind();
            UpdateGuardianTabs();
            lnkAddGuardian.Visible = true;
        }
        #endregion




        #region Get Sub-widget Data Values
        public IList<Address> GetPlayerAddresses(Guid personEditToken)
        {
            return PlayerManager.Player.Person.Addresses;
        }

        public IList<Address> GetGuardianAddresses(Guid personEditToken)
        {
            Guardian guardian = PlayerManager.Player.Guardians.First(i => i.Person.EditToken == personEditToken);
            return guardian.Person.Addresses;
        }

        public IList<PhoneNumber> GetPlayerPhoneNumberes(Guid personEditToken)
        {
            return PlayerManager.Player.Person.PhoneNumbers;
        }

        public IList<PhoneNumber> GetGuardianPhoneNumberes(Guid personEditToken)
        {
            Guardian guardian = PlayerManager.Player.Guardians.First(i => i.Person.EditToken == personEditToken);
            return guardian.Person.PhoneNumbers;
        }

        public IList<EmailAddress> GetPlayerEmailAddresses(Guid personEditToken)
        {
            return PlayerManager.Player.Person.EmailAddresses;
        }

        public IList<EmailAddress> GetGuardianEmailAddresses(Guid personEditToken)
        {
            Guardian guardian = PlayerManager.Player.Guardians.First(i => i.Person.EditToken == personEditToken);
            return guardian.Person.EmailAddresses;
        }
        #endregion
    }
}