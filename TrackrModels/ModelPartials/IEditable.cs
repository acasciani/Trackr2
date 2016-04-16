using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackrModels
{
    public interface IEditable
    {
        Guid EditToken { get; set; }
        bool WasModified { get; set; }
    }
}
