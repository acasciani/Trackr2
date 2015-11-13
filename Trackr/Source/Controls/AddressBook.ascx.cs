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

        public int PersonID { get; set; }
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

        }

        protected void lnkSaveAddress_Click(object sender, EventArgs e)
        {
            int? addressID = gvAddressBook.EditIndex == -1 ? (int?)null : AddressResults[gvAddressBook.EditIndex].AddressID;

            using (AddressesController ac = new AddressesController())
            {
                Address address = addressID.HasValue ? ac.Get(addressID.Value) : new Address();

                address.Street1 = txtAddress1.Text.Trim();
                address.Street2 = txtAddress2.Text.Trim();
                address.City = txtCity.Text.Trim();
                address.State = txtState.Text.Trim();
                address.ZipCode = txtZipCode.Text.Trim();

                if (addressID.HasValue)
                {
                    // edit
                    ac.Update(address);
                }
                else
                {
                    // add
                    address.PersonID = PersonID;
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
    }
}