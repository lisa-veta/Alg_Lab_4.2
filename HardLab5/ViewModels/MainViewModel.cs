using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using DummyDB.Core;
using HardLab5.ViewModels;

namespace HardLab5
{
    public class MainViewModel : BaseViewModel
    {
        private Dictionary<TableScheme, Table> keyTables = new Dictionary<TableScheme, Table>();
        private List<TableScheme> schemes = new List<TableScheme>();
        private int countOfTables;
        private int countOfSchemes;
        public static string folderPath = "";
        public static string tableName;
        private TableScheme selectedScheme;
        private Table selectedTable;

        private List<string> _sortingAlgorithms = new List<string> { "прямое слияние", "естественное слияние", "многопутевое слияние"};
        public List<string> ListOfSorts
        {
            get { return _sortingAlgorithms; }
            set
            {
                _sortingAlgorithms = value;
                OnPropertyChanged();
            }
        }

        private List<string> _currentColumn = new List<string> { "колонки" };
        public List<string> CurrentColumn
        {
            get { return _sortingAlgorithms; }
            set
            {
                _sortingAlgorithms = value;
                OnPropertyChanged();
            }
        }
        private string _currentAlg;
        public string CurrentSort
        {
            get { return _currentAlg; }
            set
            {
                _currentAlg = value;
                OnPropertyChanged();
            }
        }

        private DataTable _dataTable;
        public DataTable DataCurrentTable
        {
            get { return _dataTable; }
            set
            {
                _dataTable = value;
                OnPropertyChanged();
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenDataFile => new DelegateCommand(param =>
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            string path = "";

            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFolderDialog.SelectedPath;
            }
            if (path == "")
            {
                Message = "!СООБЩЕНИЕ! Вы не выбрали папку";
                if (folderPath != "")
                {
                    GetEquals(folderPath);
                }
                return;
            }
            folderPath = path;
            string folderName = folderPath.Split('\\')[folderPath.Split('\\').Length - 1];
            ((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Header = folderName;
            GetEquals(folderPath);
        });

        public void GetEquals(string folderPath)
        {
            keyTables.Clear();
            countOfSchemes = countOfTables = 0;
            schemes = RewriteList();
            ((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Items.Clear();
            foreach (string fileTable in Directory.EnumerateFiles(folderPath))
            {
                if (fileTable.Contains("csv"))
                {
                    countOfTables++;
                    AddTable(fileTable);
                }
                if (fileTable.Contains("json"))
                {
                    countOfSchemes++;
                }
            }
            GetExeption();
        }

        private List<TableScheme> RewriteList()
        {
            schemes.Clear();
            foreach (string fileScheme in Directory.EnumerateFiles(folderPath))
            {
                if (fileScheme.Contains("json"))
                {
                    TableScheme tableScheme = TableScheme.ReadFile(fileScheme);
                    schemes.Add(tableScheme);
                }
            }
            return schemes;
        }

        private void AddTable(string fileTable)
        {
            foreach (TableScheme tableScheme in schemes)
            {
                try
                {
                    Table table = TableData.GetInfoFromTable(tableScheme, fileTable);
                    keyTables.Add(tableScheme, table);
                    TreeViewItem treeViewItem = new TreeViewItem();
                    treeViewItem.Header = fileTable.Split('\\')[(fileTable.Split('\\').Length - 1)];
                    treeViewItem.Selected += TableSelected;

                    foreach (Column column in table.Scheme.Columns)
                    {
                        treeViewItem.Items.Add(column.Name + " - " + column.Type + " - isPrimary: " + column.IsPrimary);
                    }
                    ((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Items.Add(treeViewItem);
                    ((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Items.Add($"{tableScheme.Name}.json");
                    schemes.Remove(tableScheme);
                    break;
                }
                catch
                {
                    continue;
                }
            }
        }

        private void GetExeption()
        {
            if (countOfTables > countOfSchemes || keyTables.Count < countOfTables)
            {
                Message = "!СООБЩЕНИЕ! не все таблицы будут отображены, так как в файле недостаточно схем\\корректных схем";
            }
            else if (countOfTables < countOfSchemes)
            {
                Message = "!СООБЩЕНИЕ! найдены лишние схемы";
            }
            else
            {
                Message = "";
            }
        }

        public void TableSelected(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).DataGrid.Columns.Clear();
            tableName = ((TreeViewItem)sender).Header.ToString().Replace(".csv", "");
            DataTable dataTable = new DataTable();
            foreach (var keyTable in keyTables)
            {
                if (keyTable.Key.Name == tableName)
                {
                    dataTable = GreateDataTable(keyTable, dataTable);
                    break;
                }
            }
            DataCurrentTable = dataTable;
        }

        private DataTable GreateDataTable(KeyValuePair<TableScheme, Table> keyTable, DataTable dataTable)
        {
            selectedScheme = keyTable.Key;
            selectedTable = keyTable.Value;
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable.Columns.Add(column.Name);
            }

            for (int i = 0; i < keyTable.Value.Rows.Count; i++)
            {
                DataRow newRow = dataTable.NewRow();
                foreach (var element in keyTable.Value.Rows[i].Data)
                {
                    newRow[element.Key.Name] = element.Value;
                }
                dataTable.Rows.Add(newRow);
            }
            return dataTable;
        }

        public ICommand UpdateFile => new DelegateCommand(param =>
        {
            if (folderPath == "")
            {
                return;
            }
            GetEquals(folderPath);
        });

        public ICommand CreateNewDB => new DelegateCommand(param =>
        {
            WindowDB wind = new WindowDB();
            ViewModelNewDB vmNewDB = new ViewModelNewDB();
            wind.DataContext = vmNewDB;
            vmNewDB.WindowDB = wind;
            vmNewDB.folderPath = folderPath;
            wind.ShowDialog();
        });

        public ICommand CreateNewTable => new DelegateCommand(param =>
        {
            if (((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Header != null)
            {
                WindowTable wind = new WindowTable();
                ViewModelNewTable vmNewTable = new ViewModelNewTable();
                wind.DataContext = vmNewTable;
                vmNewTable.folderPath = folderPath;
                wind.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("Сначала выберите БД или создайте ее");
            }
        });

        public ICommand EditTable => new DelegateCommand(param =>
        {
            if (DataCurrentTable != null)
            {
                WindowEditTable wind = new WindowEditTable();
                ViewModelEditTable vmEditTable = new ViewModelEditTable();
                wind.DataContext = vmEditTable;
                vmEditTable.DataGrid = wind.DataGridEditTable;
                vmEditTable.DataNewTable = DataCurrentTable;
                vmEditTable.TableName = tableName;
                vmEditTable.folderPath = folderPath;
                List<string> names = new List<string>();
                foreach (var column in DataCurrentTable.Columns)
                {
                    names.Add(column.ToString());
                }
                names.Add("нет выбора");
                vmEditTable.ListOfColumns = names;
                vmEditTable.selectedScheme = selectedScheme;
                vmEditTable.selectedTable = selectedTable;
                wind.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("Сначала выберите таблицу");
            }
        });

        public ICommand EditTableData => new DelegateCommand(param =>
        {
            if (DataCurrentTable != null)
            {
                WindowEditTableData wind = new WindowEditTableData();
                ViewModelEditTableData vmEditTableData = new ViewModelEditTableData();
                wind.DataContext = vmEditTableData;
                vmEditTableData.DataGrid = wind.DataGridEditTable;
                vmEditTableData.DataNewTable = DataCurrentTable;
                vmEditTableData.TableName = tableName;
                vmEditTableData.folderPath = folderPath;
                vmEditTableData.selectedScheme = selectedScheme;
                vmEditTableData.selectedTable = selectedTable;
                wind.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("Сначала выберите таблицу");
            }
        });

        public ICommand SortTable => new DelegateCommand(param =>
        {
            if (DataCurrentTable != null)
            {
                AlgorithmSort wind = new AlgorithmSort();
                ViewModelSorting vmSorted = new ViewModelSorting();
                wind.DataContext = vmSorted;
                vmSorted.DataGrid = wind.DataNewGrid;
                vmSorted.DataGrid1 = wind.DataGrid1;
                vmSorted.DataGrid2 = wind.DataGrid2;
                vmSorted.DataNewTable = DataCurrentTable;
                vmSorted.DataTableA = new DataTable();
                vmSorted.DataTableB = new DataTable();
                vmSorted.folderPath = folderPath;
                List<string> names = new List<string>();
                foreach (var column in DataCurrentTable.Columns)
                {
                    names.Add(column.ToString());
                }
                names.Add("нет выбора");
                vmSorted.CurrentColumn = names;
                vmSorted.selectedScheme = selectedScheme;
                vmSorted.selectedTable = selectedTable;
                foreach (var keyTable in keyTables)
                {
                    if (keyTable.Key.Name == tableName)
                    {
                        vmSorted.keyTable = keyTable;
                        break;
                    }
                }
                wind.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("Сначала выберите таблицу");
            }
        });

    }
}
