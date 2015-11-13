<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddressBook.ascx.cs" Inherits="Trackr.Source.Controls.AddressBook" %>

<ui:AlertBox runat="server" ID="AlertBox" />

<div class="row">
    <div class="col-sm-12">
        <asp:GridView runat="server" ID="gvAddressBook" SelectMethod="gvAddressBook_GetData">
            <Columns>
                <asp:BoundField DataField="Address" />
            </Columns>
        </asp:GridView>
    </div>

    <div class="col-sm-12 text-right">
        <asp:LinkButton runat="server" ID="lnkAddAddress" OnClick="lnkAddAddress_Click" ToolTip="Add new address">
            <span class="glyphicon glyphicon-plus-sign"></span> Add Address
        </asp:LinkButton>
    </div>
</div>


<div class="row" runat="server" id="divEdit" visible="false">
    <div class="form-group col-sm-12">
        <label for="<%=txtAddress1.ClientID %>">Address</label>
        <asp:TextBox runat="server" ID="txtAddress1" CssClass="form-control" MaxLength="30" />
        <asp:RequiredFieldValidator runat="server" ID="valRequiredAddress1" ControlToValidate="txtAddress" CssClass="text-danger" Display="Dynamic" ErrorMessage="A street address is required." />
    </div>
    
    <div class="form-group col-sm-12">
        <label for="<%=txtAddress2.ClientID %>">Address 2</label>
        <asp:TextBox runat="server" ID="txtAddress2" CssClass="form-control" MaxLength="30" />
    </div>

    <div class="form-group col-sm-6">
        <label for="<%=txtCity.ClientID %>">City</label>
        <asp:TextBox runat="server" ID="txtCity" CssClass="form-control" MaxLength="20" />
    </div>
    <div class="form-group col-sm-2">
        <label for="<%=txtState.ClientID %>">City</label>
        <asp:TextBox runat="server" ID="txtState" CssClass="form-control" MaxLength="2" />
    </div>
    <div class="form-group col-sm-4">
        <label for="<%=txtZipCode.ClientID %>">Zip Code</label>
        <asp:TextBox runat="server" ID="txtZipCode" CssClass="form-control" MaxLength="5" />
    </div>

    <div class="col-sm-12">
        <asp:LinkButton runat="server" ID="lnkSaveAddress" CausesValidation="true" OnClick="lnkSaveAddress_Click">
            <span class="glyphicon glyphicon-save"></span> Save Address
        </asp:LinkButton>
    </div>
</div>