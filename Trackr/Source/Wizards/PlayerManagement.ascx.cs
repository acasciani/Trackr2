using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;

namespace Trackr.Source.Wizards
{
    public partial class PlayerManagement : WizardBase<int>
    {
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
        private List<PlayerPass> PlayerPasses
        {
            get { return ViewState["PlayerPasses"] as List<PlayerPass>; }
            set { ViewState["PlayerPasses"] = value; }
        }
        private List<TeamPlayer> TeamPlayers
        {
            get { return ViewState["TeamPlayers"] as List<TeamPlayer>; }
            set { ViewState["TeamPlayers"] = value; }
        }
        private List<Guardian> Guardians
        {
            get { return ViewState["Guardians"] as List<Guardian>; }
            set { ViewState["Guardians"] = value; }
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
                    if (!wuc.IsAllowed(CurrentUser.UserID, Permissions.PlayerManagement.CreatePlayer))
                    {
                        throw new UserUnauthorizedException("You do not have permission to create a new player.");
                    }
                }
                else
                {
                    if (pc.GetScopedEntity(CurrentUser.UserID, Permissions.PlayerManagement.EditPlayer, PrimaryKey.Value) == null)
                    {
                        throw new UserNotScopedException("You are not allowed to edit the selected player.");
                    }
                }
            }
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
                UpdatePlayerTabs();
            }
            else
            {
                Populate_Edit();
                UpdatePlayerTabs();
            }

            // update dropdownlists
            //ddlRole.Populate(DropDownType.Role);
        }

        private void Populate_Create()
        {
            OldPlayerPicture = null;
            NewPlayerPicture = null;
        }

        private void Populate_Edit()
        {
            using (PlayersController pc = new PlayersController())
            {
                FetchStrategy fetch = new FetchStrategy();
                fetch.LoadWith<Player>(i => i.PlayerPasses);
                fetch.LoadWith<PlayerPass>(i => i.TeamPlayers);
                fetch.LoadWith<TeamPlayer>(i => i.Team);
                fetch.LoadWith<PlayerPass>(i => i.Photo);

                Player player = pc.GetScopedEntity(CurrentUser.UserID, (WasNew ? Permissions.PlayerManagement.CreatePlayer : Permissions.PlayerManagement.EditPlayer), PrimaryKey.Value, fetch);
                txtFirstName.Text = player.Person.FName;
                txtLastName.Text = player.Person.LName;
                txtMiddleInitial.Text = player.Person.MInitial.HasValue ? player.Person.MInitial.Value.ToString() : "";

                txtDateOfBirth.Text = player.Person.DateOfBirth.HasValue ? player.Person.DateOfBirth.Value.ToString("yyyy-MM-dd") : "";

                // Load player pass info, note there should only be one there should be a constraint on the player id and expiration date
                PlayerPass playerPass = player.PlayerPasses.Where(i => DateTime.Today.ToUniversalTime() <= i.Expires).FirstOrDefault();

                divPreview.Visible = false;

                // populate player's contact books
                AddressBook_Player.Reset();
                AddressBook_Player.PersonID = player.PersonID;
                AddressBook_Player.DataBind();

                EmailAddressBook_Player.Reset();
                EmailAddressBook_Player.PersonID = player.PersonID;
                EmailAddressBook_Player.DataBind();

                PhoneNumberBook_Player.Reset();
                PhoneNumberBook_Player.PersonID = player.PersonID;
                PhoneNumberBook_Player.DataBind();
            }
        }

        protected void lnkEditAgain_Click(object sender, EventArgs e)
        {
            PlayerWizard.ActiveStepIndex = 0;
        }

        private void Save_Step1()
        {
            using (PlayersController pc = new PlayersController())
            {
                Player player = IsNew ? new Player() : pc.Get(PrimaryKey.Value);

                if (IsNew)
                {
                    player.Person = new Person();
                    player.Person.ClubID = 1;
                }

                player.Person.DateOfBirth = DateTime.Parse(txtDateOfBirth.Text);
                player.Person.FName = txtFirstName.Text;
                player.Person.MInitial = string.IsNullOrWhiteSpace(txtMiddleInitial.Text) ? (char?)null : txtMiddleInitial.Text.ToCharArray()[0];
                player.Person.LName = txtLastName.Text;

                if (IsNew)
                {
                    Player inserted = pc.AddNew(player);
                    PrimaryKey = inserted.PlayerID;
                    Populate_Edit();
                    AlertBox.SetStatus("Successfully created new player.");
                }
                else
                {
                    pc.Update(player);
                    AlertBox.SetStatus("Successfully saved player information.");
                }
            }
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
            lnkPlayerGeneral.Enabled = mvPlayerInfoTabs.ActiveViewIndex != 0;
            lnkPlayerAddress.Enabled = PrimaryKey.HasValue && mvPlayerInfoTabs.ActiveViewIndex != 1;
            lnkPlayerEmails.Enabled = PrimaryKey.HasValue && mvPlayerInfoTabs.ActiveViewIndex != 2;
            lnkPlayerPhones.Enabled = PrimaryKey.HasValue && mvPlayerInfoTabs.ActiveViewIndex != 3;
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
                    Save_Step1();
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
        }




        #region Team Administration
        public IQueryable gvTeamAssignments_GetData()
        {
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
            }
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
            Populate_TeamPlayerEdit(TeamPlayers[e.NewEditIndex].TeamPlayerID);
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

            int? teamPlayerID = gvTeamAssignments.EditIndex != -1 ? TeamPlayers[gvTeamAssignments.EditIndex].TeamPlayerID : (int?)null;
            SaveTeamPlayer(teamPlayerID);
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
            using (PlayerPassesController ppc = new PlayerPassesController())
            {
                PlayerPasses = ppc.GetWhere(i => i.PlayerID == PrimaryKey.Value).OrderByDescending(i => i.Expires).ToList();

                return PlayerPasses.Select(i => new
                {
                    Expiration = i.Expires,
                    PassNumber = i.PassNumber,
                    PlayerPassID = i.PlayerPassID,
                    Editable = DateTime.Today.ToUniversalTime() < i.Expires,
                }).AsQueryable();
            }
        }

        protected void gvPlayerPasses_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Populate_PlayerPassEdit(PlayerPasses[e.NewEditIndex].PlayerPassID);
            gvPlayerPasses.EditIndex = e.NewEditIndex;
            gvPlayerPasses.DataBind();
        }

        protected void gvPlayerPasses_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPlayerPasses.EditIndex = -1;
            gvPlayerPasses.DataBind();
        }

        private void SavePlayerPass(int? playerPassID)
        {
            using (PlayerPassesController ppc = new PlayerPassesController())
            {
                PlayerPass playerPass = playerPassID.HasValue ? ppc.Get(playerPassID.Value) : new PlayerPass();

                playerPass.Photo = NewPlayerPicture == null || NewPlayerPicture.Length == 0 ? null : NewPlayerPicture;
                playerPass.Expires = DateTime.Parse(txtPassExpires.Text);
                playerPass.PassNumber = string.IsNullOrWhiteSpace(txtPassNumber.Text) ? null : txtPassNumber.Text;

                if (!playerPassID.HasValue)
                {
                    playerPass.PlayerID = PrimaryKey.Value;

                    PlayerPass inserted = ppc.AddNew(playerPass);
                    playerPassID = inserted.PlayerPassID;
                    AlertBox.SetStatus("Successfully saved new player pass.");
                }
                else
                {
                    ppc.Update(playerPass);
                    AlertBox.SetStatus("Successfully saved existing player pass.");
                }

                NewPlayerPicture = null;
                OldPlayerPicture = null;
                pnlAddEditPass.Visible = false;
                gvPlayerPasses.EditIndex = -1;
                gvPlayerPasses.DataBind();
            }
        }

        private void Populate_PlayerPassEdit(int playerPassID)
        {
            using (PlayerPassesController ppc = new PlayerPassesController())
            {
                FetchStrategy fetch = new FetchStrategy();
                fetch.LoadWith<PlayerPass>(i => i.Photo);

                PlayerPass pass = ppc.GetWhere(i => i.PlayerPassID == playerPassID, fetch).First();
                ClearPlayerPassForm();

                // populate
                txtPassExpires.Text = pass.Expires.ToString("yyyy-MM-dd");
                txtPassNumber.Text = pass.PassNumber;
                OldPlayerPicture = pass.Photo;
                NewPlayerPicture = pass.Photo;

                if (pass.Photo != null && pass.Photo.Count() > 0)
                {
                    SetPreviewImage(pass.Photo);
                }

                pnlAddEditPass.Visible = true;
            }
        }

        protected void lnkSavePlayerPass_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            int? playerPassID = gvPlayerPasses.EditIndex != -1 ? PlayerPasses[gvPlayerPasses.EditIndex].PlayerPassID : (int?)null;
            SavePlayerPass(playerPassID);
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
            int playerPassID;
            if(int.TryParse(((LinkButton)sender).CommandArgument, out playerPassID)){
                Populate_PlayerPassEdit(playerPassID);
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
                int? playerPassID = gvPlayerPasses.EditIndex != -1 ? PlayerPasses[gvPlayerPasses.EditIndex].PlayerPassID : (int?)null;
                if (!playerPassID.HasValue)
                {
                    args.IsValid = PlayerPasses.Count(i => i.PlayerID == PrimaryKey.Value && i.Expires == exp) == 0;
                }
                else
                {
                    args.IsValid = PlayerPasses.Count(i => i.PlayerID == PrimaryKey.Value && i.Expires == exp && i.PlayerPassID != playerPassID.Value) == 0;
                }
            }
        }

        // The id parameter name should match the DataKeyNames value set on the control
        public void gvPlayerPasses_DeleteItem(int? PlayerPassID)
        {
            ClearPlayerPassForm();

            if (!PlayerPassID.HasValue)
            {
                return;
            }
            using (PlayerPassesController ppc = new PlayerPassesController())
            {
                ppc.Delete(PlayerPassID.Value);
                AlertBox.SetStatus("Successfully deleted player pass and all team assignments with that player pass.");
            }

            gvPlayerPasses.DataBind();
        }
        #endregion


        #region Guardian Administration
        public IQueryable gvGuardians_GetData()
        {
            using (GuardiansController gc = new GuardiansController())
            {
                FetchStrategy fetch = new FetchStrategy();
                fetch.LoadWith<Guardian>(i => i.Person);

                Guardians = gc.GetWhere(i => i.PlayerID == PrimaryKey.Value, fetch).ToList();

                return Guardians.Select(i => new
                {

                    Guardian = i.Person.FName + " " + i.Person.LName,
                    IsRemovable = true,
                    GuardianID = i.GuardianID
                }).OrderByDescending(i => i.Guardian).AsQueryable();
            }
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

        private void SaveGuardian(int? guardianID)
        {
            using (GuardiansController gc = new GuardiansController())
            {
                FetchStrategy fetch = new FetchStrategy();
                fetch.LoadWith<Guardian>(i => i.Person);

                Guardian guardian = guardianID.HasValue ? gc.GetWhere(i => i.GuardianID == guardianID.Value, fetch).First() : new Guardian();

                if (!guardianID.HasValue)
                {
                    guardian.Person = new Person();
                    guardian.Person.ClubID = 1;
                }

                guardian.Person.FName = txtGuardianFirstName.Text;
                guardian.Person.MInitial = string.IsNullOrWhiteSpace(txtGuardianMiddleInitial.Text) ? (char?)null : txtGuardianMiddleInitial.Text.Trim().ToCharArray()[0];
                guardian.Person.LName = txtGuardianLastName.Text;

                if (!guardianID.HasValue)
                {
                    guardian.PlayerID = PrimaryKey.Value;
                    Guardian newGuardian = gc.AddNew(guardian);
                    AlertBox.SetStatus("Successfully saved new guardian.");
                }
                else
                {
                    gc.Update(guardian);
                    AlertBox.SetStatus("Successfully saved existing guardian.");
                }
            }
        }

        private void Populate_GuardianEdit(int guardianID)
        {
            using (GuardiansController gc = new GuardiansController())
            {
                FetchStrategy fetch = new FetchStrategy();
                fetch.LoadWith<Guardian>(i => i.Person);

                Guardian guardian = gc.GetWhere(i => i.GuardianID == guardianID, fetch).First();
                ClearGuardianForm();

                // populate
                txtGuardianFirstName.Text = guardian.Person.FName;
                txtGuardianMiddleInitial.Text = guardian.Person.MInitial.HasValue ? guardian.Person.MInitial.Value.ToString() : "";
                txtGuardianLastName.Text = guardian.Person.LName;

                pnlAddGuardian.Visible = true;
            }
        }

        private void Populate_AddressBook(int personID)
        {
            AddressBook.Reset();
            AddressBook.PersonID = personID;
            AddressBook.DataBind();
        }

        private void Populate_PhoneNumberBook(int personID)
        {
            PhoneNumberBook.Reset();
            PhoneNumberBook.PersonID = personID;
            PhoneNumberBook.DataBind();
        }

        private void Populate_EmailBook(int personID)
        {
            EmailBook.Reset();
            EmailBook.PersonID = personID;
            EmailBook.DataBind();
        }

        protected void gvGuardians_RowEditing(object sender, GridViewEditEventArgs e)
        {
            int guardianID = (int)gvGuardians.DataKeys[e.NewEditIndex].Value;
            Populate_GuardianEdit(guardianID);
            gvGuardians.EditIndex = e.NewEditIndex;
            gvGuardians.DataBind();
            UpdateGuardianTabs();
        }

        protected void gvGuardians_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvGuardians.EditIndex = -1;
            gvGuardians.DataBind();
            UpdateGuardianTabs();
        }

        public void gvGuardians_DeleteItem(int? GuardianID)
        {
            ClearGuardianForm();

            if (!GuardianID.HasValue)
            {
                return;
            }
            using (GuardiansController gc = new GuardiansController())
            {
                gc.Delete(GuardianID.Value);
                AlertBox.SetStatus("Successfully deleted player's guardian.");
            }

            gvGuardians.DataBind();
        }

        protected void lnkAddGuardian_Click(object sender, EventArgs e)
        {
            ClearGuardianForm();
            pnlAddGuardian.Visible = true;
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

            int? guardianID = gvGuardians.EditIndex != -1 ? (int)gvGuardians.DataKeys[gvGuardians.EditIndex].Value : (int?)null;
            SaveGuardian(guardianID);
            gvGuardians.DataBind();

            if (!guardianID.HasValue)
            {
                ClearGuardianForm();
            }
            else
            {
                UpdateGuardianTabs();
            }
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

                int personID = Guardians.First(i => i.GuardianID == (int)gvGuardians.DataKeys[gvGuardians.EditIndex].Value).PersonID;

                switch(selectedTabIndex)
                {
                    case 1: // populate address book
                        Populate_AddressBook(personID);
                        break;

                    case 2: // populate email book
                        Populate_EmailBook(personID);
                        break;

                    case 3: // populate phone book
                        Populate_PhoneNumberBook(personID);
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
        }
        #endregion
    }
}