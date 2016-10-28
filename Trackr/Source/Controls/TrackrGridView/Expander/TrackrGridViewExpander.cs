using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Trackr.Source.Controls.TGridView
{
    [ParseChildren(true), PersistChildren(true)]
    public class TrackrGridViewExpander<T> : WebControl, INamingContainer
    {
        private TrackrGridView<T> _gridView = null;
        private List<LinkButton> _toggleButtons = new List<LinkButton>();

        /// <summary>Display the controls in the top level row</summary>
        public bool DisplayTopLevelControls { get; set; }

        /// <summary>Display the controls in the bottom level row</summary>
        public bool DisplayBottomLevelControls { get; set; }

        /// <summary>Should we allow the grid view to be expanded</summary>
        public bool IsExpandable { get; set; }

        /// <summary>The text that is displayed next to the Expand glyphicon</summary>
        public string ExpandText { get; set; }

        /// <summary>The text that is displayed next to the Collapse glyphicon</summary>
        public string CollapseText { get; set; }

        /// <summary>Is the grid view expanded? Need to use view state so we can persist the decision. This is useful if a postback happens (like the sort btn was clicked)</summary>
        public bool IsExpanded
        {
            get { return ViewState["IsGridViewExpanded"] as bool? ?? false; }
            set { ViewState["IsGridViewExpanded"] = value; }
        }

        //[Browsable(false), DefaultValue(null), Description("The TrackrGridView control"), TemplateContainer(typeof(TrackrGridView<T>)), PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual TrackrGridView<T> TrackrGridView
        {
            get
            {
                return _gridView;
            }
            set
            {
                _gridView = value;
            }
        }

        protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }

        private LinkButton CreateExpandCollapseButton()
        {
            LinkButton toggle = new LinkButton();

            if (IsExpanded)
            {
                toggle.Click += ResizeGridViewDown_Click;
            }
            else
            {
                toggle.Click += ResizeGridViewUp_Click;
            }

            return toggle;
        }

        private Panel CreateTopLevelControls()
        {
            if (!DisplayTopLevelControls)
            {
                throw new Exception("Top level controls not enabled");
            }

            Panel topRow = new Panel()
            {
                CssClass = "row top-level"
            };
            Panel topCol = new Panel()
            {
                CssClass = "col-sm-12"
            };
            topRow.Controls.Add(topCol);

            if (IsExpandable)
            {
                // can add more buttons here, but currently only a toggle bottom to maximize/minimize
                LinkButton toggleBtn = CreateExpandCollapseButton();
                _toggleButtons.Add(toggleBtn);
                topCol.Controls.Add(toggleBtn);
            }

            return topRow;
        }

        private Panel CreateBottomLevelControls()
        {
            if (!DisplayBottomLevelControls)
            {
                throw new Exception("Bottom level controls not enabled");
            }

            Panel bottomRow = new Panel()
            {
                CssClass = "row bottom-level"
            };
            Panel bottomCol = new Panel()
            {
                CssClass = "col-sm-12"
            };
            bottomRow.Controls.Add(bottomCol);

            if (IsExpandable)
            {
                // can add more buttons here, but currently only a toggle bottom to maximize/minimize
                LinkButton toggleBtn = CreateExpandCollapseButton();
                _toggleButtons.Add(toggleBtn);
                bottomCol.Controls.Add(toggleBtn);
            }

            return bottomRow;
        }

        protected void ResizeGridViewUp_Click(object sender, EventArgs e)
        {
            // Expand
            IsExpanded = true;
        }

        protected void ResizeGridViewDown_Click(object sender, EventArgs e)
        {
            // Collapse
            IsExpanded = false;
        }

        protected override void OnInit(EventArgs e)
        {
            // assign the pre load event handler for this control on the page
            Page.PreLoad += TrackrGridViewExpander_PreLoad;
            base.OnInit(e);
        }

        // do all styling/css application that happens for this control in here
        protected override void Render(HtmlTextWriter writer)
        {
            CssClass = CssClass.Replace("trackrgridview-expanded", "").Replace("trackrgridview-collapsed", "").Trim() + string.Format(" trackrgridview-{0}", IsExpanded ? "expanded" : "collapsed");
            base.Render(writer);
        }

        // do all styling/css application that happens for this control's children in here
        protected override void RenderChildren(HtmlTextWriter writer)
        {
            // only do custom rendering if the expander is enabled
            if (IsExpandable)
            {
                Panel container = (Panel)Controls[0];
                container.CssClass = string.Format("trackrgridview-{0}-container", IsExpanded ? "expanded" : "collapsed");

                // for each link button that does toggling, update styles
                foreach (LinkButton lnkBtn in _toggleButtons)
                {
                    lnkBtn.Controls.Add(new Literal() { Text = string.Format("<span class=\"glyphicons glyphicons-resize-{0}\"></span>", IsExpanded ? "small" : "full") });
                    lnkBtn.ToolTip = IsExpanded ? "Collapse results" : "Pop up results";

                    if (!IsExpanded && !string.IsNullOrWhiteSpace(ExpandText))
                    {
                        lnkBtn.Controls.Add(new Literal() { Text = ExpandText });
                    }

                    if (IsExpanded && !string.IsNullOrWhiteSpace(CollapseText))
                    {
                        lnkBtn.Controls.Add(new Literal() { Text = CollapseText });
                    }
                }
            }

            base.RenderChildren(writer);
        }

        // This event handler is fired when the Page invokes the PreLoad. We use preload instead of load for 2 reasons:
        // 1. Some pages instantiate the data source on the gridview in their Page_Load event handler. Page events happen before control events to a Page load event will fire before a Control load event
        // 2. Init event handler does not expose ViewState since the ViewState is loaded up at the Load event. We need ViewState to track if the expander is expanded or collapsed
        protected void TrackrGridViewExpander_PreLoad(object sender, EventArgs e)
        {
            CssClass = CssClass.Replace("trackrgridview-expander", "").Trim() + " trackrgridview-expander";

            // If the developer decided to make this grid view expandable, then we do our custom behavior, otherwise we do our standard bevhaior (just grid view)
            if (IsExpandable)
            {
                // This is the container that holds all controls. It is used for rendering the display
                Panel container = new Panel(); // first element
                Controls.Add(container);

                // If top level controls are allowed, the add the top level controls
                if (DisplayTopLevelControls)
                {
                    container.Controls.Add(CreateTopLevelControls());
                }

                // add the default grid view
                container.Controls.Add(_gridView);

                // If bottom level controls are allowed, the add the bottom level controls
                if (DisplayBottomLevelControls)
                {
                    container.Controls.Add(CreateBottomLevelControls());
                }
            }
            else
            {
                Controls.Add(_gridView);
            }
        }
    }
}