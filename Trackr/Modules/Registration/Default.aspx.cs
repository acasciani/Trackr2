using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.OpenAccess;
using Telerik.OpenAccess.FetchOptimization;
using Telerik.OpenAccess.Metadata;
using Trackr.Utils;
using TrackrModels;

namespace Trackr.Modules.Registration
{
    public partial class Default : Page
    {
        [Serializable]
        private class PlayerMatch
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public int PlayerID { get; set; }
        }

        private int? PlayerID { get; set; }
        public int RegistrationYear { get; private set; }
        public string ClubName { get; private set; }
        public Stack<int> StepHistory
        {
            get
            {
                Stack<int> stack = ViewState["StepHistory"] as Stack<int>;
                if (stack == null)
                {
                    stack = new Stack<int>();
                    ViewState["StepHistory"] = stack;
                }
                return stack;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Telerik.OpenAccess.BackendConfiguration configuration = new Telerik.OpenAccess.BackendConfiguration();
            configuration.Backend = "SQLite";


            using (ClubManagement cm = new ClubManagement("StagingConnection"))
            {
                var clubs = cm.Clubs;
            }





            ClubName = "Gananda Bandits";
            RegistrationYear = 2017;

            if (IsPostBack)
            {
                return;
            }

            List<PlayerMatch> matches = GetPossiblePlayersToRegister(CurrentUser.UserID);
            gvPossiblePlayerMatches.DataSource = matches;
            gvPossiblePlayerMatches.DataBind();
            divNoPlayersFoundMessage.Visible = matches.Count() == 0;
            divPlayersFoundMessage.Visible = matches.Count() > 0;
        }


        protected void validatorDateTimeParses_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime date;
            args.IsValid = DateTime.TryParse(args.Value, out date) && new DateTime(1900, 1, 1) <= date && date <= new DateTime(2200, 1, 1);
        }

        protected void lnkBackStep_Click(object sender, EventArgs e)
        {
            int previousStep = StepHistory.Pop();
            mvRegister.ActiveViewIndex = previousStep;
        }

        protected void lnkContinueStep_Click(object sender, EventArgs e)
        {
            switch (mvRegister.ActiveViewIndex)
            {
                case 0: // registration selection
                    string playerID_selection = Request["SelectedPlayer"];

                    if (string.IsNullOrWhiteSpace(playerID_selection))
                    {
                        AlertBox.AddAlert("Please select a player to register, or select to register a first year player.", false, UI.AlertBoxType.Error);
                        return;
                    }

                    if (playerID_selection == "new")
                    {
                        // new

                    }
                    else
                    {
                        //re register
                        widgetPlayerManagement.PrimaryKey = int.Parse(playerID_selection);
                    }

                    widgetPlayerManagement.Reload();


                    break;

                default: break;
            }



            if (mvRegister.ActiveViewIndex == mvRegister.Views.Count)
            {
                // last step
            }
            else
            {
                StepHistory.Push(mvRegister.ActiveViewIndex);
                mvRegister.ActiveViewIndex += 1;
            }
        }

        protected void lnkSelectPlayer_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                PlayerID = int.Parse(btn.CommandArgument);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }





        #region Find possible players to register for email address
        private List<PlayerMatch> GetPossiblePlayersToRegister(int userID)
        {
            using (PeopleController pc = new PeopleController())
            using (EmailAddressesController eac = new EmailAddressesController())
            {
                FetchStrategy fetch = new FetchStrategy() { MaxFetchDepth = 5 };
                fetch.LoadWith<Person>(i => i.Guardians);
                fetch.LoadWith<Guardian>(i => i.Player);
                fetch.LoadWith<EmailAddress>(i => i.Person);
                fetch.LoadWith<Player>(i => i.Person);

                var playersAssociatedToUserID = pc.GetWhere(i => i.UserID.HasValue && userID == i.UserID.Value, fetch).SelectMany(i => i.Guardians)
                    .Select(i => new PlayerMatch()
                    {
                        PlayerID = i.PlayerID,
                        FirstName = i.Player.Person.FName,
                        LastName = i.Player.Person.LName,
                        DateOfBirth = i.Player.Person.DateOfBirth
                    });

                return playersAssociatedToUserID.OrderBy(i => i.FirstName).ThenBy(i => i.LastName).ThenBy(i => i.DateOfBirth).ToList();
            }
        }
        #endregion


    }
}