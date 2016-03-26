<%@ Page Title="My Scheduler" Language="C#" MasterPageFile="~/Modules/Scheduler/Scheduler.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Trackr.Modules.Scheduler.Default" %>

<asp:Content ID="Content2" ContentPlaceHolderID="NestedContent" runat="server">
    <link rel="stylesheet" href="/Content/Calendar.css" />

    <style type="text/css">
        @media screen and (max-width: 991px) {
            .cal1 .event-listing {
                width:100% !important;
                clear: both;
                margin-top: 25px;
            }

            .cal1 .clndr-table {
                width: 100%;
                clear: both;
            }
        }
    </style>

    <asp:UpdatePanel runat="server" ID="updatePanelAlert" UpdateMode="Always">
        <ContentTemplate>
            <ui:AlertBox runat="server" ID="AlertBox" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="row">
        <div class="col-sm-12">
            <a href="/Modules/Scheduler/ManageEvent">Add New Event</a>
        </div>
    </div>

    <hr />

    <div class="row">
        <div class="col-sm-12">


    <asp:UpdatePanel runat="server" UpdateMode="Conditional">
        <ContentTemplate>
    <script type="text/javascript">
        // Switch to Jinja2/Twig/Nunjucks-style delimiters
        _.templateSettings = {
            escape: /\{\{\-(.+?)\}\}/g,
            evaluate: /\{\%(.+?)\%\}/g,
            interpolate: /\{\{(.+?)\}\}/g,
        };


        // Call this from the developer console and you can control both instances
        var calendars = {};

        $(document).ready(function () {
            function GetEvents(StartDate, EndDate, cb) {
                $.post('/api/TeamSchedules/GetForCurrentUser', { Start: StartDate, End: EndDate, OnlyActive: true })
                    .done(function (data) {
                        console.log('received schedules');
                        cb(data);
                    })
                    .fail(function (error) {
                        console.log('failed to get team schedules');
                    });
            }
            
            function AddClickEvent() {
                $('.event-item').click(function (v) {
                    console.log('clicked');
                    var teamScheduleID = v.currentTarget.dataset.teamScheduleId.toString();
                    var postback = "<%= Page.ClientScript.GetPostBackEventReference(updatePanel, "args") %>";
                    eval(postback.replace('args', v.currentTarget.dataset.teamScheduleId.toString()));
                });
            }

            function Init(events) {
                // Here's some magic to make sure the dates are happening this month.
                var thisMonth = moment().format('YYYY-MM');
                // Events to load into calendar
                var eventArray = [];

                for (var i = 0; i < events.length; i++) {
                    eventArray.push({
                        title: events[i].EventName,
                        endDate: new Date(events[i].EndDate),
                        startDate: new Date(events[i].StartDate),
                        teamScheduleID: events[i].TeamScheduleID,
                        teamName: events[i].TeamName
                    });
                }

                // The order of the click handlers is predictable. Direct click action
                // callbacks come first: click, nextMonth, previousMonth, nextYear,
                // previousYear, nextInterval, previousInterval, or today. Then
                // onMonthChange (if the month changed), inIntervalChange if the interval
                // has changed, and finally onYearChange (if the year changed).
                calendars.clndr1 = $('.cal1').clndr({
                    events: eventArray,
                    clickEvents: {
                        click: function (target) {
                            $('.event-listing-title').html('Events on ' + target.date._i);
                            $('.event-item').remove(); // Remove all previously added

                            for (var i = 0; i < target.events.length; i++) {
                                var element = '<div class="event-item" data-team-schedule-id="' + target.events[i].teamScheduleID + '"><div class="event-item-name"><strong>' + target.events[i].title + '</strong><br><em>' + target.events[i].teamName + '</em><br><em>' + $.format.date(target.events[i].startDate, 'MMM d h:mm a') + '</em></div></div>';
                                $('.event-listing').append(element);
                            }

                            AddClickEvent();
                        },
                        today: function () {
                            //console.log('Cal-1 today');
                        },
                        nextMonth: function () {
                            //console.log('Cal-1 next month');
                        },
                        previousMonth: function () {
                            //console.log('Cal-1 previous month');
                        },
                        onMonthChange: function (t) {
                            console.log('Cal-1 month changed');

                            var firstOfMonth = new Date(t.year(), t.month(), 1, 0, 0, 0, 0);
                            var lastOfMonth = new Date(t.year(), t.month() + 1, 0, 23, 59, 59, 0);

                            GetEvents(firstOfMonth.toUTCString(), lastOfMonth.toUTCString(), function (events) {
                                var monthChangeEvents = [];
                                for (var i = 0; i < events.length; i++) {
                                    monthChangeEvents.push({
                                        title: events[i].EventName,
                                        endDate: new Date(events[i].EndDate),
                                        startDate: new Date(events[i].StartDate),
                                        teamScheduleID: events[i].TeamScheduleID,
                                        teamName: events[i].TeamName
                                    });
                                }

                                calendars.clndr1.setEvents(monthChangeEvents);
                                AddClickEvent();
                            });
                        }
                    },
                    multiDayEvents: {
                        singleDay: 'date',
                        endDate: 'endDate',
                        startDate: 'startDate'
                    },
                    template: $('#events-calendar').html(),
                    showAdjacentMonths: true,
                    adjacentDaysChangeMonth: false,
                });

                AddClickEvent();
            }

            var today = new Date();
            var firstOfMonth = new Date(today.getFullYear(), today.getMonth(), 1, 0, 0, 0, 0);
            var lastOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0, 23, 59, 59, 0);

            GetEvents(firstOfMonth.toUTCString(), lastOfMonth.toUTCString(), Init);
        });
    </script>


    <div class="cal1">
        <script type="text/template" id="events-calendar">
            <div class='clndr-controls'>
                <div class='clndr-control-button'>
                    <span class='clndr-previous-button'>previous</span>
                </div>
                <div class='month'>{{month}} {{year}}</div>
                <div class='clndr-control-button rightalign'>
                    <span class='clndr-next-button'>next</span>
                </div>
            </div>
            <table class='clndr-table' border='0' cellspacing='0' cellpadding='0'>
                <thead>
                    <tr class='header-days'>
                        {% for(var i = 0; i < daysOfTheWeek.length; i++) { %}
                    <td class='header-day'>{{daysOfTheWeek[i]}}</td>
                        {% } %}
                    </tr>
                </thead>
                <tbody>
                    {% for(var i = 0; i < numberOfRows; i++){ %}
                <tr>
                    {% for(var j = 0; j < 7; j++){ %}
                {% var d = j + i * 7; %}
                    <td class='{{days[d].classes}}'>
                        <div class='day-contents'>{{days[d].day}}</div>
                    </td>
                    {% } %}
                </tr>
                    {% }%}
                </tbody>
            </table>
            <div class="event-listing">
              <div class="event-listing-title">EVENTS THIS MONTH</div>
              
                {% for(var i=0; i < eventsThisMonth.length; i++){ %}
                    <div class="event-item" data-team-schedule-id="{{ eventsThisMonth[i].teamScheduleID }}">
                        <div class="event-item-name">
                            <strong>{{ eventsThisMonth[i].title }}</strong><br />
                            <em>{{ eventsThisMonth[i].teamName }}</em><br />
                            <em>{{ $.format.date(eventsThisMonth[i].startDate, 'MMM d h:mm a') }}</em>
                        </div>
                    </div>
                {% } %}
                
            </div>
        </script>
    </div>
    </ContentTemplate>
    </asp:UpdatePanel>

        </div>
    </div>
                <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="updatePanel">
                <ContentTemplate>
    <div class="row" style="margin-top: 25px" runat="server" id="divWidget" visible="false">
        <div class="col-sm-12">

                    <widget:AttendanceTracking runat="server" ID="widgetAttendanceTracking" OnTeamScheduleDeleted="widgetAttendanceTracking_TeamScheduleDeleted" />

        </div>
    </div>
                                    </ContentTemplate>
            </asp:UpdatePanel>
</asp:Content>