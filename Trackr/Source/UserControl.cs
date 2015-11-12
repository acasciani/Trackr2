using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackrModels;
using Trackr.Utils;

namespace Trackr
{
    public class UserControl : System.Web.UI.UserControl
    {
        public WebUser CurrentUser
        {
            get
            {
                return PageExtensions.GetCurrentWebUser();
            }
        }
    }
}