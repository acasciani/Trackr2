<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Gauge.ascx.cs" Inherits="Trackr.Controls.Charts.Gauge" %>

<asp:ScriptManagerProxy runat="server">
    <Scripts>
        <asp:ScriptReference Path="//code.highcharts.com/highcharts.js" />
        <asp:ScriptReference Path="//code.highcharts.com/highcharts-more.js" />
        <asp:ScriptReference Path="//code.highcharts.com/modules/solid-gauge.js" />
        <asp:ScriptReference Path="~/Scripts/charting/RegistrationWidget_Gauge.js" />
    </Scripts>
</asp:ScriptManagerProxy>

<div runat="server" id="chart" style="width:300px; height:200px; float:left;"></div>
