using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Trackr.Utils;
using TrackrModels;

namespace Trackr.Source.Controls
{
    public partial class PhoneNumberBook : UserControl
    {
        public delegate IList<PhoneNumber> GetPhoneNumbers(Guid personEditToken);

        public Guid? PersonEditToken
        {
            get { return ViewState["PersonEditToken"] as Guid?; }
            set { ViewState["PersonEditToken"] = value; }
        }

        public GetPhoneNumbers GetData { get; set; }

        private IList<PhoneNumber> PhoneNumbers
        {
            get
            {
                if (PersonEditToken.HasValue)
                {
                    return GetData(PersonEditToken.Value);
                }
                else
                {
                    return new List<PhoneNumber>();
                }
            }
        }

        private void ClearForm()
        {
            txtExtension.Text = "";
            txtPhoneNumber.Text = "";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AlertBox.HideStatus();

            if (IsPostBack)
            {
                return;
            }

            gvPhoneNumberBook.DataBind();
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

        protected void lnkSavePhoneNumber_Click(object sender, EventArgs e)
        {
            Guid editToken = gvPhoneNumberBook.EditIndex == -1 ? Guid.NewGuid() : (Guid)gvPhoneNumberBook.DataKeys[gvPhoneNumberBook.EditIndex].Value;

            PhoneNumber phoneNumber = PhoneNumbers.FirstOrDefault(i => i.EditToken == editToken) ?? new PhoneNumber() { EditToken = Guid.NewGuid() };

            phoneNumber.TenDigit = string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ? null : UserInputUtils.GetTenDigitNumber(txtPhoneNumber.Text);
            phoneNumber.Extension = string.IsNullOrWhiteSpace(txtExtension.Text) ? null : txtExtension.Text.Trim();
            phoneNumber.WasModified = true;

            if (PhoneNumbers.FirstOrDefault(i => i.EditToken == editToken) == null)
            {
                phoneNumber.SortOrder = Convert.ToByte(PhoneNumbers.Count());
                PhoneNumbers.Add(phoneNumber);
            }

            divEdit.Visible = false;
            ClearForm();
            gvPhoneNumberBook.EditIndex = -1;
            gvPhoneNumberBook.DataBind();
        }

        protected void lnkAddPhoneNumber_Click(object sender, EventArgs e)
        {
            divEdit.Visible = true;
            ClearForm();
        }

        public IQueryable gvPhoneNumberBook_GetData()
        {
            return (PhoneNumbers ?? new List<PhoneNumber>()).AsQueryable();
        }

        public void gvPhoneNumberBook_DeleteItem(Guid EditToken)
        {
            PhoneNumber phoneNumber = PhoneNumbers.First(i => i.EditToken == EditToken);
            PhoneNumbers.Remove(phoneNumber);
            gvPhoneNumberBook.DataBind();

            ClearForm();
            divEdit.Visible = false;
        }

        protected void gvPhoneNumberBook_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPhoneNumberBook.EditIndex = -1;
            gvPhoneNumberBook.DataBind();

            ClearForm();
            divEdit.Visible = false;
        }

        protected void gvPhoneNumberBook_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid editToken = (Guid)gvPhoneNumberBook.DataKeys[e.NewEditIndex].Value;
            Populate_PhoneNumberEdit(editToken);
            gvPhoneNumberBook.EditIndex = e.NewEditIndex;
            gvPhoneNumberBook.DataBind();
        }

        private void Populate_PhoneNumberEdit(Guid editToken)
        {
            PhoneNumber phoneNumber = PhoneNumbers.First(i => i.EditToken == editToken);

            txtExtension.Text = phoneNumber.Extension;
            txtPhoneNumber.Text = Utils.UserInputUtils.FormatTenDigitNumber(phoneNumber.TenDigit);
            divEdit.Visible = true;
        }

        protected void valFormatPhoneNumber_ServerValidate(object source, ServerValidateEventArgs args)
        {
            MatchCollection matchesCorrectNumberOfNumbers = Regex.Matches(args.Value, @"[0-9]");
            MatchCollection matchesCorrectNumberOfNonChars = Regex.Matches(args.Value, @"[^)(\s-0-9]");

            // There should be EXACTLY 10 0-9 digits
            args.IsValid = matchesCorrectNumberOfNumbers.Count == 10 && matchesCorrectNumberOfNonChars.Count == 0;
        }
    }
}