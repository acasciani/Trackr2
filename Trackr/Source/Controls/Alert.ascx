<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Alert.ascx.cs" Inherits="Trackr.Controls.Alert" %>

<asp:Panel CssClass="alert alert-dismissible" role="alert" runat="server" id="AlertHolder">
    <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
    <div runat="server" ID="Content"></div>
</asp:Panel>