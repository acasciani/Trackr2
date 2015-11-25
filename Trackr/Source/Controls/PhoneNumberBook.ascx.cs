using System;
using System.Collections.Generic;
using System.Linq;
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

        private string GetTenDigitNumber(string input)
        {
            return input;
        }

        private string FormatTenDigitNumber(string input)
        {
            return input;
        }

        protected void lnkSavePhoneNumber_Click(object sender, EventArgs e)
        {
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
    }
}