<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttendanceTrackingWidget.ascx.cs" Inherits="Trackr.Source.Controls.AttendanceTrackingWidget" %>

<script runat="server">
    public string GetDateOfEvent(DateTime startDate, DateTime endDate)
    {
        if (startDate.Date == endDate.Date)
        {
            return string.Format("{0} from {1} to {2}", GetFriendlyDateName(startDate), startDate.ToString("h:mm tt"), endDate.ToString("h:mm tt"));
        }
        else
        {
            return string.Format("{0} {1} to {2} {3}", GetFriendlyDateName(startDate), startDate.ToString("h:mm tt"), GetFriendlyDateName(endDate), endDate.ToString("h:mm tt"));
        }
    }

    public string GetFriendlyDateName(DateTime date)
    {
        if (date.Date == DateTime.Today)
        {
            return "Today";
        }
        else if (date.Date == DateTime.Today.AddDays(1))
        {
            return "Tomorrow";
        }
        else if (date.Date == DateTime.Today.AddDays(-1))
        {
            return "Yesterday";
        }
        else
        {
            return date.Date.ToString("MMM d");
        }
    }
</script>

<style type="text/css">
    .clickable-player {
        -webkit-transition: background-color 2s ease-out;
        -moz-transition: background-color 2s ease-out;
        -o-transition: background-color 2s ease-out;
        transition: background-color 2s ease-out;
        margin-left: -15px;
        margin-right: -15px;
        padding: 10px 15px;
    }

    .clickable-player.success {
        background-color: #CCFFCC;
    }
</style>

<div class="panel panel-default">
    <div class="panel-heading">
        <div class="pull-left">
            <strong><%=EventName %> (<%=TeamName %>)</strong> - <%=GetDateOfEvent(Starts.ToLocalTime(), Ends.ToLocalTime()) %>
        </div>
        <div class="pull-right">
            <asp:HyperLink runat="server" ID="lnkEdit" NavigateUrl="/Modules/Scheduler/ManageEvent?id={0}" ToolTip="Edit Event">
                <span class="glyphicon glyphicon-edit"></span>
            </asp:HyperLink>

            <asp:LinkButton runat="server" ID="lnkDelete" OnClientClick="return confirm('Please confirm you would like to delete this event.');" ToolTip="Delete Event" OnClick="lnkDelete_Click">
                <span class="glyphicon glyphicon-trash"></span>
            </asp:LinkButton>
        </div>
        <div class="clearfix"></div>
    </div>
    <div class="panel-body">
        <asp:Repeater runat="server" ID="rptPlayer">
            <ItemTemplate>
                <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <ui:ClickablePanel runat="server" ID="btnPlayer" OnClick="btnPlayer_Click" CssClass='<%#(bool)Eval("Present") ? "clickable-player success" : "clickable-player" %>' PlayerID='<%#Eval("PlayerID") %>' IsDisabled='<%#(bool)Eval("Present")%>'>
                                <%# Eval("FirstName") %> <%#Eval("LastName")%> <span class="glyphicon glyphicon-ok" style="float:right;"></span>
                        </ui:ClickablePanel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>