using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Trackr.Source.Controls.TGridView
{
    public interface IDataControlField
    {
        /*
         * IsPinned would be true only if the column has been pinned. (effect: Changing glyphicons displayed)
         * IsPinnable would be true only if the column has already been sorted. (effect: Making linkbutton visible or not)
         * Both cases are mandated and directed from TrackrGridView
         */

        bool IsPinned { get; set; } // This is used ONLY for the Bound Field's knowledge of initializing algorithm.
        bool IsPinnable { get; set; } // This is used ONLY for the Bound Field's knowledge of initializing algorithm.
        bool AllowPinnable { get; set; } // Callable from aspx page to allow pinning on a column. False by default
        bool HiddenInExport { get; set; } // Truth if we should Hide this column from displaying in the exported document
        event EventHandler ColumnPinned; // The event handler that is called when the toggle button is clicked.
    }
}