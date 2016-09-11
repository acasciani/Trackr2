<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramTeamPlayerPicker.ascx.cs" Inherits="Trackr.Source.Controls.ProgramTeamPlayerPicker" %>


<div class="row">
    <div class="<%= divPlayer.Visible ? "form-group col-sm-6 col-md-4" : divTeam.Visible ? "form-group col-sm-6" : "form-group col-sm-12" %>">
        <label for="<%=ddlProgram.ClientID %>">Program</label>
        <asp:DropDownList runat="server" ID="ddlProgram" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlProgram_SelectedIndexChanged" AppendDataBoundItems="true">
            <asp:ListItem Text="-- Select Program --" />
        </asp:DropDownList>
        <asp:CustomValidator runat="server" ID="validatorProgram" ControlToValidate="ddlProgram" CssClass="text-danger" Display="Dynamic" ErrorMessage="A program must be selected." OnServerValidate="validateIntIsParsable" />
    </div>

        <div class='<%= divPlayer.Visible ? "form-group col-sm-6 col-md-4" : "form-group col-sm-6" %>' runat="server" id="divTeam">
            <label for="<%=ddlTeam.ClientID %>">Team</label>
            <asp:DropDownList runat="server" ID="ddlTeam" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTeam_SelectedIndexChanged" AppendDataBoundItems="true">
                <asp:ListItem Text="-- Select Team --" />
            </asp:DropDownList>
            <asp:CustomValidator runat="server" ID="validatorTeam" ControlToValidate="ddlTeam" CssClass="text-danger" Display="Dynamic" ErrorMessage="A team must be selected." OnServerValidate="validateIntIsParsable" />
        </div>

    <div class="form-group col-sm-12 col-md-4" runat="server" id="divPlayer">
        <label for="<%=ddlPlayer.ClientID %>">Player</label>
        <asp:DropDownList runat="server" ID="ddlPlayer" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlPlayer_SelectedIndexChanged" AppendDataBoundItems="true">
            <asp:ListItem Text="-- Select Player --" />
        </asp:DropDownList>
        <asp:CustomValidator runat="server" ID="validatorPlayer" ControlToValidate="ddlPlayer" CssClass="text-danger" Display="Dynamic" ErrorMessage="A player must be selected." OnServerValidate="validateIntIsParsable" />
    </div>
</div>