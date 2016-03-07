using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Trackr.Controllers.Models
{
    [Serializable]
    public class Attendances : BaseModel
    {
        public int TeamScheduleID { get; set; }
        public int PlayerID { get; set; }
        public string Notes { get; set; }
    }
}