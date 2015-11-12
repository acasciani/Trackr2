using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trackr
{
    public class UserUnauthorizedException :Exception
    {
        public UserUnauthorizedException(string message) : base(message)
        {

        }
    }
}