<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlayerManagement.ascx.cs" Inherits="Trackr.Source.Wizards.PlayerManagement" %>

<ui:AlertBox runat="server" ID="AlertBox" />

<asp:Wizard runat="server" ID="PlayerWizard" DisplaySideBar="true" OnNextButtonClick="PlayerWizard_NextButtonClick" OnFinishButtonClick="PlayerWizard_FinishButtonClick">
    <StepNavigationTemplate>
        <div class="text-right">
            <asp:Button runat="server" CommandName="MovePrevious" Text="Previous" CssClass="btn btn-default" CausesValidation="false" />
            <asp:Button runat="server" CommandName="MoveNext" Text="Save & Continue" CssClass="btn btn-default" />
        </div>
    </StepNavigationTemplate>
    <StartNavigationTemplate>
        <div class="text-right" runat="server">
            <asp:Button runat="server" ID="btnStart" CommandName="MoveNext" Text='Save & Continue' CssClass="btn btn-default" />
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

        <div>
            <asp:PlaceHolder runat="server" ID="wizardStepPlaceholder"></asp:PlaceHolder>
        </div>

        <div>
            <asp:PlaceHolder runat="server" ID="navigationPlaceholder"></asp:PlaceHolder>
        </div>
    </LayoutTemplate>
    

    <WizardSteps>
        <asp:WizardStep runat="server" ID="Step1_Info" StepType="Start" Title="Player Information">
            <div class="form-horizontal">
                <div class="form-group">
                    <label for="<%=txtFirstName.ClientID %>" class="col-sm-4 control-label">First name</label>
                    <div class="col-sm-8">
                        <asp:TextBox runat="server" ID="txtFirstName" CssClass="form-control" MaxLength="30" />
                        <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorFirstNameRequired" ControlToValidate="txtFirstName" CssClass="text-danger" ErrorMessage="A first name is required." />
                    </div>
                </div>
                <div class="form-group">
                    <label for="<%=txtMiddleInitial.ClientID %>" class="col-sm-4 control-label">Middle initial</label>
                    <div class="col-sm-8">
                        <asp:TextBox runat="server" ID="txtMiddleInitial" MaxLength="1" CssClass="form-control" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="<%=txtLastName.ClientID %>" class="col-sm-4 control-label">Last name</label>
                    <div class="col-sm-8">
                        <asp:TextBox runat="server" ID="txtLastName" CssClass="form-control" MaxLength="30" />
                        <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorLastNameRequired" ControlToValidate="txtLastName" CssClass="text-danger" ErrorMessage="A last name is required." />
                    </div>
                </div>
                <div class="form-group">
                    <label for="<%=txtDateOfBirth.ClientID %>" class="col-sm-4 control-label">Date of birth</label>
                    <div class="col-sm-8">
                        <asp:TextBox runat="server" ID="txtDateOfBirth" CssClass="form-control" MaxLength="30" TextMode="Date" />
                        <asp:CustomValidator Display="Dynamic" runat="server" ID="validatorDOBParses" ControlToValidate="txtDateOfBirth" CssClass="text-danger" ErrorMessage="The date of birth must be entered in the format: MM/DD/YYYY and the date of birth must be greater than or equal to January 1, 1900 and less than or equal to January 1, 2200." OnServerValidate="validatorDateTimeParses_ServerValidate" />
                        <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorDOBRequired" ControlToValidate="txtDateOfBirth" CssClass="text-danger" ErrorMessage="Player's date of birth is required." />
                    </div>
                </div>
            </div>
        </asp:WizardStep>

        <asp:WizardStep runat="server" ID="Step2_Picture" StepType="Step" Title="Player Passes">
            <asp:GridView runat="server" ID="gvPlayerPasses" AutoGenerateColumns="false" SelectMethod="gvPlayerPasses_GetData" EmptyDataText="This player does not have any player passes." CssClass="table table-striped table-hover" 
                OnRowEditing="gvPlayerPasses_RowEditing" OnRowCancelingEdit="gvPlayerPasses_RowCancelingEdit" DeleteMethod="gvPlayerPasses_DeleteItem" DataKeyNames="PlayerPassID">
                <Columns>
                    <asp:BoundField DataField="Expiration" HeaderText="Expiration" DataFormatString="{0:MM/dd/yyyy}" ReadOnly="true" />
                    <asp:BoundField DataField="PassNumber" HeaderText="Pass Number" ReadOnly="true" />

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lnkRemovePlayerPass" Visible='<%# (bool)Eval("Editable") %>' CommandName="Delete" ToolTip="Remove player pass" CssClass="glyphicon glyphicon-trash" CausesValidation="false"></asp:LinkButton> <span runat="server" visible='<%# (bool)Eval("Editable") %>'>&nbsp;&nbsp;</span>
                            <asp:LinkButton runat="server" ID="lnkEditPlayerPass" Visible='<%# (bool)Eval("Editable") %>' CommandName="Edit" ToolTip="Edit player pass" CssClass="glyphicon glyphicon-edit" CausesValidation="false"></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="lnkViewPlayerPass" Visible='<%# !(bool)Eval("Editable") %>' CommandArgument='<%# Eval("PlayerPassID") %>' OnClick="lnkViewPlayerPass_Click" CausesValidation="false" ToolTip="View player pass" CssClass="glyphicon glyphicon-eye-open"></asp:LinkButton>
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


        <asp:WizardStep runat="server" ID="Step3_Teams" StepType="Finish" Title="Teams">
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
            Successfully finished editing player. <asp:LinkButton runat="server" ID="lnkEditAgain" OnClick="lnkEditAgain_Click">Continue editing this player</asp:LinkButton> or <a href="Default.aspx">view all players</a>.
        </asp:WizardStep>

    </WizardSteps>
</asp:Wizard>