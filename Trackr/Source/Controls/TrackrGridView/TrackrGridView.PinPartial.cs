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
    public partial class TrackrGridView : GridView
    {
        #region Events
        public event EventHandler ColumnPinned;
        #endregion

        /// <summary>This tells us if the GRIDVIEW allows pinned columns.</summary>
        public bool IsPinnable
        {
            get { return ViewState["IsPinnable"] as bool? ?? false; }
            set { ViewState["IsPinnable"] = value; }
        }

        private List<KeyValuePair<string, bool>> PinnedColumns
        {
            // the key is the column, the value is: true if ascending, false if descending
            get
            {
                List<KeyValuePair<string, bool>> queue = ViewState["PinnedColumns"] as List<KeyValuePair<string, bool>>;
                if (queue == null)
                {
                    // store a mutable queue in view state, change data in it by reference
                    queue = new List<KeyValuePair<string, bool>>();
                    ViewState["PinnedColumns"] = queue;
                }
                return queue;
            }
        }

        private bool ContainsPinnedColumn(string column)
        {
            return PinnedColumns.Count(i => i.Key == column) > 0;
        }

        private int GetPinnedColumnPair(string column)
        {
            return PinnedColumns.IndexOf(PinnedColumns.Where(i => i.Key == column).FirstOrDefault());
        }

        protected override void InitializeRow(GridViewRow row, DataControlField[] fields)
        {
            if (row.RowType == DataControlRowType.Header && IsPinnable)
            {
                foreach (DataControlField field in fields)
                {
                    IDataControlField trackrField = field as IDataControlField;
                    if (trackrField != null)
                    {
                        string sortExp = field.SortExpression;
                        bool isPinned = ContainsPinnedColumn(sortExp);

                        trackrField.IsPinnable = trackrField.AllowPinnable && (isPinned || (!string.IsNullOrWhiteSpace(LastSortedColumn) && LastSortedColumn == sortExp));
                        trackrField.IsPinned = isPinned;
                        trackrField.ColumnPinned += HandleToggle;
                    }
                }
            }

            base.InitializeRow(row, fields);
        }

        protected void HandleToggle(object sender, EventArgs e)
        {
            ColumnPinningEventArgs cpea = (ColumnPinningEventArgs)e;

            // If we get valid event args back, then we can start processing a toggle.
            if (cpea != null)
            {
                // If the col key is in the list, we need to unpin (remove). If it is not in the list, we need to pin (Add)
                if (ContainsPinnedColumn(cpea.ColumnName))
                {
                    // unpin
                    PinnedColumns.RemoveAll(i => i.Key == cpea.ColumnName);
                }
                else
                {
                    // pin, but make sure it was the last sorted first
                    if (LastSortedColumn == cpea.ColumnName)
                    {
                        PinnedColumns.Add(new KeyValuePair<string, bool>(cpea.ColumnName, IsLastSortedColumnAscending));
                    }
                }
            }
        }
    }
}