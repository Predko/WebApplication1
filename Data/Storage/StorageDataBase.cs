using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace StorageDatabaseNameSpace
{
    public abstract class StorageDatabase : IStorageDatabase
    {
        /// <summary>
        /// Таблицы данных.
        /// </summary>
        protected readonly DataSet dataSet;

        protected string connectionString;

        protected string SelectAllQueryString = "SELECT * FROM";

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

        public virtual void Add(DataTable dt) => dataSet.Tables.Add(dt);

        public virtual DataTable this[string name]
        {
            get
            {
                string selectAll = SelectAllQueryString + $" {name}";

                LoadDataTable(name, selectAll);

                return dataSet.Tables[name];
            }
        }

        public virtual DataTable this[string name, string queryString]
        {
            get
            {
                LoadDataTable(name, queryString);

                return dataSet.Tables[name];
            }
        }

        public abstract int UpdateDataTable(string nameTable, string queryString = null);

        public virtual async Task<int> UpdateDataTableAsync(string nameTable, string queryString = null) =>
                                    await Task.Run(() => UpdateDataTable(nameTable, queryString));

        public abstract int LoadDataTable(string nameTable, string queryString = null);

        public virtual async Task<int> LoadDataTableAsync(string nameTable, string queryString = null) =>
                                    await Task.Run(() => LoadDataTable(nameTable, queryString));

        public virtual void AcceptChanges(string nameTable) => this[nameTable].AcceptChanges();

        public abstract int DeleteRecords(DataTable dt);
    }
}
