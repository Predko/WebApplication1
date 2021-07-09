using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace StorageDatabaseNameSpace
{
    public abstract class StorageDatabase : IStorageDatabase
    {
        /// <summary>
        /// Таблицы данных.
        /// </summary>
        protected readonly DataSet dataSet;

        protected string connectionString;

        protected string lastQueryString;

        public string ConnectionString { get => connectionString; set => connectionString = value; }

        /// <summary>
        /// Конструктор хранилиша данных.
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе данных.</param>
        public StorageDatabase(string connectionString)
        {
            ConnectionString = connectionString;

            lastQueryString = null;

            dataSet = new DataSet();
        }

        public virtual void Add(DataTable dt) => dataSet.Tables.Add(dt);

        public virtual DataTable this[string name]
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

        public virtual DataTable this[string name, string queryString]
        {
            get
            {
                if (dataSet.Tables.Contains(name) == false || queryString != lastQueryString)
                {
                    AsynchronousLoadDataTable(name);
                }

                return dataSet.Tables[name];
            }
        }

        public abstract void DataBaseUpdate(string nameTable, string queryString = null);

        public virtual int AsynchronousDataBaseUpdate(string nameTable, string queryString = null)
        {
            Task updateTask = new Task(() =>
            {
                DataBaseUpdate(nameTable, queryString);
            });

            updateTask.Start();

            return 0;
        }

        public abstract int LoadDataTable(string nameTable, string queryString = null);

        public virtual int AsynchronousLoadDataTable(string nameTable, string queryString = null)
        {
            Task<int> loadTask = new Task<int>(() =>
            {
                return LoadDataTable(nameTable, queryString);
            });

            loadTask.Start();

            return loadTask.Result;
        }

        public virtual void AcceptChanges(string nameTable) => this[nameTable].AcceptChanges();
    }
}
