using DummyDB.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardLab5
{
    public class ColumnAdder
    {
        public static void AddColumnInTable(Column column, Table selectedTable)
        {
            foreach (var row in selectedTable.Rows)
            {
                switch (column.Type)
                {
                    case "uint":
                        {
                            row.Data.Add(column, 0);
                            break;
                        }
                    case "int":
                        {
                            row.Data.Add(column, 0);
                            break;
                        }
                    case "double":
                        {
                            row.Data.Add(column, 0);
                            break;
                        }
                    case "datetime":
                        {
                            row.Data.Add(column, DateTime.MinValue);
                            break;
                        }
                    case "string":
                        {
                            row.Data.Add(column, "");
                            break;
                        }
                }
            }
        }

        public static string AddColumnInNewTable(List<Column> columns)
        {
            StringBuilder newFile = new StringBuilder();
            int count = 0;
            foreach (var column in columns)
            {
                count += 1;
                switch (column.Type)
                {
                    case "uint":
                        {
                            newFile.Append("0");
                            break;
                        }
                    case "int":
                        {
                            newFile.Append("0");
                            break;
                        }
                    case "double":
                        {
                            newFile.Append("0");
                            break;
                        }
                    case "datetime":
                        {
                            newFile.Append($"{DateTime.MinValue}");
                            break;
                        }
                    case "string":
                        {
                            newFile.Append($"");
                            break;
                        }
                }
                if (count != columns.Count())
                {
                    newFile.Append(";");
                }
            }
            return newFile.ToString();
        }
    }
}
