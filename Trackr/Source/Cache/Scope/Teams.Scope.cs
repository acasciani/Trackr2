using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using TrackrModels;

namespace Trackr.Cache.Scope
{
    public class Teams : ScopeCache<Team, int>
    {
        public override string KeyPrefix
        {
            get { return "Scoping.Teams"; }
        }

        protected override HashSet<int> GetScopedIDsFromDataSource(int UserID, string Permission)
        {
            using (TeamsController tc = new TeamsController())
            {
                return new HashSet<int>(tc.GetScopedIDs(UserID, Permission));
            }
        }

        protected override HashSet<int> GetScopedIDsFromDataSource(int UserID, string Permission, Expression<Func<Team, bool>> Filter)
        {
            using (TeamsController tc = new TeamsController())
            {
                return new HashSet<int>(tc.GetScopedIDs(UserID, Permission));
            }
        }
    }
}