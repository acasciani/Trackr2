<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TeamManagement.ascx.cs" Inherits="Trackr.Source.Wizards.TeamManagement" %>

<ui:AlertBox runat="server" ID="AlertBox" />

<asp:Wizard runat="server" ID="TeamWizard" DisplaySideBar="true" OnNextButtonClick="TeamWizard_NextButtonClick" OnFinishButtonClick="TeamWizard_FinishButtonClick">
    <StepNavigationTemplate>
        <div class="text-left" style="margin-bottom: 0;">
            <asp:LinkButton runat="server" CommandName="MovePrevious" Text="Previous" CausesValidation="false" />
            &nbsp;&nbsp;|&nbsp;&nbsp;
            <asp:LinkButton runat="server" CommandName="MoveNext" Text="Continue Editing Info" />
        </div>
    </StepNavigationTemplate>
    <StartNavigationTemplate>
        <div class="text-left" style="margin-bottom: 0;">
            <asp:LinkButton runat="server" CommandName="MoveNext" Text="Continue Editing Info" />
        </div>
    </StartNavigationTemplate>
    <FinishNavigationTemplate>
        <div class="text-left" style="margin-bottom: 0;">
            <asp:LinkButton runat="server" CommandName="MovePrevious" Text="Previous" CausesValidation="false" />
            &nbsp;&nbsp;|&nbsp;&nbsp;
            <asp:LinkButton runat="server" CommandName="MoveComplete" Text="Save Team Info" />
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
                <li><asp:LinkButton runat="server" ID="sideBarButton" CausesValidation="false"></asp:LinkButton></li>
            </ItemTemplate>
            <SelectedItemTemplate>
                <li class="active"><asp:LinkButton runat="server" ID="sideBarButton" CausesValidation="false"></asp:LinkButton></li>
            </SelectedItemTemplate>
        </asp:ListView>
    </SideBarTemplate>


    <LayoutTemplate>
        <div>
            <asp:PlaceHolder runat="server" ID="sideBarPlaceholder"></asp:PlaceHolder>
        </div>

        <div class="margin-bottom-20-px">
            <asp:PlaceHolder runat="server" ID="wizardStepPlaceholder"></asp:PlaceHolder>
        </div>

        <div>
            <asp:PlaceHolder runat="server" ID="navigationPlaceholder"></asp:PlaceHolder>
        </div>
    </LayoutTemplate>
    

    <WizardSteps>
        <asp:WizardStep runat="server" ID="Step1_Info" StepType="Start" Title="Team Information">
            <ui:ProgramTeamPlayerPicker runat="server" ID="ptpPicker" RequiredSelectOf="Program" />

            <div class="row form-group">
                <label for="<%=txtTeamName.ClientID %>" class="col-sm-12 control-label">Team Name</label>
                <div class="col-sm-12">
                    <asp:TextBox runat="server" ID="txtTeamName" MaxLength="100" CssClass="form-control" />
                    <asp:RequiredFieldValidator Display="Dynamic" runat="server" ControlToValidate="txtTeamName" CssClass="text-danger" ErrorMessage="A team name is required." />
                    <asp:RegularExpressionValidator Display="Dynamic" runat="server" ControlToValidate="txtTeamName" CssClass="text-danger" ErrorMessage="The team name must be between 2 and 100 characters long." ValidationExpression="^(.){2,100}$" />
                </div>
            </div>

            <div class="row form-group">
                <label for="<%=txtActiveFrom.ClientID %>" class="control-label col-sm-12">Active Range</label>
                <div class="col-xs-5">
                    <asp:TextBox runat="server" ID="txtActiveFrom" CssClass="form-control" MaxLength="30" TextMode="Date" />
                </div>
                <div class="col-xs-2 text-center">
                    <label class="form-control-static" for="<%= txtActiveTo.ClientID %>">to</label>
                </div>
                <div class="col-xs-5">
                    <asp:TextBox runat="server" ID="txtActiveTo" CssClass="form-control" MaxLength="30" TextMode="Date" />
                </div>

                <div class="col-sm-12">
                    <asp:CustomValidator Display="Dynamic" runat="server" ControlToValidate="txtActiveFrom" CssClass="text-danger" ErrorMessage="The active from date must be entered in the format: MM/DD/YYYY and must be greater than or equal to January 1, 1900 and less than or equal to January 1, 2200." OnServerValidate="validatorDateTimeParses_ServerValidate" />
                    <asp:CustomValidator Display="Dynamic" runat="server" ControlToValidate="txtActiveTo" CssClass="text-danger" ErrorMessage="The active to date must be entered in the format: MM/DD/YYYY and must be greater than or equal to January 1, 1900 and less than or equal to January 1, 2200." OnServerValidate="validatorDateTimeParses_ServerValidate" />
                    <asp:RequiredFieldValidator Display="Dynamic" runat="server" ControlToValidate="txtActiveFrom" CssClass="text-danger" ErrorMessage="The active from date is required." />
                    <asp:RequiredFieldValidator Display="Dynamic" runat="server" ControlToValidate="txtActiveTo" CssClass="text-danger" ErrorMessage="The active to date is required." />
                    <asp:CompareValidator Display="Dynamic" runat="server" ControlToValidate="txtActiveTo" CssClass="text-danger" ErrorMessage="The active to date must be greater than or equal to the active from date." ControlToCompare="txtActiveFrom" Operator="GreaterThanEqual" Type="Date" />
                </div>
            </div>

            <div class="row form-group">
                <label for="<%=txtMinRosterSize.ClientID %>" class="col-sm-12 control-label">Min. Roster Size</label>
                <div class="col-sm-12">
                    <asp:TextBox runat="server" ID="txtMinRosterSize" CssClass="form-control" MaxLength="4" TextMode="Number" />
                    <asp:RequiredFieldValidator Display="Dynamic" runat="server" ControlToValidate="txtMinRosterSize" CssClass="text-danger" ErrorMessage="Please specify a minimum roster size." />
                    <asp:RangeValidator Display="Dynamic" runat="server" ControlToValidate="txtMinRosterSize" CssClass="text-danger" ErrorMessage="The minimum roster size must be greater than 0 and less than 10,000." MinimumValue="1" MaximumValue="9999" Type="Integer" />
                </div>
            </div>

            <div class="row form-group">
                <label for="<%=txtMaxRosterSize.ClientID %>" class="col-sm-12 control-label">Max. Roster Size</label>
                <div class="col-sm-12">
                    <asp:TextBox runat="server" ID="txtMaxRosterSize" CssClass="form-control" MaxLength="4" TextMode="Number" />
                    <asp:RequiredFieldValidator Display="Dynamic" runat="server" ControlToValidate="txtMaxRosterSize" CssClass="text-danger" ErrorMessage="Please specify a maximum roster size." />
                    <asp:RangeValidator Display="Dynamic" runat="server" ControlToValidate="txtMaxRosterSize" CssClass="text-danger" ErrorMessage="The maximum roster size must be greater than 0 and less than 10,000." MinimumValue="1" MaximumValue="9999" Type="Integer" />
                    <asp:CompareValidator Display="Dynamic" runat="server" ControlToValidate="txtMaxRosterSize" CssClass="text-danger" ErrorMessage="The maximum roster size must be greater than or equal to the minimum roster size." ControlToCompare="txtMinRosterSize" Operator="GreaterThanEqual" Type="Integer" />
                </div>
            </div>

            <div class="row form-group">
                <label for="<%=txtMaxDOB.ClientID %>" class="control-label col-sm-12">Max. Date of Birth</label>
                <div class="col-sm-12">
                    <asp:TextBox runat="server" ID="txtMaxDOB" CssClass="form-control" MaxLength="30" TextMode="Date" />
                    <asp:CustomValidator Display="Dynamic" runat="server" ControlToValidate="txtMaxDOB" CssClass="text-danger" ErrorMessage="The maximum date of birth must be entered in the format: MM/DD/YYYY and must be greater than or equal to January 1, 1900 and less than or equal to January 1, 2200." OnServerValidate="validatorDateTimeParses_ServerValidate" />
                    <asp:RequiredFieldValidator Display="Dynamic" runat="server" ControlToValidate="txtMaxDOB" CssClass="text-danger" ErrorMessage="The maximum date of birth is required." />
                </div>
            </div>
        </asp:WizardStep>


        <asp:WizardStep runat="server" ID="Step2_RegistrationRule" StepType="Finish" Title="Registration">
            <asp:GridView runat="server" ID="gvRegRules" AutoGenerateColumns="false" SelectMethod="gvRegRules_GetData" EmptyDataText="This team does not have any registration rules." CssClass="table table-striped table-hover" 
                OnRowEditing="gvRegRules_RowEditing" OnRowCancelingEdit="gvRegRules_RowCancelingEdit" DeleteMethod="gvRegRules_DeleteItem" DataKeyNames="EditToken">
                <Columns>
                    <asp:BoundField DataField="TeamName" HeaderText="Previous Team" ReadOnly="true" />

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lnkRemoveRegistrationRule" Visible='<%# (bool)Eval("IsRemovable") %>' CommandName="Delete" ToolTip="Remove registration rule" CssClass="glyphicon glyphicon-trash" CausesValidation="false"></asp:LinkButton><span runat="server" visible='<%# (bool)Eval("IsRemovable") %>'>&nbsp;&nbsp;</span>
                            <asp:LinkButton runat="server" ID="lnkEditRegistrationRule" CommandName="Edit" ToolTip="Edit registration rule" CssClass="glyphicon glyphicon-edit" CausesValidation="false"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div class="row margin-bottom-5-px">
                <div class="col-sm-12">
                    <asp:LinkButton runat="server" ID="lnkAddRegistrationRule" OnClick="lnkAddRegistrationRule_Click" CausesValidation="false">
                        <i class="glyphicon glyphicon-plus-sign"></i>Add Registration Rule
                    </asp:LinkButton>
                </div>
            </div>

            <asp:Panel runat="server" ID="pnlAddRegistrationRule" Visible="false">
                <div class="well">
                    <div class="tab-pane full">
                    <div class="form-group row">
                        <label class="col-sm-12 control-label">Previous Team</label>
                        <div class="col-sm-12">
                            If players on a previous team should be allowed to join this team, then select the previous team. If anyone should be allowed to join this team regardless of their previous 
                                            team association, then do not select a team.
                        </div>
                        <div class="col-xs-offset-1 col-xs-11">
                            <ui:ProgramTeamPlayerPicker runat="server" ID="ptpPicker_PreviousTeam" EnableTeamSelect="true" />
                        </div>
                    </div>

                    <div class="row form-group">
                        <label for="<%=txtRegistrationOpenFrom.ClientID %>" class="control-label col-xs-12">Registration Active</label>
                        <div class="col-xs-5">
                            <asp:TextBox runat="server" ID="txtRegistrationOpenFrom" CssClass="form-control" MaxLength="30" TextMode="Date" />
                        </div>
                        <div class="col-xs-2 text-center">
                            <label class="form-control-static" for="<%= txtRegistrationOpenTo.ClientID %>">to</label>
                        </div>
                        <div class="col-xs-5">
                            <asp:TextBox runat="server" ID="txtRegistrationOpenTo" CssClass="form-control" MaxLength="30" TextMode="Date" />
                        </div>

                        <div class="col-sm-12">
                            <asp:CustomValidator Display="Dynamic" runat="server" ControlToValidate="txtRegistrationOpenFrom" CssClass="text-danger" ErrorMessage="The registraion open date must be entered in the format: MM/DD/YYYY and must be greater than or equal to January 1, 1900 and less than or equal to January 1, 2200." OnServerValidate="validatorDateTimeParses_ServerValidate" />
                            <asp:CustomValidator Display="Dynamic" runat="server" ControlToValidate="txtRegistrationOpenTo" CssClass="text-danger" ErrorMessage="The registration close date must be entered in the format: MM/DD/YYYY and must be greater than or equal to January 1, 1900 and less than or equal to January 1, 2200." OnServerValidate="validatorDateTimeParses_ServerValidate" />
                            <asp:RequiredFieldValidator Display="Dynamic" runat="server" ControlToValidate="txtRegistrationOpenFrom" CssClass="text-danger" ErrorMessage="The registration open date is required." />
                            <asp:RequiredFieldValidator Display="Dynamic" runat="server" ControlToValidate="txtRegistrationOpenTo" CssClass="text-danger" ErrorMessage="The registration close date is required." />
                            <asp:CompareValidator Display="Dynamic" runat="server" ControlToValidate="txtRegistrationOpenTo" CssClass="text-danger" ErrorMessage="The registration close date must be greater than or equal to the registration open date." ControlToCompare="txtRegistrationOpenFrom" Operator="GreaterThanEqual" Type="Date" />
                        </div>
                    </div>

                    <div class="row form-group">
                        <label for="<%=txtAgeCutoff.ClientID %>" class="control-label col-xs-12">Age Cutoff</label>
                        <div class="col-xs-12">
                            <asp:TextBox runat="server" ID="txtAgeCutoff" CssClass="form-control" MaxLength="30" TextMode="Date" />
                            <asp:CustomValidator Display="Dynamic" runat="server" ControlToValidate="txtAgeCutoff" CssClass="text-danger" ErrorMessage="The age cutoff date must be entered in the format: MM/DD/YYYY and must be greater than or equal to January 1, 1900 and less than or equal to January 1, 2200." OnServerValidate="validatorDateTimeParses_ServerValidate" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-12 text-left">
                            <asp:LinkButton runat="server" ID="lnkSaveRegistrationRule" CausesValidation="true" OnClick="lnkSaveRegistrationRule_Click"><span class="glyphicon glyphicon-save"></span>Save Registration Rule Information</asp:LinkButton>
                        </div>
                    </div>
                        </div>
                    <asp:LinkButton runat="server" ID="lnkAddEditRegistrationRuleClose" CausesValidation="false" OnClick="lnkAddEditRegistrationRuleClose_Click">
                        <span class="glyphicon glyphicon-folder-close"></span>Cancel
                    </asp:LinkButton>
                </div>
            </asp:Panel>

        </asp:WizardStep>



        <asp:WizardStep StepType="Complete" Title="Complete">
            <a href="<%=Request.Url.AbsoluteUri %>">Continue editing this team</a> or <a href="Default.aspx">view all teams</a>.
        </asp:WizardStep>

    </WizardSteps>
</asp:Wizard>