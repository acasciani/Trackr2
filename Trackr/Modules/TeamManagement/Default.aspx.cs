using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Trackr;
using TrackrModels;

namespace Trackr.Modules.TeamManagement
{
    public partial class Default : Page
    {
        [Serializable]
        private class TeamResult
        {
            public string ProgramName { get; set; }
            public string TeamName { get; set; }
            public string Coach { get; set; }
            public int PlayerCount { get; set; }
            public int TeamID { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            CheckAllowed(Permissions.TeamManagement.ViewTeams);
        }

        public IQueryable gvAllTeams_GetData()
        {
            using (TeamsController tc = new TeamsController())
            {
                var allInfo = tc.Get().Select(i => new TeamResult
                {
                    Age = i.Person.DateOfBirth.HasValue ? DateTime.Today.Year - i.Person.DateOfBirth.Value.Year : (int?)null,
                    BirthDate = i.Person.DateOfBirth.HasValue ? i.Person.DateOfBirth.Value : (DateTime?)null,
                    FirstName = i.Person.FName,
                    LastName = i.Person.LName,
                    PlayerID = i.PlayerID
                });

                return allInfo.OrderBy(i => i.LastName).ThenBy(i => i.FirstName).AsQueryable<PlayerResult>();
            }
        }

    }
}