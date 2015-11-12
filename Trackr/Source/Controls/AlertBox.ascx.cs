using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Trackr.UI
{
    public enum AlertBoxType { Success, Error, Warning, Info }

    public partial class AlertBox : System.Web.UI.UserControl
    {
        private string Literal
        {
            get { return ViewState["AlertBoxLiteral"] as string; }
            set { ViewState["AlertBoxLiteral"] = value; }
        }

        private AlertBoxType Type
        {
            get { return ViewState["AlertBoxType"] as AlertBoxType? ?? AlertBoxType.Success; }
            set { ViewState["AlertBoxType"] = value; }
        }

        public string AlertType
        {
            get
            {
                switch (Type)
                {
                    case AlertBoxType.Success: return "success";
                    case AlertBoxType.Error: return "danger";
                    case AlertBoxType.Info: return "info";
                    case AlertBoxType.Warning: return "warning";
                    default: return "";
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Update();
        }

        public void HideStatus()
        {
            Literal = null;
        }

        public void SetStatus(string message)
        {
            SetStatus(message, false, AlertBoxType.Success);
        }

        public void SetStatus(string message, AlertBoxType type)
        {
            SetStatus(message, false, type);
        }

        public void SetStatus(string message, bool isHTML, AlertBoxType type)
        {
            Literal = message;
            Type = type;
            Update();
        }

        private void Update()
        {
            if (!string.IsNullOrWhiteSpace(Literal))
            {
                litText.Text = Literal;
                pnlAlertContainer.CssClass = string.Format("alert alert-{0}", AlertType);
                pnlAlertContainer.Visible = true;
            }
            else
            {
                pnlAlertContainer.Visible = false;
            }
        }
    }
}