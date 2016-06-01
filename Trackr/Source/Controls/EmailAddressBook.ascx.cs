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
        public delegate IList<EmailAddress> GetEmailAddresses(Guid personEditToken);

        public Guid? PersonEditToken
        {
            get { return ViewState["PersonEditToken"] as Guid?; }
            set { ViewState["PersonEditToken"] = value; }
        }

        public GetEmailAddresses GetData { get; set; }

        private IList<EmailAddress> EmailAddresses {
            get
            {
                if (PersonEditToken.HasValue)
                {
                    return GetData(PersonEditToken.Value);
                }
                else
                {
                    return new List<EmailAddress>();
                }
            }
        }

        private void ClearForm()
        {
            txtEmailAddress.Text = "";
            chkIsHTML.Checked = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AlertBox.HideStatus();

            lnkAddEmailAddress.Visible = !divEdit.Visible;

            if (IsPostBack)
            {
                return;
            }

            Reload();
        }

        public void Reload()
        {
            gvEmailAddressBook.DataBind();
        }

        public void Reset()
        {
            ClearForm();
            divEdit.Visible = false;
        }

        public void HideForm()
        {
            ClearForm();
            divEdit.Visible = false;
        }

        protected void lnkSaveEmailAddress_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            Guid editToken = gvEmailAddressBook.EditIndex == -1 ? Guid.NewGuid() : (Guid)gvEmailAddressBook.DataKeys[gvEmailAddressBook.EditIndex].Value;

            EmailAddress emailAddress = EmailAddresses.FirstOrDefault(i => i.EditToken == editToken) ?? new EmailAddress() { EditToken = Guid.NewGuid() };

            emailAddress.Email = string.IsNullOrWhiteSpace(txtEmailAddress.Text) ? null : txtEmailAddress.Text.Trim();
            emailAddress.IsHTML = chkIsHTML.Checked;
            emailAddress.WasModified = true;

            if (EmailAddresses.FirstOrDefault(i => i.EditToken == editToken) == null)
            {
                emailAddress.Active = true;
                emailAddress.SortOrder = Convert.ToByte(EmailAddresses.Count());
                EmailAddresses.Add(emailAddress);
            }

            lnkAddEmailAddress.Visible = true;
            divEdit.Visible = false;
            ClearForm();
            AlertBox.AddAlert(string.Format("Successfully {0} email address. Your settings will be saved when you complete this wizard.", gvEmailAddressBook.EditIndex == -1 ? "added" : "edited"), false, UI.AlertBoxType.Warning);
            gvEmailAddressBook.EditIndex = -1;
            gvEmailAddressBook.DataBind();
        }

        protected void lnkAddEmailAddress_Click(object sender, EventArgs e)
        {
            lnkAddEmailAddress.Visible = false;
            divEdit.Visible = true;
            ClearForm();
        }

        public IQueryable gvEmailAddressBook_GetData()
        {
            return (EmailAddresses ?? new List<EmailAddress>()).Where(i => i.Active).AsQueryable();
        }

        public void gvEmailAddressBook_DeleteItem(Guid EditToken)
        {
            EmailAddress emailAddress = EmailAddresses.First(i => i.EditToken == EditToken);

            if (emailAddress.EmailAddressID > 0)
            {
                emailAddress.Active = false;
                emailAddress.WasModified = true;
            }
            else
            {
                EmailAddresses.Remove(emailAddress);
            }

            gvEmailAddressBook.DataBind();

            ClearForm();
            divEdit.Visible = false;
        }

        protected void gvEmailAddressBook_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvEmailAddressBook.EditIndex = -1;
            gvEmailAddressBook.DataBind();

            ClearForm();
            divEdit.Visible = false;
            lnkAddEmailAddress.Visible = true;
        }

        protected void gvEmailAddressBook_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid editToken = (Guid)gvEmailAddressBook.DataKeys[e.NewEditIndex].Value;
            Populate_EmailAddressEdit(editToken);
            gvEmailAddressBook.EditIndex = e.NewEditIndex;
            gvEmailAddressBook.DataBind();
        }

        private void Populate_EmailAddressEdit(Guid editToken)
        {
            EmailAddress emailAddress = EmailAddresses.First(i => i.EditToken == editToken);

            txtEmailAddress.Text = emailAddress.Email;
            chkIsHTML.Checked = emailAddress.IsHTML;
            divEdit.Visible = true;
        }

        protected void lnkCancel_Click(object sender, EventArgs e)
        {
            gvEmailAddressBook_RowCancelingEdit(null, null);
        }
    }
}