<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailAddressBook.ascx.cs" Inherits="Trackr.Source.Controls.EmailAddressBook" %>

<%@ Register Src="~/Source/Controls/AlertBox.ascx" TagPrefix="ui2" TagName="AlertBox" %>

<ui2:AlertBox runat="server" ID="AlertBox" />

<div class="row">
    <div class="col-sm-12">
        <asp:GridView runat="server" ID="gvEmailAddressBook" SelectMethod="gvEmailAddressBook_GetData" EmptyDataText="There are no email addresses associated to this person." CssClass="table" DataKeyNames="EmailAddressID" AutoGenerateColumns="false" 
            DeleteMethod="gvEmailAddressBook_DeleteItem" OnRowCancelingEdit="gvEmailAddressBook_RowCancelingEdit" OnRowEditing="gvEmailAddressBook_RowEditing">
            <Columns>
                <asp:BoundField DataField="EmailAddress" ReadOnly="true" />

                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lnkRemoveEmailAddress" CommandName="Delete" ToolTip="Remove person's email address" CssClass="glyphicon glyphicon-trash" CausesValidation="false"></asp:LinkButton>&nbsp;&nbsp;
                        <asp:LinkButton runat="server" ID="lnkEditEmailAddress" CommandName="Edit" ToolTip="Edit person's email address" CssClass="glyphicon glyphicon-edit" CausesValidation="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <div class="col-sm-12 text-left">
        <asp:LinkButton runat="server" ID="lnkAddEmailAddress" OnClick="lnkAddEmailAddress_Click" ToolTip="Add new email address">
            <span class="glyphicon glyphicon-plus-sign"></span> Add Email Address
        </asp:LinkButton>
    </div>
</div>


<div class="row" runat="server" id="divEdit" visible="false">
    <div class="form-group col-sm-12">
        <label for="<%=txtEmailAddress.ClientID %>">Address</label>
        <asp:TextBox runat="server" ID="txtEmailAddress" CssClass="form-control" MaxLength="100" />
        <asp:RequiredFieldValidator runat="server" ID="valRequiredEmailAddress" ControlToValidate="txtEmailAddress" CssClass="text-danger" Display="Dynamic" ErrorMessage="Am email address is required." />
    </div>
    
    <div class="form-group col-sm-12">
        <asp:CheckBox runat="server" ID="chkIsHTML" /> <label for="<%=chkIsHTML.ClientID %>">Is HTML</label>
    </div>

    <div class="clearfix"></div>

    <div class="col-sm-12">
        <asp:LinkButton runat="server" ID="lnkSaveEmailAddress" CausesValidation="true" OnClick="lnkSaveEmailAddress_Click">
            <span class="glyphicon glyphicon-save"></span> Save Email Address
        </asp:LinkButton>
    </div>
</div>