<%@ Page Title="Update Event" Language="C#" MasterPageFile="~/Modules/Scheduler/Scheduler.master" AutoEventWireup="true" CodeBehind="ManageEvent.aspx.cs" Inherits="Trackr.Modules.Scheduler.ManageEvent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="NestedContent" runat="server">
    <ui:AlertBox runat="server" ID="AlertBox_General" />

    <div class="panel panel-default" runat="server" id="divManage">
        <div class="panel-heading">
            <h4>Scheduler - <%=Title %></h4>
        </div>
        <div class="panel-body">
            <ui:AlertBox runat="server" ID="AlertBox_Manage" />               
            
            <div class="row form-group">
                <label for="<%=txtEventName.ClientID %>" class="col-sm-12 control-label">Event Name</label>
                <div class="col-sm-12">
                    <asp:TextBox runat="server" ID="txtEventName" CssClass="form-control" MaxLength="50" />
                    <asp:RequiredFieldValidator runat="server" ID="validatorEventNameRequired" Display="Dynamic" CssClass="text-danger" ControlToValidate="txtEventName" Text="An event name is required." />
                </div>
            </div>

            <ui:ProgramTeamPlayerPicker runat="server" ID="ptpPicker" RequiredSelectOf="Team" Permission="Scheduler.AddNewEvent" EnableTeamSelect="true" />
            
            <div class="row form-group">
                <label for="<%=txtEventName.ClientID %>" class="col-sm-12 control-label">Event Start</label>
                <div class="col-sm-12">
                    <asp:TextBox runat="server" ID="txtStartDate" CssClass="form-control" MaxLength="30" TextMode="DateTimeLocal" />
                    <asp:RequiredFieldValidator runat="server" ID="validatorEventStartRequired" Display="Dynamic" CssClass="text-danger" ControlToValidate="txtStartDate" Text="A start date is required." />
                    <asp:CustomValidator Display="Dynamic" runat="server" ID="validatorStartDateParses" ControlToValidate="txtStartDate" CssClass="text-danger" ErrorMessage="The start date must be entered in the format: MM/DD/YYYY and must be greater than or equal to January 1, 1900 and less than or equal to January 1, 2200." OnServerValidate="validatorStartDateParses_ServerValidate" />
                </div>
            </div>

            <div class="row form-group">
                <label for="<%=txtEventName.ClientID %>" class="col-sm-12 control-label">Event End</label>
                <div class="col-sm-12">
                    <asp:TextBox runat="server" ID="txtEndDate" CssClass="form-control" MaxLength="30" TextMode="DateTimeLocal" />
                    <asp:RequiredFieldValidator runat="server" ID="validatorEventEndRequired" Display="Dynamic" CssClass="text-danger" ControlToValidate="txtEndDate" Text="An end date is required." />
                    <asp:CustomValidator Display="Dynamic" runat="server" ID="validatorEndDateParses" ControlToValidate="txtEndDate" CssClass="text-danger" ErrorMessage="The end date must be entered in the format: MM/DD/YYYY and must be greater than or equal to January 1, 1900 and less than or equal to January 1, 2200." OnServerValidate="validatorStartDateParses_ServerValidate" />
                    <asp:CustomValidator Display="Dynamic" runat="server" ID="validatorStartEndCompare" ControlToValidate="txtEndDate" CssClass="text-danger" ErrorMessage="The end date must be after the entered start date." OnServerValidate="validatorStartEndCompare_ServerValidate" />
                </div>
            </div>

            <div class="checkbox">
                <label>
                    <asp:CheckBox runat="server" ID="chkEnableRSVP" AutoPostBack="true" OnCheckedChanged="chkEnableRSVP_CheckedChanged" />
                    <strong>Enable RSVP</strong>
                </label>
            </div>

            <div class="row form-group" runat="server" id="divRSVPBy" visible="false">
                <label for="<%=txtEventName.ClientID %>" class="col-sm-12 control-label">RSVP By</label>
                <div class="col-sm-12">
                    <asp:TextBox runat="server" ID="txtRSVPBy" CssClass="form-control" MaxLength="30" TextMode="DateTimeLocal" />
                    <asp:CustomValidator Display="Dynamic" runat="server" ID="validatorRSVPParses" ControlToValidate="txtRSVPBy" CssClass="text-danger" ErrorMessage="The RSVP by date must be entered in the format: MM/DD/YYYY and must be greater than or equal to January 1, 1900 and less than or equal to January 1, 2200." OnServerValidate="validatorStartDateParses_ServerValidate" />
                </div>
            </div>



            <div class="row">
                <div class="col-sm-12">
                    <asp:LinkButton runat="server" ID="lnkCreateEvent" OnClick="lnkCreateEvent_Click" Visible="false">
                        <span class="glyphicon glyphicon-save"></span> Create New Event
                    </asp:LinkButton>

                    <asp:LinkButton runat="server" ID="lnkUpdateEvent" OnClick="lnkUpdateEvent_Click" Visible="false">
                        <span class="glyphicon glyphicon-save"></span> Save Event Changes
                    </asp:LinkButton>
                </div>
            </div>
                        
        </div>
    </div>

</asp:Content>
