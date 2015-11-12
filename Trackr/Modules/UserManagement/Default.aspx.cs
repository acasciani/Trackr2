using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Trackr;
using TrackrModels;

namespace Trackr.Modules.UserManagement
{
    public partial class Default : Page
    {
        [Serializable]
        private class UserResult
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public int UserID { get; set; }
        }

        // The return type can be changed to IEnumerable, however to support
        // paging and sorting, the following parameters must be added:
        //     int maximumRows
        //     int startRowIndex
        //     out int totalRowCount
        //     string sortByExpression
        public IQueryable gvAllUsers_GetData()
        {
            using (WebUsersController wuc = new WebUsersController())
            using(WebUserInfosController wuic = new WebUserInfosController())
            {
                var allInfo = wuic.Get().Select(i=> new {
                    UserID = i.UserID,
                    FirstName = i.FName,
                    LastName = i.LName
                });
                var allUsers = wuc.Get();

                var joined = allUsers.GroupJoin(allInfo, i => i.UserID, j => j.UserID, (i, j) => new { Login = i, Info = j.FirstOrDefault() });

                return joined.Select(i => new UserResult()
                {
                    Email = i.Login.Email,
                    FirstName = i.Info == null ? null : i.Info.FirstName,
                    LastName = i.Info == null ? null : i.Info.LastName,
                    UserID = i.Login.UserID
                }).OrderBy(i => i.LastName).ThenBy(i => i.FirstName).AsQueryable<UserResult>();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            CheckAllowed(Permissions.UserManagement.ViewUsers);
        }
    }
}