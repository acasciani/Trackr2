using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Trackr.Controls.Charts
{
    public partial class Gauge : UserControl
    {
        public int Value
        {
            get { return ViewState["Value"] as int? ?? 0; }
            set { ViewState["Value"] = value; }
        }

        public int MaxValue
        {
            get { return ViewState["MaxValue"] as int? ?? 0; }
            set { ViewState["MaxValue"] = value; }
        }

        public string Label
        {
            get { return ViewState["Label"] as string ?? string.Empty; }
            set { ViewState["Label"] = value; }
        }

        public string SubLabel
        {
            get { return ViewState["SubLabel"] as string ?? string.Empty; }
            set { ViewState["SubLabel"] = value; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            Update();
            base.OnPreRender(e);
        }

        public void Update()
        {
            string script = string.Format("UpdateGauge($('#{0}'), '{1}', {2}, 0, {3}, '{4}', '{4}');", chart.ClientID, "{0}", "{1}", "{2}", "{3}");

            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                "gauge_" + this.ClientID,
                string.Format(script, Label.Replace("'", @"\'"),
                Value,
                MaxValue,
                SubLabel),
                true);
        }
    }
}