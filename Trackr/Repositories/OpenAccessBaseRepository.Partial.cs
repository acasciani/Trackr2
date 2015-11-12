using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Telerik.OpenAccess;
using Telerik.OpenAccess.FetchOptimization;

namespace Trackr
{
    public partial interface IOpenAccessBaseRepository<TEntity, TContext>
            where TContext : OpenAccessContext, new()
    {
        IQueryable<TEntity> GetWhere(Func<TEntity, bool> filter);
        IQueryable<TEntity> GetWhere(Func<TEntity, bool> filter, FetchStrategy fetchStrategy);
    }

    public abstract partial class OpenAccessBaseRepository<TEntity, TContext> : IOpenAccessBaseRepository<TEntity, TContext>
       where TContext : OpenAccessContext, new()
    {
        public virtual IQueryable<TEntity> GetWhere(Func<TEntity, bool> filter)
        {
            List<TEntity> allEntities = dataContext.GetAll<TEntity>().Where(filter).ToList();

            fetchStrategy.MaxFetchDepth = 4;
            List<TEntity> detachedEntities = dataContext.CreateDetachedCopy<List<TEntity>>(allEntities, fetchStrategy);

            return detachedEntities.AsQueryable();
        }

        public virtual IQueryable<TEntity> GetWhere(Func<TEntity, bool> filter, FetchStrategy fetchStrategy)
        {
            List<TEntity> allEntities = dataContext.GetAll<TEntity>().Where(filter).ToList();

            List<TEntity> detachedEntities = dataContext.CreateDetachedCopy<List<TEntity>>(allEntities, fetchStrategy);

            return detachedEntities.AsQueryable();
        }
    }
}