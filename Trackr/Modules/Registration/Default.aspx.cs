﻿using System;
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
        public int ClubID { get; private set; }
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
            ClubID = 1;

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
                        using(GuardiansController gc = new GuardiansController())
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

                                TeamPlayer addedTeamPlayer = tpc.AddNew(teamPlayer);

                                AlertBox_PlayerRegistration.AddAlert("Successfully registered player for team.");

                                FetchStrategy fetch = new FetchStrategy();
                                fetch.LoadWith<Guardian>(i=>i.Person);
                                fetch.LoadWith<Person>(i=>i.EmailAddresses);

                                var guardians = gc.GetWhere(i=>i.PlayerID == PlayerID.Value && i.Active, fetch).Select(i=>i.Person).ToList();

                                if (guardians.Count() > 0)
                                {
                                    string toName = null;

                                    if (guardians.Count() > 1)
                                    {
                                        List<string> guardianNames = guardians.Select(i => i.FName).OrderBy(i => i).ToList();
                                        toName = string.Join(", ", guardianNames.Take(guardianNames.Count() - 1)) + " and " + guardianNames.Last();
                                    }
                                    else
                                    {
                                        toName = guardians.First().FName;
                                    }

                                    FetchStrategy fetchTeam = new FetchStrategy();
                                    fetchTeam.LoadWith<TeamPlayer>(i => i.Team);
                                    fetchTeam.LoadWith<TeamPlayer>(i => i.Player);
                                    fetchTeam.LoadWith<Player>(i => i.Person);

                                    TeamPlayer tp = tpc.GetWhere(i => i.TeamPlayerID == addedTeamPlayer.TeamPlayerID, fetchTeam).First();

                                    List<Trackr.Utils.Messenger.EmailRecipient> recipients = new List<Trackr.Utils.Messenger.EmailRecipient>();

                                    guardians.ForEach(i => 
                                    {
                                        string name = (i.FName + " " + i.LName).Trim();
                                        i.EmailAddresses.Where(j => j.Active).Select(j => j.Email).Distinct().ToList().ForEach(j => recipients.Add(new Trackr.Utils.Messenger.EmailRecipient()
                                        {
                                            Email = j,
                                            Name = name,
                                            RecipientType = Trackr.Utils.Messenger.EmailRecipientType.TO
                                        }));
                                    });

                                    List<Trackr.Utils.Messenger.TemplateVariable> variables = new List<Trackr.Utils.Messenger.TemplateVariable>();
                                    variables.Add(new Trackr.Utils.Messenger.TemplateVariable()
                                    {
                                        VariableName = "GuardianNames",
                                        VariableContent = toName
                                    });
                                    variables.Add(new Trackr.Utils.Messenger.TemplateVariable()
                                    {
                                        VariableName = "TeamName",
                                        VariableContent = tp.Team.TeamName
                                    });
                                    variables.Add(new Trackr.Utils.Messenger.TemplateVariable()
                                    {
                                        VariableName = "ChildName",
                                        VariableContent = tp.Player.Person.FName
                                    });

                                    Trackr.Utils.Messenger.SendEmail("registration-successful", null, variables, recipients, false, false);
                                }
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
            using (ClubManagement cm = new ClubManagement())
            using (PeopleController pc = new PeopleController())
            using (EmailAddressesController eac = new EmailAddressesController())
            {
                var playersAssociatedToGuardian = cm.Guardians.Where(i => i.Active && i.Person.UserID.HasValue && i.Person.UserID.Value == userID).Select(i => i.Player)
                    .Select(i => new PlayerMatch()
                    {
                        PlayerID = i.PlayerID,
                        FirstName = i.Person.FName,
                        LastName = i.Person.LName,
                        DateOfBirth = i.Person.DateOfBirth
                    });

                return playersAssociatedToGuardian.OrderBy(i => i.FirstName).ThenBy(i => i.LastName).ThenBy(i => i.DateOfBirth).ToList();
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

            SetUpRegistrationWorkFlow(arg.PlayerID.Value);

            mvRegister.ActiveViewIndex += 1;
            lnkContinueStep.Visible = mvRegister.ActiveViewIndex != 1;
            divNavigation.Visible = mvRegister.ActiveViewIndex != 3;
        }

        private void SetUpRegistrationWorkFlow(int playerID)
        {
            // get all teams they were on

            using(ClubManagement cm = new ClubManagement())
            using (PlayersController pc = new PlayersController())
            using(RegistrationRulesController rrc = new RegistrationRulesController())
            {
                DateTime currentTime = DateTime.Now.ToUniversalTime();

                var allOpenTeams = cm.RegistrationRules.Where(i => i.RegistrationOpens <= currentTime && currentTime <= i.RegistrationCloses && i.NewTeam.Program.ClubID == ClubID)
                    .Select(i => new
                    {
                        NewTeamID = i.NewTeamID,
                        DateOfBirthCutoff = i.DateOfBirthCutoff,
                        NewTeamName = i.NewTeam.TeamName,
                        NewTeamProgramName = i.NewTeam.Program.ProgramName,
                        NewTeamMaxRoster = i.NewTeam.MaxRosterSize,
                        NewTeamActivePlayerCount = 0, // needs to be fixed
                        NewTeamStartYear = i.NewTeam.StartYear,
                        NewTeamEndYear = i.NewTeam.EndYear,
                        MaxCoRegistrationsPerProgram = i.NewTeam.Program.MaxCoRegistrationsPerSeason,
                        SeasonStart = i.NewTeam.StartYear,
                        SeasonEnd = i.NewTeam.EndYear,
                        ProgramID = i.NewTeam.ProgramID
                    })
                    .ToList();

                var teamsOn = cm.PlayerPasses.Where(i => i.PlayerID == playerID && i.Active).SelectMany(i => i.TeamPlayers).Union(cm.TeamPlayers.Where(i => i.PlayerID == playerID && i.Active))
                    .Select(i => new
                    {
                        TeamID = i.TeamID,
                        ProgramID = i.Team.ProgramID,
                        SeasonStart = i.Team.StartYear,
                        SeasonEnd = i.Team.EndYear
                    })
                    .Distinct()
                    .ToList();

                List<int> teamIDsOn = teamsOn.Select(i => i.TeamID).Distinct().ToList();

                Player player = cm.Players.Where(i => i.PlayerID == playerID).First();

                // incorporate max co registration count

                var newTeams = allOpenTeams
                    // We first want to get New teams to register to, those can only be teams the Player meets the age qualification (they are younger than the date of birth cutoff)
                    // We then group by the ProgramID and Max co registrations per program (for instance, for travel soccer, you can only be registered to 1 team per program, for a development program it may 
                    // be more)... We do this based on the season of the team (e.g. Aug 1- Jul 31). That means if one program, like Travel Boys has 2 teams in different season (e.g. Jul 1-Aug 31 and Aug 1 - Jul 31),
                    // then this logic check is INVALID... if this becomes a problem, we may need to add another column called "FunctionalSeasonStart" and "FunctionalSeasonEnd" and have a separate column
                    // for display purposes. Once we group by, we then select the new teams to register ordering by eligible age (youngest teams first), and take (i.e. in SQL "SELECT TOP X") 
                    // the max registrations per program MINUS the number of teams currently registered for that program..
                    .Where(i => !teamIDsOn.Contains(i.NewTeamID) && i.DateOfBirthCutoff.HasValue && player.Person.DateOfBirth.HasValue &&
                                player.Person.DateOfBirth.Value > i.DateOfBirthCutoff.Value)
                    .GroupBy(i => new { i.ProgramID, i.MaxCoRegistrationsPerProgram, i.SeasonStart, i.SeasonEnd })
                    .SelectMany(i =>
                        i.OrderByDescending(j => j.DateOfBirthCutoff.Value).Take(i.Key.MaxCoRegistrationsPerProgram - teamsOn.Count(j => j.ProgramID == i.Key.ProgramID && j.SeasonEnd == i.Key.SeasonEnd && j.SeasonStart == i.Key.SeasonStart))
                     )
                    .OrderByDescending(i => i.DateOfBirthCutoff.Value);

                var possibleTeams = newTeams
                    .Select(i => new
                    {
                        TeamName = i.NewTeamName,
                        ProgramName = i.NewTeamProgramName,
                        PercentRegistered = decimal.Divide(Convert.ToDecimal(i.NewTeamActivePlayerCount), Convert.ToDecimal(i.NewTeamMaxRoster))*100,
                        IsTeamFull = i.NewTeamActivePlayerCount == i.NewTeamMaxRoster,
                        TeamID = i.NewTeamID,
                        TeamStart = i.NewTeamStartYear,
                        TeamEnd = i.NewTeamEndYear,
                        AlreadyRegistered = false
                    }).Distinct().OrderBy(i => i.TeamName).ToList();

                var alreadyRegisteredButOpen = allOpenTeams.Where(i => teamIDsOn.Contains(i.NewTeamID))
                    .Select(i => new
                    {
                        TeamName = i.NewTeamName,
                        ProgramName = i.NewTeamProgramName,
                        PercentRegistered = decimal.Divide(Convert.ToDecimal(i.NewTeamActivePlayerCount), Convert.ToDecimal(i.NewTeamMaxRoster)) * 100,
                        IsTeamFull = i.NewTeamActivePlayerCount == i.NewTeamMaxRoster,
                        TeamID = i.NewTeamID,
                        TeamStart = i.NewTeamStartYear,
                        TeamEnd = i.NewTeamEndYear,
                        AlreadyRegistered = true
                    })
                    .Distinct().OrderBy(i => i.TeamName);
                
                possibleTeams.AddRange(alreadyRegisteredButOpen);

                gvTeamsToRegisterFor.DataSource = possibleTeams;
                gvTeamsToRegisterFor.DataBind();
            }
        }
    }
}