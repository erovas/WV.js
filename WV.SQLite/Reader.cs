using System.Data;
using System.Data.SQLite;

namespace WV.SQLite
{
    public class Reader
    {
        private SQLiteDataReader InnerDataReader { get; }

        #region PROPERTIES

        public bool HasRows => this.InnerDataReader.HasRows;

        public int RecordsAffected => this.InnerDataReader.RecordsAffected;

        public bool IsClosed => this.InnerDataReader.IsClosed;

        public int StepCount => this.InnerDataReader.StepCount;

        public int FieldCount => this.InnerDataReader.FieldCount;

        #endregion

        public Reader(SQLiteDataReader dataReader) 
        {
            this.InnerDataReader = dataReader;
        }

        public object? this[string name]
        {
            get
            {
                object value = this.InnerDataReader[name];
                return Convert.IsDBNull(value)? null : value;
            }
        }

        public object? this[int i]
        {
            get 
            {
                object value = this.InnerDataReader[i];
                return Convert.IsDBNull(value)? null : value; 
            }
        }

        public bool Read() 
        {
            return this.InnerDataReader.Read();
        }

        public bool NextResult()
        {
            return this.InnerDataReader.NextResult();
        }

        public void Close()
        {
            this.InnerDataReader.Close();
        }

        public Table? GetTable()
        {
            try
            {
                var datatable = new DataTable();
                datatable.Load(this.InnerDataReader);
                return new Table(datatable);
            }
            catch (Exception) { }

            return null;
        }
    }
}
