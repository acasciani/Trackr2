using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackrModels;

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
        public Func<Player, bool> PlayerWhereClause { get; set; }
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

                ddlProgram.DataSource = programs;
                ddlProgram.DataTextField = "Label";
                ddlProgram.DataValueField = "Value";
                ddlProgram.DataBind();
            }
        }

        private void BindTeams()
        {
            using (TeamsController tc = new TeamsController())
            {
                var teams = tc.GetScopedEntities(CurrentUser.UserID, Permission).Where(i => i.ProgramID == SelectedProgramID.Value)
                    .Where(i => TeamWhereClause == null ? true : TeamWhereClause(i))
                    .Select(i => new
                    {
                        Label = i.TeamName,
                        Value = i.TeamID
                    })
                    .OrderBy(i => i.Label).ToList();

                ddlTeam.DataSource = teams;
                ddlTeam.DataTextField = "Label";
                ddlTeam.DataValueField = "Value";
                ddlTeam.DataBind();
            }
        }

        private void BindPlayers()
        {

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
        }

        protected void validateIntIsParsable(object source, ServerValidateEventArgs args)
        {
            int value;
            args.IsValid = int.TryParse(args.Value, out value);
        }
    }
}