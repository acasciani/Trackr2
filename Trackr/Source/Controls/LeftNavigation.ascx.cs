using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;
using Telerik.OpenAccess;

namespace Trackr.Source.Controls
{
    public partial class LeftNavigation : UserControl
    {
        [Serializable]
        private class MyLink
        {
            public string Name { get; set; }
            public string Glyphicon { get; set; }
            public List<MyLink> MyLinks { get; set; }
            public bool IsGroup { get; set; }
            public string CurrentModulePattern { get; set; }
            public string LinkURL { get; set; }
            public string Tooltip { get; set; }
            public bool ShowInMobileNav { get; set; }
        }

        public bool IsLeftSide { get; set; }

        private List<MyLink> MyLinks
        {
            get
            {
                List<MyLink> myLinks = Session["MyPermissionedLinks"] as List<MyLink>;

                if (myLinks == null)
                {
                    myLinks = GetPermissionedLinksFromDB();
                    Session["MyPermissionedLinks"] = myLinks;
                }

                return myLinks;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            List<MyLink> myLinks = MyLinks;

            List<MyLink> noGroups = myLinks.Where(i => !i.IsGroup && ((!IsLeftSide && i.ShowInMobileNav) || IsLeftSide)).ToList();
            List<MyLink> groups = myLinks.Where(i => i.IsGroup && ((!IsLeftSide && i.ShowInMobileNav) || IsLeftSide)).ToList();

            navLeftSide.Visible = IsLeftSide;
            navTopPart.Visible = !IsLeftSide;

            if (noGroups.Count() > 0)
            {
                rptNoGroup.DataSource = noGroups;
                rptNoGroup.DataBind();

                rptNavMobile_NoGroups.DataSource = noGroups;
                rptNavMobile_NoGroups.DataBind();

                ulNoGroups.Visible = true;
            }
            else
            {
                ulNoGroups.Visible = false;
            }

            if (groups.Count() > 0)
            {
                rptGroup.DataSource = groups;
                rptGroup.DataBind();

                rptNavMobile_Groups.DataSource = groups;
                rptNavMobile_Groups.DataBind();

                ulGroups.Visible = true;
            }
            else
            {
                ulGroups.Visible = false;
            }

            if (noGroups.Count() > 0 || groups.Count() > 0)
            {
                ulMobileNav.Visible = true;
            }
            else
            {
                ulMobileNav.Visible = false;
            }
        }

        public bool IsCurrentModule(string moduleName)
        {
            if (moduleName == null)
            {
                return false;
            }
            return Request.Url.LocalPath.ToUpper().StartsWith("/MODULES/" + moduleName.ToUpper());
        }


        private List<MyLink> GetPermissionedLinksFromDB()
        {
            using(UserManagement um = new UserManagement())
            using (WebUsersController wuc = new WebUsersController())
            using(LinksController lc = new LinksController())
            {
                FetchStrategy fetch = new FetchStrategy();
                fetch.LoadWith<Link>(i => i.LinkGroup);
                fetch.LoadWith<Link>(i => i.Glyphicon);
                fetch.LoadWith<LinkGroup>(i => i.Glyphicon);
                fetch.LoadWith<Link>(i => i.LinkPermissions);


                IEnumerable<ScopeAssignment> assignedScopes = um.ScopeAssignments.Where(i => i.UserID == CurrentUser.UserID);

                // if user has even one allowed permission, then we need to add it. need to do it by permission then by role
                var allowedByPermission = assignedScopes.Where(i => i.PermissionID.HasValue && !i.IsDeny).Select(i => i.PermissionID.Value);
                var allowedByRole = assignedScopes.Where(i => i.RoleID.HasValue && !i.IsDeny).Select(i => i.Role).SelectMany(i => i.Permissions).Select(i => i.PermissionID);

                List<int> allowed = allowedByPermission.Union(allowedByRole).Distinct().ToList();
                
                // now get the links
                var links = lc.GetWhere(i => i.LinkPermissions.Count() == 0 || i.LinkPermissions.Where(j => !j.IsDeny && allowed.Contains(j.PermissionID)).Count() > 0, fetch);

                var noGroups = links.Where(i => !i.LinkGroupID.HasValue)
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new MyLink()
                    {
                        IsGroup = false,
                        Glyphicon = i.GlyphiconID.HasValue ? i.Glyphicon.Glyphicon1 : null,
                        Name = i.LinkName,
                        CurrentModulePattern = i.CurrentModulePattern,
                        LinkURL = i.LinkURL,
                        Tooltip = i.LinkTitle,
                        ShowInMobileNav = i.ShowInMobileNav
                    }).ToList();

                var groups = links.Where(i => i.LinkGroupID.HasValue)
                    .GroupBy(i => i.LinkGroupID)
                    .OrderBy(i => i.First().LinkGroup.SortOrder)
                    .Select(i => new MyLink()
                    {
                        IsGroup = true,
                        Name = i.First().LinkGroup.LinkGroupName,
                        Glyphicon = i.First().LinkGroup.GlyphiconID.HasValue ? i.First().LinkGroup.Glyphicon.Glyphicon1 : null,
                        CurrentModulePattern = i.First().LinkGroup.CurrentModulePattern,
                        Tooltip = i.First().LinkGroup.LinkGroupName,
                        ShowInMobileNav = i.First().LinkGroup.ShowInMobileNav,
                        MyLinks = i.OrderBy(j => j.SortOrder).Select(j => new MyLink()
                        {
                            Glyphicon = j.GlyphiconID.HasValue ? j.Glyphicon.Glyphicon1 : null,
                            IsGroup = false,
                            Name = j.LinkName,
                            CurrentModulePattern = j.CurrentModulePattern,
                            Tooltip = j.LinkTitle,
                            LinkURL = j.LinkURL,
                            ShowInMobileNav = j.ShowInMobileNav
                        }).ToList()
                    }).ToList();

                List<MyLink> myLinks = new List<MyLink>();
                myLinks.AddRange(noGroups);
                myLinks.AddRange(groups);

                return myLinks;
            }
        }


    }
}