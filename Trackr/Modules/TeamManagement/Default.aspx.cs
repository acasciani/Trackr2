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

namespace Trackr.Modules.TeamManagement
{
    public partial class Default : Page
    {
        private bool ShowHidden
        {
            get { return ((Trackr.Page)Page).Profile.TeamManagement.RegistrationProgress_ShowHidden; }
            set
            {
                Trackr.Providers.Profiles.UserProfile profile = ((Trackr.Page)Page).Profile;
                profile.TeamManagement.RegistrationProgress_ShowHidden = value;
                profile.Save();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            CheckAllowed(Permissions.TeamManagement.ViewTeams);

            if (ShowHidden)
            {
                lnkShowHideRegProg.Text = "Hide Hidden Teams";
            }
            else
            {
                lnkShowHideRegProg.Text = "Show Hidden Teams";
            }

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

            // bind widgets
            BindRegistrationProgressWidgets(ShowHidden);            
        }

        

        protected void lnkFilter_Click(object sender, EventArgs e)
        {
            gvAllTeams.DataBind();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                LinkButton btn = FindControl(Request.Params.Get("__EVENTTARGET")) as LinkButton;

                if (btn == lnkShowHideRegProg)
                {
                    BindRegistrationProgressWidgets(ShowHidden);
                }
            }
        }

        protected void lnkShowHideRegProg_Click(object sender, EventArgs e)
        {
            LinkButton btn = sender as LinkButton;

            if (ShowHidden)
            {
                btn.Text = "Hide Hidden Teams";
            }
            else
            {
                btn.Text = "Show Hidden Teams";
            }

            ShowHidden = !ShowHidden;
        }

        private void BindRegistrationProgressWidgets(bool isShow)
        {
            List<int> hiddenTeamIDsRegProg = isShow ? new List<int>() : Profile.TeamManagement.RegistrationProgress_TeamIDsHide;

            IEnumerable<TeamsController.TeamViewObject> data = gvAllTeams.GridViewItems.Data.Cast<TeamsController.TeamViewObject>().ToList();

            rptWidgets.DataSource = data.Where(i => DateTime.Today <= i.End && !hiddenTeamIDsRegProg.Contains(i.TeamID))
                .OrderBy(i => i.ProgramName).ThenByDescending(i => i.AgeCutoff)
                .Select(i => i.TeamID);
            rptWidgets.DataBind();
        }
    }
}