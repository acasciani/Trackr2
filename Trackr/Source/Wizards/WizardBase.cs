using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Trackr.Utils;

namespace Trackr.Source.Wizards
{
    public class WizardBase<K> : UserControl where K : struct
    {
        public K? PrimaryKey
        {
            get
            {
                if (ViewState["PrimaryKey"] as K? == null && Request.QueryString["id"] != null)
                {
                    K? converted = Request.QueryString["id"].TryParse<K>();
                    if (converted.HasValue)
                    {
                        ViewState["PrimaryKey"] = converted;
                        return converted;
                    }
                }

                return ViewState["PrimaryKey"] as K?;
            }

            set
            {
                ViewState["PrimaryKey"] = value;
            }
        }

        public bool IsNew { get { return !PrimaryKey.HasValue; } }

        public bool WasNew
        {
            get
            {
                // If no primary key set then it is new and therefore was new
                if (!PrimaryKey.HasValue) { return true; }

                // If primary key set, need to make sure query string did not parse to a valid K type
                K? converted = Request.QueryString["id"].TryParse<K>();
                return !converted.HasValue;
            }
        }
    }
}