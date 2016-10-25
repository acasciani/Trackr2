using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using TrackrModels;

namespace Trackr.Cache.Scope
{
    public class Players : ScopeCache<Player, int>
    {
        public override string KeyPrefix
        {
            get { return "Scoping.Players"; }
        }

        protected override HashSet<int> GetScopedIDsFromDataSource(int UserID, string Permission)
        {
            using (PlayersController pc = new PlayersController())
            {
                return new HashSet<int>(pc.GetScopedIDs(UserID, Permission));
            }
        }

        protected override HashSet<int> GetScopedIDsFromDataSource(int UserID, string Permission, Expression<Func<Player, bool>> Filter)
        {
            using (PlayersController pc = new PlayersController())
            {
                return new HashSet<int>(pc.GetScopedIDs(UserID, Permission, Filter));
            }
        }
    }
}