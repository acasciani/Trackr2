<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AlertBox.ascx.cs" Inherits="Trackr.UI.AlertBox" %>

<%@ Reference Control="~/Source/Controls/Alert.ascx" %>

<asp:Panel ID="AlertContainer" runat="server" CssClass="alert-box" />

<asp:Panel role="alert" runat="server" id="pnlAlertContainer" Visible="false">
  <asp:Literal runat="server" ID="litText"></asp:Literal>
</asp:Panel>