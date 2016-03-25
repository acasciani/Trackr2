using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;

namespace Trackr
{
    public interface IScopable<T, X>
    {
        List<X> SelectIDListByScopeAssignment(ScopeAssignment scopeAssignment, Expression<Func<T, bool>> preFilter);
    }
}
