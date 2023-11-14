using DummyDB.Core;
using HardLab5.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace HardLab5.ViewModels
{
    public class ViewModelSorting : BaseViewModel
    {
        public AlgorithmSort WindowDB { get; set; }
        public KeyValuePair<TableScheme, Table> keyTable = new KeyValuePair<TableScheme, Table>();
        private List<string> _sortingAlgorithms = new List<string> { "прямое слияние", "естественное слияние", "трехпутевое слияние" };
        public ObservableCollection<string> Movements { get; set; } = new ObservableCollection<string>();

        public List<string> ListOfSorts
        {
            get { return _sortingAlgorithms; }
            set
            {
                _sortingAlgorithms = value;
                OnPropertyChanged();
            }
        }

        private string _selectedSort;
        public string SelectedSort
        {
            get { return _selectedSort; }
            set
            {
                _selectedSort = value;
                OnPropertyChanged();
            }
        }

        public string folderPath;

        private List<string> _names;
        public List<string> CurrentColumns
        {
            get { return _names; }
            set
            {
                _names = value;
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

        private bool _isEnable = true;
        public bool IsEnable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                OnPropertyChanged();
            }
        }

        public DataGrid DataGrid { get; set; }
        public DataGrid DataGrid1 { get; set; }
        public DataGrid DataGrid2 { get; set; }
        public DataGrid DataGrid3 { get; set; }

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

        private DataTable _dataTableA = new DataTable();
        public DataTable DataTableA
        {
            get { return _dataTableA; }
            set
            {
                _dataTableA = value;
                OnPropertyChanged();
            }
        }
        private DataTable _dataTableB = new DataTable();
        public DataTable DataTableB
        {
            get { return _dataTableB; }
            set
            {
                _dataTableB = value;
                OnPropertyChanged();
            }
        }

        private DataTable _dataTableC = new DataTable();
        public DataTable DataTableС
        {
            get { return _dataTableC; }
            set
            {
                _dataTableC = value;
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

        private int _iterations = 1;
        private int _segments;

        public ICommand Start => new DelegateCommand(param =>
        {
            Movements.Clear();
            if (CheckChooses()) return;
            StartWork();
        });

        private bool CheckChooses()
        {
            if (CurrentColumns.Contains(SelectedColumn)) return false;
            else
            {
                MessageBox.Show("Выберите столбец");
                return true;
            }
        }

        private void StartWork()
        {
            switch (SelectedSort)
            {
                case "прямое слияние":
                    StartDirectMerger();
                    break;
                case "естественное слияние":
                    break;
                case "трехпутевое слияние":
                    StartMerger();
                    break;
                default:
                    MessageBox.Show("Выберите сортировку");
                    break;
            }
        }

        private void StartMerger()
        {
            DataTable dataTable = new DataTable();
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable.Columns.Add(column.Name);
            }
            DataTableA.Rows.Clear();
            DataTableA = dataTable;

            DataTable dataTable1 = new DataTable();
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable1.Columns.Add(column.Name);
            }
            DataTableB.Rows.Clear();
            DataTableB = dataTable1;

            DataTable dataTable2 = new DataTable();
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable2.Columns.Add(column.Name);
            }
            DataTableС.Rows.Clear();
            DataTableС = dataTable2;
            DoThreeWaySort();
        }

        private void StartDirectMerger()
        {
            _iterations = 1;
            _segments = 1;
            DataTable dataTable = new DataTable();
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable.Columns.Add(column.Name);
            }
            DataTableA.Rows.Clear();
            DataTableA = dataTable;
            DataTable dataTable1 = new DataTable();
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable1.Columns.Add(column.Name);
            }
            DataTableB.Rows.Clear();
            DataTableB = dataTable1;
            //DirectMerger directMerger = new DirectMerger(IsEnable, Slider);
            //directMerger.Sort(SelectedColumn, _segments, _iterations, DataNewTable, DataTableA, DataTableB, keyTable);
            DirectSort();
        }

        private void ChangeMainTable()
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
                    string data = DataNewTable.Rows[i][selectedScheme.Columns[j].Name].ToString();
                    selectedTable.Rows[i].Data[selectedScheme.Columns[j]] = data;
                }
            }
        }

        private void AddRowInTable(DataRow newRow, DataTable dataTable)
        {
            dataTable.Rows.Add(newRow.ItemArray);
        }
       
        async void DirectSort()
        {
            Movements.Add("Внешняя сортировка: прямое слияние");
            IsEnable = false;
            while (true)
            {
                _segments = 1;
                int counter = 0;
                bool flag = true;
                int counter1 = 0;
                Movements.Add("\nДелим данные на два вспомогательных файла\n" +
                    $"c итерацией равной {_iterations}\n");
                foreach (DataRow row in DataNewTable.Rows)
                {
                    if (counter == _iterations)
                    {
                        flag = !flag;
                        counter = 0;
                        _segments++;
                    }
                    if (flag)
                    {
                        Movements.Add($"Добавляем в таблицу A строку номер {DataNewTable.Rows.IndexOf(row)}");
                        AddRowInTable(row, DataTableA);
                        counter++;
                        await Task.Delay(1010 - Slider);
                    }
                    else
                    {
                        Movements.Add($"Добавляем в таблицу B строку номер {DataNewTable.Rows.IndexOf(row)}");
                        AddRowInTable(row, DataTableB);
                        counter++;
                        await Task.Delay(1010 - Slider);
                    }
                    counter1++;

                }
                flag = true;
                
                if (_segments == 1)
                {
                    Movements.Add($"\nПосле разделения на файлы получили\n всего один сегмент, значит \nсортировка окончена");
                    break;
                }
                Movements.Add($"\nТеперь сливаем таблицу A и B в одну\n");
                DataNewTable.Rows.Clear();
                DataRow newRowA = DataNewTable.NewRow();
                DataRow newRowB = DataNewTable.NewRow();
                int counterA = _iterations;
                int counterB = _iterations;
                bool pickedA = false, pickedB = false, endA = false, endB = false;
                int positionA = 0;
                int positionB = 0;
                int currentPA = 0;
                int currentPB = _iterations;
                for(int i = 0; i < 1000; i++)
                {
                    if (endA && endB)
                    {
                        break;
                    }

                    if (counterA == 0 && counterB == 0)
                    {
                        counterA = _iterations;
                        counterB = _iterations;
                    }

                    if (positionA != DataTableA.Rows.Count)
                    {
                        if (counterA > 0)
                        {
                            if (!pickedA)
                            {
                                newRowA = DataTableA.Rows[positionA];
                                positionA += 1;
                                pickedA = true;
                            }
                        }
                    }
                    else
                    {
                        endA = true;
                    }

                    if (positionB != DataTableB.Rows.Count)
                    {
                        if (counterB > 0)
                        {
                            if (!pickedB)
                            {
                                newRowB = DataTableB.Rows[positionB];
                                positionB += 1;
                                pickedB = true;
                            }
                        }
                    }
                    else
                    {
                        endB = true;
                    }
                    DataColumn myColunm = DataNewTable.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == SelectedColumn);
                    string tempA = string.Format("{0}", newRowA[myColunm.ToString()]);
                    string tempB = string.Format("{0}", newRowB[myColunm.ToString()]);
                    if (endA && endB && pickedA == false && pickedB == false)
                    {
                        break;
                    }
                    if (pickedA)
                    {
                        if (pickedB)
                        {
                            if (CompareDifferentTypes(tempA, tempB))
                            {
                                Movements.Add($"{tempA} < {tempB}, записываем {tempA}  в таблицу");
                                AddRowInTable(newRowA, DataNewTable);
                                counterA--;
                                pickedA = false;
                                await Task.Delay(1010 - Slider);
                            }
                            else
                            {
                                Movements.Add($"{tempB} < {tempA}, записываем {tempB}  в таблицу");
                                AddRowInTable(newRowB, DataNewTable);
                                counterB--;
                                pickedB = false;
                            }
                        }
                        else
                        {
                            Movements.Add($"записываем {tempA}  в таблицу");
                            AddRowInTable(newRowA, DataNewTable);
                            counterA--;
                            pickedA = false;
                            await Task.Delay(1010 - Slider);
                        }
                    }
                    else if (pickedB)
                    {
                        Movements.Add($"записываем {tempB}  в таблицу");
                        AddRowInTable(newRowB, DataNewTable);
                        counterB--;
                        pickedB = false;
                        await Task.Delay(1010 - Slider);
                    }
                    currentPA += positionA;
                    currentPB += positionB;
                }
                Movements.Add($"\nУвеличиваем итерацию деления в 2 раза:\n" +
                    $"{_iterations}*2 = {_iterations*2}");
                _iterations *= 2;
                DataTableA.Rows.Clear();
                DataTableB.Rows.Clear();
            }
            IsEnable = true;
            ChangeMainTable();
            FileRewriter.RewriteCSV(folderPath, selectedTable, selectedScheme);
        }

        private bool CompareDifferentTypes(string tempA, string tempB)
        {
            string type = "";
            foreach (Column column in keyTable.Key.Columns)
            {
                if(column.Name == SelectedColumn)
                {
                    type = column.Type;
                }
            }
            switch (type)
            {
                case "uint":
                    {
                        return CheckDouble(tempA, tempB);
                    }
                case "int":
                    {
                        return CheckDouble(tempA, tempB);
                    }
                case "double":
                    {
                        return CheckDouble(tempA, tempB);
                    }
                case "datetime":
                    {
                        return CheckDateTime(tempA, tempB);
                    }
                case "string":
                    {
                        return CheckString(tempA, tempB);
                    }
                case "bool":
                    {
                        return CheckDouble(tempA, tempB);
                    }
            }
            return true;
        }

        private bool CheckDouble(string tempA, string tempB)
        {
            double tA = double.Parse(tempA);
            double tB = double.Parse(tempB);
            if(tA < tB)
            {
                return true;
            }
            return false;
        }

        private bool CheckDateTime(string tempA, string tempB)
        {
            DateTime tA = DateTime.Parse(tempA);
            DateTime tB = DateTime.Parse(tempB);
            if (tA < tB)
            {
                return true;
            }
            return false;
        }

        private bool CheckString(string tempA, string tempB)
        {
            int b = tempA.CompareTo(tempB);
            if (b == -1)
            {
                return true;
            }
            return false;
        }


        async void DoThreeWaySort()
        {
            Movements.Add("Внешняя сортировка: трехпутевое слияние");
            IsEnable = false;
            while (true)
            {
                _segments = 1;
                bool flag = true;
                Movements.Add("\nДелим данные на два вспомогательных файла,\n" +
                    "сравнивая предыдущий с текущим чтобы в файлах\nэлементы были от меньшего к большему\n" +
                    "для последующего корректного сравнения");
                DataRow prev = DataNewTable.Rows[0];
                Movements.Add($"Сначала добавляем в таблицу A строку номер {DataNewTable.Rows.IndexOf(prev)}");
                AddRowInTable(prev, DataTableA);
                foreach (DataRow row in DataNewTable.Rows)
                {
                    if (flag)
                    {
                        flag = false;
                        continue;
                    }
                    DataColumn myColunm = DataNewTable.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == SelectedColumn);
                    string tempA = string.Format("{0}", prev[myColunm.ToString()]);
                    string tempB = string.Format("{0}", row[myColunm.ToString()]);
                    if (CompareDifferentTypes(tempB, tempA))
                    {
                        Movements.Add($"{tempA} > {tempB}, следующий элемент записывается в новый файл");
                        _segments++;
                    }
                    if (_segments % 3 == 1)
                    {
                        Movements.Add($"Добавляем в таблицу A строку номер {DataNewTable.Rows.IndexOf(row)}");
                        AddRowInTable(row, DataTableA);
                        await Task.Delay(1010 - Slider);
                    }
                    else if (_segments % 3 == 2)
                    {
                        Movements.Add($"Добавляем в таблицу B строку номер {DataNewTable.Rows.IndexOf(row)}");
                        AddRowInTable(row, DataTableB);
                        await Task.Delay(1010 - Slider);
                    }
                    else
                    {
                        Movements.Add($"Добавляем в таблицу C строку номер {DataNewTable.Rows.IndexOf(row)}");
                        AddRowInTable(row, DataTableС);
                        await Task.Delay(1010 - Slider);
                    }
                    prev = row;
                }

                if (_segments == 1)
                {
                    Movements.Add($"\nПосле разделения на файлы получили\n всего один сегмент, значит \nсортировка окончена");
                    break;
                }
                Movements.Add($"\nТеперь сливаем таблицу A и B в одну\n");
                DataNewTable.Rows.Clear();
                DataRow newRowA = DataNewTable.NewRow();
                DataRow newRowB = DataNewTable.NewRow();
                DataRow newRowC = DataNewTable.NewRow();
                int counterA = _iterations;
                int counterB = _iterations;
                int counterC = _iterations;
                bool pickedA = false, pickedB = false, pickedC = false, endA = false, endB = false, endC = false;
                int positionA = 0; int positionB = 0; int positionC = 0;
                int currentPA = 0; int currentPB = 0; int currentPC = 0;
                while(true)
                {
                    if (endA && endB && endC && pickedA == false && pickedB == false && pickedC == false)
                    {
                        break;
                    }

                    if (counterA == 0 && counterB == 0 && counterC == 0)
                    {
                        counterA = _iterations;
                        counterB = _iterations;
                        counterC = _iterations;
                    }
                    if (endA)
                    {
                        counterA = 0;
                    }
                    if (endB)
                    {
                        counterB = 0;
                    }
                    if (endC)
                    {
                        counterC = 0;
                    }

                    if (positionA != DataTableA.Rows.Count)
                    {
                        if (counterA > 0)
                        {
                            if (!pickedA)
                            {
                                newRowA = DataTableA.Rows[positionA];
                                positionA += 1;
                                pickedA = true;
                            }
                        }
                    }
                    else
                    {
                        endA = true;
                    }

                    if (positionB != DataTableB.Rows.Count)
                    {
                        if (counterB > 0)
                        {
                            if (!pickedB)
                            {
                                newRowB = DataTableB.Rows[positionB];
                                positionB += 1;
                                pickedB = true;
                            }
                        }
                    }
                    else
                    {
                        endB = true;
                    }

                    if (positionC != DataTableС.Rows.Count)
                    {
                        if (counterC > 0)
                        {
                            if (!pickedC)
                            {
                                newRowC = DataTableС.Rows[positionC];
                                positionC += 1;
                                pickedC = true;
                            }
                        }
                    }
                    else
                    {
                        endC = true;
                    }

                    if (endA && endB && endC && pickedA == false && pickedB == false && pickedC == false)
                    {
                        break;
                    }
                    DataColumn myColumn = DataNewTable.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == SelectedColumn);
                    string tempA = string.Format("{0}", newRowA[myColumn.ToString()]);
                    string tempB = string.Format("{0}", newRowB[myColumn.ToString()]);
                    string tempC = string.Format("{0}", newRowC[myColumn.ToString()]);
                    if (pickedA)
                    {
                        if (pickedB)
                        {
                            if (CompareDifferentTypes(tempA, tempB))
                            {
                                if (pickedC)
                                {
                                    if (CompareDifferentTypes(tempA, tempC))
                                    {
                                        Movements.Add($"{tempA} < {tempC} и {tempA} < {tempB},\nзаписываем {tempA}  в таблицу");
                                        AddRowInTable(newRowA, DataNewTable);
                                        counterA--;
                                        pickedA = false;
                                        await Task.Delay(1010 - Slider);
                                    }
                                    else
                                    {
                                        Movements.Add($"{tempC} < {tempA} и {tempC} < {tempB}, записываем {tempC}  в таблицу");
                                        AddRowInTable(newRowC, DataNewTable);
                                        counterC--;
                                        pickedC = false;
                                        await Task.Delay(1010 - Slider);
                                    }
                                }
                                else
                                {
                                    Movements.Add($"записываем {tempA} <  {tempB}, записываем {tempA}  в таблицу");
                                    AddRowInTable(newRowA, DataNewTable);
                                    counterA--;
                                    pickedA = false;
                                    await Task.Delay(1010 - Slider);
                                }
                            }
                            else
                            {
                                if (pickedC)
                                {
                                    if (CompareDifferentTypes(tempB, tempC))
                                    {
                                        Movements.Add($"{tempB} < {tempA} и {tempB} < {tempC},\nзаписываем {tempB}  в таблицу");
                                        AddRowInTable(newRowB, DataNewTable);
                                        counterB--;
                                        pickedB = false;
                                        await Task.Delay(1010 - Slider);
                                    }
                                    else
                                    {
                                        Movements.Add($"{tempC} < {tempA} и {tempC} < {tempB},\nзаписываем {tempC}  в таблицу");
                                        AddRowInTable(newRowC, DataNewTable);
                                        counterC--;
                                        pickedC = false;
                                        await Task.Delay(1010 - Slider);
                                    }
                                }
                                else
                                {
                                    Movements.Add($"{tempB} < {tempA},\nзаписываем {tempB}  в таблицу");
                                    AddRowInTable(newRowB, DataNewTable);
                                    counterB--;
                                    pickedB = false;
                                    await Task.Delay(1010 - Slider);
                                }
                            }
                        }
                        else if (pickedC)
                        {
                            if (CompareDifferentTypes(tempA, tempC))
                            {
                                Movements.Add($"{tempA} < {tempC}, записываем {tempA}  в таблицу");
                                AddRowInTable(newRowA, DataNewTable);
                                counterA--;
                                pickedA = false;
                                await Task.Delay(1010 - Slider);
                            }
                            else
                            {
                                Movements.Add($"{tempC} < {tempA}, записываем {tempC}  в таблицу");
                                AddRowInTable(newRowC, DataNewTable);
                                counterC--;
                                pickedC = false;
                                await Task.Delay(1010 - Slider);
                            }
                        }
                        else
                        {
                            Movements.Add($"записываем {tempA}  в таблицу");
                            AddRowInTable(newRowA, DataNewTable);
                            counterA--;
                            pickedA = false;
                            await Task.Delay(1010 - Slider);
                        }
                    }
                    else if (pickedB)
                    {
                        if (pickedC)
                        {
                            if (CompareDifferentTypes(tempB, tempC))
                            {
                                Movements.Add($"{tempB} < {tempC}, записываем {tempB}  в таблицу");
                                AddRowInTable(newRowB, DataNewTable);
                                counterB--;
                                pickedB = false;
                                await Task.Delay(1010 - Slider);
                            }
                            else
                            {
                                Movements.Add($"{tempC} < {tempB}, записываем {tempC}  в таблицу");
                                AddRowInTable(newRowC, DataNewTable);
                                counterC--;
                                pickedC = false;
                                await Task.Delay(1010 - Slider);
                            }
                        }
                        else
                        {
                            Movements.Add($"записываем {tempB}  в таблицу");
                            AddRowInTable(newRowB, DataNewTable);
                            counterB--;
                            pickedB = false;
                            await Task.Delay(1010-Slider);
                        }
                    }
                    else if (pickedC)
                    {
                        Movements.Add($"записываем {tempC}  в таблицу");
                        AddRowInTable(newRowC, DataNewTable);
                        counterC--;
                        pickedC = false;
                        await Task.Delay(1010 - Slider);
                    }
                    currentPA += positionA;
                    currentPB += positionB;
                    currentPC += positionC;
                }
                _iterations *= 2; // увеличиваем размер серии в 2 раза
                DataTableA.Rows.Clear();
                DataTableB.Rows.Clear();
                DataTableС.Rows.Clear();
            }
            IsEnable = true;
            ChangeMainTable();
            FileRewriter.RewriteCSV(folderPath, selectedTable, selectedScheme);
        }

        public AlgorithmSort AlgorithmSort
        {
            get => default;
            set
            {
            }
        }

        internal DelegateCommand DelegateCommand
        {
            get => default;
            set
            {
            }
        }

        public FileRewriter FileRewriter
        {
            get => default;
            set
            {
            }
        }

        public Table Table
        {
            get => default;
            set
            {
            }
        }

        public TableScheme TableScheme
        {
            get => default;
            set
            {
            }
        }
    }
}
