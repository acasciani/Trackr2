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

<ul class="utilities">
    <li>
        <a href="#"><span class="glyphicon glyphicon-envelope"></span>Messages</a>
    </li>
    <li>
        <a href="#"><span class="glyphicon glyphicon-user"></span>My Account</a>
    </li>
</ul>

<ul class="links">
    <li>
        <ul class="sub links <%= IsCurrentModule("PlayerManagement") ? "display lock" : "" %>">
            <li class="is-active"><a class="module">Player Management</a></li>
            <li><a href="/Modules/PlayerManagement">View Players</a></li>
            <li><a href="/Modules/PlayerManagement/Manage">Add New Player</a></li>
        </ul>
    </li>

    <li>
        <ul class="sub links">
            <li><a class="module">User Management</a></li>
            <li><a href="#">Chapter 32</a></li>
            <li><a href="#">Chapter 33</a></li>
            <li><a href="#">Chapter 34</a></li>
            <li><a href="#">Chapter 35</a></li>
        </ul>
    </li>

    <li>
        <ul class="sub links">
            <li class=""><a class="module">Chapter 4</a></li>
            <li><a href="#">Chapter 32</a></li>
            <li><a href="#">Chapter 33</a></li>
            <li><a href="#">Chapter 34</a></li>
            <li><a href="#">Chapter 35</a></li>
        </ul>
    </li>

</ul>
