<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeftNavigation.ascx.cs" Inherits="Trackr.Source.Controls.LeftNavigation" %>

<!-- renders the Left Navigation -->
<asp:PlaceHolder runat="server" ID="navLeftSide" Visible="false">
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
                    <a href="<%#Eval("LinkURL") %>" title="<%#Eval("Tooltip")%>"><span class='<%#Eval("Glyphicon")%>' runat="server" visible='<%#Eval("Glyphicon") != null %>'></span><%#Eval("Name") %></a>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>

    <ul class="links" runat="server" id="ulGroups" visible="false">
        <asp:Repeater runat="server" ID="rptGroup">
            <ItemTemplate>
                <li>
                    <ul class="sub links <%# IsCurrentModule(Eval("CurrentModulePattern") as string) ? "display lock" : "" %>">
                        <li>
                            <a class="module" title="<%#Eval("Tooltip")%>">
                                <span class='<%#Eval("Glyphicon")%>' runat="server" visible='<%#Eval("Glyphicon") != null %>'></span><%#Eval("Name") %>
                            </a>
                        </li>

                        <asp:Repeater runat="server" ID="rptLink" DataSource='<%#Eval("MyLinks") %>'>
                            <ItemTemplate>
                                <li class="<%# IsCurrentModule(Eval("CurrentModulePattern") as string) ? "is-active" : "" %>">
                                    <a href="<%#Eval("LinkURL")%>" title="<%#Eval("Tooltip")%>">
                                        <span class='<%#Eval("Glyphicon")%>' runat="server" visible='<%#Eval("Glyphicon") != null %>'></span><%#Eval("Name") %>
                                    </a>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>


                    </ul>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
</asp:PlaceHolder>



<!-- renders top nav -->
<asp:PlaceHolder runat="server" ID="navTopPart" Visible="false">
    <ul class="nav navbar-nav navbar-right hidden-sm hidden-md hidden-lg" runat="server" id="ulMobileNav" visible="false">

        <asp:Repeater runat="server" ID="rptNavMobile_NoGroups">
            <ItemTemplate>
                <li>
                    <a href="<%#Eval("LinkURL") %>" title="<%#Eval("Tooltip")%>"><span class='<%#Eval("Glyphicon")%>' runat="server" visible='<%#Eval("Glyphicon") != null %>'></span><%#Eval("Name") %></a>
                </li>
            </ItemTemplate>
        </asp:Repeater>

        <asp:Repeater runat="server" ID="rptNavMobile_Groups">
            <ItemTemplate>
                <li>
                    <ul class="sub links <%# IsCurrentModule(Eval("CurrentModulePattern") as string) ? "display lock" : "" %>">
                        <li>
                            <a class="module" title="<%#Eval("Tooltip")%>">
                                <span class='<%#Eval("Glyphicon")%>' runat="server" visible='<%#Eval("Glyphicon") != null %>'></span><%#Eval("Name") %>
                            </a>
                        </li>

                        <asp:Repeater runat="server" ID="rptLink" DataSource='<%#Eval("MyLinks") %>'>
                            <ItemTemplate>
                                <li class="<%# IsCurrentModule(Eval("CurrentModulePattern") as string) ? "is-active" : "" %>">
                                    <a href="<%#Eval("LinkURL")%>" title="<%#Eval("Tooltip")%>">
                                        <span class='<%#Eval("Glyphicon")%>' runat="server" visible='<%#Eval("Glyphicon") != null %>'></span><%#Eval("Name") %>
                                    </a>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>


                    </ul>
                </li>
            </ItemTemplate>

            <ItemTemplate>

                <li class="dropdown<%# IsCurrentModule(Eval("CurrentModulePattern") as string) ? " active" : "" %>">
                    <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">
                        <span class='<%#Eval("Glyphicon")%>' runat="server" visible='<%#Eval("Glyphicon") != null %>'></span><%#Eval("Name") %>
                    </a>
                    <ul class="dropdown-menu">
                        <asp:Repeater runat="server" ID="rptLink" DataSource='<%#Eval("MyLinks") %>'>
                            <ItemTemplate>
                                <li class="<%# IsCurrentModule(Eval("CurrentModulePattern") as string) ? "active" : "" %>">
                                    <a href="<%#Eval("LinkURL")%>" title="<%#Eval("Tooltip")%>">
                                        <span class='<%#Eval("Glyphicon")%>' runat="server" visible='<%#Eval("Glyphicon") != null %>'></span><%#Eval("Name") %>
                                    </a>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </li>
            </ItemTemplate>
        </asp:Repeater>

    </ul>
</asp:PlaceHolder>