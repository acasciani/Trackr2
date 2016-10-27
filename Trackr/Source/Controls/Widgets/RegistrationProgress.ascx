<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RegistrationProgress.ascx.cs" Inherits="Trackr.Source.Controls.Widgets.RegistrationProgress" %>

<asp:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel">
    <ProgressTemplate>
        <progress:Bounce runat="server" />
    </ProgressTemplate>
</asp:UpdateProgress>


<asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="lnkHideTeam" />
        <asp:AsyncPostBackTrigger ControlID="lnkUnHideTeam" />
    </Triggers>
    <ContentTemplate>
        <div style="width: 300px;" runat="server" id="divHolder">
            <chart:Gauge runat="server" ID="chartGauge" />

            <div class="clearfix"></div>
            <div class="text-center" style="position: relative;">
                <asp:LinkButton runat="server" ID="lnkShowChange" OnClick="lnkShowChange_Click" CommandArgument="ShowApproved">
                    Show Approved Players
                </asp:LinkButton>
                <span style="position: absolute; right: 20px;">
                    <asp:LinkButton runat="server" ID="lnkHideTeam" OnClick="lnkHideTeam_Click" ToolTip="Hide Team" Visible="false">
                        <span class="glyphicon glyphicon-eye-close"></span>
                    </asp:LinkButton>
                    <asp:LinkButton runat="server" ID="lnkUnHideTeam" OnClick="lnkUnHideTeam_Click" ToolTip="Unhide Team" Visible="false">
                        <span class="glyphicon glyphicon-eye-open"></span>
                    </asp:LinkButton>
                </span>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
