using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Trackr.Source.Controls
{
    public partial class TrackrGridView<T> : GridView
    {
        public bool DisplayPagingSummary { get; set; }
        public bool DisplayResultsPerPageOptions { get; set; }

        private int[] _ResultsPerPageOptions = new int[] { 10, 20, 50, 100, 250, 500, 1000, 2500 }; // default values.
        public int[] ResultsPerPageOptions
        {
            get
            {
                if (!_ResultsPerPageOptions.Contains(PageSize))
                {
                    List<int> list = _ResultsPerPageOptions.ToList();
                    list.Add(PageSize);
                    return list.ToArray();
                }
                return _ResultsPerPageOptions;
            }
            set { _ResultsPerPageOptions = value; }
        }

        protected override void OnPageIndexChanging(GridViewPageEventArgs e)
        {
            PageIndex = e.NewPageIndex;
            DataSource = GridViewItems.Data;
            DataBind();
        }

        private void AddPageResultsSummary()
        {
            Panel containerFormHorizontal = new Panel()
            {
                CssClass = "form-horizontal trackrgridview-summary-row"
            };

            Panel containerRow = new Panel()
            {
                CssClass = "row"
            };
            Panel summaryContainerCol = new Panel()
            {
                CssClass = "col-sm-6 trackrgridview-summary-description"
            };
            Panel ddlContainerCol = new Panel()
            {
                CssClass = "col-sm-6 trackrgridview-summary-resultsperpage"
            };

            containerRow.Controls.Add(summaryContainerCol);
            containerRow.Controls.Add(ddlContainerCol);

            containerFormHorizontal.Controls.Add(containerRow);

            // Add left side (summary line)
            if (DisplayPagingSummary)
            {
                Literal summaryLit = new Literal()
                {
                    Text = string.Format("<div class=\"form-control-static\">Displaying {0} - {1} of {2} results.</div>", ((PageIndex * PageSize) + 1), ((PageIndex * PageSize) + Rows.Count), GridViewItems.Data.Count())
                };
                summaryContainerCol.Controls.Add(summaryLit);
            }

            // Add right side (drop down) for results per page changer
            if (DisplayResultsPerPageOptions)
            {
                Panel ddlFormContainer = new Panel()
                {
                    CssClass = "form-group"
                };

                Panel ddlContainer = new Panel()
                {
                    CssClass = "col-sm-6"
                };

                DropDownList ddl = new DropDownList()
                {
                    AutoPostBack = true,
                    CssClass = "form-control",
                    ID = "ResultsPerPage"
                };

                ddl.DataSource = ResultsPerPageOptions
                    .OrderBy(i => i)
                    .Select(i => new
                    {
                        Text = i == int.MaxValue ? "All" : i.ToString(),
                        Value = i.ToString()
                    });
                ddl.DataTextField = "Text";
                ddl.DataValueField = "Value";
                ddl.DataBind();

                if (ResultsPerPageOptions.Contains(PageSize))
                {
                    ddl.SelectedValue = PageSize.ToString();
                }

                ddl.SelectedIndexChanged += ResultsPerPage_SelectedIndexChanged;
                ddlContainer.Controls.Add(ddl);

                // Add the drop down and label now that we have a client id.
                ddlFormContainer.Controls.Add(new Literal()
                {
                    Text = string.Format("<label for=\"{0}\" class=\"col-sm-6 control-label\">Results Per Page</label>", ddl.ClientID)
                });
                ddlFormContainer.Controls.Add(ddlContainer);

                ddlContainerCol.Controls.Add(ddlFormContainer);
            }

            // Add the row div to the control.
            this.Controls.Add(containerFormHorizontal);
        }

        protected void ResultsPerPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;

            // update PageSize view state object, rebind data with new page size
            int size;
            if (ddl != null && int.TryParse(ddl.SelectedValue, out size) && size != PageSize)
            {
                PageSize = size;
                PageIndex = 0;
                DataSource = GridViewItems.Data;
                DataBind();
            }
        }
    }
}