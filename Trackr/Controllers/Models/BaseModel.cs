using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Trackr.Controllers.Security;

namespace Trackr.Controllers.Models
{
    [Serializable]
    public abstract class BaseModel
    {
        public Guid Token { get; set; }

        public bool IsTokenValid()
        {
            return Authenticate.TokenExists(Token);
        }
    }
}