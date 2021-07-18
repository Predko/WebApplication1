using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace StorageDatabaseNameSpace
{
    public partial class SqliteDatabaseStore
    {
        private class ColumnsAndValuesInfo
        {
            public int[] IndexesPrimaryKey { get; private set; }

            public List<string> ColumnsNames = new();

            // Columns.
            public string[] AllColumnsExceptId { get; private set; }

            public string[] ColumnsNotPrimaryKey { get; private set; }

            public string[] PrimaryKeyColumns { get; private set; }

            public int Count { get; private set; }

            public ColumnsAndValuesInfo(DataTable dt)
            {
                IndexesPrimaryKey = (from col in dt.PrimaryKey select col.Ordinal).ToArray();

                foreach(DataColumn col in dt.Columns)
                {
                    ColumnsNames.Add(col.ColumnName);
                }

                Count = dt.Columns.Count;

                AllColumnsExceptId = ColumnsNames.ToArray();

                ColumnsNotPrimaryKey = ColumnsNames.Where((o, i) => !IndexesPrimaryKey.Contains(i)).Select(c => c).ToArray();
                
                PrimaryKeyColumns = ColumnsNames.Where((o, i) => IndexesPrimaryKey.Contains(i)).Select(c => c).ToArray();
            }

            // Values.
            public string[] AllValues(DataRow row) => row.ItemArray.Select(v => v.ToString()).ToArray();

            public string[] ValuesNotPrimaryKey(DataRow row) => row.ItemArray.Where((o, i) => !IndexesPrimaryKey.Contains(i))
                                                                             .Select(v => v.ToString()).ToArray();

            public string[] AllValuesExceptId(DataRow row) => row.ItemArray.Where((o,i) => ColumnsNames[i].ToLower() != "id")
                                                                           .Select(v => v.ToString()).ToArray();

            public string[] PrimaryKeyValues(DataRow row) => row.ItemArray.Where((o, i) => IndexesPrimaryKey.Contains(i))
                                                                    .Select(v => v.ToString()).ToArray();
            
        }
    }
}
