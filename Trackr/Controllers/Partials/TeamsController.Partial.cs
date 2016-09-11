using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackrModels;

namespace Trackr
{
    public partial class TeamsController : OpenAccessBaseApiController<TrackrModels.Team, TrackrModels.ClubManagement>
    {
        public class TeamViewObject
        {
            public int ProgramID { get; set; }
            public int TeamID { get; set; }
            public string TeamName { get; set; }
            public string ProgramName { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }


        public IEnumerable<TeamViewObject> GetScopedTeamViewObject(int userID, string permission)
        {
            List<int> scopedIDs = GetScopedIDs(userID, permission);

            using (ClubManagement cm = new ClubManagement())
            {
                return cm.Teams.Where(i => scopedIDs.Contains(i.TeamID)).Select(i => new TeamViewObject()
                        {
                            ProgramID = i.ProgramID,
                            ProgramName = i.Program.ProgramName,
                            TeamID = i.TeamID,
                            TeamName = i.TeamName,
                            Start = i.StartYear,
                            End = i.EndYear
                        }).ToList();
            }
        }
    }
}