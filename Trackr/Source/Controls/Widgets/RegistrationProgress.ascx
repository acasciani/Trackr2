<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RegistrationProgress.ascx.cs" Inherits="Trackr.Source.Controls.Widgets.RegistrationProgress" %>

<asp:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel">
    <ProgressTemplate>
        <div style="width: 300px; height: 220px; position: absolute; background-color: rgba(255,255,255, 0.85); z-index: 5000;">
        </div>

        <div style="width: 300px; height: 220px; position: absolute; z-index: 5001; padding-top: 101px;">
            <div class="loading spinner">
                <div class="bounce1"></div>
                <div class="bounce2"></div>
                <div class="bounce3"></div>
            </div>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>


<asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
    <ContentTemplate>
        <%
            RegistrationProgressDTO dto = ShowTotalRegistered(true);
        %>

        <asp:ScriptManagerProxy runat="server">
            <Scripts>
                <asp:ScriptReference Path="//code.highcharts.com/highcharts.js" />
                <asp:ScriptReference Path="//code.highcharts.com/highcharts-more.js" />
                <asp:ScriptReference Path="//code.highcharts.com/modules/solid-gauge.js" />
                <asp:ScriptReference Path="~/Scripts/charting/RegistrationWidget_Gauge.js" />
            </Scripts>
        </asp:ScriptManagerProxy>

        <div style="width: 300px;">
            <div runat="server" id="chart" style="width: 300px; height: 200px; float: left"></div>
            <div class="clearfix"></div>
            <div class="text-center" style="position: relative;">
                <asp:LinkButton runat="server" ID="lnkShowChange" OnClick="lnkShowChange_Click" CommandArgument="ShowApproved">
                    Show Approved Players
                </asp:LinkButton>
                <span style="position: absolute; right: 20px;">
                    <asp:LinkButton runat="server" ID="lnkHideTeam" OnClick="lnkHideTeam_Click" ToolTip="Hide Team">
                                <span class="glyphicon glyphicon-eye-close"></span>
                    </asp:LinkButton>
                </span>
            </div>
        </div>

        <script type="text/javascript">
            UpdateGauge($('#<%= chart.ClientID %>'), '<%= string.Format("{0} ({1})", dto.TeamName, dto.ProgramName).Replace("'", @"\'") %>', 
                <%= dto.CurrentSize %>, 0, <%= dto.MaxRosterSize %>, 'all players', 'all players');
        </script>
    </ContentTemplate>
</asp:UpdatePanel>