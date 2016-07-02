<%@ Page Title="Inbox" Language="C#" MasterPageFile="~/Modules/Messenger/Messenger.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Trackr.Modules.Messenger.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="NestedContent" runat="server">





            <div class="row">
            <div class="col-sm-12">
                <!-- Split button -->
                <div class="btn-group">
                    <button type="button" class="btn btn-default">
                        <div class="checkbox" style="margin: 0;">
                            <label>
                                <input type="checkbox">
                            </label>
                        </div>
                    </button>
                    <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
                        <span class="caret"></span><span class="sr-only">Toggle Dropdown</span>
                    </button>
                    <ul class="dropdown-menu" role="menu">
                        <li><a href="#">All</a></li>
                        <li><a href="#">None</a></li>
                        <li><a href="#">Read</a></li>
                        <li><a href="#">Unread</a></li>
                        <li><a href="#">Starred</a></li>
                        <li><a href="#">Unstarred</a></li>
                    </ul>
                </div>
                <button type="button" class="btn btn-default" data-toggle="tooltip" title="Refresh">
                       <span class="glyphicon glyphicon-refresh"></span>   </button>
                <!-- Single button -->
                <div class="btn-group">
                    <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
                        More <span class="caret"></span>
                    </button>
                    <ul class="dropdown-menu" role="menu">
                        <li><a href="#">Mark all as read</a></li>
                        <li class="divider"></li>
                        <li class="text-center"><small class="text-muted">Select messages to see more actions</small></li>
                    </ul>
                </div>
                <div class="pull-right" runat="server" id="divResultsPP" visible="false">
                    <span class="text-muted"><b><asp:Literal runat="server" ID="litResultsPPMin" /></b>–<b><asp:Literal runat="server" ID="litResultsPPMax" /></b> of <b><asp:Literal runat="server" ID="litResultsPPTotal" /></b></span>
                    <div class="btn-group btn-group-sm">
                        <button type="button" class="btn btn-default">
                            <span class="glyphicon glyphicon-chevron-left"></span>
                        </button>
                        <button type="button" class="btn btn-default">
                            <span class="glyphicon glyphicon-chevron-right"></span>
                        </button>
                    </div>
                </div>
            </div>
            </div>

            <hr />

            <div class="row">
                <div class="col-sm-12">



            <!-- Nav tabs -->
            <ul class="nav nav-tabs">
                <li class="active"><a href="#home" data-toggle="tab"><span class="glyphicon glyphicon-inbox">
                </span>Primary</a></li>
                <li><a href="#profile" data-toggle="tab"><span class="glyphicon glyphicon-user"></span>
                    Social</a></li>
                <li><a href="#messages" data-toggle="tab"><span class="glyphicon glyphicon-tags"></span>
                    Promotions</a></li>
                <li><a href="#settings" data-toggle="tab"><span class="glyphicon glyphicon-plus no-margin">
                </span></a></li>
            </ul>
            <!-- Tab panes -->
            <div class="tab-content">
                <div class="tab-pane fade in active" id="home">
                    <div class="list-group">
                            <asp:Repeater runat="server" ID="rptEmail">

                                <ItemTemplate>
                                    <a href="#" class="list-group-item">
                                    <div class="checkbox">
                                        <label>
                                            <input type="checkbox">
                                        </label>
                                    </div>
                                    <span class="glyphicon <%# (bool)Eval("IsStarred") ? "glyphicon-star" : "glyphicon-star-empty" %>"></span>
                                        <span class="name" style="min-width: 120px; display: inline-block;">
                                            <%#Eval("Sender") %>
                                        </span>
                                    <span class="text-muted" style="font-size: 11px;"><%#Eval("Message") %></span> <span
                                        class="badge"><%#Eval("DateSent") %></span> <span class="pull-right"><span class="glyphicon glyphicon-paperclip">
                                        </span></span></a>
                                </ItemTemplate>


                            </asp:Repeater>

                        <div runat="server" id="divNoMessages" visible="false">
                            <strong>You have no messages to display.</strong>
                            </div>


                    </div>
                </div>
                <div class="tab-pane fade in" id="profile">
                    <div class="list-group">
                        <div class="list-group-item">
                            <span class="text-center">This tab is empty.</span>
                        </div>
                    </div>
                </div>
                <div class="tab-pane fade in" id="messages">
                    ...</div>
                <div class="tab-pane fade in" id="settings">
                    This tab is empty.</div>
            </div>
            <!-- Ad -->
            <div class="row-md-12">
                <div class="ad">
                    <a href="http://www.jquery2dotnet.com" class="title">jQuery2DotNet</a>
                    <div>
                        Cool jQuery, CSS3, HTML5, Bootstrap, and MVC tutorial</div>
                    <a href="http://www.jquery2dotnet.com" class="url">www.jquery2dotnet.com</a>
                </div>
            </div>
                </div>
            </div>






</asp:Content>
