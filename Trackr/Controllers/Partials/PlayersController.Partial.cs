using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;
using Telerik.OpenAccess;

namespace Trackr
{
    public partial class PlayersController : OpenAccessBaseApiController<TrackrModels.Player, TrackrModels.ClubManagement>
    {
        public class PlayerViewObject
        {
            public int? Age { get; set; }
            public DateTime? BirthDate { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int PlayerID { get; set; }
            public IEnumerable<int> TeamIDs { get; set; }
            public IEnumerable<int> ProgramIDs { get; set; }
        }

        public List<PlayerViewObject> GetAllScopedPlayerViewObjects(int UserID, string Permission, Expression<Func<Player, bool>> filter)
        {
            // due to the depth of data we need, we need to use the model directly so objects aren't detached until we supply a select
            using (ClubManagement cm = new ClubManagement())
            {
                List<int> playerIDs = GetScopedIDs(UserID, Permission);

                FetchStrategy playerPassFetch = new FetchStrategy() { MaxFetchDepth = 2 };
                playerPassFetch.LoadWith<Player>(i => i.Person);
                playerPassFetch.LoadWith<Player>(i => i.TeamPlayers);
                playerPassFetch.LoadWith<TeamPlayer>(i => i.Team);

                // player pass possibility first
                var players = cm.Players
                    .LoadWith(playerPassFetch)
                    .Where(i => playerIDs.Contains(i.PlayerID))
                    .Select(i => new PlayerViewObject()
                {
                    Age = i.Person.DateOfBirth.HasValue ? DateTime.Today.ToUniversalTime().Year - i.Person.DateOfBirth.Value.Year : (int?)null,
                    BirthDate = i.Person.DateOfBirth,
                    FirstName = i.Person.FName,
                    LastName = i.Person.LName,
                    PlayerID = i.PlayerID,

                    TeamIDs = i.TeamPlayers.Select(j => j.TeamID).Union(i.PlayerPasses.SelectMany(j => j.TeamPlayers).Select(j => j.TeamID)).Distinct(),
                    ProgramIDs = i.TeamPlayers.Select(j => j.Team.ProgramID).Union(i.PlayerPasses.SelectMany(j => j.TeamPlayers).Select(j => j.Team.ProgramID)).Distinct(),
                }).ToList();

                return players;
            }
        }
    }
}