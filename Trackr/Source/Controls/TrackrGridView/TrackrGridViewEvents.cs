using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trackr.Source.Controls.TGridView
{
    public class ColumnPinningEventArgs : EventArgs
    {
        public string ColumnName { get; private set; }

        public ColumnPinningEventArgs(string columnName)
        {
            ColumnName = columnName;
        }
    }

    public class ExportCompletedEventArgs : EventArgs
    {
        public byte[] FileData { get; private set; }
        public int FileSize { get; private set; }
        public string ContentType { get; private set; }
        public string Mime { get; private set; }

        public ExportCompletedEventArgs(byte[] data, int fileSize, string contentType, string mime)
        {
            FileData = data;
            FileSize = fileSize;
            ContentType = contentType;
            Mime = mime;
        }
    }
}