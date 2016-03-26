<%@ Page Title="View All Players" Language="C#" MasterPageFile="~/Modules/PlayerManagement/PlayerManagement.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Trackr.Modules.PlayerManagement.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="NestedContent" runat="server">

    <div class="panel panel-default">
        <div class="panel-heading">
            <h4>Player Management - View All Players</h4>
        </div>
        <div class="panel-body">
            <ui:ProgramTeamPlayerPicker runat="server" ID="ptpPicker" Permission="PlayerManagement.ViewPlayers" EnableTeamSelect="true" />
            
            <div class="row">
                <div class="col-sm-12">
                    <asp:LinkButton runat="server" ID="lnkFilter" CssClass="btn btn-default" OnClick="lnkFilter_Click">Filter</asp:LinkButton>
                </div>
            </div>
            <hr />
            <div class="row">
                <div class="col-sm-12 table-responsive">
                    <asp:GridView runat="server" ID="gvAllPlayers" AutoGenerateColumns="false" CssClass="table table-striped" SelectMethod="gvAllPlayers_GetData" AllowSorting="true">
                        <Columns>
                            <asp:BoundField DataField="LastName" SortExpression="LastName" HeaderText="Last name" ItemStyle-CssClass="col-xs-3" />
                            <asp:BoundField DataField="FirstName" SortExpression="FirstName" HeaderText="First name" ItemStyle-CssClass="col-xs-3" />
                            <asp:BoundField DataField="BirthDate" SortExpression="BirthDate" HeaderText="Birth Date" DataFormatString="{0:MM/dd/yyyy}" />
                            <asp:TemplateField ItemStyle-CssClass="col-xs-1">
                                <ItemTemplate>
                                    <a href="Manage.aspx?id=<%#Eval("PlayerID") %>" class="glyphicon glyphicon-edit" title="Edit player"></a>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            

            <a href="Manage.aspx" title="Create new player">Create New Player</a>
        </div>
    </div>


</asp:Content>
