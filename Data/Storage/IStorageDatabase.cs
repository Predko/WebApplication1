using System.Data;

namespace StorageDatabaseNameSpace
{
    public interface IStorageDatabase
    {
        /// <summary>
        /// Возвращает таблицу с именем name.
        /// </summary>
        /// <param name="name">Имя таблицы.</param>
        /// <returns>Таблица DataTable.</returns>
        DataTable this[string name] { get; }

        /// <summary>
        /// Строка подключения к данным.
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Фиксирует изменения в указанной таблице данных.
        /// </summary>
        /// <param name="nameTable"></param>
        void AcceptChanges(string nameTable);
        
        /// <summary>
        /// Добавляет таблицу в хранилище данных.
        /// </summary>
        /// <param name="dt">Таблица данных.</param>
        void Add(DataTable dt);

        /// <summary>
        /// Асинхронно обновляет базу данных из соответствующей таблицы данных.
        /// </summary>
        /// <param name="nameTable">Имя таблицы данных.</param>
        /// <param name="queryString">Строка запроса для загрузки базы данных или её части.</param>
        int AsynchronousDataBaseUpdate(string nameTable, string queryString = null);

        /// <summary>
        /// Обновляет базу данных из соответствующей таблицы данных.
        /// </summary>
        /// <param name="nameTable">Имя таблицы данных.</param>
        /// <param name="queryString">Строка запроса для загрузки базы данных или её части.</param>
        void DataBaseUpdate(string nameTable, string queryString = null);

        /// <summary>
        /// Загружает указанную таблицу из базы данных.
        /// </summary>
        /// <param name="nameTable">Имя таблицы.</param>
        /// <param name="queryString">Строка sql запроса данных таблицы. 
        /// Если null - будет загружена вся таблица.
        /// </param>
        /// <returns>Количество записей.</returns>
        int LoadDataTable(string nameTable, string queryString = null);

        /// <summary>
        /// Загружает указанную таблицу из базы данных асинхронно.
        /// </summary>
        /// <param name="nameTable"></param>
        /// <param name="queryString"></param>
        /// <returns>Количество записей.</returns>
        public int AsynchronousLoadDataTable(string nameTable, string queryString = null);

    }
}