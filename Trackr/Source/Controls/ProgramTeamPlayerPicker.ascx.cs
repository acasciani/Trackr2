using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackrModels;
using Trackr.Utils;
using Telerik.OpenAccess.FetchOptimization;

namespace Trackr.Source.Controls
{
    public partial class ProgramTeamPlayerPicker : UserControl
    {
        public enum RequiredSelection
        {
            None,
            Program,
            Team,
            Player
        }

        public string Permission { get; set; }
        public Func<Program, bool> ProgramWhereClause { get; set; }
        public Func<Team, bool> TeamWhereClause { get; set; }
        public Func<TeamPlayer, bool> PlayerWhereClause { get; set; }
        public bool EnableTeamSelect { get; set; }
        public bool EnablePlayerSelect { get; set; }
        public RequiredSelection RequiredSelectOf { get; set; }
        public int? SelectedProgramID
        {
            get { return ViewState["SelectedProgramID"] as int?; }
            set { ViewState["SelectedProgramID"] = value; }
        }
        public int? SelectedTeamID
        {
            get { return ViewState["SelectedTeamID"] as int?; }
            set { ViewState["SelectedTeamID"] = value; }
        }
        public int? SelectedPlayerID
        {
            get { return ViewState["SelectedPlayerID"] as int?; }
            set { ViewState["SelectedPlayerID"] = value; }
        }

        public EventHandler ProgramSelected { get; set; }
        public EventHandler TeamSelected { get; set; }
        public EventHandler PlayerSelected { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            divPlayer.Visible = EnablePlayerSelect;
            divTeam.Visible = EnableTeamSelect;

            validatorProgram.Enabled = RequiredSelectOf == RequiredSelection.Program;
            validatorTeam.Enabled = RequiredSelectOf == RequiredSelection.Team;
            validatorPlayer.Enabled = RequiredSelectOf == RequiredSelection.Player;

            if (IsPostBack)
            {
                return;
            }

            Populate();
        }

        public void Populate()
        {
            BindPrograms();

            if (SelectedProgramID.HasValue)
            {
                ddlProgram.SelectedValue = SelectedProgramID.Value.ToString();

                if (EnableTeamSelect)
                {
                    BindTeams();
                    if (SelectedTeamID.HasValue)
                    {
                        ddlTeam.SelectedValue = SelectedTeamID.Value.ToString();

                        if (EnablePlayerSelect)
                        {
                            BindPlayers();
                            if (SelectedPlayerID.HasValue)
                            {
                                ddlPlayer.SelectedValue = SelectedPlayerID.Value.ToString();
                            }
                        }
                    }
                }
            }
            else
            {
                ddlProgram_SelectedIndexChanged(null, null);
            }
        }

        private void BindPrograms()
        {
            using (ProgramsController pc = new ProgramsController())
            {
                var programs = pc.GetScopedEntities(CurrentUser.UserID, Permission).Where(i => ProgramWhereClause == null ? true : ProgramWhereClause(i)).Select(i => new
                {
                    Label = i.ProgramName,
                    Value = i.ProgramID
                }).OrderBy(i => i.Label).ToList();

                ddlProgram.Reset(true);
                ddlProgram.DataSource = programs;
                ddlProgram.DataTextField = "Label";
                ddlProgram.DataValueField = "Value";
                ddlProgram.DataBind();

                if (programs.Count() == 1)
                {
                    ddlProgram.SelectedIndex = 1;
                    ddlProgram_SelectedIndexChanged(null, null);
                }
            }
        }

        private void BindTeams()
        {
            using (TeamsController tc = new TeamsController())
            {
                // if there is two teams with the same name, then we want to also say start/end date
                var currentTeams = tc.GetScopedEntities(CurrentUser.UserID, Permission).Where(i => i.ProgramID == SelectedProgramID.Value)
                    .Where(i => TeamWhereClause == null ? true : TeamWhereClause(i))
                    .Select(i => new {
                        Label = i.TeamName,
                        Start = i.StartYear,
                        End = i.EndYear,
                        Value = i.TeamID,
                        IsPresent = DateTime.Today <= i.EndYear.Date
                    })
                    .GroupBy(i => i.Label).ToList();

                var nonDuplicatedLabels = currentTeams.Where(i => i.Count() == 1).Select(i => i.First());
                var duplicatedLabels = currentTeams.Where(i => i.Count() != 1).SelectMany(i => i.Select(j => new
                {
                    Label = j.Label + string.Format(" ({0} to {1})", j.Start.ToString("MMM yyyy"), j.End.ToString("MMM yyyy")),
                    Start = j.Start,
                    End = j.End,
                    Value = j.Value,
                    IsPresent = j.IsPresent
                }));

                var teams = nonDuplicatedLabels.Union(duplicatedLabels).OrderByDescending(i => i.IsPresent).ThenBy(i => i.Start).ThenBy(i => i.Label).ToList();


                ddlTeam.Reset(true);
                ddlTeam.DataSource = teams;
                ddlTeam.DataTextField = "Label";
                ddlTeam.DataValueField = "Value";
                ddlTeam.DataBind();

                if (teams.Count() == 1)
                {
                    ddlTeam.SelectedIndex = 1;
                    ddlTeam_SelectedIndexChanged(null, null);
                }
            }
        }

        private void BindPlayers()
        {
            using (TeamPlayersController tpc = new TeamPlayersController())
            {
                FetchStrategy fetch = new FetchStrategy();
                fetch.LoadWith<TeamPlayer>(i => i.Player);
                fetch.LoadWith<Player>(i => i.Person);

                var players = tpc.GetScopedEntities(CurrentUser.UserID, Permission, fetch).Where(i => i.TeamID == SelectedTeamID.Value)
                    .Where(i => PlayerWhereClause == null ? true : PlayerWhereClause(i))
                    .Select(i => new
                    {
                        Label = i.Player.Person.FName + " " + i.Player.Person.LName,
                        Value = i.PlayerID
                    })
                    .OrderBy(i => i.Label).ToList();

                ddlPlayer.Reset(true);
                ddlPlayer.DataSource = players;
                ddlPlayer.DataTextField = "Label";
                ddlPlayer.DataValueField = "Value";
                ddlPlayer.DataBind();
            }
        }

        protected void ddlProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            int programID;
            if (int.TryParse(ddlProgram.SelectedValue, out programID))
            {
                SelectedProgramID = programID;

                if (EnableTeamSelect)
                {
                    BindTeams();
                }
            }
            else
            {
                SelectedProgramID = null;
                SelectedTeamID = null;
                SelectedPlayerID = null;

                ddlTeam.Reset(true);
                ddlPlayer.Reset(true);
            }

            if (ProgramSelected != null)
            {
                ProgramSelected(this, e);
            }
        }

        protected void ddlTeam_SelectedIndexChanged(object sender, EventArgs e)
        {
            int teamID;
            if (int.TryParse(ddlTeam.SelectedValue, out teamID))
            {
                SelectedTeamID = teamID;

                if (EnablePlayerSelect)
                {
                    BindPlayers();
                }
            }
            else
            {
                SelectedTeamID = null;
                SelectedPlayerID = null;

                ddlPlayer.Reset(true);
            }

            if (TeamSelected != null)
            {
                TeamSelected(this, e);
            }
        }

        protected void ddlPlayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            int playerID;
            if (int.TryParse(ddlPlayer.SelectedValue, out playerID))
            {
                SelectedPlayerID = playerID;
            }
            else
            {
                SelectedPlayerID = null;
            }

            if (PlayerSelected != null)
            {
                PlayerSelected(this, e);
            }
        }

        protected void validateIntIsParsable(object source, ServerValidateEventArgs args)
        {
            int value;
            args.IsValid = int.TryParse(args.Value, out value);
        }
    }
}