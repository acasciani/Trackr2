using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Trackr.Source.Controls
{
    public class ClickablePanel : Panel, IPostBackEventHandler
    {
        public int PlayerID
        {
            get { return ViewState["PlayerID"] as int? ?? 0; }
            set { ViewState["PlayerID"] = value; }
        }

        public bool IsDisabled
        {
            get { return ViewState["IsDisabled"] as bool? ?? false; }
            set { ViewState["IsDisabled"] = value; }
        }

        public event EventHandler Click;

        protected virtual void OnClick(EventArgs e)
        {
            if (Click != null)
            {
                Click(this, e);
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            OnClick(new EventArgs());
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!IsDisabled)
            {
                writer.AddAttribute("onclick", "javascript:" + Page.ClientScript.GetPostBackEventReference(this, (1).ToString()));
            }
            base.Render(writer);
        }
    }
}