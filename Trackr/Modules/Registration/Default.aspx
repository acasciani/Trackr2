<%@ Page Title="Season Registration" Language="C#" MasterPageFile="~/SiteNoNavigation.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Trackr.Modules.Registration.Default" %>

<%@ Register Src="~/Source/Wizards/PlayerManagement.ascx" TagName="PlayerManagement" TagPrefix="ui" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script runat="server">
        public string GetPercentageFilled(decimal percent)
        {
            if (percent == 100)
            {
                return "<span class=\"label label-danger\">Full</span>";
            }
            else if (percent >= 75)
            {
                return "<span class=\"label label-warning\">Near Full</span>";
            }
            else
            {
                return "<span class=\"label label-success\">Open</span>";
            }
        }
    </script>

    <style type="text/css">
        .player-matches table, .proceed-registration table {
            color: initial;
            margin-top: 10px;
            margin-bottom: 0px;
        }

        .player-matches table td, .proceed-registration table td {
            cursor: pointer;
        }
    </style>
    <script>
        function pageLoad() {
            var name = '<%=gvPossiblePlayerMatches.ClientID.ToString()%>';
            $('#' + name + ' tr').click(function (element) {
                $(element.target.parentElement).find('input:radio').attr('checked', 'true')
            });

            var name2 = '<%=gvTeamsToRegisterFor.ClientID.ToString()%>';
            $('#' + name2 + ' tr').click(function (element) {
                $(element.target.parentElement).find('input:radio').attr('checked', 'true')
            });
        }
    </script>

    <div class="panel panel-default">
        <div class="panel-heading">
            <h4><%=RegistrationYear.ToString() %> <%=ClubName %> Registration</h4>
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-sm-12">
                    Registration for the <%=RegistrationYear.ToString() %> season is now open. Please complete your registration by completing this step by step process. If you are registering multiple players, you will need to complete this form for each player.<br /><br />
                    <a href="Default.aspx">If you need to restart, please click here</a>.
                </div>
            </div>
        </div>
    </div>


    <asp:MultiView runat="server" ID="mvRegister" ActiveViewIndex="0">
        <asp:View runat="server" ID="vPreviouslyPlayed">
            <!-- previously played - true, try and find -->
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>Lookup Player Record</h4>
                </div>
                <div class="panel-body player-matches">
                    <ui:AlertBox runat="server" ID="AlertBox" />

                    <div class="row">
                        <div class="col-sm-12" runat="server" id="divPlayersFoundMessage" visible="false">
                            <strong>Your Players:</strong> The following players are all linked to your account. Select the player you would like to register and press the Continue button. 
                            If you do not see your son/daughter in the list below and they are a returning player, please contact the club.
                        </div>
                        <div class="col-sm-12" runat="server" id="divNoPlayersFoundMessage" visible="false">
                            No players were found that are associated to your account. If your son/daughter played for <%= ClubName %> in previous years, please contact the club.
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-12">
                            <div class="radio">
                                <label>
                                    <input type="radio" name="SelectedPlayer" value='new' /> Click here if your son/daughter is a first year player
                                </label>
                            </div>                            
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-12">
                            <div>
                                <asp:GridView runat="server" ID="gvPossiblePlayerMatches" AutoGenerateColumns="false" CssClass="table table-striped table-hover" Style="padding-bottom: 0px;">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Player's name">
                                            <ItemTemplate>
                                                <%# Eval("FirstName") + " " + Eval("LastName") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="DateOfBirth" HeaderText="Date of birth" DataFormatString="{0:MMM dd, yyyy}" ItemStyle-CssClass="col-xs-3" />
                                        <asp:TemplateField ItemStyle-CssClass="col-xs-1 text-center">
                                            <ItemTemplate>
                                                <input type="radio" name="SelectedPlayer" value='<%#Eval("PlayerID") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </asp:View>

        
        <asp:View runat="server" ID="vWizardNewPlayer">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>
                        Update Player's Information
                    </h4>
                </div>
                <div class="panel-body">
                    <ui:AlertBox runat="server" ID="AlertBox_PlayerInfo" />

                    <ui:PlayerManagement runat="server" ID="widgetPlayerManagement" CreatePermission="PlayerManagement.RegisterNewPlayer" EditPermission="PlayerManagement.ReRegisterPlayer" OnPlayerSavedError="widgetPlayerManagement_PlayerSavedError" OnPlayerSavedSuccess="widgetPlayerManagement_PlayerSavedSuccess" />
                </div>
            </div>
        </asp:View>

        <asp:View runat="server" ID="wizardSelectTeam">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>
                        Register <asp:Literal runat="server" ID="litPlayerFirstName" Text="Player" /> For <%=RegistrationYear%>
                    </h4>
                </div>
                <div class="panel-body">
                    <div class="row proceed-registration">
                        <div class="col-sm-12">
                            <ui:AlertBox runat="server" ID="AlertBox_PlayerRegistration" />

                            <div>
                                Select a team from below to register for next year and click "Continue".
                            </div>
                            <div>
                                <asp:GridView runat="server" ID="gvTeamsToRegisterFor" AutoGenerateColumns="false" CssClass="table table-striped table-hover" Style="padding-bottom: 0px;" EmptyDataText="There are currently no available teams to register this player for.">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Team">
                                            <ItemTemplate>
                                                <strong><%# Eval("TeamName") %></strong> <%# ((DateTime)Eval("TeamStart")).ToString("MMM yyyy") %> to <%# ((DateTime)Eval("TeamEnd")).ToString("MMM yyyy") %> - <%# Eval("ProgramName") %> <%# (bool)Eval("AlreadyRegistered") ? "<span class=\"label label-success\">Already Registered</span>" : GetPercentageFilled((decimal)Eval("PercentRegistered")).ToString() %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-CssClass="col-xs-1 text-center">
                                            <ItemTemplate>
                                                <%# !(bool)Eval("IsTeamFull") && !(bool)Eval("AlreadyRegistered") ? "<input type=\"radio\" name=\"SelectedTeam\" value='" + Eval("TeamID") + "' />" : "" %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </asp:View>

        <asp:View runat="server" ID="wizardComplete">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>
                        Registration Complete
                    </h4>
                </div>
                <div class="panel-body">
                    You have successfully completed registration for this player. <a href="Default.aspx">Click here to register another player.</a>
                </div>
            </div>
        </asp:View>
    </asp:MultiView>

    <div class="row" runat="server" id="divNavigation">
        <div class="col-sm-12">
            <%{
                                                    if (StepHistory.Count > 0)
                                                    { %>
                <span style="margin-right: 20px;">
                <asp:LinkButton runat="server" ID="lnkBackStep" OnClick="lnkBackStep_Click" CausesValidation="false" CssClass="btn btn-default">Back</asp:LinkButton>
                </span>
                <%}
                                                } %>

            <span>
                <asp:LinkButton runat="server" ID="lnkContinueStep" OnClick="lnkContinueStep_Click" CausesValidation="true" CssClass="btn btn-default">
                    <%=mvRegister.ActiveViewIndex == (mvRegister.Views.Count - 2) ? "Complete" : "Continue" %> Registration
                </asp:LinkButton>
            </span>
        </div>
    </div>



</asp:Content>
