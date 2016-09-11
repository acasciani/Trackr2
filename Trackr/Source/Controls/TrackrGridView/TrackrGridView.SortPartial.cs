using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Trackr.Utils;

namespace Trackr.Source.Controls
{
    public partial class TrackrGridView : GridView
    {
        protected override void OnSorting(GridViewSortEventArgs e)
        {
            // We need to first sort by all the ones that have been pinned, then lastly sort by the selected column.
            List<KeyValuePair<string, bool>> sortCriteria;

            if (ContainsPinnedColumn(e.SortExpression))
            {
                // update the sort direction
                int index = GetPinnedColumnPair(e.SortExpression);
                PinnedColumns[index] = new KeyValuePair<string, bool>(PinnedColumns[index].Key, !PinnedColumns[index].Value);
                sortCriteria = PinnedColumns.ToList(); // clone it
            }
            else
            {
                // Just sort, not pinned
                sortCriteria = PinnedColumns.ToList();

                IsLastSortedColumnAscending = (LastSortedColumn == e.SortExpression) ? !IsLastSortedColumnAscending : true; // true by default.
                LastSortedColumn = e.SortExpression;

                sortCriteria.Add(new KeyValuePair<string, bool>(e.SortExpression, IsLastSortedColumnAscending));
            }

            List<object> sorted = OrderObjects(sortCriteria).ToList();

            GridViewItems.ResetData();
            GridViewItems.AddData(sorted);

            PageIndex = 0; // always reset page index on sorting.
            DataSource = sorted;
            DataBind();
        }

        private string LastSortedColumn
        {
            get { return ViewState["GridViewResults_SortExpression"] as string; }
            set { ViewState["GridViewResults_SortExpression"] = value; }
        }

        private bool IsLastSortedColumnAscending
        {
            get { return ViewState["GridViewResults_SortDirection"] as bool? ?? true; }
            set { ViewState["GridViewResults_SortDirection"] = value; }
        }

        private IOrderedEnumerable<object> OrderObjects(List<KeyValuePair<string, bool>> sortBys)
        {
            Type viewModelType = GridViewItems.DataType;
            List<object> unordered = GridViewItems.Data;

            IOrderedEnumerable<object> Ordered = null;

            foreach (KeyValuePair<string, bool> sortBy in sortBys)
            {
                string sortExp = sortBy.Key;
                bool isAsc = sortBy.Value;

                System.Reflection.PropertyInfo sortProp = viewModelType.GetProperty(sortExp);

                if (sortBys.IsFirst(sortBy))
                {
                    if (isAsc)
                    {
                        Ordered = unordered.OrderBy(i => sortProp.GetValue(i, null));
                    }
                    else
                    {
                        Ordered = unordered.OrderByDescending(i => sortProp.GetValue(i, null));
                    }
                }
                else
                {
                    if (isAsc)
                    {
                        Ordered = Ordered.ThenBy(i => sortProp.GetValue(i, null));
                    }
                    else
                    {
                        Ordered = Ordered.ThenByDescending(i => sortProp.GetValue(i, null));
                    }
                }
            }

            return Ordered;
        }
    }
}