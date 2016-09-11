using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Trackr;
using Trackr.Source.Controls.TGridView;
using TrackrModels;

namespace Trackr.Modules.TeamManagement
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            CheckAllowed(Permissions.TeamManagement.ViewTeams);

            BindTeams();
        }

        private void BindTeams()
        {
            List<TeamsController.TeamViewObject> data = null;
            using (TeamsController tc = new TeamsController())
            {
                data = tc.GetScopedTeamViewObject(CurrentUser.UserID, Permissions.TeamManagement.ViewTeams).ToList();
            }

            GridViewData gvd = new GridViewData(typeof(TeamsController.TeamViewObject));
            gvd.AddData(data);
            gvAllTeams.GridViewItems = gvd;
            gvAllTeams.DataSource = data;
            gvAllTeams.DataBind();
        }

        protected void lnkFilter_Click(object sender, EventArgs e)
        {
            gvAllTeams.DataBind();
        }

    }
}