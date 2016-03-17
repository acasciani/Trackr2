using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
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
                    List<int> scopedTeamIDs = tc.GetScopedIDs(int.Parse(HttpContext.Current.User.Identity.Name), "PlayerManagement.ViewPlayers");

                    if (start.HasValue && end.HasValue)
                    {
                        return Request.CreateResponse(tsc.GetWhere(i => scopedTeamIDs.Contains(i.TeamID) && start <= i.StartDate && i.EndDate <= end));
                    }
                    else if (start.HasValue)
                    {
                        return Request.CreateResponse(tsc.GetWhere(i => scopedTeamIDs.Contains(i.TeamID) && start <= i.StartDate));
                    }
                    else if (end.HasValue)
                    {
                        return Request.CreateResponse(tsc.GetWhere(i => scopedTeamIDs.Contains(i.TeamID) && i.StartDate <= end));
                    }
                    else
                    {
                        return Request.CreateResponse(tsc.GetWhere(i => scopedTeamIDs.Contains(i.TeamID)));
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
