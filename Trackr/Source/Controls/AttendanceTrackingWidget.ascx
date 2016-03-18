<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttendanceTrackingWidget.ascx.cs" Inherits="Trackr.Source.Controls.AttendanceTrackingWidget" %>

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
        <%=EventName %> (<%=TeamName %>) - <%= Starts.ToShortDateString() + " " + Starts.ToShortTimeString() %> to <%=Ends.ToShortDateString() + " " +Ends.ToShortTimeString() %>
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