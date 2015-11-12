<%@ Page Title="Login" Language="C#" MasterPageFile="~/SiteNotLoggedIn.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Trackr.Account.Login" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-sm-7">

            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>Login</h4>
                </div>
                <div class="panel-body">
                    <section id="loginForm">
                        <div class="form-horizontal">
                            <ui:AlertBox runat="server" ID="AlertBox" />

                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="Email" CssClass="col-md-2 control-label">Email</asp:Label>
                                <div class="col-md-10">
                                    <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="Email" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="Email" CssClass="text-danger" ErrorMessage="The email field is required." Display="Dynamic" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-2 control-label">Password</asp:Label>
                                <div class="col-md-10">
                                    <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="The password field is required." Display="Dynamic" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-md-offset-2 col-md-10">
                                    <div class="checkbox">
                                        <asp:CheckBox runat="server" ID="RememberMe" />
                                        <asp:Label runat="server" AssociatedControlID="RememberMe">Remember me?</asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-md-offset-2 col-md-10">
                                    <asp:Button runat="server" OnClick="LogIn" Text="Login" CssClass="btn btn-default" />
                                </div>
                            </div>
                        </div>
                    </section>
                </div>
            </div>
        </div>

        <div class="col-sm-5">
            <div class="panel panel-default">
                <div class="panel-body">
                    <section id="loginHelp">
                        <p>
                            Please enter your login information to access the Gananda Bandits intranet.
                        </p>
                        <p>
                            <strong>Don't have an account yet?</strong><br />
                            <asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled">Click here to register</asp:HyperLink>
                        </p>
                        <p>
                            <strong>Have an account but forgot your password?</strong><br />
                            <asp:HyperLink runat="server" ID="ForgotPasswordHyperLink" ViewStateMode="Disabled">Click here to reset your password</asp:HyperLink>
                        </p>
                    </section>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
