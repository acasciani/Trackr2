using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Trackr.Source.Controls
{
    public partial class AttendanceTrackingWidget : UserControl
    {


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lnk_Click(object sender, EventArgs e)
        {
            (sender as LinkButton).Visible = false;
        }
    }
}