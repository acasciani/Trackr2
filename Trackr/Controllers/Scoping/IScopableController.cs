using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.OpenAccess.FetchOptimization;

namespace Trackr
{
    public interface IScopableController<T, X>
        where T : class
        where X : struct
    {
        bool IsUserScoped(int UserID, string permission, X entityID);

        T GetScopedEntity(int UserID, string permission, X primaryKey);
        T GetScopedEntity(int UserID, string permission, X primaryKey, FetchStrategy fetch);
        IEnumerable<T> GetScopedEntities(int UserID, string permission, FetchStrategy fetch);
    }
}