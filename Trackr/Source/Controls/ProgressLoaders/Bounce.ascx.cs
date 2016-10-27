using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Trackr.Controls.ProgressLoaders
{
    public partial class Bounce : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "resizeBounceProgress_" + this.ClientID, "bounceProgressAttach('#" + divOverlay.ClientID + "','#" + divBounce.ClientID + "');", true);
        }
    }
}