<%@ Page Title="Manage User" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="Trackr.Modules.UserManagement.Manage" %>

<%@ Register Src="~/Source/Wizards/UserManagement.ascx" TagName="UserManagement" TagPrefix="ui" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="panel panel-default">
        <div class="panel-heading">
            <h4>User Management - <%= UserManagement.IsNew ? "Create User" : "Edit User" %></h4>
        </div>
        <div class="panel-body">
            <ui:UserManagement runat="server" ID="UserManagement" />
        </div>
    </div>

</asp:Content>
