<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PhoneNumberBook.ascx.cs" Inherits="Trackr.Source.Controls.PhoneNumberBook" %>

<%@ Register Src="~/Source/Controls/AlertBox.ascx" TagPrefix="ui2" TagName="AlertBox" %>

<ui2:AlertBox runat="server" ID="AlertBox" />

<div class="row">
    <div class="col-sm-12">
        <asp:GridView runat="server" ID="gvPhoneNumberBook" SelectMethod="gvPhoneNumberBook_GetData" EmptyDataText="There are no phone numbers associated to this person." CssClass="table" DataKeyNames="PhoneNumberID" AutoGenerateColumns="false" 
            DeleteMethod="gvPhoneNumberBook_DeleteItem" OnRowCancelingEdit="gvPhoneNumberBook_RowCancelingEdit" OnRowEditing="gvPhoneNumberBook_RowEditing">
            <Columns>
                <asp:BoundField DataField="PhoneNumber" ReadOnly="true" />

                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lnkRemovePhoneNumber" CommandName="Delete" ToolTip="Remove person's phone number" CssClass="glyphicon glyphicon-trash" CausesValidation="false"></asp:LinkButton>&nbsp;&nbsp;
                        <asp:LinkButton runat="server" ID="lnkEditPhoneNumber" CommandName="Edit" ToolTip="Edit person's phone number" CssClass="glyphicon glyphicon-edit" CausesValidation="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <div class="col-sm-12 text-left">
        <asp:LinkButton runat="server" ID="lnkAddPhoneNumber" OnClick="lnkAddPhoneNumber_Click" ToolTip="Add new phone number">
            <span class="glyphicon glyphicon-plus-sign"></span> Add Phone Number
        </asp:LinkButton>
    </div>
</div>


<div class="row" runat="server" id="divEdit" visible="false">
    <div class="form-group col-sm-12">
        <label for="<%=txtPhoneNumber.ClientID %>">Phone Number</label>
        <asp:TextBox runat="server" ID="txtPhoneNumber" CssClass="form-control" MaxLength="14" />
        <asp:RequiredFieldValidator runat="server" ID="valRequiredPhoneNumber" ControlToValidate="txtPhoneNumber" CssClass="text-danger" Display="Dynamic" ErrorMessage="A phone number is required." />
    </div>
    
    <div class="form-group col-sm-12">
        <label for="<%=txtExtension.ClientID %>">Extension</label>
        <asp:TextBox runat="server" ID="txtExtension" CssClass="form-control" MaxLength="10" />
    </div>

    <div class="clearfix"></div>

    <div class="col-sm-12">
        <asp:LinkButton runat="server" ID="lnkSavePhoneNumber" CausesValidation="true" OnClick="lnkSavePhoneNumber_Click">
            <span class="glyphicon glyphicon-save"></span> Save Phone Number
        </asp:LinkButton>
    </div>
</div>