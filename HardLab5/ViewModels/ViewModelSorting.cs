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
            MessageBox.Show("Сначала выберите таблицу");
        });
    }
}
