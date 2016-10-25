using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Trackr;
using Trackr.Utils;

namespace Trackr.Cache.Scope
{
    public abstract class ScopeCache<T, X>
        where T : class
        where X : struct
    {
        public abstract string KeyPrefix { get; }

        // Key Format (NO SPACES):
        // PREFIX : UserID : Permission : Filter

        private string RenewKey(int UserID, string Permission, Expression<Func<T, bool>> Filter)
        {
            return string.Format("{0}:{1}:{2}:{3}:Renew", KeyPrefix, UserID, (string.IsNullOrWhiteSpace(Permission) ? "" : Permission), (Filter == null ? "" : Filter.GetCacheKey()));
        }
        private string StandardKey(int UserID, string Permission, Expression<Func<T, bool>> Filter)
        {
            return string.Format("{0}:{1}:{2}:{3}", KeyPrefix, UserID, (string.IsNullOrWhiteSpace(Permission) ? "" : Permission), (Filter == null ? "" : Filter.GetCacheKey()));
        }

        #region Check if we need to renew cache
        protected bool Renewable(int UserID, string Permission, Expression<Func<T, bool>> Filter)
        {
            return HttpContext.Current.Cache.Get(RenewKey(UserID, Permission, Filter)) as bool? ?? true;
        }
        #endregion

        #region Renew cache on next pull
        public void Renew(int UserID)
        {
            Renew(UserID, null, null);
        }

        public void Renew(int UserID, string Permission)
        {
            Renew(UserID, Permission, null);
        }

        public void Renew(int UserID, Expression<Func<T, bool>> Filter)
        {
            Renew(UserID, null, Filter);
        }

        public void Renew(int UserID, string Permission, Expression<Func<T, bool>> Filter)
        {
            HttpContext.Current.Cache.Insert(RenewKey(UserID, Permission, Filter), true, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
        }
        #endregion

        public HashSet<X> GetScopedIDs(int UserID, string Permission)
        {
            return GetScopedIDs(UserID, Permission, null);
        }

        public HashSet<X> GetScopedIDs(int UserID, string Permission, Expression<Func<T, bool>> filter)
        {
            string key = StandardKey(UserID, Permission, filter);

            HashSet<X> data = HttpContext.Current.Cache.Get(key) as HashSet<X>;

            if (data == null || Renewable(UserID, Permission, filter))
            {
                string renewKey = RenewKey(UserID, Permission, filter);

                if (filter == null)
                {
                    data = GetScopedIDsFromDataSource(UserID, Permission);
                }
                else
                {
                    data = GetScopedIDsFromDataSource(UserID, Permission, filter);
                }

                // add the renew entry
                HttpContext.Current.Cache.Insert(renewKey, false, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));

                System.Web.Caching.CacheDependency _cache = new System.Web.Caching.CacheDependency(null, new string[] { renewKey });

                // This hashset cache should not be remvoed UNLESS the renewable entry expires or .NET removes the entry
                HttpContext.Current.Cache.Insert(key, data, _cache, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration);
            }

            return data;
        }

        protected abstract HashSet<X> GetScopedIDsFromDataSource(int UserID, string Permission);
        protected abstract HashSet<X> GetScopedIDsFromDataSource(int UserID, string Permission, Expression<Func<T, bool>> Filter);
    }
}