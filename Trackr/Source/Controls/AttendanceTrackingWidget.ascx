<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttendanceTrackingWidget.ascx.cs" Inherits="Trackr.Source.Controls.AttendanceTrackingWidget" %>

<style type="text/css">
    .clickable-player {
      -webkit-transition: background-color 2s ease-out;
      -moz-transition: background-color 2s ease-out;
      -o-transition: background-color 2s ease-out;
      transition: background-color 2s ease-out;
    }

    .clickable-player.success {
        background-color: green;
    }
</style>

<div class="panel panel-default">
    <div class="panel-heading">
        <%=EventName %> (<%=TeamName %>) - <%= Starts.ToShortDateString() + " " + Starts.ToShortTimeString() %> to <%=Ends.ToShortDateString() + " " +Ends.ToShortTimeString() %>
    </div>
    <div class="panel-body">
        <asp:Repeater runat="server" ID="rptPlayer">
            <ItemTemplate>
                <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <ui:ClickablePanel runat="server" ID="btnPlayer" OnClick="btnPlayer_Click" CssClass="clickable-player" PlayerID='<%#Eval("Player.PlayerID") %>'>
                                <%# Eval("Player.Person.FName") %> <%#Eval("Player.Person.LName")%> <span class="glyphicon glyphicon-ok" style="float:right;"></span>
                        </ui:ClickablePanel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>