using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace StorageDatabaseNameSpace
{
    public class SqlStorageDatabase : StorageDatabase
    {
        /// <summary>
        /// Конструктор хранилиша данных.
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе данных.</param>
        
        public SqlStorageDatabase(string connectionString):base(connectionString) {}

        public override void DataBaseUpdate(string nameTable, string queryString = null)
        {
            if (queryString == null)
            {
                queryString = $"SELECT * FROM {nameTable}";
            }

            using SqlConnection sqlConnection = new(ConnectionString);

            SqlDataAdapter dataAdapter = new(queryString, sqlConnection);

            SqlCommandBuilder commandBuilder = new(dataAdapter);

            string s = commandBuilder.GetInsertCommand().CommandText;

            dataAdapter.Update(dataSet.Tables[nameTable]);
        }

        public override int AsynchronousDataBaseUpdate(string nameTable, string queryString = null)
        {
            Task<int> updateTask = new Task<int>(() =>
                        {
                            if (queryString == null)
                            {
                                queryString = $"SELECT * FROM {nameTable}";
                            }

                            using SqlConnection sqlConnection = new(ConnectionString);

                            SqlDataAdapter dataAdapter = new(queryString, sqlConnection);

                            SqlCommandBuilder commandBuilder = new(dataAdapter);

                            string s = commandBuilder.GetInsertCommand().CommandText;
                            return dataAdapter.Update(dataSet.Tables[nameTable]);
                        });

            updateTask.Start();

            return updateTask.Result;
        }

        public override int LoadDataTable(string nameTable, string queryString = null)
        {
            string newQueryString = queryString;

            if (newQueryString == null)
            {
                newQueryString = $"SELECT * FROM {nameTable}";
            }

            SqlConnection sqlConnection = new(ConnectionString);

            SqlDataAdapter dataAdapter = new(newQueryString, sqlConnection);

            if (dataSet.Tables.Contains(nameTable) == true)
            {
                dataSet.Tables[nameTable].Clear();
            }
            else
            {
                dataSet.Tables.Add(nameTable);
            }

            return dataAdapter.Fill(dataSet.Tables[nameTable]);
        }

        public override int AsynchronousLoadDataTable(string nameTable, string queryString = null)
        {
            Task<int> loadTask = new Task<int>(() =>
            {
                string newQueryString = queryString;

                if (newQueryString == null)
                {
                    newQueryString = $"SELECT * FROM {nameTable}";
                }

                SqlConnection sqlConnection = new(ConnectionString);

                SqlDataAdapter dataAdapter = new(newQueryString, sqlConnection);

                if (dataSet.Tables.Contains(nameTable) == true)
                {
                    dataSet.Tables[nameTable].Clear();
                }
                else
                {
                    dataSet.Tables.Add(nameTable);
                }

                return dataAdapter.Fill(dataSet.Tables[nameTable]);
            });

            loadTask.Start();

            return loadTask.Result;
        }
    }
}
