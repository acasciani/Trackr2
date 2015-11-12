<%@ Page Title="Manage Player" Language="C#" MasterPageFile="~/Modules/PlayerManagement/PlayerManagement.master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="Trackr.Modules.PlayerManagement.Manage" %>

<%@ Register Src="~/Source/Wizards/PlayerManagement.ascx" TagName="PlayerManagement" TagPrefix="ui" %>

<asp:Content ID="Content1" ContentPlaceHolderID="NestedContent" runat="server">

    <div class="panel panel-default">
        <div class="panel-heading">
            <h4>Player Management - <%= PlayerManagement.WasNew ? "Create Player" : "Edit Player" %></h4>
        </div>
        <div class="panel-body">
            <ui:PlayerManagement runat="server" ID="PlayerManagement" />
        </div>
    </div>

</asp:Content>