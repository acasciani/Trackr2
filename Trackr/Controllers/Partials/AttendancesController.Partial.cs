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
    public partial class AttendancesController : OpenAccessBaseApiController<TrackrModels.Attendance, TrackrModels.ClubManagement>
    {
        [HttpPost]
        [ActionName("CreateTokenTemp")]
        public bool CreateTokenTemp()
        {
            Authenticate.RegenerateToken(int.Parse(HttpContext.Current.User.Identity.Name));
            return true;
        }

        [HttpPost]
        [ActionName("MarkPresent")]
        public HttpResponseMessage MarkPresent(Attendances item)
        {
            try
            {
                if (!item.IsTokenValid())
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "An invalid token was specified.");
                }

                int userID = Authenticate.GetUserID(item.Token);

                using (PlayersController pc = new PlayersController())
                using (AttendancesController ac = new AttendancesController())
                {
                    List<int> scopedPlayerIDs = pc.GetScopedIDs(userID, "PlayerManagement.ViewPlayers");

                    if (!scopedPlayerIDs.Contains(item.PlayerID))
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "The passed in token does not have the selected player ID to scope.");
                    }

                    ac.AddNew(new Attendance()
                    {
                        CreateDate = DateTime.Now,
                        CreateUserID = userID,
                        IsActive = true,
                        PlayerID = item.PlayerID,
                        TeamScheduleID = item.TeamScheduleID
                    });

                    return Request.CreateResponse(true);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
