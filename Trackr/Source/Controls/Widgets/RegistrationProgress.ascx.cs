using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Trackr.Cache.Scope;
using TrackrModels;
using Trackr.Utils;
using Trackr.Providers;
using Trackr.Providers.Profiles;

namespace Trackr.Source.Controls.Widgets
{
    public partial class RegistrationProgress : UserControl
    {
        public class RegistrationProgressDTO
        {
            public string TeamName { get; set; }
            public string ProgramName { get; set; }
            public int MaxRosterSize { get; set; }
            public int CurrentSize { get; set; }
        }

        public int TeamID {
            get { return ViewState["TeamID"] as int? ?? 0; }
            set { ViewState["TeamID"] = value; }
        }

        public string Permission { get; set; }
        public bool? Approved { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {
            using (TeamsController tc = new TeamsController())
            {
                if (!tc.IsUserScoped(CurrentUser.UserID, Permission, TeamID))
                {
                    throw new UserUnauthorizedException("You do not have permission to view this team.");
                }
            }
        }

        public RegistrationProgressDTO ShowTotalRegistered(bool Active)
        {
            // this includes all Registered... it will also include those who are NOT approved though
            int UserID = CurrentUser.UserID;

            Expression<Func<Player, bool>> filter = i => i.TeamPlayers.Where(j => TeamID == j.TeamID && j.Active == Active && (!Approved.HasValue || Approved == j.Approved)).Count() > 0 ||
                i.PlayerPasses.SelectMany(j => j.TeamPlayers).Where(j => TeamID == j.TeamID && j.Active == Active && (!Approved.HasValue || Approved == j.Approved)).Count() > 0;

            Players cachePlayers = new Players();

            using (ClubManagement cm = new ClubManagement())
            //using(PlayersController pc = new PlayersController())
            {
                HashSet<int> playerIDs = cachePlayers.GetScopedIDs(UserID, Permission, filter);

                int players = cm.Players
                    .Where(filter)
                    .Where(i => playerIDs.Contains(i.PlayerID))
                    .Count();

                var teamInfo = cm.Teams.Where(i => i.TeamID == TeamID).Select(i => new { TeamName = i.TeamName, ProgramName = i.Program.ProgramName, MaxRosterSize = i.MaxRosterSize }).First();

                return new RegistrationProgressDTO()
                {
                    TeamName = teamInfo.TeamName,
                    ProgramName = teamInfo.ProgramName,
                    MaxRosterSize = teamInfo.MaxRosterSize,
                    CurrentSize = players
                };
            }
        }

        protected void lnkShowChange_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            RegistrationProgressDTO dto = null;

            string script = string.Format("UpdateGauge($('#{0}'), '{1}', {2}, 0, {3}, '{4}', '{4}');", 
                chart.ClientID,
                "{0}", "{1}", "{2}", "{3}");

            switch (btn.CommandArgument)
            {
                case "ShowApproved": // only approved
                    Approved = true;
                    dto = ShowTotalRegistered(true);
                    ScriptManager.RegisterClientScriptBlock(UpdatePanel, UpdatePanel.GetType(), "widgetChart", string.Format(script, string.Format("{0} ({1}", dto.TeamName, dto.ProgramName).Replace("'", @"\'"), dto.CurrentSize, dto.MaxRosterSize, "approved players"), true);
                    btn.CommandArgument = "ShowNonApproved";
                    btn.Text = "Show Non-Approved Players";
                    break;

                case "ShowNonApproved": // only non approved
                    Approved = false;
                    dto = ShowTotalRegistered(true);
                    ScriptManager.RegisterClientScriptBlock(UpdatePanel, UpdatePanel.GetType(), "widgetChart", string.Format(script, string.Format("{0} ({1}", dto.TeamName, dto.ProgramName).Replace("'", @"\'"), dto.CurrentSize, dto.MaxRosterSize, "non-approved players"), true);
                    btn.CommandArgument = "ShowAll";
                    btn.Text = "Show All Players";
                    break;

                default: // all
                    Approved = null;
                    dto = ShowTotalRegistered(true);
                    ScriptManager.RegisterClientScriptBlock(UpdatePanel, UpdatePanel.GetType(), "widgetChart", string.Format(script, string.Format("{0} ({1}", dto.TeamName, dto.ProgramName).Replace("'", @"\'"), dto.CurrentSize, dto.MaxRosterSize, "all players"), true);
                    btn.CommandArgument = "ShowApproved";
                    btn.Text = "Show Approved Players";
                    break;
            }
        }

        protected void lnkHideTeam_Click(object sender, EventArgs e)
        {
            try
            {
                Trackr.Page page = (Trackr.Page)Page;

                // add team id to hide list
                UserProfile profile = ((Trackr.Page)Page).Profile;
                if (profile.TeamManagement.RegistrationProgress_TeamIDsHide == null)
                {
                    profile.TeamManagement.RegistrationProgress_TeamIDsHide = new List<int>();
                }
                profile.TeamManagement.RegistrationProgress_TeamIDsHide.Add(TeamID);
                profile.Save();

                UpdatePanel.Visible = false;

                Page.Master.AddNotificationTooltip("This team will be hidden from your Registration Progress widget in the future. To unhide, click \"Show Hidden Teams\" and unhide the team.", UI.AlertBoxType.Info);
            }
            catch (Exception ex)
            {
                Page.Master.HandleException(ex);
            }
        }
    }
}