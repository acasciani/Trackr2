<%@ Page Title="Spoof User" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Spoof.aspx.cs" Inherits="Trackr.Modules.UserManagement.Spoof" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="panel panel-default">
        <div class="panel-heading">
            <h4>User Management - Spoof User</h4>
        </div>
        <div class="panel-body">

            <div class="row">
                <div class="form-group col-sm-12">
                    <label for="<%=ddlUsers.ClientID %>">User</label>
                    <asp:DropDownList runat="server" ID="ddlUsers" CssClass="form-control" AppendDataBoundItems="false" SelectMethod="ddlUsers_GetData" DataTextField="Label" DataValueField="Value" />
                </div>

                <div class="form-group col-sm-12">
                    <asp:Button runat="server" ID="btnSpoof" CssClass="btn btn-default" Text="Spoof As User" OnClick="btnSpoof_Click" />
                </div>


            </div>
        </div>
    </div>

</asp:Content>
