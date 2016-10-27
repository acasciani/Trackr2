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

        public bool? Approved
        {
            get { return ViewState["Approved"] as bool?; }
            set { ViewState["Approved"] = value; }
        }

        public string Permission { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {
            using (TeamsController tc = new TeamsController())
            {
                if (!tc.IsUserScoped(CurrentUser.UserID, Permission, TeamID))
                {
                    throw new UserUnauthorizedException("You do not have permission to view this team.");
                }
            }

            if (!IsPostBack)
            {
                RegistrationProgressDTO dto = ShowTotalRegistered(true);
                chartGauge.Value = dto.CurrentSize;
                chartGauge.MaxValue = dto.MaxRosterSize;
                chartGauge.Label = string.Format("{0} ({1})", dto.TeamName, dto.ProgramName);
                chartGauge.SubLabel = "all players";
            }

            var teamIDs = ((Trackr.Page)Page).Profile.TeamManagement.RegistrationProgress_TeamIDsHide;
            bool isHidden = teamIDs != null && teamIDs.Contains(TeamID);
            lnkHideTeam.Visible = !isHidden;
            lnkUnHideTeam.Visible = isHidden;
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

                return cm.Teams.Where(i => i.TeamID == TeamID).Select(i => new RegistrationProgressDTO { TeamName = i.TeamName, ProgramName = i.Program.ProgramName, MaxRosterSize = i.MaxRosterSize, CurrentSize = players }).First();
            }
        }

        protected void lnkShowChange_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            
            switch (btn.CommandArgument)
            {
                case "ShowApproved": // only approved
                    Approved = true;
                    btn.CommandArgument = "ShowNonApproved";
                    btn.Text = "Show Non-Approved Players";
                    chartGauge.SubLabel = "approved players";
                    break;

                case "ShowNonApproved": // only non approved
                    Approved = false;
                    btn.CommandArgument = "ShowAll";
                    btn.Text = "Show All Players";
                    chartGauge.SubLabel = "non-approved players";
                    break;

                default: // all
                    Approved = null;                    
                    btn.CommandArgument = "ShowApproved";
                    btn.Text = "Show Approved Players";
                    chartGauge.SubLabel = "all players";
                    break;
            }

            RegistrationProgressDTO dto = ShowTotalRegistered(true);
            chartGauge.Value = dto.CurrentSize;
            chartGauge.MaxValue = dto.MaxRosterSize;
            chartGauge.Label = string.Format("{0} ({1})", dto.TeamName, dto.ProgramName);
            chartGauge.Update();
            
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

                divHolder.Visible = false;

                lnkHideTeam.Visible = false;
                lnkUnHideTeam.Visible = true;
                
                Page.Master.AddNotificationTooltip("This team will be hidden from your Registration Progress widget in the future. To unhide, click \"Show Hidden Teams\" and unhide the team.", UI.AlertBoxType.Info);
            }
            catch (Exception ex)
            {
                Page.Master.HandleException(ex);
            }
        }

        protected void lnkUnHideTeam_Click(object sender, EventArgs e)
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
                profile.TeamManagement.RegistrationProgress_TeamIDsHide.RemoveAll(i => i == TeamID);
                profile.Save();

                lnkHideTeam.Visible = true;
                lnkUnHideTeam.Visible = false;

                Page.Master.AddNotificationTooltip("This team is no longer hidden from your Registration Progress widget.", UI.AlertBoxType.Info);
            }
            catch (Exception ex)
            {
                Page.Master.HandleException(ex);
            }
        }

        public void Show(bool show)
        {
            divHolder.Visible = show;
        }
    }
}