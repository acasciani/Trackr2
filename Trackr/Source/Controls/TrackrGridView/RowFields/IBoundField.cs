using System;
using System.Web.UI.WebControls;
using Trackr.Source.Controls.TGridView;

namespace Trackr.Source.Controls
{
    public class TrackrBoundField : BoundField, IDataControlField
    {
        public bool IsPinned { get; set; }
        public bool IsPinnable { get; set; }
        public bool AllowPinnable { get; set; }
        public bool HiddenInExport { get; set; }
        public event EventHandler ColumnPinned;

        public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            base.InitializeCell(cell, cellType, rowState, rowIndex);

            // We need to have a header that has a sort expression.
            if (cellType == DataControlCellType.Header && !string.IsNullOrWhiteSpace(this.SortExpression))
            {
                cell.Controls.Add(Common.CreatePinningButton(IsPinned, IsPinnable, this.SortExpression, Toggle_Click));
            }
        }

        protected void Toggle_Click(object sender, EventArgs e)
        {
            LinkButton toggleBtn = (LinkButton)sender;
            toggleBtn.ToggleLinkButtonDisplay(IsPinned);
            ColumnPinned(sender, new ColumnPinningEventArgs(this.SortExpression));
        }
    }
}