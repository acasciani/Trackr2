<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeftNavigation.ascx.cs" Inherits="Trackr.Source.Controls.LeftNavigation" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('.sub.links:not(.lock) a.module').click(function () {
            // remove all previously dropped down ones
            $('.sub.links:not(.lock)').not($(this).parent().parent()).removeClass('display');
            var elementsToShow = $(this).parent().parent().toggleClass('display');
        });
    })
    
</script>

<ul class="utilities" runat="server" id="ulNoGroups" visible="false">
    <asp:Repeater runat="server" ID="rptNoGroup">
        <ItemTemplate>
            <li>
                <a href="<%#Eval("LinkURL") %>"><span class='<%#Eval("Glyphicon")%>' runat="server" visible='<%#Eval("Glyphicon") != null %>'></span><%#Eval("LinkURL") %></a>
            </li>
        </ItemTemplate>
    </asp:Repeater>
</ul>

<ul class="links">
    <li>
        <ul class="sub links <%= IsCurrentModule("Scheduler") ? "display lock" : "" %>">
            <li><a class="module">Scheduler</a></li>
            <li class="<%= IsCurrentModule("Scheduler/Default") ? "is-active" : "" %>"><a href="/Modules/Scheduler">View Calendar</a></li>
            <li class="<%= IsCurrentModule("Scheduler/ManageEvent") ? "is-active" : "" %>"><a href="/Modules/Scheduler/ManageEvent">Add New Event</a></li>
        </ul>
    </li>

    <li>
        <ul class="sub links <%= IsCurrentModule("PlayerManagement") ? "display lock" : "" %>">
            <li><a class="module">Player Management</a></li>
            <li class="<%= IsCurrentModule("PlayerManagement/Default") ? "is-active" : "" %>"><a href="/Modules/PlayerManagement">View Players</a></li>
            <li class="<%= IsCurrentModule("PlayerManagement/Manage") ? "is-active" : "" %>"><a href="/Modules/PlayerManagement/Manage">Add New Player</a></li>
        </ul>
    </li>

    <li>
        <ul class="sub links <%= IsCurrentModule("UserManagement") ? "display lock" : "" %>">
            <li><a class="module">User Management</a></li>
            <li class="<%= IsCurrentModule("UserManagement/Default") ? "is-active" : "" %>"><a href="/Modules/UserManagement">View Users</a></li>
            <li class="<%= IsCurrentModule("UserManagement/Manage") ? "is-active" : "" %>"><a href="/Modules/UserManagement/Manage">Add New User</a></li>
        </ul>
    </li>

</ul>
