using System.Data.SQLite;
using System.Data;
using WV;
using WV.WebView;

namespace SQLite
{
    public class SQLite : Plugin, IPlugin
    {
        private const string PREFIX = "@";

        private static readonly Type JSObjectType = typeof(WV.JavaScript.Object);
        private static readonly Type ArrayObjectType = typeof(object[]);

        public static string JScript => Resources.SQLiteScript;

        #region PRIVATE PROPERTIES

        private SQLiteConnection InnerConexion { get; set; }
        private SQLiteTransaction InnerTransaction { get; set; }

        #endregion

        #region PUBLIC PROPERTIES

        public string Engine => "SQLite";
        public bool IsOpen { get; private set; }
        public bool IsTransaction { get; private set; }

        private string _ConnectionString;
        private string ConnectionString
        {
            get => _ConnectionString;
            set
            {
                if (IsOpen)
                    return;

                _ConnectionString = value;
            }
        }

        private int _Timeout = 30;  // 30 segundos por defecto
        public int Timeout
        {
            get => _Timeout;
            set
            {
                if (value <= 0)
                    _Timeout = 0;
                else
                    _Timeout = value;
            }
        }

        public DateTime? SysDate
        {
            get
            {
                if (IsOpen)
                    return DateTime.Now;

                return null;
            }
        }

        #endregion

        #region CONSTRUCTORS

        public SQLite(IWebView webView) : base(webView)
        {
        }

        public SQLite(IWebView webView, string ConnectionString) : this(webView)
        {
            this.ConnectionString = ConnectionString;
        }

        #endregion

        #region TRANSACTION

        public void BeginTransaction()
        {
            try
            {
                ThrowIsNotOpen();
                ThrowIsTransaction();

                this.InnerTransaction = InnerConexion.BeginTransaction();
                IsTransaction = true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CommitTransaction()
        {
            try
            {
                ThrowIsNotOpen();
                ThrowIsNotTransaction();

                this.InnerTransaction.Commit();
                this.InnerTransaction.Dispose();
                this.InnerTransaction = null;
                this.IsTransaction = false;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public void RollbackTransaction()
        {
            try
            {
                ThrowIsNotOpen();
                ThrowIsNotTransaction();

                this.InnerTransaction.Rollback();
                this.InnerTransaction.Dispose();
                this.InnerTransaction = null;
                this.IsTransaction = false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        public void Open()
        {
            try
            {
                ThrowIsOpen();

                this.InnerConexion = new SQLiteConnection(this.ConnectionString);
                this.InnerConexion.Open();
                this.IsOpen = true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Close()
        {
            try
            {
                ThrowIsNotOpen();

                if (this.IsTransaction)
                    RollbackTransaction();

                this.InnerConexion.Close();
                this.InnerConexion.Dispose();

                //Fuerza a liberar el archivo
                SQLiteConnection.ClearPool(this.InnerConexion);

                this.InnerConexion = null;
                this.IsOpen = false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region PUBLIC METHODS

        public int ExecuteNonQuery(string query, object? parameters = null)
        {
            try
            {
                using var command = GetCommand(query, parameters);
                return command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //===================================//

        public object ExecuteEscalar(string query, object? parameters = null)
        {
            try
            {
                using var command = GetCommand(query, parameters);
                return command.ExecuteScalar();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //===================================//

        public Table ExecuteTable(string query, object? parameters = null)
        {
            try
            {
                using var command = GetCommand(query, parameters);
                using var result = command.ExecuteReader();

                var datatable = new DataTable();
                datatable.Load(result);

                return new Table(datatable);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //===================================//

        public Reader ExecuteReader(string query, object? parameters = null)
        {
            try
            {
                using var command = GetCommand(query, parameters);
                return new Reader(command.ExecuteReader());
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion

        #region PRIVATE METHODS

        private SQLiteCommand GetCommand(string query, object? parameters)
        {
            var command = AUX_GetCommand(query);

            if (parameters != null)
                if (JSObjectType.IsAssignableFrom(parameters.GetType()))
                    SetParameters(command, (WV.JavaScript.Object)parameters);
                else if (ArrayObjectType.IsAssignableFrom(parameters.GetType()))
                    SetParameters(command, (object[])parameters);

            return command;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.Disposed)
                return;

            if (disposing)
            {

            }

            try
            {
                this.Close();
            }
            catch (Exception) { }

            this.IsOpen = false;
            this.IsTransaction = false;
            this.ConnectionString = null;
            this.Timeout = 30;
        }

        private static void SetParameters(SQLiteCommand command, WV.JavaScript.Object parameters)
        {
            foreach (var param in parameters.CSValue)
                command.Parameters.AddWithValue(PREFIX + param.Key, param.Value ?? Convert.DBNull);
        }

        private static void SetParameters(SQLiteCommand command, object[] parameters)
        {
            if (parameters == null)
                return;

            foreach (var item in parameters)
                command.Parameters.Add(new SQLiteParameter(DbType.Object, item ?? Convert.DBNull));

        }

        private void ThrowIsNotOpen()
        {
            if (this.IsOpen)
                return;

            throw new Exception("Connection is no open");
        }

        private void ThrowIsOpen()
        {
            if (this.IsOpen)
                throw new Exception("Connection is already open");
        }

        private void ThrowIsNotTransaction()
        {
            if (this.IsTransaction)
                return;

            throw new Exception("Transaction not started");
        }

        private void ThrowIsTransaction()
        {
            if (this.IsTransaction)
                throw new Exception("Transaction has already been started");
        }


        #endregion

        #region HELPERS

        private SQLiteCommand AUX_GetCommand(string query)
        {
            ThrowIsNotOpen();

            return new SQLiteCommand
            {
                Connection = this.InnerConexion,
                Transaction = this.InnerTransaction,
                CommandText = query,
                CommandType = CommandType.Text,
                CommandTimeout = this.Timeout
            };
        }

        #endregion

    }
}