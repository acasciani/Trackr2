<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserManagement.ascx.cs" Inherits="Trackr.Source.Wizards.UserManagement" %>

<ui:AlertBox runat="server" ID="AlertBox" />

<asp:CreateUserWizard runat="server" ID="UserWizard" LoginCreatedUser="false" OnCreatedUser="UserWizard_CreatedUser" DisplaySideBar="true" OnCreateUserError="UserWizard_CreateUserError">
    <CreateUserButtonStyle CssClass="btn btn-default" />

    <StepNavigationTemplate>
        <div class="text-right">
            <asp:Button runat="server" CommandName="MovePrevious" Text="Previous" CssClass="btn btn-default" CausesValidation="false" />
            <asp:Button runat="server" CommandName="MoveNext" Text="Save & Continue" CssClass="btn btn-default" />
        </div>
    </StepNavigationTemplate>
    <StartNavigationTemplate>
        <div class="text-right">
            <asp:Button runat="server" CommandName="MoveNext" Text="Save & Continue" CssClass="btn btn-default" />
        </div>
    </StartNavigationTemplate>
    <FinishNavigationTemplate>
        <div class="text-right">
            <asp:Button runat="server" CommandName="MovePrevious" Text="Previous" CssClass="btn btn-default" CausesValidation="false" />
            <asp:Button runat="server" CommandName="MoveComplete" Text="Save & Finish" CssClass="btn btn-default" />
        </div>
    </FinishNavigationTemplate>


    <SideBarTemplate>
        <asp:ListView runat="server" ID="sideBarList">
            <LayoutTemplate>
                <ol class="breadcrumb">
                    <asp:PlaceHolder runat="server" ID="ItemPlaceHolder" />
                </ol>
            </LayoutTemplate>

            <ItemTemplate>
                <li runat="server" visible='<%# Container.DataItemIndex > 0 %>'><asp:LinkButton runat="server" ID="sideBarButton" CausesValidation="false"></asp:LinkButton></li>
            </ItemTemplate>
            <SelectedItemTemplate>
                <li class="active"><asp:Label runat="server" ID="SideBarLabel"></asp:Label></li>
            </SelectedItemTemplate>
        </asp:ListView>
    </SideBarTemplate>


    <LayoutTemplate>
        <div>
            <asp:PlaceHolder runat="server" ID="sideBarPlaceholder"></asp:PlaceHolder>
        </div>

        <div>
            <asp:PlaceHolder runat="server" ID="wizardStepPlaceholder"></asp:PlaceHolder>
        </div>

        <div>
            <asp:PlaceHolder runat="server" ID="navigationPlaceholder"></asp:PlaceHolder>
        </div>
    </LayoutTemplate>
    
    <WizardSteps>
        <asp:CreateUserWizardStep runat="server" Title="Credentials">
            <CustomNavigationTemplate>
                <div class="col-sm-12 text-right">
                    <asp:Button runat="server" CommandName="MoveNext" Text="Create User" CssClass="btn btn-default" />
                </div>
            </CustomNavigationTemplate>

            <ContentTemplate>
                <div class="form-horizontal">
                    <div class="form-group">
                        <label class="col-sm-4 control-label">Email</label>
                        <div class="col-sm-8">
                            <asp:TextBox runat="server" ID="UserName" placeholder="e.g. jsmith@gmail.com" TextMode="Email" CssClass="form-control" />
                            <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorEmailRequired" ControlToValidate="UserName" CssClass="text-danger" ErrorMessage="A valid email is required." />
                            <asp:CustomValidator Display="Dynamic" runat="server" ID="validatorEmailExists" ControlToValidate="UserName" CssClass="text-danger" ErrorMessage="The entered email address is already in use." OnServerValidate="validatorEmailExists_ServerValidate" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-4 control-label">Password</label>
                        <div class="col-sm-8">
                            <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                            <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="Password" ID="validatorPasswordRegex" ValidationExpression="^[\s\S]{5,8}$" runat="server" ErrorMessage="The password must be at least 8 characters long." CssClass="text-danger" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-4 control-label">Confirm Password</label>
                        <div class="col-sm-8">
                            <asp:TextBox runat="server" ID="txtPasswordConfirm" TextMode="Password" CssClass="form-control" />
                            <asp:CompareValidator runat="server" ControlToCompare="txtPasswordConfirm" ControlToValidate="Password" CssClass="text-danger" Display="Dynamic" ErrorMessage="The password and confirmation password do not match." />
                        </div>
                    </div>

                    <asp:TextBox runat="server" ID="Email" Visible="false" />
                </div>
            </ContentTemplate>
        </asp:CreateUserWizardStep>


        <asp:WizardStep runat="server" ID="Step1_Edit" OnDeactivate="Step1_Edit_Deactivate" StepType="Start" Title="Credentials">
            <div class="form-horizontal">
                <div class="form-group">
                    <label for="<%=txtEmail.ClientID %>" class="col-sm-4 control-label">Email</label>
                    <div class="col-sm-8">
                        <asp:TextBox runat="server" ID="txtEmail" placeholder="e.g. jsmith@gmail.com" TextMode="Email" CssClass="form-control" />
                        <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorEmailRequired" ControlToValidate="txtEmail" CssClass="text-danger" ErrorMessage="A valid email is required." />
                    </div>
                </div>
            </div>
        </asp:WizardStep>


        <asp:WizardStep runat="server" ID="Step2_Personal" OnDeactivate="Step2_Personal_Deactivate" Title="Personal">
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


        <asp:WizardStep runat="server" ID="Step3_RoleAssignments" OnDeactivate="Step3_RoleAssignments_Deactivate" Title="Assignments">
            <asp:GridView runat="server" ID="gvRoleAssignments" AutoGenerateColumns="false" SelectMethod="gvRoleAssignments_GetData" DeleteMethod="gvRoleAssignments_DeleteItem" DataKeyNames="ScopeAssignmentID,ScopeID,ResourceID,RoleID" CssClass="table table-striped">
                <Columns>
                    <asp:BoundField DataField="PermissionName" HeaderText="Permission" />
                    <asp:BoundField DataField="RoleName" HeaderText="Role" />
                    <asp:BoundField DataField="ScopeType" HeaderText="Scope Type" />
                    <asp:BoundField DataField="ScopeValue" HeaderText="Scoped To" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CSSClass="glyphicon glyphicon-trash" CommandName="Delete" ToolTip="Remove role assignment"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div class="form-group text-right">
                <asp:LinkButton runat="server" ID="lnkAddRoleAssignment" OnClick="lnkAddRoleAssignment_Click" ToolTip="Add role assignment">
                    <span class="glyphicon glyphicon-plus-sign"></span> Add Role Assignment
                </asp:LinkButton>
            </div>

            <asp:Panel runat="server" ID="pnlAddRoleAssignment" Visible="false">
                <div class="form-horizontal">
                    <div class="form-group">
                        <label for="<%=ddlRole.ClientID %>" class="col-sm-3 control-label">Role</label>
                        <div class="col-sm-9">
                            <asp:DropDownList runat="server" ID="ddlRole" CssClass="form-control" AppendDataBoundItems="true">
                                <asp:ListItem Text="-- Select a Role --"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:CustomValidator runat="server" ID="validatorRoleRequired" ControlToValidate="ddlRole" CssClass="text-danger" Display="Dynamic" ErrorMessage="Select a role or permission." OnServerValidate="validatorRolePermissionRequired_ServerValidate" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="<%=ddlPermission.ClientID %>" class="col-sm-3 control-label">Permission</label>
                        <div class="col-sm-9">
                            <asp:DropDownList runat="server" ID="ddlPermission" CssClass="form-control" AppendDataBoundItems="true">
                                <asp:ListItem Text="-- Select a Permission --"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:CustomValidator runat="server" ID="validatorPermissionRequired" ControlToValidate="ddlRole" CssClass="text-danger" Display="Dynamic" ErrorMessage="Select a role or permission." OnServerValidate="validatorRolePermissionRequired_ServerValidate" />
                            <asp:CustomValidator runat="server" ID="validatorPermissionNotAlsoEntered" ControlToValidate="ddlPermission" CssClass="text-danger" Display="Dynamic" ErrorMessage="A role or a permission must be selected, but both cannot be selected." OnServerValidate="validatorPermissionNotAlsoEntered_ServerValidate" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="<%=chkDenyFlag.ClientID %>" class="col-sm-3 control-label">Deny</label>
                        <div class="col-sm-9">
                            <asp:CheckBox runat="server" ID="chkDenyFlag" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="<%=ddlScopeType.ClientID %>" class="col-sm-3 control-label">Scope</label>
                        <div class="col-sm-3">
                            <asp:DropDownList runat="server" ID="ddlScopeType" CssClass="form-control" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlScopeType_SelectedIndexChanged">
                                <asp:ListItem Text="-- Select a Scope Type --"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:CustomValidator runat="server" ID="validatorSelectAScope" ControlToValidate="ddlScopeType" CssClass="text-danger" Display="Dynamic" ErrorMessage="Select a scope type." OnServerValidate="validatorSelectItem_ServerValidate" />
                        </div>
                        <div class="col-sm-6">
                            <asp:DropDownList runat="server" ID="ddlScopeValue" CssClass="form-control" AppendDataBoundItems="false" />
                            <asp:CustomValidator runat="server" ID="validatorSelectAResource" ControlToValidate="ddlScopeValue" CssClass="text-danger" Display="Dynamic" ErrorMessage="Select a resource." OnServerValidate="validatorSelectItem_ServerValidate" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-sm-9 col-sm-offset-3">
                            <asp:Button Text="Add Assignment" CssClass="btn btn-default" runat="server" ID="btnAddAssignment" OnClick="btnAddAssignment_Click" />
                        </div>
                    </div>
                </div>


            </asp:Panel>
        </asp:WizardStep>


        <asp:CompleteWizardStep Title="Complete">
            <CustomNavigationTemplate></CustomNavigationTemplate>
            <ContentTemplate>
                Successfully finished editing user. <asp:LinkButton runat="server" ID="lnkEditAgain" OnClick="lnkEditAgain_Click">Continue editing this user</asp:LinkButton> or <a href="Default.aspx">view all users</a>.
            </ContentTemplate>
        </asp:CompleteWizardStep>

    </WizardSteps>
</asp:CreateUserWizard>