<%@ Page Title="Manage Team" Language="C#" MasterPageFile="~/Modules/TeamManagement/TeamManagement.master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="Trackr.Modules.TeamManagement.Manage" %>

<%@ Register Src="~/Source/Wizards/TeamManagement.ascx" TagName="TeamManagement" TagPrefix="ui" %>

<asp:Content ID="Content1" ContentPlaceHolderID="NestedContent" runat="server">

    <div class="panel panel-default">
        <div class="panel-heading">
            <h4>Team Management - <%= TeamManagement.WasNew ? "Create Team" : "Edit Team" %></h4>
        </div>
        <div class="panel-body">
            <ui:TeamManagement runat="server" ID="TeamManagement" EditPermission="TeamManagement.EditTeam" />
        </div>
    </div>

</asp:Content>