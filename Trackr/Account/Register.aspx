<%@ Page Title="Register" Language="C#" MasterPageFile="~/SiteNotLoggedIn.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Trackr.Account.Register" %>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-sm-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>Register - <%= CreateWizard.ActiveStep.Title %></h4>
                </div>
                <div class="panel-body">
                    <ui:AlertBox runat="server" ID="AlertBox" />

                    <asp:CreateUserWizard runat="server" ID="CreateWizard" LoginCreatedUser="false" OnCreatedUser="CreateWizard_CreatedUser" OnCreateUserError="CreateWizard_CreateUserError">
                        <CreateUserButtonStyle CssClass="btn btn-default" />
                        <StepNavigationTemplate>
                            <div class="text-right">
                                <asp:Button runat="server" CommandName="MoveNext" Text="Continue" CssClass="btn btn-default" />
                            </div>
                        </StepNavigationTemplate>
                        <StartNavigationTemplate>
                            <div class="text-right">
                                <asp:Button runat="server" CommandName="MoveNext" Text="Continue" CssClass="btn btn-default" />
                            </div>
                        </StartNavigationTemplate>
                        <FinishNavigationTemplate>
                            <div class="text-right">
                                <asp:Button runat="server" CommandName="MoveComplete" Text="Finish" CssClass="btn btn-default" />
                            </div>
                        </FinishNavigationTemplate>

                        <LayoutTemplate>
                            <div>
                                <asp:PlaceHolder runat="server" ID="wizardStepPlaceholder"></asp:PlaceHolder>
                            </div>

                            <div>
                                <asp:PlaceHolder runat="server" ID="navigationPlaceholder"></asp:PlaceHolder>
                            </div>
                        </LayoutTemplate>

                        <WizardSteps>
                            <asp:CreateUserWizardStep runat="server" Title="Enter Account Credentials">
                                <CustomNavigationTemplate>
                                    <div class="text-right">
                                        <asp:Button runat="server" CommandName="MoveNext" Text="Continue" CssClass="btn btn-default" />
                                    </div>
                                </CustomNavigationTemplate>

                                <ContentTemplate>
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <label class="col-sm-3 control-label" for="MainContent_CreateWizard_CreateUserStepContainer_UserName">Email</label>
                                            <div class="col-sm-9">
                                                <asp:TextBox runat="server" ID="UserName" placeholder="e.g. jsmith@gmail.com" TextMode="Email" CssClass="form-control" />
                                                <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorEmailRequired" ControlToValidate="UserName" CssClass="text-danger" ErrorMessage="A valid email is required." />
                                                <asp:CustomValidator Display="Dynamic" runat="server" ID="validatorEmailExists" ControlToValidate="UserName" CssClass="text-danger" ErrorMessage="The entered email address is already in use." OnServerValidate="validatorEmailExists_ServerValidate" />
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <label class="col-sm-3 control-label" for="MainContent_CreateWizard_CreateUserStepContainer_Password">Password</label>
                                            <div class="col-sm-9">
                                                <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                                                <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorPasswordRequired" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="A password is required." />
                                                <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="Password" ID="validatorPasswordRegex" ValidationExpression="^[\s\S]{5,8}$" runat="server" ErrorMessage="The password must be at least 8 characters long." CssClass="text-danger" />
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <label class="col-sm-3 control-label" for="MainContent_CreateWizard_CreateUserStepContainer_txtPasswordConfirm">Confirm</label>
                                            <div class="col-sm-9">
                                                <asp:TextBox runat="server" ID="txtPasswordConfirm" TextMode="Password" CssClass="form-control" />
                                                <asp:CompareValidator runat="server" ControlToCompare="txtPasswordConfirm" ControlToValidate="Password" CssClass="text-danger" Display="Dynamic" ErrorMessage="The password and confirmation password do not match." />
                                            </div>
                                        </div>

                                        <asp:TextBox runat="server" ID="Email" Visible="false" />
                                    </div>
                                </ContentTemplate>
                            </asp:CreateUserWizardStep>

                            <asp:WizardStep runat="server" StepType="Finish" AllowReturn="false" ID="Step2_Personal" OnDeactivate="Step2_Personal_Deactivate" Title="Enter Personal Information">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <label for="<%=txtFirstName.ClientID %>" class="col-sm-3 control-label">First name</label>
                                        <div class="col-sm-9">
                                            <asp:TextBox runat="server" ID="txtFirstName" MaxLength="30" CssClass="form-control" />
                                            <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorFirstNameRequired" ControlToValidate="txtFirstName" CssClass="text-danger" ErrorMessage="A first name is required." />
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="<%=txtMiddleInitial.ClientID %>" class="col-sm-3 control-label">Middle initial</label>
                                        <div class="col-sm-9">
                                            <asp:TextBox runat="server" ID="txtMiddleInitial" MaxLength="1" CssClass="form-control" />
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="<%=txtLastName.ClientID %>" class="col-sm-3 control-label">Last name</label>
                                        <div class="col-sm-9">
                                            <asp:TextBox runat="server" ID="txtLastName" MaxLength="30" CssClass="form-control" />
                                            <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorLastNameRequired" ControlToValidate="txtLastName" CssClass="text-danger" ErrorMessage="A last name is required." />
                                        </div>
                                    </div>
                                </div>
                            </asp:WizardStep>


                            <asp:WizardStep runat="server" AllowReturn="false" StepType="Complete" ID="Step3_Completed" OnActivate="Step3_Completed_Activate" Title="Completed">
                                <p>Your account has been successfully created. You are now logged in. Please check your email for more information.</p>
                                <p><strong><a href="/Default.aspx">Click here to continue to the intranet.</a></strong></p>
                            </asp:WizardStep>
                        </WizardSteps>
                    </asp:CreateUserWizard>
                </div>
            </div>
        </div>

        <div class="col-sm-12 text-right" runat="server" visible='<%# CreateWizard.ActiveStepIndex == 0 %>'>
            Already have an account? <a href="Login.aspx">Click here to login</a>
        </div>
    </div>
</asp:Content>
