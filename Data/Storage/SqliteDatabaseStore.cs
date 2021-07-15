using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace StorageDatabaseNameSpace
{
    public partial class SqliteDatabaseStore : StorageDatabase
    {
        /// <summary>
        /// Конструктор хранилиша данных.
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе данных.</param>
        public SqliteDatabaseStore(string connectionString) : base(connectionString) { }

        public override int UpdateDataTable(string nameTable, string queryString = null)
        {
            DataTable dt = dataSet.Tables[nameTable]?.GetChanges(DataRowState.Added);

            int count = 0;

            if (dt != null)
            {
                ColumnsAndValuesInfo columnsAndValues = new(dt);

                dt.TableName = nameTable;
                count = ExecuteInsertCommand(dt, columnsAndValues);
            }

            dt = dataSet.Tables[nameTable]?.GetChanges(DataRowState.Modified);

            if (dt != null)
            {
                ColumnsAndValuesInfo columnsAndValues = new(dt);

                dt.TableName = nameTable;
                count = ExecuteUpdateCommand(dt, columnsAndValues);
            }

            dt = dataSet.Tables[nameTable]?.GetChanges(DataRowState.Deleted);

            if (dt != null)
            {
                ColumnsAndValuesInfo columnsAndValues = new(dt);

                dt.TableName = nameTable;
                count = ExecuteDeleteCommand(dt, columnsAndValues);
            }

            dataSet.Tables[nameTable].AcceptChanges();

            return count;
        }

        private int ExecuteInsertCommand(DataTable dt, ColumnsAndValuesInfo columnsAndValues)
        {
            string commandString = GenerateInsertCommand(dt.TableName, columnsAndValues);

            using SqliteConnection connection = new(ConnectionString);

            SqliteCommand command = connection.CreateCommand();

            connection.Open();

            int count = 0;

            foreach (DataRow row in dt.Rows)
            {
                command.CommandText = string.Format(commandString, columnsAndValues.AllValues(row));

                count += command.ExecuteNonQuery();
            }

            connection.Close();

            return count;
        }

        private int ExecuteUpdateCommand(DataTable dt, ColumnsAndValuesInfo columnsAndValues)
        {
            string commandString = GenerateUpdateCommand(dt.TableName, columnsAndValues);

            using SqliteConnection connection = new(ConnectionString);

            SqliteCommand command = connection.CreateCommand();

            connection.Open();

            int count = 0;

            foreach (DataRow row in dt.Rows)
            {
                var parameters = columnsAndValues.ValuesNotPrimaryKey(row).Concat(columnsAndValues.PrimaryKeyValues(row)).ToArray();
                command.CommandText = string.Format(commandString, parameters);

                count += command.ExecuteNonQuery();
            }

            connection.Close();

            return count;
        }

        private int ExecuteDeleteCommand(DataTable dt, ColumnsAndValuesInfo columnsAndValues)
        {
            string commandString = GenerateDeleteCommand(dt.TableName, columnsAndValues);

            using SqliteConnection connection = new(ConnectionString);

            SqliteCommand command = connection.CreateCommand();

            connection.Open();

            int count = 0;

            foreach (DataRow row in dt.Rows)
            {
                command.CommandText = string.Format(commandString, columnsAndValues.PrimaryKeyValues(row));

                count += command.ExecuteNonQuery();
            }

            connection.Close();

            return count;
        }

        private string GenerateInsertCommand(string nameTable, ColumnsAndValuesInfo columnsAndValues)
        {
            StringBuilder lc = new($"INSERT INTO {nameTable}(");

            // Create sets list.

            foreach (string name in columnsAndValues.AllColumns)
            {
                lc.Append(name).Append(',');
            }

            lc.Remove(lc.Length - 1, 1);

            lc.Append("\nVALUES(");

            for (int i = 0; i != columnsAndValues.Count; i++)
            {
                lc.Append($"'{{{i}}},");
            }

            // Remove last 'AND'
            lc.Remove(lc.Length - 1, 1).Append(");\n");

            return lc.ToString();
        }

        private string GenerateUpdateCommand(string nameTable, ColumnsAndValuesInfo columnsAndValues)
        {
            StringBuilder lc = new($"UPDATE {nameTable} SET\n");

            // Create sets list.

            string[] columns = columnsAndValues.ColumnsNotPrimaryKey;

            int indexValue;
            for (indexValue = 0; indexValue != columnsAndValues.ColumnsNotPrimaryKey.Length; indexValue++)
            {
                lc.Append(columns[indexValue]).Append($"='{{{indexValue}}}',\n");
            }

            lc.Remove(lc.Length - 2, 1);

            lc.Append("WHERE ");

            foreach (string column in columnsAndValues.PrimaryKeyColumns)
            {
                lc.Append(column).Append($"='{{{indexValue++}}}'").Append("\n AND ");
            }

            // Remove last 'AND'
            lc.Remove(lc.Length - 6, 6).Append(";\n");

            return lc.ToString();
        }

        private string GenerateDeleteCommand(string nameTable, ColumnsAndValuesInfo columnsAndValues)
        {
            StringBuilder lc = new($"DELETE FROM {nameTable}\nWHERE ");

            int indexValue = 0;
            foreach (string column in columnsAndValues.PrimaryKeyColumns)
            {
                lc.Append(column).Append($"='{{{indexValue++}}}'").Append("\n AND ");
            }

            // Remove last 'AND'
            lc.Remove(lc.Length - 6, 6).Append(";\n");

            return lc.ToString();
        }

        public override int LoadDataTable(string nameTable, string queryString = null)
        {
            string newQueryString = queryString;

            if (newQueryString == null)
            {
                newQueryString = $"SELECT * FROM {nameTable}";
            }

            DataTable dt;

            int count = 0;

            using (SqliteConnection connection = new(ConnectionString))
            {
                SqliteDataReader reader;
                SqliteCommand command;

                connection.Open();

                using (command = new SqliteCommand(newQueryString, connection))
                {
                    using (reader = command.ExecuteReader())
                    {
                        if (dataSet.Tables.Contains(nameTable) == true)
                        {
                            dataSet.Tables[nameTable].Clear();

                            dt = dataSet.Tables[nameTable];
                        }
                        else
                        {
                            // Create DataTable.
                            if (reader.HasRows)
                            {
                                dt = CreateDataTableFromSchema(nameTable, reader);
                            }
                            else
                            {
                                return 0;
                            }

                            dataSet.Tables.Add(dt);
                        }

                        // Loading table.
                        if (!reader.HasRows)
                        {
                            return 0;
                        }

                        count = 0;

                        while (reader.Read())
                        {
                            DataRow dr = dt.NewRow();

                            for (int i = 0; i != reader.FieldCount; i++)
                            {
                                if (reader[i].GetType() != dt.Columns[i].DataType)
                                {
                                    Debug.Write($" {i}={dt.Columns[i].DataType}=<{reader[i].GetType()}>{reader[i]}||");

                                    Type type = dt.Columns[i].DataType;

                                    if (type == typeof(long))
                                    {
                                        dr[i] = 0L;
                                    }
                                    else
                                    if (type == typeof(DateTime))
                                    {
                                        dr[i] = DateTime.MinValue;
                                    }
                                    else
                                    if (type == typeof(decimal))
                                    {
                                        dr[i] = 0m;
                                    }
                                    else
                                    if (type == typeof(bool))
                                    {
                                        dr[i] = false;
                                    }
                                    else
                                    if (type == typeof(string))
                                    {
                                        dr[i] = "";
                                    }
                                    else
                                    {
                                        dr[i] = null;
                                    }
                                }
                                else
                                {
                                    dr[i] = reader[i];
                                }
                            }

                            dt.Rows.Add(dr);

                            count++;
                        }
                    }
                }

                connection.Close();
            }

            dt.AcceptChanges();

            return count;
        }

        /// <summary>
        /// Create data table from database table schema.
        /// </summary>
        /// <param name="nameTable">Database table name</param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static DataTable CreateDataTableFromSchema(string nameTable, SqliteDataReader reader)
        {
            DataTable dt = new DataTable(nameTable);
            var ds = reader.GetSchemaTable();

            var rows = ds.Rows;

            List<DataColumn> primaryKeys = new();

            for (int i = 0; i != reader.FieldCount; i++)
            {
                DataColumn dc = new DataColumn()
                {
                    ColumnName = reader.GetName(i),
                    DataType = reader.GetFieldType(i),
                    AllowDBNull = (bool)rows[i]["AllowDBNull"],
                    Unique = (bool)rows[i]["IsUnique"],
                };

                if ((bool)rows[i]["IsKey"])
                {
                    primaryKeys.Add(dc);
                }

                dt.Columns.Add(dc);
            }

            dt.PrimaryKey = primaryKeys.ToArray();

            return dt;
        }
    }
}
