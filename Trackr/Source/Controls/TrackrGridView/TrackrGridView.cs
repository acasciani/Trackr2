using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Trackr.Source.Controls.TGridView;

namespace Trackr.Source.Controls
{
    [ToolboxData("<{0}:TrackrGridView runat=server></{0}:TrackrGridView>")]
    public partial class TrackrGridView : GridView
    {
        public GridViewData GridViewItems
        {
            get { return HttpContext.Current.Session["TrackrGridViewResults"] as GridViewData; }
            set { HttpContext.Current.Session["TrackrGridViewResults"] = value; }
        }

        // All export functions (i.e. Excel, PDF, Word, etc) will call this event when completed.
        public event EventHandler ExportCompleted;

        protected override void OnDataBound(EventArgs e)
        {
            base.OnDataBound(e);

            // the following line adds thead to the header (this is required by bootstrap)
            if (Rows.Count > 0) { HeaderRow.TableSection = TableRowSection.TableHeader; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // the following line adds thead to the header (this is required by bootstrap)
            if (Rows.Count > 0) HeaderRow.TableSection = TableRowSection.TableHeader;
            if (base.CssClass.Length == 0)//if we didn't get a CSSClass from the tag, add the default
            {
                base.CssClass = "table table-hover table-striped";
            }

            base.GridLines = System.Web.UI.WebControls.GridLines.None;

            if (base.EmptyDataText.Length == 0)
            {
                base.EmptyDataText = "No Results Found";
            }

            PagerSettings.Mode = PagerButtons.NumericFirstLast;
            PagerStyle.CssClass = "gridview-pager";
        }

        protected override int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
        {
            // Default behavior to create ChildTable.
            int result = base.CreateChildControls(dataSource, dataBinding);

            // Custom behavior to add additional user tools at the bottom of the ChildTable.
            if (GridViewItems != null && GridViewItems.Data != null && GridViewItems.Data.Count() > 0)
            {
                AddExportControls();

                if (AllowPaging && (DisplayPagingSummary || DisplayResultsPerPageOptions))
                {
                    AddPageResultsSummary();
                }
            }

            return result;
        }

        private void AddExportControls()
        {
            List<Control> exportControls = new List<Control>();

            // Excel here...
            if (DisplayExcelExportButton)
            {
                LinkButton button = Trackr.Source.Controls.TGridView.Common.CreateExportButton("Excel Export", "filetypes filetypes-xlsx", "Export data to Excel");
                button.ID = "lnkExportToExcel"; // need an id so we can reference it in the PostBack trigger
                exportControls.Add(button);
                button.Click += ExportToExcel_Click;
            }

            // Add others here as needed...


            // Add a new row/col panel and add all export buttons to it.
            if (exportControls.Count() > 0)
            {
                Panel exportControlsRow = new Panel()
                {
                    CssClass = "row gridview-export-row"
                };
                Panel exportControlsCol = new Panel()
                {
                    CssClass = "col-sm-12 text-right"
                };
                exportControlsRow.Controls.Add(exportControlsCol);

                // Create update panel so we can allow content to be written back to output stream and downloaded
                // We will use conditional so that if we do not specify a ID to trigger, then it uses the page's default update panel behavior (e.g. allow async when not specified)
                UpdatePanel updatePanel = new UpdatePanel()
                {
                    UpdateMode = UpdatePanelUpdateMode.Conditional
                };
                exportControlsCol.Controls.Add(updatePanel);

                // Add each export control to the column div
                foreach (Control controlToAdd in exportControls)
                {
                    // Add a trigger for the control.
                    PostBackTrigger trigger = new PostBackTrigger()
                    {
                        ControlID = controlToAdd.ID
                    };
                    updatePanel.ContentTemplateContainer.Controls.Add(controlToAdd);
                    updatePanel.Triggers.Add(trigger);
                }

                // Add the row div to the control.
                this.Controls.Add(exportControlsRow);
            }
        }
    }
}