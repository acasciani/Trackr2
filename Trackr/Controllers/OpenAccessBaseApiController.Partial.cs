using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Linq;
using Telerik.OpenAccess;
using System;
using System.Linq.Expressions;
using Telerik.OpenAccess.FetchOptimization;

namespace Trackr
{
    public abstract partial class OpenAccessBaseApiController<TEntity, TContext> : ApiController
        where TContext : OpenAccessContext, new()
    {
        public virtual TEntity AddNew(TEntity entity)
        {
            TEntity newEntity = repository.AddNew(entity);
            return newEntity;
        }

        public virtual void Update(TEntity entity)
        {
            repository.Update(entity);
        }

        public virtual IQueryable<TEntity> GetWhere(Func<TEntity, bool> filter)
        {
            var entities = repository.GetWhere(filter);

            return entities;
        }

        public virtual IQueryable<TEntity> GetWhere(Func<TEntity, bool> filter, FetchStrategy fetchStrategy)
        {
            var entities = repository.GetWhere(filter, fetchStrategy);

            return entities;
        }
    }
}