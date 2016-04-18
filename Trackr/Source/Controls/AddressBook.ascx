<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddressBook.ascx.cs" Inherits="Trackr.Source.Controls.AddressBook" %>

<%@ Register Src="~/Source/Controls/AlertBox.ascx" TagPrefix="ui2" TagName="AlertBox" %>

<ui2:AlertBox runat="server" ID="AlertBox" />

<div class="row">
    <div class="col-sm-12">
        <asp:GridView runat="server" ID="gvAddressBook" SelectMethod="gvAddressBook_GetData" EmptyDataText="There are no addresses associated to this person." CssClass="table" DataKeyNames="EditToken" AutoGenerateColumns="false" 
            DeleteMethod="gvAddressBook_DeleteItem" OnRowCancelingEdit="gvAddressBook_RowCancelingEdit" OnRowEditing="gvAddressBook_RowEditing">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <%#Eval("Street1") %>, <%# Eval("City") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lnkRemoveAddress" CommandName="Delete" ToolTip="Remove person's address" CssClass="glyphicon glyphicon-trash" CausesValidation="false"></asp:LinkButton>&nbsp;&nbsp;
                        <asp:LinkButton runat="server" ID="lnkEditAddress" CommandName="Edit" ToolTip="Edit person's address" CssClass="glyphicon glyphicon-edit" CausesValidation="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <div class="col-sm-12 text-left">
        <asp:LinkButton runat="server" ID="lnkAddAddress" OnClick="lnkAddAddress_Click" ToolTip="Add new address">
            <span class="glyphicon glyphicon-plus-sign"></span>Add Address
        </asp:LinkButton>
    </div>
</div>


<div class="row" runat="server" id="divEdit" visible="false">
    <div class="form-group col-sm-12">
        <label for="<%=txtAddress1.ClientID %>">Address</label>
        <asp:TextBox runat="server" ID="txtAddress1" CssClass="form-control" MaxLength="30" />
        <asp:RequiredFieldValidator runat="server" ID="valRequiredAddress1" ControlToValidate="txtAddress1" CssClass="text-danger" Display="Dynamic" ErrorMessage="A street address is required." />
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
        <label for="<%=txtState.ClientID %>">State</label>
        <asp:TextBox runat="server" ID="txtState" CssClass="form-control" MaxLength="2" />
    </div>
    <div class="form-group col-sm-4">
        <label for="<%=txtZipCode.ClientID %>">Zip Code</label>
        <asp:TextBox runat="server" ID="txtZipCode" CssClass="form-control" MaxLength="5" />
    </div>

    <div class="clearfix"></div>

    <div class="col-sm-6 text-left">
        <asp:LinkButton runat="server" ID="lnkSaveAddress" CausesValidation="true" OnClick="lnkSaveAddress_Click">
            <span class="glyphicon glyphicon-save"></span>Save Address
        </asp:LinkButton>
    </div>
    <div class="col-sm-6 text-right">
        <asp:LinkButton runat="server" ID="lnkCancel" OnClick="lnkCancel_Click" CausesValidation="false">
            <span class="glyphicon glyphicon-folder-close"></span>Cancel
        </asp:LinkButton>
    </div>
</div>