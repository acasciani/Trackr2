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
        <asp:WizardStep runat="server" ID="Step1_Info" StepType="Start" Title="Team Information" OnActivate="Step1_Info_Activate">

            <!-- Tab panes -->
            <div class="tab-content">
                <div class="tab-pane active">


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
                                    <asp:LinkButton runat="server" ID="lnkSaveTeam" CausesValidation="true" OnClick="lnkSaveTeam_Click" Visible="false"><span class="glyphicon glyphicon-save"></span> Save General Information</asp:LinkButton>
                                </div>
                            </div>
                </div>
            </div>
        </asp:WizardStep>


        <asp:WizardStep runat="server" ID="Step2_RegistrationRule" StepType="Step" Title="Registration Rules">
            TODO
        </asp:WizardStep>



        <asp:WizardStep StepType="Complete" Title="Complete">
            <a href="<%=Request.Url.AbsoluteUri %>">Continue editing this team</a> or <a href="Default.aspx">view all teams</a>.
        </asp:WizardStep>

    </WizardSteps>
</asp:Wizard>