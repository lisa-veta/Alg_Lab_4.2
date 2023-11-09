using DummyDB.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardLab5
{
    public  class ElementRemover
    {

        public static void DeleteTableColumn(TableScheme selectedScheme, Table selectedTable, string selectedColumnDelete)
        {
            foreach (Column column in selectedScheme.Columns)
            {
                if (column.Name == selectedColumnDelete)
                {
                    selectedScheme.Columns.Remove(column);
                    foreach (var row in selectedTable.Rows)
                    {
                        row.Data.Remove(column);
                    }
                    break;
                }
            }
        }

        public static Table DeleteTableRow(int rowNumber, Table selectedTable)
        {
            int count = 0;
            foreach (Row row in selectedTable.Rows)
            {
                if (count == rowNumber)
                {
                    selectedTable.Rows.Remove(row);
                    break;
                }
                count += 1;
            }
            return selectedTable;
        }

        public static void RemoveColumnName(TableScheme selectedScheme, string selectedColumn, string newColumnName)
        {
            foreach (Column column in selectedScheme.Columns)
            {
                if (column.Name == selectedColumn)
                {
                    column.Name = newColumnName;
                }
            }
        }


    }
}
