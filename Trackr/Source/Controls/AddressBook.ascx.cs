using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackrModels;

namespace Trackr.Source.Controls
{
    public partial class AddressBook : UserControl
    {
        [Serializable]
        private class AddressResult
        {
            public string Address { get; set; }
            public int AddressID { get; set; }
        }

        public int? PersonID
        {
            get { return ViewState["AddressBookPersonID"] as int?; }
            set { ViewState["AddressBookPersonID"] = value; }
        }

        private List<AddressResult> AddressResults
        {
            get { return ViewState["AddressResults"] as List<AddressResult>; }
            set { ViewState["AddressResults"] = value; }
        }

        private void ClearForm()
        {
            txtAddress1.Text = "";
            txtAddress2.Text = "";
            txtCity.Text = "";
            txtState.Text = "";
            txtZipCode.Text = "";
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

        protected void lnkSaveAddress_Click(object sender, EventArgs e)
        {
            if (!PersonID.HasValue)
            {
                throw new Exception("Address book does not have a person ID.");
            }

            int? addressID = gvAddressBook.EditIndex == -1 ? (int?)null : (int)gvAddressBook.DataKeys[gvAddressBook.EditIndex].Value;

            using (AddressesController ac = new AddressesController())
            {
                Address address = addressID.HasValue ? ac.Get(addressID.Value) : new Address();

                address.Street1 = string.IsNullOrWhiteSpace(txtAddress1.Text) ? null : txtAddress1.Text.Trim();
                address.Street2 = string.IsNullOrWhiteSpace(txtAddress2.Text) ? null : txtAddress2.Text.Trim();
                address.City = string.IsNullOrWhiteSpace(txtCity.Text) ? null : txtCity.Text.Trim();
                address.State = string.IsNullOrWhiteSpace(txtState.Text) ? null : txtState.Text.Trim();
                address.ZipCode = string.IsNullOrWhiteSpace(txtZipCode.Text) ? null : txtZipCode.Text.Trim();

                if (addressID.HasValue)
                {
                    // edit
                    ac.Update(address);
                }
                else
                {
                    // add
                    address.PersonID = PersonID.Value;
                    address.SortOrder = Convert.ToByte(ac.GetWhere(i => i.PersonID == PersonID).Count());
                    ac.AddNew(address);
                }
            }

            gvAddressBook.DataBind();
            divEdit.Visible = false;
            ClearForm();
            AlertBox.SetStatus("Successfully saved address.");
        }

        protected void lnkAddAddress_Click(object sender, EventArgs e)
        {
            divEdit.Visible = true;
            ClearForm();
        }

        public IQueryable gvAddressBook_GetData()
        {
            using (AddressesController ac = new AddressesController())
            {
                AddressResults = ac.GetWhere(i => i.PersonID == PersonID).OrderBy(i => i.SortOrder).Select(i => new AddressResult()
                {
                    Address = i.Street1,
                    AddressID = i.AddressID
                }).ToList();

                return AddressResults.AsQueryable();
            }
        }

        public void gvAddressBook_DeleteItem(int AddressID)
        {
            using (AddressesController ac = new AddressesController())
            {
                ac.Delete(AddressID);
                gvAddressBook.DataBind();

                ClearForm();
                divEdit.Visible = false;

                AlertBox.SetStatus("Successfully removed address.");
            }
        }

        protected void gvAddressBook_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvAddressBook.EditIndex = -1;
            gvAddressBook.DataBind();

            ClearForm();
            divEdit.Visible = false;
        }

        protected void gvAddressBook_RowEditing(object sender, GridViewEditEventArgs e)
        {
            int addressID = (int)gvAddressBook.DataKeys[e.NewEditIndex].Value;
            Populate_AddressEdit(addressID);
            gvAddressBook.EditIndex = e.NewEditIndex;
            gvAddressBook.DataBind();
        }

        private void Populate_AddressEdit(int addressID)
        {
            using (AddressesController ac = new AddressesController())
            {
                Address address = ac.Get(addressID);

                txtAddress1.Text = address.Street1;
                txtAddress2.Text = address.Street2;
                txtCity.Text = address.City;
                txtState.Text = address.State;
                txtZipCode.Text = address.ZipCode;

                divEdit.Visible = true;
            }
        }
    }
}