using System.Data;

namespace WV.SQLite
{
    public class Table
    {
        public string Name { get; }
        private string[] Columns { get; }
        private object[] Rows { get; }

        public int ColumnsCount => this.Columns.Length;
        public int RowsCount => this.Rows.Length;


        public Table(DataTable dataTable) 
        {
            this.Name = dataTable.TableName;

            var columns = dataTable.Columns;
            var rows = dataTable.Rows;

            this.Columns = new string[columns.Count];
            this.Rows = new object[rows.Count];

            for (int i = 0; i < columns.Count; i++)
                this.Columns[i] = columns[i].Caption;

            for (int i = 0; i < rows.Count; i++)
            {
                DataRow row = rows[i];
                object[] values = new object[columns.Count];

                for (int j = 0; j < columns.Count; j++)
                    values[j] = row[j];
                
                this.Rows[i] = values;
            }

            dataTable.Clear();
            dataTable.Dispose();
        }

        public string[] GetColumns()
        {
            return this.Columns;
        }

        public string? GetColumn(int index)
        {
            if (index < 0 || index >= this.Columns.Length)
                return null;

            return this.Columns[index];
        }

        public object? GetRow(int index)
        {
            if (index < 0 || index >= this.Rows.Length)
                return null;

            return this.Rows[index];
        }

    }
}