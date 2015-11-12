using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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
            public int Age { get; set; }
            public int PlayerID { get; set; }
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
            using (PlayersController pc = new PlayersController())
            {
                var allInfo = pc.Get().Select(i => new PlayerResult
                {
                    Age = DateTime.Today.Year - i.DateOfBirth.Year,
                    FirstName = i.FName,
                    LastName = i.LName,
                    PlayerID = i.PlayerID
                });

                return allInfo.OrderBy(i => i.LastName).ThenBy(i => i.FirstName).AsQueryable<PlayerResult>();
            }
        }
    }
}