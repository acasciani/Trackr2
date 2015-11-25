using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackrModels;

namespace Trackr.Source.Controls
{
    public partial class EmailAddressBook : UserControl
    {
        [Serializable]
        private class EmailResult
        {
            public string EmailAddress { get; set; }
            public int EmailAddressID { get; set; }
        }

        public int? PersonID
        {
            get { return ViewState["EmailAddressBookPersonID"] as int?; }
            set { ViewState["EmailAddressBookPersonID"] = value; }
        }

        private List<EmailResult> EmailResults
        {
            get { return ViewState["EmailResults"] as List<EmailResult>; }
            set { ViewState["EmailResults"] = value; }
        }

        private void ClearForm()
        {
            txtEmailAddress.Text = "";
            chkIsHTML.Checked = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AlertBox.HideStatus();
        }

        public void Reset()
        {
            ClearForm();
            divEdit.Visible = false;
            PersonID = null;
        }

        protected void lnkSaveEmailAddress_Click(object sender, EventArgs e)
        {
            if (!PersonID.HasValue)
            {
                throw new Exception("Email address book does not have a person ID.");
            }

            int? emailAddressID = gvEmailAddressBook.EditIndex == -1 ? (int?)null : (int)gvEmailAddressBook.DataKeys[gvEmailAddressBook.EditIndex].Value;

            using (EmailAddressesController eac = new EmailAddressesController())
            {
                EmailAddress emailAddress = emailAddressID.HasValue ? eac.Get(emailAddressID.Value) : new EmailAddress();

                emailAddress.EmailAddress1 = string.IsNullOrWhiteSpace(txtEmailAddress.Text) ? null : txtEmailAddress.Text.Trim();
                emailAddress.IsHTML = chkIsHTML.Checked;

                if (emailAddressID.HasValue)
                {
                    // edit
                    eac.Update(emailAddress);
                }
                else
                {
                    // add
                    emailAddress.PersonID = PersonID.Value;
                    emailAddress.SortOrder = Convert.ToByte(eac.GetWhere(i => i.PersonID == PersonID).Count());
                    eac.AddNew(emailAddress);
                }
            }

            gvEmailAddressBook.DataBind();
            divEdit.Visible = false;
            ClearForm();
            AlertBox.SetStatus("Successfully saved email address.");
        }

        protected void lnkAddEmailAddress_Click(object sender, EventArgs e)
        {
            divEdit.Visible = true;
            ClearForm();
        }

        public IQueryable gvEmailAddressBook_GetData()
        {
            using (EmailAddressesController eac = new EmailAddressesController())
            {
                EmailResults = eac.GetWhere(i => i.PersonID == PersonID).OrderBy(i => i.SortOrder).Select(i => new EmailResult()
                {
                    EmailAddress = i.EmailAddress1,
                    EmailAddressID = i.EmailAddressID
                }).ToList();

                return EmailResults.AsQueryable();
            }
        }

        public void gvEmailAddressBook_DeleteItem(int EmailAddressID)
        {
            using (EmailAddressesController eac = new EmailAddressesController())
            {
                eac.Delete(EmailAddressID);
                gvEmailAddressBook.DataBind();

                ClearForm();
                divEdit.Visible = false;

                AlertBox.SetStatus("Successfully removed email address.");
            }
        }

        protected void gvEmailAddressBook_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvEmailAddressBook.EditIndex = -1;
            gvEmailAddressBook.DataBind();

            ClearForm();
            divEdit.Visible = false;
        }

        protected void gvEmailAddressBook_RowEditing(object sender, GridViewEditEventArgs e)
        {
            int emailAddressID = (int)gvEmailAddressBook.DataKeys[e.NewEditIndex].Value;
            Populate_EmailAddressEdit(emailAddressID);
            gvEmailAddressBook.EditIndex = e.NewEditIndex;
            gvEmailAddressBook.DataBind();
        }

        private void Populate_EmailAddressEdit(int emailAddressID)
        {
            using (EmailAddressesController eac = new EmailAddressesController())
            {
                EmailAddress emailAddress = eac.Get(emailAddressID);

                txtEmailAddress.Text = emailAddress.EmailAddress1;
                chkIsHTML.Checked = emailAddress.IsHTML;

                divEdit.Visible = true;
            }
        }
    }
}