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
        public delegate IList<Address> GetAddresses(Guid personEditToken);

        public Guid? PersonEditToken
        {
            get { return ViewState["PersonEditToken"] as Guid?; }
            set { ViewState["PersonEditToken"] = value; }
        }

        public GetAddresses GetData { get; set; }

        private IList<Address> Addresses
        {
            get
            {
                if (PersonEditToken.HasValue)
                {
                    return GetData(PersonEditToken.Value);
                }
                else
                {
                    return new List<Address>();
                }
            }
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
            lnkAddAddress.Visible = !divEdit.Visible;

            if (IsPostBack)
            {
                return;
            }

            Reload();
        }

        public void Reload()
        {
            gvAddressBook.DataBind();
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

        protected void lnkSaveAddress_Click(object sender, EventArgs e)
        {
            Guid editToken = gvAddressBook.EditIndex == -1 ? Guid.NewGuid() : (Guid)gvAddressBook.DataKeys[gvAddressBook.EditIndex].Value;

            Address address = Addresses.FirstOrDefault(i => i.EditToken == editToken) ?? new Address() { EditToken = Guid.NewGuid() };

            address.Street1 = string.IsNullOrWhiteSpace(txtAddress1.Text) ? null : txtAddress1.Text.Trim();
            address.Street2 = string.IsNullOrWhiteSpace(txtAddress2.Text) ? null : txtAddress2.Text.Trim();
            address.City = string.IsNullOrWhiteSpace(txtCity.Text) ? null : txtCity.Text.Trim();
            address.State = string.IsNullOrWhiteSpace(txtState.Text) ? null : txtState.Text.Trim();
            address.ZipCode = string.IsNullOrWhiteSpace(txtZipCode.Text) ? null : txtZipCode.Text.Trim();
            address.WasModified = true;

            if (Addresses.FirstOrDefault(i => i.EditToken == editToken) == null)
            {
                address.Active = true;
                address.SortOrder = Convert.ToByte(Addresses.Count());
                Addresses.Add(address);
            }

            lnkAddAddress.Visible = true;
            divEdit.Visible = false;
            ClearForm();
            AlertBox.AddAlert(string.Format("Successfully {0} address. Your settings will be saved when you complete this wizard.", gvAddressBook.EditIndex == -1 ? "added" : "edited"), false, UI.AlertBoxType.Warning);
            gvAddressBook.EditIndex = -1;
            gvAddressBook.DataBind();
        }

        protected void lnkAddAddress_Click(object sender, EventArgs e)
        {
            divEdit.Visible = true;
            lnkAddAddress.Visible = false;
            ClearForm();
        }

        public IQueryable gvAddressBook_GetData()
        {
            return (Addresses ?? new List<Address>()).Where(i => i.Active).AsQueryable();
        }

        public void gvAddressBook_DeleteItem(Guid EditToken)
        {
            Address address = Addresses.First(i => i.EditToken == EditToken);

            if (address.AddressID > 0)
            {
                address.Active = false;
                address.WasModified = true;
            }
            else
            {
                Addresses.Remove(address);
            }

            gvAddressBook.DataBind();

            ClearForm();
            divEdit.Visible = false;
        }

        protected void gvAddressBook_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvAddressBook.EditIndex = -1;
            gvAddressBook.DataBind();

            ClearForm();
            divEdit.Visible = false;
            lnkAddAddress.Visible = true;
        }

        protected void gvAddressBook_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid editToken = (Guid)gvAddressBook.DataKeys[e.NewEditIndex].Value;
            Populate_AddressEdit(editToken);
            gvAddressBook.EditIndex = e.NewEditIndex;
            gvAddressBook.DataBind();
        }

        private void Populate_AddressEdit(Guid editToken)
        {
            Address address = Addresses.First(i => i.EditToken == editToken);

            txtAddress1.Text = address.Street1;
            txtAddress2.Text = address.Street2;
            txtCity.Text = address.City;
            txtState.Text = address.State;
            txtZipCode.Text = address.ZipCode;

            divEdit.Visible = true;
        }

        protected void lnkCancel_Click(object sender, EventArgs e)
        {
            gvAddressBook_RowCancelingEdit(null, null);
        }
    }
}