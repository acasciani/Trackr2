using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Trackr.Modules.UserManagement
{
    public partial class Spoof : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            CheckAllowed(Permissions.UserManagement.SpoofUsers);
        }

        public IQueryable ddlUsers_GetData()
        {
            Dictionary<int, string> users =new Dictionary<int,string>();

            using (TrackrModels.UserManagement um = new TrackrModels.UserManagement())
            {
                users =um.WebUsers.Select(i => new { UserID = i.UserID, Email = i.Email }).ToDictionary(i => i.UserID, i => i.Email);
            }

            List<int> userIDs = users.Keys.ToList();
            using (TrackrModels.ClubManagement cm = new TrackrModels.ClubManagement())
            {
                cm.People.Where(i => i.UserID.HasValue && userIDs.Contains(i.UserID.Value)).Select(i => new { UserID = i.UserID.Value, FName = i.FName, LName = i.LName }).ToList().ForEach(i =>
                {
                    bool hasL = !string.IsNullOrWhiteSpace(i.LName);
                    bool hasF = !string.IsNullOrWhiteSpace(i.FName);
                    users[i.UserID] = hasL && hasF ? string.Format("{0}, {1} - {2}", i.LName, i.FName, users[i.UserID]) : hasL || hasF ? string.Format("{0} - {1}", string.IsNullOrWhiteSpace(i.LName) ? i.FName : i.LName, users[i.UserID]) : users[i.UserID];
                });
            }

            return users.Select(i => new { Label = i.Value, Value = i.Key, HasName = i.Value.IndexOf(" - ") > -1 })
                .OrderBy(i=>i.HasName).ThenBy(i=>i.Label).AsQueryable();
        }

        protected void btnSpoof_Click(object sender, EventArgs e)
        {
            int userID = int.Parse(ddlUsers.SelectedValue);

            FormsAuthentication.RedirectFromLoginPage(userID.ToString(), false);
        }
    }
}