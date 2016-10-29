<%@ Page Title="Manage My Password" Language="C#" MasterPageFile="~/Account/Account.master" AutoEventWireup="true" CodeBehind="ManagePassword.aspx.cs" Inherits="Trackr.Account.ManagePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="NestedContent" runat="server">
    <asp:UpdatePanel runat="server" ID="updPanel">
        <ContentTemplate>
            <asp:UpdateProgress runat="server" AssociatedUpdatePanelID="updPanel">
                <ProgressTemplate>
                    <progress:Bounce runat="server" />
                </ProgressTemplate>
            </asp:UpdateProgress>

            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>My Account - Manage Password</h4>
                </div>
                <div class="panel-body">
                    <ui:AlertBox runat="server" ID="AlertBox" />

                    <p class="bg-info bulky">
                        <strong>To Change Your Password:</strong> Enter your current password and new password below.
                    </p>

                    <div class="row form-group">
                        <label for="<%=txtCurrentPassword.ClientID %>" class="col-sm-12 control-label">Current Password</label>
                        <div class="col-sm-12">
                            <asp:TextBox runat="server" ID="txtCurrentPassword" CssClass="form-control" MaxLength="32" TextMode="Password" />
                            <asp:RequiredFieldValidator Display="Dynamic" runat="server" ControlToValidate="txtCurrentPassword" CssClass="text-danger" ErrorMessage="Enter your current password." />
                        </div>
                    </div>

                    <hr />

                    <div class="row form-group">
                        <label for="<%=txtPassword.ClientID %>" class="col-sm-12 control-label">New Password</label>
                        <div class="col-sm-12">
                            <asp:TextBox runat="server" ID="txtPassword" CssClass="form-control" MaxLength="32" TextMode="Password" />
                            <asp:RequiredFieldValidator Display="Dynamic" runat="server" ControlToValidate="txtPassword" CssClass="text-danger" ErrorMessage="A password is required." />
                            <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtPassword" ValidationExpression="^[\s\S]{8,}$" runat="server" ErrorMessage="The password must be at least 8 characters long." CssClass="text-danger" />
                        </div>
                    </div>

                    <div class="row form-group">
                        <label for="<%=txtPasswordConfirm.ClientID %>" class="col-sm-12 control-label">Confirm New Password</label>
                        <div class="col-sm-12">
                            <asp:TextBox runat="server" ID="txtPasswordConfirm" CssClass="form-control" MaxLength="32" TextMode="Password" />
                            <asp:CompareValidator runat="server" ControlToCompare="txtPasswordConfirm" ControlToValidate="txtPassword" CssClass="text-danger" Display="Dynamic" ErrorMessage="The password and confirmation password do not match." />
                        </div>
                    </div>

                    <asp:Button runat="server" ID="lnkSavePassword" Text="Change Password" OnClick="lnkSavePassword_Click" CssClass="btn btn-default" />
                </div>
            </div>




        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
