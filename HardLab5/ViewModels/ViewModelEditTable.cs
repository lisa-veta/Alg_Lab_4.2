using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Forms;
using System.Windows.Input;
using DummyDB.Core;
using HardLab5.ViewModels;

namespace HardLab5
{
    class ViewModelEditTable : BaseViewModel
    {
        public ObservableCollection<ViewModelEditTable> Items { get; } = new ObservableCollection<ViewModelEditTable>();

        public string folderPath;

        private List<string> _names;
        public List<string> ListOfColumns
        {
            get { return _names; }
            set
            {
                _names = value;
                OnPropertyChanged();
            }
        }

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

        private string _newColumnName;
        public string NewColumnName
        {
            get { return _newColumnName; }
            set
            {
                _newColumnName = value;
                OnPropertyChanged();
            }
        }

        private string _selectedColumn;
        public string SelectedColumn
        {
            get { return _selectedColumn; }
            set
            {
                _selectedColumn = value;
                OnPropertyChanged();
            }
        }

        private string _selectedColumnDelete;
        public string SelectedColumnDelete
        {
            get { return _selectedColumnDelete; }
            set
            {
                _selectedColumnDelete = value;
                OnPropertyChanged();
            }
        }

        private string _columnName;
        public string ColumnName
        {
            get { return _columnName; }
            set
            {
                _columnName = value;
                OnPropertyChanged();
            }
        }
        private string _type;
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        private bool _primary;
        public bool Primary
        {
            get { return _primary; }
            set
            {
                _primary = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateNewColumn => new DelegateCommand(param =>
        {
            Items.Add(new ViewModelEditTable());
        });

        public ICommand RemoveNewColumn => new DelegateCommand(param =>
        {
            if (Items.Count > 0)
            {
                Items.RemoveAt(Items.Count - 1);
            }
            return;
        });

        public ICommand EditTable => new DelegateCommand(param =>
        {
            if (GetExeption())
            {
                return;
            }
            if (MainViewModel.tableName != TableName && TableName != null && TableName != "")
            {
                FileRewriter.RewriteFileName(folderPath, selectedScheme, TableName);
            }
            if(AddNewColumn())
            {
                System.Windows.MessageBox.Show("Данные не заполнены до конца");
                return;
            }
            if(RemoveColumn())
            {
                return;
            }
        });

        private bool GetExeption()
        {
            if (SelectedColumn != null && SelectedColumn != "" && NewColumnName != null && NewColumnName != "" && SelectedColumn != "нет выбора")
            {
                if (CheckEqualsNames("EditColumnName"))
                {
                    System.Windows.MessageBox.Show("Попытка переименования столбца в уже существующее имя");
                    return true;
                }
            }
            if (Items != null)
            {
                if (CheckEqualsNames("CreateNewColumn"))
                {
                    System.Windows.MessageBox.Show("Попытка добавления столбца с уже существующим именем");
                    return true;
                }
            }
            return false;
        }

        private bool AddNewColumn()
        {
            if(Items != null)
            {
                foreach (var item in Items)
                {
                    if (item.ColumnName == null || item.ColumnName == "" || item.Type == null)
                    {
                        return true;
                    }
                    Column column = new Column();
                    column.Name = item.ColumnName;
                    column.Type = item.Type;
                    column.IsPrimary = item.Primary;
                    selectedScheme.Columns.Add(column);
                    ColumnAdder.AddColumnInTable(column, selectedTable);
                    FileRewriter.RewriteCSV(folderPath, selectedTable, selectedScheme);
                }
            }
            Items.Clear();
            UpdateTable();
            return false;
        }

        private bool RemoveColumn()
        {
            if (SelectedColumnDelete != null && SelectedColumnDelete != "нет выбора")
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите безвозвратно удалить столбец?", "Подтверждение действий", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    ElementRemover.DeleteTableColumn(selectedScheme, selectedTable, SelectedColumnDelete);
                    FileRewriter.RewriteCSV(folderPath, selectedTable, selectedScheme);
                    SelectedColumnDelete = null;
                    UpdateTable();
                    return false;
                }
                return true;
            }
            return false;
        }

        private bool CheckEqualsNames(string location)
        {
            if(location == "EditColumnName")
            {
                foreach (Column column in selectedScheme.Columns)
                {
                    if (column.Name == NewColumnName)
                    {
                        return true;
                    }
                }
                EditColumnName();
            }
            else if(location == "CreateNewColumn")
            {
                foreach (var item in Items)
                {
                    foreach (Column column in selectedScheme.Columns)
                    {
                        if ((column.Name == item.ColumnName || item.ColumnName == NewColumnName) && item.ColumnName != null)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        private void EditColumnName()
        {
            if (SelectedColumn != null && SelectedColumn != "" && NewColumnName != null && NewColumnName != "" && SelectedColumn != "нет выбора")
            {
                ElementRemover.RemoveColumnName(selectedScheme, SelectedColumn, NewColumnName);
                UpdateTable();
                NewColumnName = null;
                SelectedColumn = null;
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
            UpdateList();
        }

        private void UpdateList()
        {
            ListOfColumns.Clear();
            List<string> names = new List<string>();
            foreach (var column in DataNewTable.Columns)
            {
                names.Add(column.ToString());
            }
            names.Add("нет выбора");
            ListOfColumns = names;
        }
    }
}
