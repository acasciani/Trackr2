using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.OpenAccess;
using Telerik.OpenAccess.FetchOptimization;
using Telerik.OpenAccess.Metadata;
using Trackr.Utils;
using TrackrModels;

namespace Trackr.Modules.Registration
{
    public partial class Default : Page
    {
        [Serializable]
        private class PlayerMatch
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public int PlayerID { get; set; }
        }

        private int? PlayerID
        {
            get { return ViewState["PlayerID"] as int?; }
            set { ViewState["PlayerID"] = value; }
        }

        public int RegistrationYear { get; private set; }
        public string ClubName { get; private set; }
        public Stack<int> StepHistory
        {
            get
            {
                Stack<int> stack = ViewState["StepHistory"] as Stack<int>;
                if (stack == null)
                {
                    stack = new Stack<int>();
                    ViewState["StepHistory"] = stack;
                }
                return stack;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ClubName = "Gananda Bandits";
            RegistrationYear = 2017;

            if (!string.IsNullOrWhiteSpace(Request["SelectedPlayer"]))
            {
                // temporarily set to allow the widget to build proper steps
                widgetPlayerManagement.PrimaryKey = Request["SelectedPlayer"] == "new" ? (int?)null : int.Parse(Request["SelectedPlayer"]);
            }

            if (IsPostBack)
            {
                return;
            }

            List<PlayerMatch> matches = GetPossiblePlayersToRegister(CurrentUser.UserID);
            gvPossiblePlayerMatches.DataSource = matches;
            gvPossiblePlayerMatches.DataBind();
            divNoPlayersFoundMessage.Visible = matches.Count() == 0;
            divPlayersFoundMessage.Visible = matches.Count() > 0;
        }


        protected void validatorDateTimeParses_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime date;
            args.IsValid = DateTime.TryParse(args.Value, out date) && new DateTime(1900, 1, 1) <= date && date <= new DateTime(2200, 1, 1);
        }

        protected void lnkBackStep_Click(object sender, EventArgs e)
        {
            int previousStep = StepHistory.Pop();
            mvRegister.ActiveViewIndex = previousStep;

            lnkContinueStep.Visible = mvRegister.ActiveViewIndex != 1;
            divNavigation.Visible = mvRegister.ActiveViewIndex != 3;
        }

        protected void lnkContinueStep_Click(object sender, EventArgs e)
        {
            switch (mvRegister.ActiveViewIndex)
            {
                case 0: // registration selection
                    string playerID_selection = Request["SelectedPlayer"];

                    if (string.IsNullOrWhiteSpace(playerID_selection))
                    {
                        AlertBox.AddAlert("Please select a player to register, or select to register a first year player.", false, UI.AlertBoxType.Error);
                        return;
                    }

                    if (playerID_selection == "new")
                    {
                        // new
                        using (PeopleController pc = new PeopleController())
                        {
                            Person person = pc.GetWhere(i => i.UserID.HasValue && i.UserID.Value == CurrentUser.UserID).FirstOrDefault();
                            if (person != null)
                            {
                                widgetPlayerManagement.AddThesePeopleAsDefaultGuardians = new List<int>() { person.PersonID };
                            }
                        }
                    }
                    else
                    {
                        //re register
                        widgetPlayerManagement.AddThesePeopleAsDefaultGuardians = null;
                        widgetPlayerManagement.PrimaryKey = int.Parse(playerID_selection);

                        using (PlayersController pc = new PlayersController())
                        {
                            FetchStrategy fetch = new FetchStrategy();
                            fetch.LoadWith<Player>(i => i.Person);
                            Player player = pc.GetWhere(i => i.PlayerID == int.Parse(playerID_selection), fetch).First();
                            litPlayerFirstName.Text = string.Format("{0}", player.Person.FName);
                        }
                    }

                    widgetPlayerManagement.Reload();
                    break;

                case 1: // player info
                    if (!PlayerID.HasValue)
                    {
                        AlertBox_PlayerInfo.AddAlert("Please completely go through the player wizard to ensure information is up to date.", false, UI.AlertBoxType.Info);
                        return;
                    }
                    break;

                case 2: // register
                    string teamID_selection = Request["SelectedTeam"];

                    if (string.IsNullOrWhiteSpace(teamID_selection))
                    {
                        AlertBox_PlayerRegistration.AddAlert("Please select a team to register this player to.", false, UI.AlertBoxType.Error);
                        return;
                    }

                    int teamID = int.Parse(teamID_selection);

                        using (TeamPlayersController tpc = new TeamPlayersController())
                        {
                            // check to ensure they are already not assigned to the team.
                            if (tpc.GetWhere(i => i.TeamID == teamID && i.Active && (i.PlayerID == PlayerID.Value || (i.PlayerPass != null && i.PlayerPass.PlayerID==PlayerID.Value))).Count() > 0)
                            {
                                AlertBox_PlayerRegistration.AddAlert("This player is already assigned to the selected team.", false, UI.AlertBoxType.Warning);
                                return;
                            }
                            else
                            {
                                TeamPlayer teamPlayer = new TeamPlayer()
                                {
                                    Active = true,
                                    IsSecondary = false,
                                    LastModifiedAt = DateTime.Now.ToUniversalTime(),
                                    LastModifiedBy = CurrentUser.UserID,
                                    PlayerID = PlayerID.Value,
                                    TeamID = teamID,
                                    Approved = false
                                };

                                tpc.AddNew(teamPlayer);

                                AlertBox_PlayerRegistration.AddAlert("Successfully registered player for team.");
                            }
                        }

                    break;

                case 3:
                    PlayerID = null;
                    StepHistory.Clear();
                    break;

                default: break;
            }

            if (mvRegister.ActiveViewIndex == mvRegister.Views.Count - 1)
            {
                // last step
            }
            else
            {
                StepHistory.Push(mvRegister.ActiveViewIndex);
                mvRegister.ActiveViewIndex += 1;

                lnkContinueStep.Visible = mvRegister.ActiveViewIndex != 1;
                divNavigation.Visible = mvRegister.ActiveViewIndex != 3;
            }
        }

        protected void lnkSelectPlayer_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                PlayerID = int.Parse(btn.CommandArgument);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }





        #region Find possible players to register for email address
        private List<PlayerMatch> GetPossiblePlayersToRegister(int userID)
        {
            using (PeopleController pc = new PeopleController())
            using (EmailAddressesController eac = new EmailAddressesController())
            {
                FetchStrategy fetch = new FetchStrategy() { MaxFetchDepth = 5 };
                fetch.LoadWith<Person>(i => i.Guardians);
                fetch.LoadWith<Guardian>(i => i.Player);
                fetch.LoadWith<EmailAddress>(i => i.Person);
                fetch.LoadWith<Player>(i => i.Person);

                var playersAssociatedToUserID = pc.GetWhere(i => i.UserID.HasValue && userID == i.UserID.Value, fetch).SelectMany(i => i.Guardians)
                    .Select(i => new PlayerMatch()
                    {
                        PlayerID = i.PlayerID,
                        FirstName = i.Player.Person.FName,
                        LastName = i.Player.Person.LName,
                        DateOfBirth = i.Player.Person.DateOfBirth
                    });

                return playersAssociatedToUserID.OrderBy(i => i.FirstName).ThenBy(i => i.LastName).ThenBy(i => i.DateOfBirth).ToList();
            }
        }
        #endregion

        protected void widgetPlayerManagement_PlayerSavedError(object sender, EventArgs e)
        {
            widgetPlayerManagement.Reload();
        }

        protected void widgetPlayerManagement_PlayerSavedSuccess(object sender, EventArgs e)
        {
            widgetPlayerManagement.Reload();

            Trackr.Source.Wizards.PlayerManagement.PlayerSavedEventArgs arg = (Trackr.Source.Wizards.PlayerManagement.PlayerSavedEventArgs)e;
            PlayerID = arg.PlayerID;

            // make sure user has scope to them
            using (ScopeAssignmentsController sac = new ScopeAssignmentsController())
            {
                // check they dont have it first
                var assignmentsForPlayer = sac.GetWhere(i => i.UserID == CurrentUser.UserID && i.ResourceID == PlayerID.Value && i.ScopeID == 4 && i.RoleID==6);
                if (assignmentsForPlayer.Count() == 0)
                {
                    ScopeAssignment assignment = new ScopeAssignment()
                    {
                        IsDeny = false,
                        ResourceID = PlayerID.Value,
                        RoleID = 6,
                        ScopeID = 4,
                        UserID = CurrentUser.UserID
                    };

                    sac.AddNew(assignment);
                }
            }

            SetUpRegistrationWorkFlow(arg.PlayerID.Value);

            mvRegister.ActiveViewIndex += 1;
            lnkContinueStep.Visible = mvRegister.ActiveViewIndex != 1;
            divNavigation.Visible = mvRegister.ActiveViewIndex != 3;
        }

        private void SetUpRegistrationWorkFlow(int playerID)
        {
            // get all teams they were on
            FetchStrategy fetch = new FetchStrategy();
            fetch.LoadWith<Player>(i => i.PlayerPasses);
            fetch.LoadWith<PlayerPass>(i => i.TeamPlayers);
            fetch.LoadWith<Player>(i => i.TeamPlayers);
            fetch.LoadWith<Player>(i=>i.Person);

            FetchStrategy fetchTeam = new FetchStrategy();
            fetchTeam.LoadWith<RegistrationRule>(i => i.NewTeam);
            fetchTeam.LoadWith<Team>(i => i.Program);

            using (PlayersController pc = new PlayersController())
            using(RegistrationRulesController rrc = new RegistrationRulesController())
            {
                var player = pc.GetWhere(i => i.PlayerID == playerID, fetch).First();
                List<int> teamIDsPreviouslyOn = player.PlayerPasses.SelectMany(i => i.TeamPlayers).Where(i => i.Active).Select(i => i.TeamID)
                    .Union(player.TeamPlayers.Where(i => i.Active).Select(i => i.TeamID)).Distinct().ToList();

                DateTime currentTime = DateTime.Now.ToUniversalTime();

                var allOpenTeams = rrc.GetWhere(i => i.RegistrationOpens <= currentTime && currentTime <= i.RegistrationCloses && i.NewTeam.Program.ClubID == player.Person.ClubID,fetchTeam);

                var previousTeams = allOpenTeams.Where(i=>i.OldTeamID.HasValue && teamIDsPreviouslyOn.Contains(i.OldTeamID.Value));
                var newTeams = allOpenTeams.Where(i=>i.DateOfBirthCutoff.HasValue && player.Person.DateOfBirth.HasValue && player.Person.DateOfBirth.Value > i.DateOfBirthCutoff.Value).OrderByDescending(i=>i.DateOfBirthCutoff.Value).Take(1);//take the youngest one

                var possibleTeams = previousTeams.Union(newTeams)
                    .Select(i => new
                    {
                        TeamName = i.NewTeam.TeamName,
                        ProgramName = i.NewTeam.Program.ProgramName,
                        PercentRegistered = 10.0,
                        IsTeamFull = false,
                        TeamID = i.NewTeamID
                    }).Distinct().ToList().OrderBy(i => i.TeamName);

                gvTeamsToRegisterFor.DataSource = possibleTeams;
                gvTeamsToRegisterFor.DataBind();
            }
        }
    }
}