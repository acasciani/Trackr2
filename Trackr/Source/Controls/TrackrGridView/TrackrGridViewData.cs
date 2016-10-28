using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trackr.Source.Controls.TGridView
{
    public class GridViewData<T>
    {
        /// <summary>The data that populates the grid view. This should be set using the AddData method.</summary>
        public List<T> Data { get; private set; }

        private List<T> OriginalData { get; set; }

        public List<T> ManipulateOriginal(Func<T, bool> filter)
        {
            if (OriginalData == null)
            {
                OriginalData = Data;
            }

            Data = OriginalData.Where(i => filter(i)).ToList();
            return Data;
        }

        public List<T> RestoreOriginal()
        {
            Data = OriginalData;
            OriginalData = null;
            return Data;
        }

        /// <summary>The Type that is in the Data list. This is typically a ViewModel type.</summary>
        public Type DataType { get; private set; }

        /// <param name="viewModelType">The type that is in the list.</param>
        public GridViewData(Type viewModelType)
        {
            Data = new List<T>();
            DataType = viewModelType;
        }

        /// <summary>Add the data from the list into this struct.</summary>
        /// <typeparam name="T">The ViewModel class type</typeparam>
        /// <param name="data">The actual data that should populate the GV</param>
        public void AddData(List<T> data)
        {
            data.ForEach(i => Data.Add(i));
        }

        /// <summary>Empties out all the entries in Data.</summary>
        public void ResetData()
        {
            Data.Clear();
        }
    }
}