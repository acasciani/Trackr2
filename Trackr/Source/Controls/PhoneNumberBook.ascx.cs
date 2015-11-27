using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackrModels;

namespace Trackr.Source.Controls
{
    public partial class PhoneNumberBook : UserControl
    {
        [Serializable]
        private class PhoneNumberResult
        {
            public string PhoneNumber { get; set; }
            public int PhoneNumberID { get; set; }
        }

        public int? PersonID
        {
            get { return ViewState["PhoneNumberBookPersonID"] as int?; }
            set { ViewState["PhoneNumberBookPersonID"] = value; }
        }

        private List<PhoneNumberResult> PhoneNumberResults
        {
            get { return ViewState["PhoneNumberResults"] as List<PhoneNumberResult>; }
            set { ViewState["PhoneNumberResults"] = value; }
        }

        private void ClearForm()
        {
            txtPhoneNumber.Text = "";
            txtExtension.Text = "";
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

        public void HideForm()
        {
            ClearForm();
            divEdit.Visible = false;
        }

        protected void lnkSavePhoneNumber_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            if (!PersonID.HasValue)
            {
                throw new Exception("Phone number book does not have a person ID.");
            }

            int? phoneNumberID = gvPhoneNumberBook.EditIndex == -1 ? (int?)null : (int)gvPhoneNumberBook.DataKeys[gvPhoneNumberBook.EditIndex].Value;

            using (PhoneNumbersController pnc = new PhoneNumbersController())
            {
                PhoneNumber phoneNumber = phoneNumberID.HasValue ? pnc.Get(phoneNumberID.Value) : new PhoneNumber();

                phoneNumber.TenDigit = string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ? null : GetTenDigitNumber(txtPhoneNumber.Text);
                phoneNumber.Extension = string.IsNullOrWhiteSpace(txtExtension.Text) ? null : txtExtension.Text.Trim();

                if (phoneNumberID.HasValue)
                {
                    // edit
                    pnc.Update(phoneNumber);
                }
                else
                {
                    // add
                    phoneNumber.PersonID = PersonID.Value;
                    phoneNumber.SortOrder = Convert.ToByte(pnc.GetWhere(i => i.PersonID == PersonID).Count());
                    pnc.AddNew(phoneNumber);
                }
            }
            
            gvPhoneNumberBook.DataBind();
            divEdit.Visible = false;
            ClearForm();
            AlertBox.SetStatus("Successfully saved phone number.");
        }

        protected void lnkAddPhoneNumber_Click(object sender, EventArgs e)
        {
            divEdit.Visible = true;
            ClearForm();
        }

        public IQueryable gvPhoneNumberBook_GetData()
        {
            using (PhoneNumbersController pnc = new PhoneNumbersController())
            {
                PhoneNumberResults = pnc.GetWhere(i => i.PersonID == PersonID).OrderBy(i => i.SortOrder).Select(i => new PhoneNumberResult()
                {
                    PhoneNumber = FormatTenDigitNumber(i.TenDigit),
                    PhoneNumberID = i.PhoneNumberID
                }).ToList();

                return PhoneNumberResults.AsQueryable();
            }
        }

        public void gvPhoneNumberBook_DeleteItem(int PhoneNumberID)
        {
            using (PhoneNumbersController pnc = new PhoneNumbersController())
            {
                pnc.Delete(PhoneNumberID);
                gvPhoneNumberBook.DataBind();

                ClearForm();
                divEdit.Visible = false;

                AlertBox.SetStatus("Successfully removed phone number.");
            }
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
            int phoneNumberID = (int)gvPhoneNumberBook.DataKeys[e.NewEditIndex].Value;
            Populate_PhoneNumberEdit(phoneNumberID);
            gvPhoneNumberBook.EditIndex = e.NewEditIndex;
            gvPhoneNumberBook.DataBind();
        }

        private void Populate_PhoneNumberEdit(int phoneNumberID)
        {
            using (PhoneNumbersController pnc = new PhoneNumbersController())
            {
                PhoneNumber phoneNumber = pnc.Get(phoneNumberID);

                txtExtension.Text = phoneNumber.Extension;
                txtPhoneNumber.Text = FormatTenDigitNumber(phoneNumber.TenDigit);

                divEdit.Visible = true;
            }
        }

        protected void valFormatPhoneNumber_ServerValidate(object source, ServerValidateEventArgs args)
        {
            MatchCollection matchesCorrectNumberOfNumbers = Regex.Matches(args.Value, @"[0-9]");
            MatchCollection matchesCorrectNumberOfNonChars = Regex.Matches(args.Value, @"[^)(\s-0-9]");

            // There should be EXACTLY 10 0-9 digits
            args.IsValid = matchesCorrectNumberOfNumbers.Count == 10 && matchesCorrectNumberOfNonChars.Count == 0;
        }

        private string GetTenDigitNumber(string input)
        {
            // ##########
            MatchCollection matches = Regex.Matches(input, @"[0-9]");

            if (matches.Count != 10)
            {
                throw new Exception("The phone number does not have ten digits. Unable to get the ten digit phone number.");
            }

            return string.Join("", matches.Cast<Match>().Select(m => m.Value));
        }

        private string FormatTenDigitNumber(string input)
        {
            // (###) ###-####
            char[] ten = GetTenDigitNumber(input).ToCharArray();
            return string.Format("({0}{1}{2}) {3}{4}{5}-{6}{7}{8}{9}", ten[0], ten[1], ten[2], ten[3], ten[4], ten[5], ten[6], ten[7], ten[8], ten[9]);
        }
    }
}