using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.InteropServices;
using System.Web.UI.HtmlControls;
using System.IO;
using OfficeOpenXml;
using Trackr.Utils;
using Trackr.Source.Controls.TGridView;

namespace Trackr.Source.Controls
{
    public partial class TrackrGridView<T> : GridView
    {
        public bool DisplayExcelExportButton { get; set; }

        public byte[] ExportToExcel()
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                // Need to make page size max length so all rows get returned. Store the previously set data in local memory so we can reset after we're done.
                int pageIndex = PageIndex;
                int pageSize = PageSize;

                // Now set to required values to show all rows, and bind data
                PageIndex = 0;
                PageSize = GridViewItems.Data.Count;
                DataSource = GridViewItems.Data;
                DataBind();

                // Create an excel workbook instance and open it from the predefined location
                string tmpFile = System.IO.Path.GetTempFileName();
                ExcelWorksheet excelWorkSheet = package.Workbook.Worksheets.Add("Sheet1");

                // We store the controls that implement our interface. If a control (e.g. plain old BoundField) is passed in, then we gracefully ignore since it is unsupported.
                IDataControlField[] controlFields = new IDataControlField[Columns.Count];

                // Loop over all the columns in the GridView (both supported and unsupported fields)
                for (int i = 0; i < Columns.Count; i++)
                {
                    // Null if unsupported, object if supported
                    IDataControlField trackrField = Columns[i] as IDataControlField;

                    // if the Column does not implement the interface, or it is hidden in the export, then we will make sure it is null in the controlFields array
                    if (trackrField != null && !trackrField.HiddenInExport)
                    {
                        controlFields[i] = trackrField;
                    }
                }

                // Let's grab a copy of all the supported fields.
                List<IDataControlField> controlFieldsNotNull = controlFields.Where(i => i != null).OrderBy(i => Array.IndexOf(controlFields, i)).ToList();

                // This will store the max length for each column so we can make viewing the spreadsheet easier.
                int[] maxCharLengthForRange = new int[controlFieldsNotNull.Count];

                // Now add the headers
                for (int i = 1; i < controlFieldsNotNull.Count + 1; i++)
                {
                    excelWorkSheet.Cells[1, i].Value = ((DataControlField)controlFieldsNotNull[i - 1]).HeaderText;
                    excelWorkSheet.Cells[1, i].Style.Font.Bold = true;

                    // If the header text is longer than the current max length, set the max length to that of header text
                    if (((DataControlField)controlFieldsNotNull[i - 1]).HeaderText.Length > maxCharLengthForRange[i - 1])
                    {
                        maxCharLengthForRange[i - 1] = ((DataControlField)controlFieldsNotNull[i - 1]).HeaderText.Length;
                    }
                }                

                // Iterate over each data item.
                for (int i = 0; i < Rows.Count; i++)
                {
                    foreach (IDataControlField trackrField in controlFieldsNotNull)
                    {
                        int j = controlFieldsNotNull.IndexOf(trackrField); // This is the column index we want to write out to the document
                        int k = Array.IndexOf(controlFields, trackrField); // this is the column index in the Columns collection (it is not what we want to display out)

                        // Some fields have controls instead of plain old text. So we need to append known controls as text to output.
                        // If a control is used that isn't listed below, add within the if/elseif
                        List<string> allInnerContent = new List<string>();
                        foreach (Control innerControl in Rows[i].Cells[k].Controls)
                        {
                            if (innerControl as DataBoundLiteralControl != null)
                            {
                                allInnerContent.Add(HttpUtility.HtmlDecode(((DataBoundLiteralControl)innerControl).Text));
                            }
                            else if (innerControl as HtmlContainerControl != null)
                            {
                                allInnerContent.Add(HttpUtility.HtmlDecode(((HtmlContainerControl)innerControl).InnerText));
                            }
                            else if (innerControl as LiteralControl != null)
                            {
                                allInnerContent.Add(HttpUtility.HtmlDecode(((LiteralControl)innerControl).Text));
                            }
                        }

                        // By default, use the bounded cell's text. Template Fields will have empty text, Bound Fields will have text. If the text is longer than the current max length for this column, reset.
                        string innerContent = HttpUtility.HtmlDecode(Rows[i].Cells[k].Text);
                        if (Rows[i].Cells[k].Text.Length > maxCharLengthForRange[j]) { maxCharLengthForRange[j] = Rows[i].Cells[k].Text.Length; }

                        // ForEach inner control found earlier, let's convert the html content to plain text and seperate with a line break (\r\n)
                        foreach (string t in allInnerContent)
                        {
                            string trimmedAndStripped = string.Format("{0}{1}", allInnerContent.IsFirst(t) ? "" : "\r\n", Trackr.Source.Controls.TGridView.Common.ConvertToPlainText(t).Trim());
                            if (trimmedAndStripped.Length > maxCharLengthForRange[j]) { maxCharLengthForRange[j] = trimmedAndStripped.Length; } // If the text is longer than the current max length for this column, reset.
                            innerContent += trimmedAndStripped;
                        }

                        // Set spreadsheet cell text to the inner content.
                        excelWorkSheet.Cells[i + 2, j + 1].Value = innerContent.Replace("&nbsp;", " ").Trim();
                    }
                }

                // Make column widths appropriate size according to which cell has the widest width.
                for (int j = 0; j < maxCharLengthForRange.Length; j++)
                {
                    excelWorkSheet.Column(j + 1).Style.WrapText = true;
                    excelWorkSheet.Column(j + 1).Width = maxCharLengthForRange[j] + 2;
                    //excelWorkSheet.Column(j+.Cells[1, j + 1].EntireColumn.VerticalAlignment = 1; // make cells use top vertical alignment to remove top padding
                }

                //51 = xlOpenXMLWorkbook (without macro's in 2007-2013, xlsx)
                FileInfo fInfo = new FileInfo(tmpFile);
                package.SaveAs(fInfo);
                package.Dispose();

                // Covnert newly created excel doc to byte array to return for stream.
                byte[] data = File.ReadAllBytes(tmpFile);

                // data is in byte array struct, no longer need temp file. We can now delete.
                File.Delete(tmpFile);

                // Export is completed and we have bytes. Now reset GridView so it is presented appropriately in the website.
                PageIndex = pageIndex;
                PageSize = pageSize;
                DataSource = GridViewItems.Data;
                DataBind();

                return data;
            }
        }



        protected void ExportToExcel_Click(object sender, EventArgs e)
        {
            // Make sure an event handler was passed in for when the export is completed.
            if (ExportCompleted == null)
            {
                return;
            }

            LinkButton btn = (LinkButton)sender;
            
            // Update link text as needed...
            System.Web.UI.WebControls.Label text = btn.Controls[1] as System.Web.UI.WebControls.Label;

            string labelText = "";

            if (text != null)
            {
                // Create a copy of the old text so we can change back when done.
                labelText = text.Text;
                text.Text = "Exporting ...";
            }

            // Do export
            byte[] data = ExportToExcel();

            // Send event completed to be handled by pages
            ExportCompleted(sender, new ExportCompletedEventArgs(data, data.Length, "application/vnd.xls", "application/Excel"));

            // Update link text back as needed...
            if (text != null)
            {
                text.Text = labelText;
            }
        }
    }
}