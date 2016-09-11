using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.OpenAccess.FetchOptimization;
using Trackr;
using TrackrModels;

namespace Trackr.Modules.PlayerManagement
{
    public partial class Default : Page
    {
        private class PlayerResultEqualityComparer : IEqualityComparer<Trackr.PlayersController.PlayerViewObject>
        {
            public bool Equals(Trackr.PlayersController.PlayerViewObject x, Trackr.PlayersController.PlayerViewObject y)
            {
                return x.BirthDate == y.BirthDate && x.FirstName == y.FirstName && x.LastName == y.LastName && x.PlayerID == y.PlayerID;
            }

            public int GetHashCode(Trackr.PlayersController.PlayerViewObject obj)
            {
                return (obj.BirthDate.HasValue ? obj.BirthDate.Value.GetHashCode() : 0) |
                        (obj.FirstName ?? "").GetHashCode() |
                        (obj.LastName ?? "").GetHashCode() |
                        obj.PlayerID.GetHashCode();
            }
        }

        private List<Trackr.PlayersController.PlayerViewObject> Data
        {
            get { return Session["DataSet"] as List<Trackr.PlayersController.PlayerViewObject>; }
            set { Session["DataSet"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            CheckAllowed(Permissions.PlayerManagement.ViewPlayers);

            // show the create link if allowed to create new player
            lnkCreatePlayer.Visible = IsAllowed(Permissions.PlayerManagement.CreatePlayer);
        }

        public IQueryable gvAllPlayers_GetData()
        {
            using (TeamPlayersController tpc = new TeamPlayersController())
            using (PlayersController pc = new PlayersController())
            {
                if (!IsPostBack || Data == null)
                {
                    FetchStrategy fetch = new FetchStrategy() { MaxFetchDepth = 5};
                    fetch.LoadWith<TeamPlayer>(i => i.Player);
                    fetch.LoadWith<Player>(i => i.Person);
                    fetch.LoadWith<Player>(i => i.PlayerPasses);
                    fetch.LoadWith<Player>(i => i.TeamPlayers);

                    fetch.LoadWith<TeamPlayer>(i => i.PlayerPass);
                    fetch.LoadWith<TeamPlayer>(i => i.Team);
                    fetch.LoadWith<PlayerPass>(i => i.Player);
                    fetch.LoadWith<PlayerPass>(i => i.TeamPlayers);

                    // put all players into 2 buckets, those that are assigned by player pass and those assigned directly to teamplayer

                    var allPlayers = pc.GetAllScopedPlayerViewObjects(CurrentUser.UserID, "PlayerManagement.ViewPlayers", i=>true==true);
                    Data = allPlayers.Distinct(new PlayerResultEqualityComparer()).ToList();
                }

                var filteredData = Data;

                if (ptpPicker.SelectedTeamID.HasValue)
                {
                    filteredData = Data.Where(i => i.TeamIDs.Contains(ptpPicker.SelectedTeamID.Value)).ToList();
                }
                else
                {
                    if (ptpPicker.SelectedProgramID.HasValue)
                    {
                        filteredData = Data.Where(i => i.ProgramIDs.Contains(ptpPicker.SelectedProgramID.Value)).ToList();
                    }
                }

                return filteredData.OrderBy(i => i.LastName).ThenBy(i => i.FirstName).AsQueryable<Trackr.PlayersController.PlayerViewObject>();
            }
        }

        protected void lnkFilter_Click(object sender, EventArgs e)
        {
            gvAllPlayers.DataBind();
        }
    }
}