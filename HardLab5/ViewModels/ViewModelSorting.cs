using DummyDB.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HardLab5.ViewModels
{
    public class ViewModelSorting : BaseViewModel
    {
        public AlgorithmSort WindowDB { get; set; }
        public KeyValuePair<TableScheme, Table> keyTable = new KeyValuePair<TableScheme, Table>();
        private List<string> _sortingAlgorithms = new List<string> { "прямое слияние", "естественное слияние", "многопутевое слияние" };
        public List<string> ListOfSorts
        {
            get { return _sortingAlgorithms; }
            set
            {
                _sortingAlgorithms = value;
                OnPropertyChanged();
            }
        }

        public string folderPath;

        private List<string> _names;
        public List<string> CurrentColumn
        {
            get { return _names; }
            set
            {
                _names = value;
                OnPropertyChanged();
            }
        }

        public System.Windows.Controls.DataGrid DataGrid { get; set; }
        public System.Windows.Controls.DataGrid DataGrid1 { get; set; }
        public System.Windows.Controls.DataGrid DataGrid2 { get; set; }

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

        private DataTable _dataTableA;
        public DataTable DataTableA
        {
            get { return _dataNewTable; }
            set
            {
                _dataTableA = value;
                OnPropertyChanged();
            }
        }
        private DataTable _dataTableB;
        public DataTable DataTableB
        {
            get { return _dataNewTable; }
            set
            {
                _dataTableB = value;
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
        public string SelectedSort
        {
            get { return _selectedColumnDelete; }
            set
            {
                _selectedColumnDelete = value;
                OnPropertyChanged();
            }
        }

        private int _slider = 500;
        public int Slider
        {
            get { return _slider; }
            set
            {
                _slider = value;
                OnPropertyChanged();
            }
        }

        public ICommand Start => new DelegateCommand(param =>
        {
            CreateTwoTables();
        });

        private void CreateTwoTables()
        {
            //GreateDataTable(1);
            //GreateDataTable(0);
        }


        //private void GreateDataTable(int temp)
        //{
        //    DataTable dataTable = new DataTable();
        //    selectedScheme = keyTable.Key;
        //    selectedTable = keyTable.Value;
        //    foreach (Column column in keyTable.Key.Columns)
        //    {
        //        dataTable.Columns.Add(column.Name);
        //    }

        //    for (int i = temp; i < keyTable.Value.Rows.Count; i+=2)
        //    {
        //        DataRow newRow = dataTable.NewRow();
        //        foreach (var element in keyTable.Value.Rows[i].Data)
        //        {
        //            newRow[element.Key.Name] = element.Value;
        //        }
        //        dataTable.Rows.Add(newRow);
        //    }
        //    if(temp == 0)
        //    {
        //        DataTableA = dataTable;
        //    }
        //    else
        //    {
        //        DataTableB = dataTable;
        //    }
        //}
    }
}
