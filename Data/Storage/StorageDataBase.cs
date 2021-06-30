using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace StorageDatabaseNameSpace
{
    public class StorageDatabase : IStorageDatabase
    {
        /// <summary>
        /// Таблицы данных.
        /// </summary>
        private readonly DataSet dataSet;

        private string connectionString;
        
        public string ConnectionString { get => connectionString; set => connectionString = value; }

        /// <summary>
        /// Конструктор хранилиша данных.
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе данных.</param>
        public StorageDatabase(string connectionString)
        {
            ConnectionString = connectionString;

            dataSet = new DataSet();
        }

        public void Add(DataTable dt) => dataSet.Tables.Add(dt);

        public DataTable this[string name]
        {
            get
            {
                if (dataSet.Tables.Contains(name) == false)
                {
                    LoadDataTable(name);
                }
                return dataSet.Tables[name];
            }
        }
        
        public void DataBaseUpdate(string nameTable, string queryString = null)
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

        public int AsynchronousDataBaseUpdate(string nameTable, string queryString = null)
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

        public int LoadDataTable(string nameTable, string queryString = null)
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

        public int AsynchronousLoadDataTable(string nameTable, string queryString = null)
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

        public void AcceptChanges(string nameTable) => this[nameTable].AcceptChanges();
    }
}
