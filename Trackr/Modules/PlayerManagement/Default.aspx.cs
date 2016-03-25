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
        private class PlayerResultEqualityComparer : IEqualityComparer<PlayerResult>
        {
            public bool Equals(PlayerResult x, PlayerResult y)
            {
                return x.BirthDate == y.BirthDate && x.FirstName == y.FirstName && x.LastName == y.LastName && x.PlayerID == y.PlayerID;
            }

            public int GetHashCode(PlayerResult obj)
            {
                return (obj.BirthDate.HasValue ? obj.BirthDate.Value.GetHashCode() : 0) |
                        (obj.FirstName ?? "").GetHashCode() |
                        (obj.LastName ?? "").GetHashCode() |
                        obj.PlayerID.GetHashCode();
            }
        }

        [Serializable]
        private class PlayerResult
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int? Age { get; set; }
            public DateTime? BirthDate { get; set; }
            public int PlayerID { get; set; }
            public List<int> ProgramIDs { get; set; }
            public List<int> TeamIDs { get; set; }
        }

        private List<PlayerResult> Data
        {
            get { return Session["DataSet"] as List<PlayerResult>; }
            set { Session["DataSet"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            CheckAllowed(Permissions.PlayerManagement.ViewPlayers);
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

                    var allPlayers = tpc.GetScopedEntities(CurrentUser.UserID, "PlayerManagement.ViewPlayers", fetch);

                    var playerPassPlayers = allPlayers.Where(i=>i.PlayerPassID.HasValue)
                        .Select(i=> new PlayerResult(){
                            Age = i.PlayerPass.Player.Person.DateOfBirth.HasValue ? DateTime.Today.ToUniversalTime().Year - i.PlayerPass.Player.Person.DateOfBirth.Value.Year : (int?)null,
                            BirthDate = i.PlayerPass.Player.Person.DateOfBirth.HasValue ? i.PlayerPass.Player.Person.DateOfBirth.Value : (DateTime?)null,
                            FirstName = i.PlayerPass.Player.Person.FName,
                            LastName = i.PlayerPass.Player.Person.LName,
                            PlayerID = i.PlayerPass.Player.PlayerID,
                            TeamIDs = i.PlayerPass.Player.PlayerPasses.Where(j => DateTime.Now.ToUniversalTime() < j.Expires).SelectMany(j => j.TeamPlayers).Select(j => j.TeamID).Union(i.PlayerPass.Player.TeamPlayers.Select(j => j.TeamID)).Distinct().ToList(),
                            ProgramIDs = i.PlayerPass.Player.PlayerPasses.Where(j => DateTime.Now.ToUniversalTime() < j.Expires).SelectMany(j => j.TeamPlayers).Select(j => j.Team.ProgramID).Union(i.PlayerPass.Player.TeamPlayers.Select(j => j.Team.ProgramID)).Distinct().ToList()
                        }).ToList();

                    var playerTeamPlayers = allPlayers.Where(i=>i.PlayerID.HasValue)
                        .Select(i=> new PlayerResult(){
                            Age = i.Player.Person.DateOfBirth.HasValue ? DateTime.Today.ToUniversalTime().Year - i.Player.Person.DateOfBirth.Value.Year : (int?)null,
                            BirthDate = i.Player.Person.DateOfBirth.HasValue ? i.Player.Person.DateOfBirth.Value : (DateTime?)null,
                            FirstName = i.Player.Person.FName,
                            LastName = i.Player.Person.LName,
                            PlayerID = i.PlayerID.Value,
                            TeamIDs = i.Player.PlayerPasses.Where(j => DateTime.Now.ToUniversalTime() < j.Expires).SelectMany(j => j.TeamPlayers).Select(j => j.TeamID).Union(i.Player.TeamPlayers.Select(j => j.TeamID)).Distinct().ToList(),
                            ProgramIDs = i.Player.PlayerPasses.Where(j => DateTime.Now.ToUniversalTime() < j.Expires).SelectMany(j => j.TeamPlayers).Select(j => j.Team.ProgramID).Union(i.Player.TeamPlayers.Select(j => j.Team.ProgramID)).Distinct().ToList()
                        }).ToList();

                    Data = playerPassPlayers.Union(playerTeamPlayers).Distinct(new PlayerResultEqualityComparer()).ToList();
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

                return filteredData.OrderBy(i => i.LastName).ThenBy(i => i.FirstName).AsQueryable<PlayerResult>();
            }
        }

        protected void lnkFilter_Click(object sender, EventArgs e)
        {
            gvAllPlayers.DataBind();
        }
    }
}