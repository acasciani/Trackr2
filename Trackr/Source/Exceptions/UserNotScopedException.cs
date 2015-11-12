using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trackr
{
    public class UserNotScopedException : Exception
    {
        public UserNotScopedException(string message)
            : base(message)
        {

        }
    }
}