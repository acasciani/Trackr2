<%@ Page Title="View All Users" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Trackr.Modules.UserManagement.Default" %>

<%@ Register Src="~/Source/Wizards/UserManagement.ascx" TagName="UserManagement" TagPrefix="ui" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="panel panel-default">
        <div class="panel-heading">
            <h4>User Management - View All Users</h4>
        </div>
        <div class="panel-body">
            <a href="Manage.aspx" title="Create new user">Create New User</a>
            <asp:GridView runat="server" ID="gvAllUsers" AutoGenerateColumns="false" CssClass="table table-striped" SelectMethod="gvAllUsers_GetData" AllowSorting="true">
                <Columns>
                    <asp:BoundField DataField="LastName" SortExpression="LastName" HeaderText="Last name" ItemStyle-CssClass="col-xs-3" />
                    <asp:BoundField DataField="FirstName" SortExpression="FirstName" HeaderText="First name" ItemStyle-CssClass="col-xs-3" />
                    <asp:BoundField DataField="Email" SortExpression="Email" HeaderText="Email" />
                    <asp:TemplateField ItemStyle-CssClass="col-xs-1">
                        <ItemTemplate>
                            <a href="Manage.aspx?id=<%#Eval("UserID") %>" class="glyphicon glyphicon-edit" title="Edit user"></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>
