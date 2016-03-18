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
                    FetchStrategy fetch = new FetchStrategy();
                    fetch.LoadWith<TeamPlayer>(i => i.Player);
                    fetch.LoadWith<Player>(i => i.Person);
                    fetch.LoadWith<TeamPlayer>(i => i.Team);
                    fetch.LoadWith<Player>(i => i.TeamPlayers);

                    var allPlayers = pc.GetScopedEntities(CurrentUser.UserID, "PlayerManagement.ViewPlayers", fetch)
                        .Select(i => new PlayerResult()
                        {
                            Age = i.Person.DateOfBirth.HasValue ? DateTime.Today.ToUniversalTime().Year - i.Person.DateOfBirth.Value.Year : (int?)null,
                            BirthDate = i.Person.DateOfBirth.HasValue ? i.Person.DateOfBirth.Value : (DateTime?)null,
                            FirstName = i.Person.FName,
                            LastName = i.Person.LName,
                            PlayerID = i.PlayerID,
                            TeamIDs = i.TeamPlayers.Select(j => j.TeamID).Distinct().ToList(),
                            ProgramIDs = i.TeamPlayers.Select(j => j.Team.ProgramID).Distinct().ToList()
                        }).ToList();

                    Data = allPlayers;
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