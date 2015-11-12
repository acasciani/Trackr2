using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Trackr.Modules.PlayerManagement
{
    public partial class Manage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            int playerID; // check edited vs new
            if (int.TryParse(Request.QueryString["id"], out playerID))
            {
                // edit
                CheckAllowed<PlayersController, TrackrModels.Player, int>(Permissions.PlayerManagement.EditPlayer, playerID);
            }
            else
            {
                // create
                CheckAllowed(Permissions.PlayerManagement.CreatePlayer);
            }
        }
    }
}