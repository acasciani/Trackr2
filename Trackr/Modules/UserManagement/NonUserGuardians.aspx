<%@ Page Title="Guardians Without Accounts" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NonUserGuardians.aspx.cs" Inherits="Trackr.Modules.UserManagement.NonUserGuardians" %>

<%@ Register Src="~/Source/Wizards/UserManagement.ascx" TagName="UserManagement" TagPrefix="ui" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel runat="server" ID="uP_Main">

        <ContentTemplate>
    <div class="panel panel-default">
        <div class="panel-heading">
            <h4>User Management - Guardians Without Accounts</h4>
        </div>
        <div class="panel-body">
            <ui:AlertBox runat="server" ID="AlertBox" />

            <asp:GridView runat="server" ID="gvAllNonUsers" AutoGenerateColumns="false" CssClass="table table-striped" SelectMethod="gvAllNonUsers_GetData" AllowSorting="true" EmptyDataText="No guardians found who are not setup with user accounts.">
                <Columns>
                    <asp:BoundField DataField="LastName" SortExpression="LastName" HeaderText="Last name" ItemStyle-CssClass="" />
                    <asp:BoundField DataField="FirstName" SortExpression="FirstName" HeaderText="First name" ItemStyle-CssClass="col-xs-3" />
                    <asp:TemplateField ItemStyle-CssClass="col-xs-1">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lnkCreateAccount" ToolTip="Click here to create an account for this guardian" OnClick="lnkCreateAccount_Click" CommandArgument='<%#Eval("PersonID") %>' CausesValidation="false">Create</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>


    <!-- create user modal -->
    <div class="modal fade add-non-user-guardian-as-user" tabindex="-1" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel runat="server" ID="upCreate" UpdateMode="Conditional">
                    <ContentTemplate>
                

                <asp:HiddenField runat="server" ID="hdnPersonID" />
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title">Create Guardian User Account</h4>
                </div>
                <div class="modal-body">
                    <ui:AlertBox runat="server" ID="CreateAlertBox" />
                    <p>
                        <strong>Guardian:</strong>
                        <asp:Literal runat="server" ID="litGuardianName" /><br />
                        <strong>Past and Current Players:</strong>
                        <asp:Literal runat="server" ID="litPlayerNames" />
                    </p>

                    <p>
                        Below is a list of all emails associated to this guardian. Please select an email account to use as their login.
                    </p>

                    <asp:RadioButtonList runat="server" ID="radioList">
                    </asp:RadioButtonList>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="radioList" CssClass="text-danger" Display="Dynamic" ErrorMessage="Please select an email to use as the user's login." />

                    <asp:TextBox runat="server" ID="txtPassword" placeholder="Password" CssClass="form-control" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword" CssClass="text-danger" Display="Dynamic" ErrorMessage="Please enter a password." />
                    <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtPassword" ValidationExpression="^[\s\S]{5,8}$" runat="server" ErrorMessage="The password must be at least 8 characters long." CssClass="text-danger" />

                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                    <asp:Button runat="server" ID="btnCreateUserAccounts" OnClick="btnCreateUserAccounts_Click" Text="Create User Accounts" CssClass="btn btn-primary" CausesValidation="true" />
                </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>
            </div>
        </div>
    </div>

            </ContentTemplate></asp:UpdatePanel>

</asp:Content>
