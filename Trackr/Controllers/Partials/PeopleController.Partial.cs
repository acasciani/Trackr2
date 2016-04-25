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
    public partial class PeopleController : OpenAccessBaseApiController<TrackrModels.Person, TrackrModels.ClubManagement>
    {
        [Serializable]
        public class PersonMatch
        {
            public int PersonID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public double FirstName_Distance { get; set; }
            public double LastName_Distance { get; set; }
        }

        // for now we will just use the one that is used by players. but we can always create a new stored proc if needed
        public List<PersonMatch> GetPossibleMatches(int clubID, string firstName, string lastName, char? gender, DateTime? dob, double? firstNamePrecision = null, double? lastNamePrecision = null, int? dobPrecision = null)
        {
            using (PlayersController pc = new PlayersController())
            {
                var matches = pc.GetPossibleMatches(clubID, firstName, lastName, gender, dob, firstNamePrecision, lastNamePrecision, dobPrecision);

                return matches.Select(i => new PersonMatch()
                {
                    PersonID = i.PersonID,
                    FirstName = i.FirstName,
                    LastName = i.LastName,
                    FirstName_Distance = i.FirstName_Distance,
                    LastName_Distance = i.LastName_Distance
                }).Distinct().ToList();
            }
        }
    }
}