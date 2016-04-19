<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlayerManagement.ascx.cs" Inherits="Trackr.Source.Wizards.PlayerManagement" %>

<style type="text/css">
    .alert.alert-danger.player-matches > div >table {
        color: initial;
        margin-top: 10px;
    }
    .alert.alert-danger.player-matches > div {
        background-color: white;
    }
</style>

<ui:AlertBox runat="server" ID="AlertBox" />

<asp:Wizard runat="server" ID="PlayerWizard" DisplaySideBar="true" OnNextButtonClick="PlayerWizard_NextButtonClick" OnFinishButtonClick="PlayerWizard_FinishButtonClick">
    <StepNavigationTemplate>
        <div class="pull-left">
            <asp:LinkButton runat="server" CommandName="MovePrevious" Text="Previous" CausesValidation="false" />
        </div>
        <div class="pull-right">
            <asp:LinkButton runat="server" CommandName="MoveNext" Text="Continue" />
        </div>
    </StepNavigationTemplate>
    <StartNavigationTemplate>
        <div class="pull-right">
            <asp:LinkButton runat="server" CommandName="MoveNext" Text="Continue" />
        </div>
    </StartNavigationTemplate>
    <FinishNavigationTemplate>
        <div class="pull-left">
            <asp:LinkButton runat="server" CommandName="MovePrevious" Text="Previous" CausesValidation="false" />
        </div>
        <div class="pull-right">
            <asp:LinkButton runat="server" CommandName="MoveComplete" Text="Save Player" />
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
        <asp:WizardStep runat="server" ID="Step1_Info" StepType="Start" Title="Player Information" OnActivate="Step1_Info_Activate">
            <!-- Nav tabs -->
            <ul class="nav nav-tabs">
                <li class="<%= mvPlayerInfoTabs.ActiveViewIndex == 0 ? "active" : "" %>"><asp:LinkButton runat="server" ID="lnkPlayerGeneral" Text="General" OnClick="lnkPlayerTab_Click" CommandArgument="0" CausesValidation="false" /></li>
                <li class="<%= mvPlayerInfoTabs.ActiveViewIndex == 1 ? "active" : !lnkPlayerAddress.Enabled ? "disabled" : "" %>"><asp:LinkButton runat="server" ID="lnkPlayerAddress" Text="Address" OnClick="lnkPlayerTab_Click" CommandArgument="1" CausesValidation="false" /></li>
                <li class="<%= mvPlayerInfoTabs.ActiveViewIndex == 2 ? "active" : !lnkPlayerEmails.Enabled ? "disabled" : "" %>"><asp:LinkButton runat="server" ID="lnkPlayerEmails" Text="Emails" OnClick="lnkPlayerTab_Click" CommandArgument="2" CausesValidation="false" /></li>
                <li class="<%= mvPlayerInfoTabs.ActiveViewIndex == 3 ? "active" : !lnkPlayerPhones.Enabled ? "disabled" : "" %>"><asp:LinkButton runat="server" ID="lnkPlayerPhones" Text="Phones" OnClick="lnkPlayerTab_Click" CommandArgument="3" CausesValidation="false" /></li>
            </ul>

            <!-- Tab panes -->
            <div class="tab-content">
                <div class="tab-pane active">
                    <asp:MultiView runat="server" ID="mvPlayerInfoTabs" ActiveViewIndex="0">
                        <asp:View runat="server">
                            <asp:Panel runat="server" ID="pnlPossiblePlayerMatches" CssClass="row" Visible="false">
                                <div class="col-sm-12">
                                    <div class="alert alert-danger player-matches" style="padding-bottom: 0px;">
                                        <strong>Possible Matches:</strong> The following players already exist and may match the one you are trying to create. If none of the players match, 
                                        <asp:LinkButton runat="server" ID="lnkContinueAnywaysPlayer" CausesValidation="true" OnClick="lnkContinueAnywaysPlayer_Click">then click here to add the player</asp:LinkButton>.

                                        <asp:GridView runat="server" ID="gvPossiblePlayerMatches" AutoGenerateColumns="false" CssClass="table table-striped">
                                            <Columns>
                                                <asp:BoundField DataField="FirstName" HeaderText="First name" />
                                                <asp:BoundField DataField="LastName" HeaderText="Last name" />
                                                <asp:BoundField DataField="DateOfBirth" HeaderText="Date of birth" DataFormatString="{0:MMM dd, yyyy}" />
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Repeater runat="server" DataSource='<%# Eval("Teams") %>'>
                                                            <ItemTemplate>
                                                                <%#Eval("Name") %><%#(int)Eval("Year") > DateTime.MinValue.Year ? " (" + Eval("Year").ToString() + ")" : "" %>
                                                            </ItemTemplate>
                                                            <SeparatorTemplate>
                                                                <br />
                                                            </SeparatorTemplate>
                                                        </asp:Repeater>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>


                                </div>
                            </asp:Panel>

                            <div class="row form-group">
                                <label for="<%=txtFirstName.ClientID %>" class="col-sm-12 control-label">First name</label>
                                <div class="col-sm-12">
                                    <asp:TextBox runat="server" ID="txtFirstName" CssClass="form-control" MaxLength="30" />
                                    <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorFirstNameRequired" ControlToValidate="txtFirstName" CssClass="text-danger" ErrorMessage="A first name is required." />
                                </div>
                            </div>

                            <div class="row form-group">
                                <label for="<%=txtMiddleInitial.ClientID %>" class="col-sm-12 control-label">Middle initial</label>
                                <div class="col-sm-12">
                                    <asp:TextBox runat="server" ID="txtMiddleInitial" MaxLength="1" CssClass="form-control" />
                                </div>
                            </div>

                            <div class="row form-group">
                                <label for="<%=txtLastName.ClientID %>" class="control-label col-sm-12">Last name</label>
                                <div class="col-sm-12">
                                    <asp:TextBox runat="server" ID="txtLastName" CssClass="form-control" MaxLength="30" />
                                    <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorLastNameRequired" ControlToValidate="txtLastName" CssClass="text-danger" ErrorMessage="A last name is required." />
                                </div>
                            </div>

                            <div class="row form-group">
                                <label for="<%=txtDateOfBirth.ClientID %>" class="col-sm-12 control-label">Date of birth</label>
                                <div class="col-sm-12">
                                    <asp:TextBox runat="server" ID="txtDateOfBirth" CssClass="form-control" MaxLength="30" TextMode="Date" />
                                    <asp:CustomValidator Display="Dynamic" runat="server" ID="validatorDOBParses" ControlToValidate="txtDateOfBirth" CssClass="text-danger" ErrorMessage="The date of birth must be entered in the format: MM/DD/YYYY and the date of birth must be greater than or equal to January 1, 1900 and less than or equal to January 1, 2200." OnServerValidate="validatorDateTimeParses_ServerValidate" />
                                    <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorDOBRequired" ControlToValidate="txtDateOfBirth" CssClass="text-danger" ErrorMessage="Player's date of birth is required." />
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-sm-12">
                                    <asp:LinkButton runat="server" ID="lnkSavePlayer" CausesValidation="true" OnClick="lnkSavePlayer_Click"><span class="glyphicon glyphicon-save"></span> Save General Information</asp:LinkButton>
                                </div>
                            </div>
                        </asp:View>

                        <asp:View runat="server">
                            <ui:AddressBook runat="server" ID="AddressBook_Player" />
                        </asp:View>

                        <asp:View runat="server">
                            <ui:EmailAddressBook runat="server" ID="EmailAddressBook_Player" />
                        </asp:View>

                        <asp:View runat="server">
                            <ui:PhoneNumberBook runat="server" ID="PhoneNumberBook_Player" />
                        </asp:View>
                    </asp:MultiView>
                </div>
            </div>
        </asp:WizardStep>


        <asp:WizardStep runat="server" ID="Step2_Guardian" StepType="Step" Title="Guardians">
            <asp:GridView runat="server" ID="gvGuardians" AutoGenerateColumns="false" SelectMethod="gvGuardians_GetData" EmptyDataText="This player does not have any guardians." CssClass="table table-striped table-hover" 
                OnRowEditing="gvGuardians_RowEditing" OnRowCancelingEdit="gvGuardians_RowCancelingEdit" DeleteMethod="gvGuardians_DeleteItem" DataKeyNames="EditToken">
                <Columns>
                    <asp:BoundField DataField="Guardian" HeaderText="Guardian" ReadOnly="true" />

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lnkRemoveGuardian" Visible='<%# (bool)Eval("IsRemovable") %>' CommandName="Delete" ToolTip="Remove guardian" CssClass="glyphicon glyphicon-trash" CausesValidation="false"></asp:LinkButton><span runat="server" visible='<%# (bool)Eval("IsRemovable") %>'>&nbsp;&nbsp;</span>
                            <asp:LinkButton runat="server" ID="lnkEditGuardian" CommandName="Edit" ToolTip="Edit guardian" CssClass="glyphicon glyphicon-edit" CausesValidation="false"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div class="row margin-bottom-5-px">
                <div class="col-sm-12">
                    <asp:LinkButton runat="server" ID="lnkAddGuardian" OnClick="lnkAddGuardian_Click" CausesValidation="false">
                        <i class="glyphicon glyphicon-plus-sign"></i>Add Guardian
                    </asp:LinkButton>
                </div>
            </div>

            <asp:Panel runat="server" ID="pnlAddGuardian" Visible="false">
                <div class="well">
                    <!-- Nav tabs -->
                    <ul class="nav nav-tabs">
                        <li class="<%= mvGuardianTabs.ActiveViewIndex == 0 ? "active" : "" %>"><asp:LinkButton runat="server" ID="lnkGuardianGeneral" Text="General" OnClick="lnkGuardianTab_Click" CommandArgument="0" CausesValidation="false" /></li>
                        <li class="<%= mvGuardianTabs.ActiveViewIndex == 1 ? "active" : gvGuardians.EditIndex == -1 ? "disabled" : "" %>"><asp:LinkButton runat="server" ID="lnkGuardianAddress" Text="Address" OnClick="lnkGuardianTab_Click" CommandArgument="1" CausesValidation="false" /></li>
                        <li class="<%= mvGuardianTabs.ActiveViewIndex == 2 ? "active" : gvGuardians.EditIndex == -1 ? "disabled" : "" %>"><asp:LinkButton runat="server" ID="lnkGuardianEmails" Text="Emails" OnClick="lnkGuardianTab_Click" CommandArgument="2" CausesValidation="false" /></li>
                        <li class="<%= mvGuardianTabs.ActiveViewIndex == 3 ? "active" : gvGuardians.EditIndex == -1 ? "disabled" : "" %>"><asp:LinkButton runat="server" ID="lnkGuardianPhones" Text="Phones" OnClick="lnkGuardianTab_Click" CommandArgument="3" CausesValidation="false" /></li>
                    </ul>

                    <!-- Tab panes -->
                    <div class="tab-content margin-bottom-5-px">
                        <div class="tab-pane active">
                            <asp:MultiView runat="server" ID="mvGuardianTabs" ActiveViewIndex="0">
                                <asp:View runat="server">
                                    <div class="form-group row">
                                        <label class="col-sm-12 control-label" for="<%=txtGuardianFirstName.ClientID %>">First name</label>
                                        <div class="col-sm-12">
                                            <asp:TextBox runat="server" ID="txtGuardianFirstName" CssClass="form-control" MaxLength="30" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtGuardianFirstName" CssClass="text-danger" ErrorMessage="The guardian's first name is required." Display="Dynamic" />
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <label class="col-sm-12 control-label" for="<%=txtGuardianMiddleInitial.ClientID %>">Middle initial</label>
                                        <div class="col-sm-12">
                                            <asp:TextBox runat="server" ID="txtGuardianMiddleInitial" CssClass="form-control" MaxLength="1" />
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <label class="col-sm-12 control-label" for="<%=txtGuardianLastName.ClientID %>">Last name</label>
                                        <div class="col-sm-12">
                                            <asp:TextBox runat="server" ID="txtGuardianLastName" CssClass="form-control" MaxLength="30" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtGuardianLastName" CssClass="text-danger" ErrorMessage="The guardian's last name is required." Display="Dynamic" />
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-sm-6 text-left">
                                            <asp:LinkButton runat="server" ID="lnkSaveGuardian" CausesValidation="true" OnClick="lnkSaveGuardian_Click"><span class="glyphicon glyphicon-save"></span>Save General Information</asp:LinkButton>
                                        </div>
                                    </div>
                                </asp:View>

                                <asp:View runat="server">
                                    <ui:AddressBook runat="server" ID="AddressBook" />
                                </asp:View>

                                <asp:View runat="server">
                                    <ui:EmailAddressBook runat="server" ID="EmailBook" />
                                </asp:View>

                                <asp:View runat="server">
                                    <ui:PhoneNumberBook runat="server" ID="PhoneNumberBook" />
                                </asp:View>
                            </asp:MultiView>
                        </div>
                    </div>
                    
                    <asp:LinkButton runat="server" ID="lnkAddEditGuardianClose" CausesValidation="false" OnClick="lnkAddEditGuardianClose_Click">
                        <span class="glyphicon glyphicon-folder-close"></span>Cancel
                    </asp:LinkButton>
                </div>
            </asp:Panel>
        </asp:WizardStep>


        <asp:WizardStep runat="server" ID="Step3_Picture" StepType="Step" Title="Player Passes">
            <asp:GridView runat="server" ID="gvPlayerPasses" AutoGenerateColumns="false" SelectMethod="gvPlayerPasses_GetData" EmptyDataText="This player does not have any player passes." CssClass="table table-striped table-hover" 
                OnRowEditing="gvPlayerPasses_RowEditing" OnRowCancelingEdit="gvPlayerPasses_RowCancelingEdit" DeleteMethod="gvPlayerPasses_DeleteItem" DataKeyNames="EditToken">
                <Columns>
                    <asp:BoundField DataField="Expiration" HeaderText="Expiration" DataFormatString="{0:MM/dd/yyyy}" ReadOnly="true" />
                    <asp:BoundField DataField="PassNumber" HeaderText="Pass Number" ReadOnly="true" />

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lnkRemovePlayerPass" Visible='<%# (bool)Eval("Editable") %>' CommandName="Delete" ToolTip="Remove player pass" CssClass="glyphicon glyphicon-trash" CausesValidation="false"></asp:LinkButton> <span runat="server" visible='<%# (bool)Eval("Editable") %>'>&nbsp;&nbsp;</span>
                            <asp:LinkButton runat="server" ID="lnkEditPlayerPass" Visible='<%# (bool)Eval("Editable") %>' CommandName="Edit" ToolTip="Edit player pass" CssClass="glyphicon glyphicon-edit" CausesValidation="false"></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="lnkViewPlayerPass" Visible='<%# !(bool)Eval("Editable") %>' CommandArgument='<%# Eval("EditToken") %>' OnClick="lnkViewPlayerPass_Click" CausesValidation="false" ToolTip="View player pass" CssClass="glyphicon glyphicon-eye-open"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div class="row">
                <div class="col-sm-12">
                    <asp:LinkButton runat="server" ID="lnkAddPlayerPass" OnClick="lnkAddPlayerPass_Click" CausesValidation="false">
                        <i class="glyphicon glyphicon-plus-sign"></i> Add Player Pass
                    </asp:LinkButton>
                </div>
            </div>

            <asp:Panel runat="server" ID="pnlAddEditPass" Visible="false">
                <div class="well">
                    <div class="row">
                        <div class="<%=string.Format("col-sm-{0}", divPreview.Visible ? 6:12) %>">
                            <div class="form-group row">
                                <label class="col-sm-12 control-label" for="<%=txtPassExpires.ClientID %>">Pass Expires On</label>
                                <div class="col-sm-12">
                                    <asp:TextBox runat="server" ID="txtPassExpires" TextMode="Date" CssClass="form-control" MaxLength="30" />
                                    <asp:CustomValidator runat="server" ID="validatorPlayerPassExpiresValid" ControlToValidate="txtPassExpires" CssClass="text-danger" ErrorMessage="The expiration date must be entered in the format: MM/DD/YYYY and the expiration date must be greater than or equal to January 1, 1900 and less than or equal to January 1, 2200." OnServerValidate="validatorDateTimeParses_ServerValidate" Display="Dynamic" />
                                    <asp:CustomValidator runat="server" ID="validatorPlayerPassExpiresDuplicate" ControlToValidate="txtPassExpires" CssClass="text-danger" ErrorMessage="There is already a player pass for this player with the specified expiration date." OnServerValidate="validatorPlayerPassExpiresDuplicate_ServerValidate" Display="Dynamic" />
                                    <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorPlayerPassExpiresRequired" ControlToValidate="txtPassExpires" CssClass="text-danger" ErrorMessage="The player pass expiration date is required." />
                                </div>
                            </div>

                            <div class="form-group row">
                                <label class="col-sm-12 control-label" for="<%=txtPassNumber.ClientID %>">Pass Number</label>
                                <div class="col-sm-12">
                                    <asp:TextBox runat="server" ID="txtPassNumber" CssClass="form-control" MaxLength="50" />
                                </div>
                            </div>

                            <asp:Panel runat="server" ID="pnlPhotoUpload">
                                <div class="form-group row" runat="server">
                                    <label class="col-sm-12 control-label" for="<%=uploadPlayerPass.ClientID %>">Player Pass Photo</label>
                                    <div class="col-sm-12">
                                        <asp:FileUpload runat="server" ID="uploadPlayerPass" AllowMultiple="false" />
                                    </div>
                                </div>

                                <div class="form-group row">
                                    <div class="col-sm-12">
                                        <p>Once you select the player's photo, click <strong>Preview Photo</strong>. <br />
                                            Once you are satisfied with the preview, click <strong>Save Player Pass</strong>.<br />
                                            <strong>Note:</strong> If you do not choose a file and click <strong>Preview Photo</strong>, then the previous image will be removed.
                                        </p>                                    
                                    </div>
                                
                                    <div class="col-sm-12">
                                        <asp:LinkButton runat="server" ID="lnkUpload" OnClick="btnUpload_Click" CausesValidation="false"><i class="glyphicon glyphicon-upload"></i> Preview Photo</asp:LinkButton><br />
                                        <asp:LinkButton runat="server" ID="lnkReloadImage" OnClick="lnkReloadImage_Click" CausesValidation="false"><i class="glyphicon glyphicon-refresh"></i> Reload Existing</asp:LinkButton><br />
                                        <asp:LinkButton runat="server" ID="lnkSavePlayerPass" CausesValidation="true" OnClick="lnkSavePlayerPass_Click"><i class="glyphicon glyphicon-save"></i> Save Player Pass</asp:LinkButton>
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                        <div class="col-sm-6" runat="server" id="divPreview" visible="false">
                            <div class="form-group row">
                                <div class="col-sm-12 text-right"><strong>Preview:</strong></div>
                                <div class="col-sm-12 text-right">
                                    <asp:Image runat="server" ID="imgUploadPreview" CssClass="img-responsive" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </asp:WizardStep>


        <asp:WizardStep runat="server" ID="Step4_Teams" StepType="Finish" Title="Teams">
            <asp:GridView runat="server" ID="gvTeamAssignments" AutoGenerateColumns="false" SelectMethod="gvTeamAssignments_GetData" EmptyDataText="This player is not assigned to any teams." CssClass="table table-striped table-hover" 
                OnRowEditing="gvTeamAssignments_RowEditing" OnRowCancelingEdit="gvTeamAssignments_RowCancelingEdit" DeleteMethod="gvTeamAssignmentss_DeleteItem" DataKeyNames="TeamPlayerID">
                <Columns>
                    <asp:BoundField DataField="Season" HeaderText="Season" ReadOnly="true" />
                    <asp:BoundField DataField="ProgramName" HeaderText="Program Name" ReadOnly="true" />
                    <asp:BoundField DataField="TeamName" HeaderText="Team Name" ReadOnly="true" />
                    <asp:BoundField DataField="PlayerPassNumber" HeaderText="Pass #" ReadOnly="true" />

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lnkRemoveTeamPlayer" Visible='<%# (bool)Eval("IsRemovable") %>' CommandName="Delete" ToolTip="Remove team assignment" CssClass="glyphicon glyphicon-trash" CausesValidation="false"></asp:LinkButton> <span runat="server" visible='<%# (bool)Eval("IsRemovable") %>'>&nbsp;&nbsp;</span>
                            <asp:LinkButton runat="server" ID="lnkEditTeamPlayer" Visible='<%# (bool)Eval("IsRemovable") %>' CommandName="Edit" ToolTip="Edit team assignment" CssClass="glyphicon glyphicon-edit" CausesValidation="false"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div class="row">
                <div class="col-sm-12">
                    <asp:LinkButton runat="server" ID="lnkAddTeamPlayer" OnClick="lnkAddTeamPlayer_Click" CausesValidation="false">
                        <i class="glyphicon glyphicon-plus-sign"></i> Add Team Assignment
                    </asp:LinkButton>
                </div>
            </div>

            <asp:Panel runat="server" ID="pnlAddEditTeamPlayer" Visible="false">
                <div class="well">
                    <ui:ProgramTeamPlayerPicker runat="server" ID="ptpPicker" Permission="PlayerManagement.EditPlayer" EnableTeamSelect="true" RequiredSelectOf="Team" />

                    <div class="form-group row">
                        <label class="col-sm-12 control-label" for="<%=ddlPlayerPassForTeam.ClientID %>">Player Pass</label>
                        <div class="col-sm-12">
                            <asp:DropDownList runat="server" ID="ddlPlayerPassForTeam" CssClass="form-control" AppendDataBoundItems="true" SelectMethod="ddlPlayerPassForTeam_GetData" DataTextField="Label" DataValueField="Value">
                                <asp:ListItem Text="-- Select Player Pass --" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-group row">
                        <div class="col-sm-12">
                            <asp:CheckBox runat="server" ID="chkIsSecondary" /> <label for="<%=chkIsSecondary.ClientID %>">Is Secondary Player</label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-12">
                            <asp:LinkButton runat="server" ID="lnkSaveTeamPlayer" CausesValidation="true" OnClick="lnkSaveTeamPlayer_Click"><i class="glyphicon glyphicon-save"></i> Save Team Assignment</asp:LinkButton>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </asp:WizardStep>


        <asp:WizardStep StepType="Complete" Title="Complete">
            <asp:LinkButton runat="server" ID="lnkEditAgain" OnClick="lnkEditAgain_Click">Continue editing this player</asp:LinkButton> or <a href="Default.aspx">view all players</a>.
        </asp:WizardStep>

    </WizardSteps>
</asp:Wizard>