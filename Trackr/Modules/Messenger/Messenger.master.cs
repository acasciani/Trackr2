using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Trackr.Modules.Messenger
{
    public partial class Messenger : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lnkCompose_Click(object sender, EventArgs e)
        {
            divCompose.Visible = true;
            ScriptManager.RegisterStartupScript(divCompose, divCompose.GetType(), "ToggleCompose", "$('.compose-message').modal('toggle');", true);
        }
    }
}