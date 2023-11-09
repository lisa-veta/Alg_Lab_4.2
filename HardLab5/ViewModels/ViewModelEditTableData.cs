using DummyDB.Core;
using HardLab5.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace HardLab5
{
    public class ViewModelEditTableData : BaseViewModel
    {
        public string folderPath;
        public System.Windows.Controls.DataGrid DataGrid { get; set; }
        public TableScheme selectedScheme;
        public Table selectedTable;
        private DataTable _dataNewTable;
        public DataTable DataNewTable
        {
            get { return _dataNewTable; }
            set
            {
                _dataNewTable = value;
                OnPropertyChanged();
            }
        }

        private string _tableName;
        public string TableName
        {
            get { return _tableName; }
            set
            {
                _tableName = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddRow => new DelegateCommand(param =>
        {
            DataRow newRow = DataNewTable.NewRow();
            DataNewTable.Rows.Add(newRow);
        });

        public ICommand DeleteRow => new DelegateCommand(param =>
        {
            if (DataGrid.SelectedItem == null)
            {
                MessageBox.Show("Строка для удаления не была выделена");
                return;
            }
            RemoveRow();
        });

        public ICommand EditRows => new DelegateCommand(param =>
        {
            if (EditTableData())
            {
                return;
            }
            UpdateTable();
            FileRewriter.RewriteCSV(folderPath, selectedTable, selectedScheme);
        });

        private void RemoveRow()
        {
            for (int i = 0; i < DataGrid.Columns.Count; i++)
            {
                if (DataGrid.Items[i] == DataGrid.SelectedItem)
                {
                    DialogResult dialogResult = MessageBox.Show($"Вы уверены, что хотите безвозвратно удалить {i + 1} строку?", "Подтверждение действий", MessageBoxButtons.YesNo); ;
                    if (dialogResult == DialogResult.Yes)
                    {
                        selectedTable = ElementRemover.DeleteTableRow(i, selectedTable);
                        FileRewriter.RewriteCSV(folderPath, selectedTable, selectedScheme);
                        UpdateTable();
                        return;
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        return;
                    }
                }
            }
        }

        private void UpdateTable()
        {
            DataNewTable.Clear();
            DataTable dataTable = new DataTable();
            foreach (Column column in selectedScheme.Columns)
            {
                dataTable.Columns.Add(column.Name);
            }

            for (int i = 0; i < selectedTable.Rows.Count; i++)
            {
                DataRow newRow = dataTable.NewRow();
                foreach (var rows in selectedTable.Rows[i].Data)
                {
                    newRow[rows.Key.Name] = rows.Value;
                }
                dataTable.Rows.Add(newRow);
            }
            DataNewTable = dataTable;
            FileRewriter.RewriteJson(folderPath, selectedScheme);
        }

        private bool EditTableData()
        {
            int count = selectedTable.Rows.Count - 1;
            for (int i = 0; i < DataNewTable.Rows.Count; i++)
            {
                for (int j = 0; j < selectedScheme.Columns.Count; j++)
                {
                    if (i >= selectedTable.Rows.Count)
                    {
                        selectedTable.Rows.Add(new Row() { Data = new Dictionary<Column, object>() });
                    }
                    object data = CheckCorrectData(i, j);
                    if (data == null && i < count)
                    {
                        DataNewTable.Rows[i][selectedScheme.Columns[j].Name] = selectedTable.Rows[i].Data[selectedScheme.Columns[j]];
                        return true;
                    }
                    else if (data == null) { return true; }
                    selectedTable.Rows[i].Data[selectedScheme.Columns[j]] = data;
                    DataNewTable.Rows[i][selectedScheme.Columns[j].Name] = data;
                }
            }
            return false;
        }

        private object CheckCorrectData(int numOfRow, int numOfColumn)
        {
            string data = DataNewTable.Rows[numOfRow][selectedScheme.Columns[numOfColumn].Name].ToString();
            switch (selectedScheme.Columns[numOfColumn].Type)
            {
                case "uint":
                    {
                        return TryUint(data, numOfRow + 1, numOfColumn + 1);
                    }
                case "int":
                    {
                        return TryInt(data, numOfRow + 1, numOfColumn + 1);
                    }
                case "double":
                    {
                        return TryDouble(data, numOfRow + 1, numOfColumn + 1);
                    }
                case "datetime":
                    {
                        return TryDateTime(data, numOfRow + 1, numOfColumn + 1);
                    }
                case "string":
                    {
                        return data;
                    }
            }
            return null;
        }

        private object TryUint(string data, int numOfRow, int numOfColumn)
        {
            if (uint.TryParse(data, out uint number))
            {
                return number;
            }
            else if (ShowMessage(numOfRow + 1, numOfColumn + 1, "uint"))
            {
                return 0;
            }
            return null;
        }
        private object TryInt(string data, int numOfRow, int numOfColumn)
        {
            if (int.TryParse(data, out int number))
            {
                return number;
            }
            else if (ShowMessage(numOfRow + 1, numOfColumn + 1, "int"))
            {
                return 0;
            }
            return null;
        }
        private object TryDouble(string data, int numOfRow, int numOfColumn)
        {
            if (double.TryParse(data, out double number))
            {
                return number;
            }
            else if (ShowMessage(numOfRow + 1, numOfColumn + 1, "double"))
            {
                return 0;
            }
            return null;
        }

        private object TryDateTime(string data, int numOfRow, int numOfColumn)
        {
            if (DateTime.TryParse(data, out DateTime number))
            {
                return number;
            }
            else if (ShowMessage(numOfRow + 1, numOfColumn + 1, "DateTime"))
            {
                return DateTime.MinValue;
            }
            return null;
        }

        private bool ShowMessage(int numOfRow, int numOfColumn, string type)
        {
            DialogResult dialogResult = MessageBox.Show($"В строке {numOfRow} столбце {numOfColumn} введены некорректные данные или найдены пустые ячейки, они будут заполнены значениями по умолчанию \n(Ячейка должна быть типа {type})", "Подтверждение действий", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                return true;
            }
            else if (dialogResult == DialogResult.No)
            {
                return false;
            }
            return false;
        }
    }
}
