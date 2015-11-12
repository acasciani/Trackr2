using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Trackr.Modules.UserManagement
{
    public partial class Manage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            int userID; // check edited vs new
            if (int.TryParse(Request.QueryString["id"], out userID))
            {
                // edit
                CheckAllowed<WebUsersController, TrackrModels.WebUser, int>(Permissions.UserManagement.EditUser, userID);
            }
            else
            {
                // create
                CheckAllowed(Permissions.UserManagement.CreateUser);
            }
        }
    }
}