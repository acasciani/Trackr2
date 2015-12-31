<%@ Page Title="View All Teams" Language="C#" MasterPageFile="~/Modules/TeamManagement/TeamManagement.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Trackr.Modules.TeamManagement.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="NestedContent" runat="server">

    <div class="panel panel-default">
        <div class="panel-heading">
            <h4>Player Management - View All Teams</h4>
        </div>
        <div class="panel-body">
            <a href="Manage.aspx" title="Create new team">Create New Team</a>
            <asp:GridView runat="server" ID="gvAllTeams" AutoGenerateColumns="false" CssClass="table table-striped" SelectMethod="gvAllTeams_GetData" AllowSorting="true">
                <Columns>
                    <asp:BoundField DataField="LastName" SortExpression="LastName" HeaderText="Last name" ItemStyle-CssClass="col-xs-3" />
                    <asp:BoundField DataField="FirstName" SortExpression="FirstName" HeaderText="First name" ItemStyle-CssClass="col-xs-3" />
                    <asp:BoundField DataField="BirthDate" SortExpression="BirthDate" HeaderText="Birth Date" DataFormatString="{0:MM/dd/yyyy}" />
                    <asp:TemplateField ItemStyle-CssClass="col-xs-1">
                        <ItemTemplate>
                            <a href="Manage.aspx?id=<%#Eval("TeamID") %>" class="glyphicon glyphicon-edit" title="Edit team"></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>
