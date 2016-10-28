using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Trackr;
using Trackr.Source.Controls.TGridView;
using TrackrModels;
using Trackr.Utils;
using Trackr.Providers;
using Trackr.Source.Controls;

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

            DisplayHidden(false);
        }

        private void BindTeams()
        {
            List<TeamsController.TeamViewObject> data = null;
            using (TeamsController tc = new TeamsController())
            {
                data = tc.GetScopedTeamViewObject(CurrentUser.UserID, Permissions.TeamManagement.ViewTeams).ToList();
            }

            GridViewData<TeamsController.TeamViewObject> gvd = new GridViewData<TeamsController.TeamViewObject>(typeof(TeamsController.TeamViewObject));
            gvd.AddData(data);
            gvAllTeams.GridViewItems = gvd;
            gvAllTeams.DataSource = data;
            gvAllTeams.DataBind();

            // bind widgets

            rptWidgets.DataSource = data.Where(i => DateTime.Today <= i.End)
                .OrderBy(i => i.ProgramName).ThenByDescending(i => i.AgeCutoff)
                .Select(i => i.TeamID);
            rptWidgets.DataBind();            
        }

        

        protected void lnkFilter_Click(object sender, EventArgs e)
        {
            var data = gvAllTeams.GridViewItems.ManipulateOriginal(i => (!ptpPicker.SelectedTeamID.HasValue || ptpPicker.SelectedTeamID == i.TeamID) && (!ptpPicker.SelectedProgramID.HasValue || ptpPicker.SelectedProgramID == i.ProgramID));

            gvAllTeams.DataSource = data;
            gvAllTeams.DataBind();
        }

        private void DisplayHidden(bool display)
        {
            foreach (Control rptItem in rptWidgets.Controls)
            {
                Trackr.Source.Controls.Widgets.RegistrationProgress control = rptItem.Controls[1] as Trackr.Source.Controls.Widgets.RegistrationProgress;

                if (display)
                {
                    control.Show(true);
                }
                else
                {
                    if (Profile.TeamManagement.RegistrationProgress_TeamIDsHide.Contains(control.TeamID))
                    {
                        control.Show(false);
                    }
                    else
                    {
                        control.Show(true);
                    }
                }
            }
        }

        protected void lnkShowHideRegProg_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;

            if (btn.CommandArgument == "show")
            {
                DisplayHidden(true);
                btn.Text = "Hide Hidden Teams";
                btn.CommandArgument = "hide";
            }
            else
            {
                DisplayHidden(false);
                btn.Text = "Show Hidden Teams";
                btn.CommandArgument = "show";
            }
        }
    }
}