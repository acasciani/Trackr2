using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackr
{
    public interface IScopableController<X> where X : struct
    {
        bool IsUserScoped(int UserID, string permission, X entityID);
    }
}
