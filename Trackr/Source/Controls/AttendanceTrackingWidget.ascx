<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttendanceTrackingWidget.ascx.cs" Inherits="Trackr.Source.Controls.AttendanceTrackingWidget" %>


<div class="panel panel-default">
    <div class="panel-heading">
        <%=EventName %> (<%=TeamName %>) - <%= Starts.ToShortDateString() + " " + Starts.ToShortTimeString() %> to <%=Ends.ToShortDateString() + " " +Ends.ToShortTimeString() %>
    </div>
    <div class="panel-body">
        <asp:Repeater runat="server" ID="rptPlayer">
            <ItemTemplate>
                <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                            <div class="attendance-ticker" data-player-id="<%#Eval("Player.PlayerID") %>" data-team-schedule-id="<%=TeamScheduleID.ToString() %>">
                                <%# Eval("Player.Person.FName") %> <%#Eval("Player.Person.LName")%> <span class="glyphicon glyphicon-ok" style="float:right;"></span>
                            </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>