using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Telerik.OpenAccess.FetchOptimization;
using Trackr.Controllers.Models;
using Trackr.Controllers.Security;
using Trackr.Utils;
using TrackrModels;

namespace Trackr
{
    public partial class TeamSchedulesController : OpenAccessBaseApiController<TrackrModels.TeamSchedule, TrackrModels.ClubManagement>
    {
        public class Input
        {
            public string Start { get; set; }
            public string End { get; set; }
            public bool OnlyActive { get; set; }
        }

        public class GetOutput
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string EventName { get; set; }
            public int TeamScheduleID { get; set; }
            public string TeamName { get; set; }
            public bool Active { get; set; }
        }

        [HttpPost]
        [Route("api/TeamSchedules/GetForCurrentUser")]
        public HttpResponseMessage GetForCurrentUser(Input input)
        {
            try
            {
                DateTime tryParse;
                DateTime? start = DateTime.TryParse(input.Start, out tryParse) ? tryParse : (DateTime?)null;
                DateTime? end = DateTime.TryParse(input.End, out tryParse) ? tryParse : (DateTime?)null;

                using (TeamSchedulesController tsc = new TeamSchedulesController())
                using (TeamsController tc = new TeamsController())
                {
                    FetchStrategy fetch = new FetchStrategy();
                    fetch.LoadWith<TeamSchedule>(i => i.Team);
                    fetch.LoadWith<Team>(i => i.Program);

                    List<int> scopedTeamIDs = tc.GetScopedIDs(int.Parse(HttpContext.Current.User.Identity.Name), Permissions.Scheduler.ViewSchedule);

                    IQueryable<TeamSchedule> response = null;

                    if (start.HasValue && end.HasValue)
                    {
                        response = tsc.GetWhere(i => scopedTeamIDs.Contains(i.TeamID) && start <= i.StartDate && i.EndDate <= end && (input.OnlyActive ? i.IsActive : true == true), fetch);
                    }
                    else if (start.HasValue)
                    {
                        response = tsc.GetWhere(i => scopedTeamIDs.Contains(i.TeamID) && start <= i.StartDate && (input.OnlyActive ? i.IsActive : true == true), fetch);
                    }
                    else if (end.HasValue)
                    {
                        response = tsc.GetWhere(i => scopedTeamIDs.Contains(i.TeamID) && i.StartDate <= end && (input.OnlyActive ? i.IsActive : true == true), fetch);
                    }
                    else
                    {
                        response = tsc.GetWhere(i => scopedTeamIDs.Contains(i.TeamID) && (input.OnlyActive ? i.IsActive : true == true), fetch);
                    }

                    return Request.CreateResponse(response.Select(i => new GetOutput()
                        {
                            EndDate = i.EndDate,
                            StartDate = i.StartDate,
                            EventName = i.EventName,
                            TeamName = i.Team.Program.ProgramName + " - " + i.Team.TeamName,
                            TeamScheduleID = i.TeamScheduleID,
                            Active = i.IsActive
                        }));
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}