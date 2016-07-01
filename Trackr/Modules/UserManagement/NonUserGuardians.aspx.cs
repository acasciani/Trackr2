using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Trackr.Utils;
using TrackrModels;

namespace Trackr.Modules.UserManagement
{
    public partial class NonUserGuardians : Page
    {
        [Serializable]
        private class NonUserGuardianResult
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public List<string> EmailAddresses { get; set; }
            public List<string> Players { get; set; }
            public List<int> PlayerIDs { get; set; }
            public int PersonID { get; set; }
        }

        private List<NonUserGuardianResult> NonUserGuardianResults
        {
            get
            {
                return Session["Results"] as List<NonUserGuardianResult>;
            }
            set
            {
                Session["Results"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            CheckAllowed(Permissions.UserManagement.CreateUsersFromGuardians);
        }


        public IQueryable gvAllNonUsers_GetData()
        {
            using(ClubManagement cm = new  ClubManagement())
            {
                var guardians = cm.Guardians.Where(i => i.Active && !i.Person.UserID.HasValue).Select(i => new
                {
                    PersonID = i.PersonID,
                    Player = i.Player.Person.FName + " " + i.Player.Person.LName,
                    PlayerID = i.PlayerID
                })
                .GroupBy(i => i.PersonID)
                .Select(i => new
                {
                    PersonID = i.Key,
                    PlayerNames = i.Select(j => j.Player).ToList(),
                    PlayerIDs = i.Select(j=>j.PlayerID).Distinct().ToList()
                })
                .ToDictionary(i => i.PersonID);

                List<int> peopleIDs = guardians.Keys.ToList();

                List<NonUserGuardianResult> nonUsers = cm.People.Where(i => peopleIDs.Contains(i.PersonID)).Select(i => new NonUserGuardianResult()
                {
                     FirstName = i.FName,
                     LastName = i.LName,
                     PersonID = i.PersonID,
                     EmailAddresses = i.EmailAddresses.Where(j=>j.Active).Select(j=>j.Email.Trim()).Distinct().ToList(),
                     Players = guardians[i.PersonID].PlayerNames,
                     PlayerIDs = guardians[i.PersonID].PlayerIDs
                }).ToList();

                NonUserGuardianResults = nonUsers;

                return nonUsers.OrderBy(i => i.LastName).ThenBy(i => i.FirstName).AsQueryable<NonUserGuardianResult>();
            }
        }

        protected void lnkCreateAccount_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int personID = int.Parse(btn.CommandArgument);

            hdnPersonID.Value = personID.ToString();

            using (TrackrModels.UserManagement um = new TrackrModels.UserManagement())
            {
                List<string> emailAddressesForGuardian = NonUserGuardianResults.First(i => i.PersonID == personID).EmailAddresses;

                radioList.DataSource = emailAddressesForGuardian;
                radioList.DataBind();

                litGuardianName.Text = NonUserGuardianResults.First(i => i.PersonID == personID).FirstName + " " + NonUserGuardianResults.First(i => i.PersonID == personID).LastName;
                litPlayerNames.Text = string.Join(", ", NonUserGuardianResults.First(i => i.PersonID == personID).Players);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "modal", "$('.add-non-user-guardian-as-user').modal('toggle')", true);
            }
        }

        protected void btnCreateUserAccounts_Click(object sender, EventArgs e)
        {
            int personID = int.Parse(hdnPersonID.Value);
            string email = radioList.SelectedValue.Trim();
            string password = txtPassword.Text.Trim();

            List<int> playerIDs = NonUserGuardianResults.First(i=>i.PersonID == personID).PlayerIDs.Distinct().ToList();

            using(TrackrModels.UserManagement um = new TrackrModels.UserManagement())
            using (ClubManagement cm = new ClubManagement())
            using (NewUserMappingsController numc = new NewUserMappingsController())
            {
                MembershipCreateStatus status;
                MembershipUser user = Membership.CreateUser(email, password, email, null, null, true, out status);

                switch (status)
                {
                    case MembershipCreateStatus.Success:
                        int userID = (int)user.ProviderUserKey;

                        um.WebUsers.First(i => i.UserID == userID).ClubID = CurrentUser.ClubID;
                        um.SaveChanges();

                        var roleIDs = numc.GetWhere(i => i.ClubID == CurrentUser.ClubID).Select(i => i.RoleID).Distinct().ToList();

                        foreach (int roleID in roleIDs)
                        {
                            ScopeAssignment assignment = new ScopeAssignment()
                            {
                                IsDeny = false,
                                ScopeID = 5,
                                UserID = userID,
                                ResourceID = userID,
                                RoleID = roleID
                            };
                            um.Add(assignment);
                        }

                        foreach (int playerID in playerIDs)
                        {
                            ScopeAssignment assignment = new ScopeAssignment()
                            {
                                IsDeny = false,
                                ScopeID = 4, //player scope
                                UserID = userID,
                                ResourceID = playerID,
                                RoleID = 6 //parent role
                            };
                            um.Add(assignment);
                        }

                        cm.Add(new WebUserInfo()
                        {
                            FName = "",
                            LName = "",
                            UserID = userID
                        });

                        try
                        {
                            // try to save scoep assignments, if it fails then rollback user. i.e. delete user
                            um.SaveChanges();
                            cm.SaveChanges();

                            Person person =cm.People.First(i => i.PersonID == personID);
                            person.UserID = userID;

                            cm.SaveChanges();

                            try
                            {
                                List<Trackr.Utils.Messenger.EmailRecipient> recipients = new List<Trackr.Utils.Messenger.EmailRecipient>();
                                recipients.Add(new Trackr.Utils.Messenger.EmailRecipient()
                                {
                                    Email = email,
                                    Name = person.FName + " " + person.LName,
                                    RecipientType = Trackr.Utils.Messenger.EmailRecipientType.TO
                                });

                                List<Trackr.Utils.Messenger.TemplateVariable> variables = new List<Trackr.Utils.Messenger.TemplateVariable>();
                                variables.Add(new Trackr.Utils.Messenger.TemplateVariable()
                                {
                                    VariableName = "PersonName",
                                    VariableContent = person.FName + " " + person.LName
                                });
                                variables.Add(new Trackr.Utils.Messenger.TemplateVariable()
                                {
                                    VariableName = "LoginEmail",
                                    VariableContent = email
                                });
                                variables.Add(new Trackr.Utils.Messenger.TemplateVariable()
                                {
                                    VariableName = "LoginPassword",
                                    VariableContent = password
                                });

                                Trackr.Utils.Messenger.SendEmail("user-account-created", null, variables, recipients, false, false);

                                AlertBox.AddAlert(string.Format("Successfully created user account for {0} and emailed them their login information.", email));
                            }
                            catch (Exception ex)
                            {
                                Guid guid = ex.HandleException();
                                AlertBox.AddAlert(string.Format("Successfully created user account for {0} but failed to email them their login information. Reference #: " + guid, email), false, UI.AlertBoxType.Error);
                            }
                            finally
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "modal", "$('.modal-backdrop').remove()", true);
                                gvAllNonUsers.DataBind();

                                uP_Main.Update();
                            }
                        }
                        catch (Exception ex)
                        {
                            Guid guid = ex.HandleException();
                            CreateAlertBox.AddAlert("An error occurred while trying to create new user from guardian. Please try again. Reference #: " + guid, false, UI.AlertBoxType.Error);
                            um.ClearChanges();
                            cm.ClearChanges();

                            // remove any assignments
                            um.Delete(um.ScopeAssignments.Where(i => i.UserID == userID).ToList());
                            um.Delete(um.WebUsers.First(i => i.UserID == userID));
                            um.SaveChanges();
                        }
                        break;

                    default:
                        CreateAlertBox.AddAlert("Unable to create new user from guardian for the following reason: " + status.ToString(), false, UI.AlertBoxType.Error);
                        break;
                }
            }
        }
    }
}