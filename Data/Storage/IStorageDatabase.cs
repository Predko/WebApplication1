using System.Data;
using System.Threading.Tasks;

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
        /// Возвращает таблицу с именем name, включающей данные запроса queryString.
        /// </summary>
        /// <param name="name">Имя таблицы.</param>
        /// <param name="queryString">Строка запроса.</param>
        /// <returns>Таблица DataTable.</returns>
        DataTable this[string name, string queryString] { get; }

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
        Task<int> UpdateDataTableAsync(string nameTable, string queryString = null);

        /// <summary>
        /// Обновляет базу данных из соответствующей таблицы данных.
        /// </summary>
        /// <param name="nameTable">Имя таблицы данных.</param>
        /// <param name="queryString">Строка запроса для загрузки базы данных или её части.</param>
        int UpdateDataTable(string nameTable, string queryString = null);

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
        public Task<int> LoadDataTableAsync(string nameTable, string queryString = null);

        public int DeleteRecords(DataTable dt);

    }
}