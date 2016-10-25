<%@ Page Title="View All Teams" Language="C#" MasterPageFile="~/Modules/TeamManagement/TeamManagement.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Trackr.Modules.TeamManagement.Default" %>

<asp:Content ID="Content2" ContentPlaceHolderID="NestedContent" runat="server">



            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4>Team Management - View All Players</h4>
                </div>
                <div class="panel-body">
                    <ui:ProgramTeamPlayerPicker runat="server" ID="ptpPicker" Permission="TeamManagement.ViewTeams" />

                    <div class="row">
                        <div class="col-sm-12">
                            <asp:LinkButton runat="server" ID="lnkFilter" CssClass="btn btn-default" OnClick="lnkFilter_Click">Filter</asp:LinkButton>
                        </div>
                    </div>
                    <hr />
                    <div class="row">
                        <div class="col-sm-12 table-responsive">
                            <ui:TrackrGridView runat="server" ID="gvAllTeams" AutoGenerateColumns="false" CssClass="table table-striped" AllowSorting="true" AllowPaging="true" DisplayResultsPerPageOptions="true" DisplayPagingSummary="true" IsPinnable="true">
                                <Columns>
                                    <ui:TrackrBoundField DataField="ProgramName" SortExpression="ProgramName" HeaderText="Program Name" ItemStyle-CssClass="col-xs-3" AllowPinnable="true" IsPinnable="true" />
                                    <ui:TrackrBoundField DataField="TeamName" SortExpression="TeamName" HeaderText="Team name" AllowPinnable="true" IsPinnable="true" />
                                    <ui:TrackrBoundField DataField="Start" SortExpression="Start" HeaderText="Starts" DataFormatString="{0:MM/dd/yyyy}" ItemStyle-CssClass="col-xs-2" AllowPinnable="true" IsPinnable="true" />
                                    <ui:TrackrBoundField DataField="End" SortExpression="End" HeaderText="Ends" DataFormatString="{0:MM/dd/yyyy}" ItemStyle-CssClass="col-xs-2" AllowPinnable="true" IsPinnable="true" />
                                    <asp:TemplateField ItemStyle-CssClass="col-xs-1">
                                        <ItemTemplate>
                                            <a href="Manage.aspx?id=<%#Eval("TeamID") %>" class="glyphicon glyphicon-edit" title="Edit team"></a>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </ui:TrackrGridView>
                        </div>
                    </div>


                    <a href="Manage.aspx" title="Create new team">Create New Team</a>
                </div>
            </div>

    <div class="panel panel-default">
        <div class="panel-heading">
            <h4>Team Management - Registration Progress (<asp:LinkButton EnableViewState="true" runat="server" ID="lnkShowHideRegProg" OnClick="lnkShowHideRegProg_Click" CommandArgument="true" Text="Show Hidden Teams" />)</h4>
        </div>
        <div class="panel-body">

            <asp:Repeater runat="server" ID="rptWidgets">
                <ItemTemplate>
                    <div style="float: left">

                        <widget:RegistrationProgress runat="server" ID="widgetProgress" TeamID='<%# (int)Container.DataItem %>' Permission="TeamManagement.ViewTeams" />

                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <div class="clearfix"></div>
        </div>
    </div>
</asp:Content>
