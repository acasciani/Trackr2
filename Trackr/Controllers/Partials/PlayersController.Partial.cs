using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;
using Telerik.OpenAccess;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Trackr.Utils;

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

        public class PlayerMatch
        {
            public int PlayerID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public double FirstName_Distance { get; set; }
            public double LastName_Distance { get; set; }
            public double DOB_Distance { get; set; }
            public string TeamName { get; set; }
            public DateTime? TeamStart { get; set; }
            public DateTime? TeamEnd { get; set; }
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

        public List<PlayerMatch> GetPossibleMatches(int clubID, string firstName, string lastName, char? gender, DateTime? dob, double? firstNamePrecision = null, double? lastNamePrecision = null, int? dobPrecision = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ClubManagementConnection"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand("GetPossiblePlayerMatches", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ClubID", clubID);
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@Gender", gender);
                    cmd.Parameters.AddWithValue("@DOB", dob);
                    cmd.Parameters.AddWithValue("@FirstNamePrecision", firstNamePrecision);
                    cmd.Parameters.AddWithValue("@LastNamePrecision", lastNamePrecision);
                    cmd.Parameters.AddWithValue("@DOBPrecision", dobPrecision);

                    conn.Open();

                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr != null)
                        {
                            List<PlayerMatch> results = new List<PlayerMatch>();

                            while (dr.Read())
                            {
                                results.Add(new PlayerMatch()
                                {
                                    PlayerID = Convert.ToInt32(dr["PlayerID"]),
                                    FirstName = dr["FName"].ToString(),
                                    LastName = dr["LName"].ToString(),
                                    DateOfBirth = dr["DateOfBirth"].ToNullableDateTime(),
                                    FirstName_Distance = Convert.ToDouble(dr["FName_Distance"]),
                                    LastName_Distance = Convert.ToDouble(dr["LName_Distance"]),
                                    DOB_Distance = Convert.ToDouble(dr["DOB_Distance"]),
                                    TeamName = dr["TeamName"].ToNullableString(),
                                    TeamStart = dr["StartYear"].ToNullableDateTime(),
                                    TeamEnd = dr["EndYear"].ToNullableDateTime()
                                });
                            }

                            return results.OrderBy(i => i.DOB_Distance).ThenBy(i => i.LastName_Distance).ThenBy(i => i.FirstName_Distance).ToList();
                        }
                        else
                        {
                            return Enumerable.Empty<PlayerMatch>().ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An Error Occurred getting possible player matches.", ex);
            }
        }
    }
}