<%@ Page Title="Spoof User" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Spoof.aspx.cs" Inherits="Trackr.Modules.UserManagement.Spoof" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="panel panel-default">
        <div class="panel-heading">
            <h4>User Management - Spoof User</h4>
        </div>
        <div class="panel-body">
            <asp:DropDownList runat="server" ID="ddlUsers" CssClass="form-control" AppendDataBoundItems="false" SelectMethod="ddlUsers_GetData" DataTextField="Label" DataValueField="Value" />
        </div>
    </div>

</asp:Content>
