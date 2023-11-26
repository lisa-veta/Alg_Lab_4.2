using DummyDB.Core;
using HardLab5.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
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

        private List<int> _seriesA = new List<int>();
        public List<int> _seriesB = new List<int>();
        public List<int> _seriesC = new List<int>();
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
            DataGrid.Columns[0].Visibility = Visibility.Collapsed;
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
                    StartNatureMerger();
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
            DataGrid1.Columns[0].Visibility = Visibility.Collapsed;
            DataTable dataTable1 = new DataTable();
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable1.Columns.Add(column.Name);
            }
            DataTableB.Rows.Clear();
            DataTableB = dataTable1;
            DataGrid2.Columns[0].Visibility = Visibility.Collapsed;
            DataTable dataTable2 = new DataTable();
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable2.Columns.Add(column.Name);
            }
            DataTableС.Rows.Clear();
            DataTableС = dataTable2;
            DataGrid3.Columns[0].Visibility = Visibility.Collapsed;
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
            DataGrid1.Columns[0].Visibility = Visibility.Collapsed;
            DataTable dataTable1 = new DataTable();
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable1.Columns.Add(column.Name);
            }
            DataTableB.Rows.Clear();
            DataTableB = dataTable1;
            DataGrid2.Columns[0].Visibility = Visibility.Collapsed;
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

        private void ChangeTable(DataTable dataTable, int position, string log)
        {
            DataTable temp = new DataTable();
            foreach(Column column in keyTable.Key.Columns)
            {
                temp.Columns.Add(column.Name);
            }
            foreach (DataRow row in dataTable.Rows)
            {
                temp.Rows.Add(row.ItemArray);
            }
            temp.Rows[position]["Hidden"] = log;
            dataTable.Rows.Clear();
            foreach (DataRow row in temp.Rows)
            {
                dataTable.Rows.Add(row.ItemArray);
            }
        }

        private void AddRowInTable(DataRow newRow, DataTable dataTable, string flag = null)
        {
            dataTable.Rows.Add(newRow.ItemArray);
            if(flag != null)
            {
                dataTable.Rows[dataTable.Rows.Count - 1]["Hidden"] = flag;
            }
        }
       
        async void DirectSort()
        {
            _series.Clear();
            Movements.Add("Внешняя сортировка: прямое слияние");
            IsEnable = false;
            while (true)
            {
                _segments = 1;
                int counter = 0;
                bool flag = true;
                int counter1 = 0;
                int seriaCounter = 0;
                string seria = "seria1";
                Movements.Add("\nДелим данные на два вспомогательных файла\n" +
                    $"c итерацией равной {_iterations}\n");
                foreach (DataRow row in DataNewTable.Rows)
                {
                    if (counter == _iterations)
                    {
                        flag = !flag;
                        _series.Add(counter + 1);
                        counter = 0;
                        _segments++;
                        seriaCounter++;
                        if(seriaCounter == 2)
                        {
                            seria = (seria == "seria1") ? "seria2" : "seria1";
                            seriaCounter = 0;
                        }
                    }
                    if (flag)
                    {
                        Movements.Add($"Добавляем в таблицу A строку номер {DataNewTable.Rows.IndexOf(row)}");
                        AddRowInTable(row, DataTableA, seria);
                        counter++;
                        await Task.Delay(1010 - Slider);
                    }
                    else
                    {
                        Movements.Add($"Добавляем в таблицу B строку номер {DataNewTable.Rows.IndexOf(row)}");
                        AddRowInTable(row, DataTableB, seria);
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
                string strSeriaA = "seria1";
                string strSeriaB = "seria1";
                int currentPB = _iterations;
                int seriaA = 0; int seriaB = 0;
                int indA = 0; int indB = 1;
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
                                DataColumn colunm = DataTableA.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == "Hidden");
                                strSeriaA = string.Format("{0}", DataTableA.Rows[positionA][colunm.ToString()]);
                                ChangeTable(DataTableA, positionA, "current");
                                newRowA = DataTableA.Rows[positionA];
                                //DataTableA.Rows[positionA]["Hidden"] = "current";
                                positionA += 1;
                                pickedA = true;
                                await Task.Delay(1010 - Slider);
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
                                DataColumn colunm = DataTableB.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == "Hidden");
                                strSeriaB = string.Format("{0}", DataTableB.Rows[positionB][colunm.ToString()]);
                                ChangeTable(DataTableB, positionB, "current");
                                newRowB = DataTableB.Rows[positionB];
                                //DataTableB.Rows[positionB]["Hidden"] = "current";
                                positionB += 1;
                                pickedB = true;
                                await Task.Delay(1010 - Slider);
                            }
                        }
                    }
                    else
                    {
                        endB = true;
                    }
                    DataColumn myColunm = DataNewTable.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == SelectedColumn);
                    string tempA = string.Format("{0}", DataTableA.Rows[positionA-1][myColunm.ToString()]);
                    string tempB = string.Format("{0}", DataTableB.Rows[positionB-1][myColunm.ToString()]);
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
                                ChangeTable(DataTableA, positionA-1, strSeriaA);
                                AddRowInTable(DataTableA.Rows[positionA-1], DataNewTable);
                                counterA--;
                                pickedA = false;
                                await Task.Delay(1010 - Slider);
                            }
                            else
                            {
                                Movements.Add($"{tempB} < {tempA}, записываем {tempB}  в таблицу");
                                ChangeTable(DataTableB, positionB-1, strSeriaB);
                                AddRowInTable(DataTableB.Rows[positionB-1], DataNewTable);
                                counterB--;
                                pickedB = false;
                            }
                        }
                        else
                        {
                            Movements.Add($"записываем {tempA}  в таблицу");
                            ChangeTable(DataTableA, positionA-1, strSeriaA);
                            AddRowInTable(DataTableA.Rows[positionA-1], DataNewTable);
                            counterA--;
                            pickedA = false;
                            await Task.Delay(1010 - Slider);
                        }
                    }
                    else if (pickedB)
                    {
                        Movements.Add($"записываем {tempB}  в таблицу");
                        ChangeTable(DataTableB, positionB-1, strSeriaB);
                        AddRowInTable(DataTableB.Rows[positionB-1], DataNewTable);
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

        private void StartNatureMerger()
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
            DataGrid1.Columns[0].Visibility = Visibility.Collapsed;
            DataTable dataTable1 = new DataTable();
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable1.Columns.Add(column.Name);
            }
            DataTableB.Rows.Clear();
            DataTableB = dataTable1;
            DataGrid2.Columns[0].Visibility = Visibility.Collapsed;
            NativeOuterSort2();
        }

        private List<int> _series = new List<int>();

        async void NativeOuterSort()
        {
            Movements.Add("Внешняя сортировка - естественное слияние");
            IsEnable = false;
            while (true)
            {
                _segments = 1;
                DataRow prev = DataNewTable.Rows[0];
                bool flag = true;
                bool flagf = true;
                Movements.Add("\nФайл разделяется на 2 вспомогательных,\nразделяя основной файл на серии\n"+
                              "(уже отсортированные подмассивы).\nНечетные серии в таблицу А, нечётные - B\n");
                AddRowInTable(prev, DataTableA);
                int counter = 0;
                foreach (DataRow cur in DataNewTable.Rows)
                {
                    if (flagf)
                    {
                        flagf = false;
                        continue;
                    }
                    DataColumn myColunm = DataNewTable.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == SelectedColumn);
                    string tempA = string.Format("{0}", prev[myColunm.ToString()]);
                    string tempB = string.Format("{0}", cur[myColunm.ToString()]);
                    if (CompareDifferentTypes(tempB, tempA))
                    {
                        Movements.Add($"\nЭлемент {tempA} > {tempB}\nНачинается новая серия элементов.\n");
                        flag = !flag;
                        _segments++;
                        _series.Add(counter + 1);
                        counter = 0;
                    }
                    if (flag)
                    {
                        Movements.Add($"Добавление строки под номером {DataNewTable.Rows.IndexOf(cur)} в таблицу А\n"+
                                      "Продолжается серия элементов");
                        AddRowInTable(cur, DataTableA);
                        await Task.Delay(1010 - Slider);
                        counter++;
                    }
                    else
                    {
                        Movements.Add($"Добавление строки под номером {DataNewTable.Rows.IndexOf(cur)} в таблицу B\n"+
                                      "Продолжается серия элементов");
                        AddRowInTable(cur, DataTableB);
                        await Task.Delay(1010 - Slider);
                        counter++;
                    }
                    prev = cur;
                }


                if (_segments == 1)
                {
                    Movements.Add("\nПосле разделения на файлы остался один сегмент\n(одна серия), значит сортировка закончена\n");
                    break;
                }

                DataNewTable.Rows.Clear();
                DataRow newRowA = DataNewTable.NewRow();
                DataRow newRowB = DataNewTable.NewRow();

                bool pickedA = false, pickedB = false;
                int positionA = 0, positionB = 0;
                int seriaA = 0; int seriaB = 0;
                int indA = 0; int indB = 1;
                while (positionA != DataTableA.Rows.Count || positionB != DataTableB.Rows.Count || pickedA || pickedB)
                {
                    if (positionA != DataTableA.Rows.Count)
                    {
                        if (_series[indA] != seriaA && !pickedA)
                        {
                            newRowA = DataTableA.Rows[positionA];
                            pickedA = true;
                            positionA += 1;
                        }
                        if (_series[indA] == seriaA && indA <= _series.Count - 1)
                        {
                            pickedA = false;
                            indA += 2;
                            seriaA = 0;
                        }
                    }
                    if (positionB != DataTableB.Rows.Count)
                    {
                        if (_series[indB] != seriaB && !pickedB)
                        {
                            newRowB = DataTableB.Rows[positionB];
                            pickedB = true;
                            positionB += 1;
                        }
                        if (_series[indB] == seriaB && indB <= _series.Count - 1)
                        {
                            pickedB = false;
                            indB += 2;
                            seriaB = 0;
                        }
                    }
                    DataColumn myColumn = DataNewTable.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == SelectedColumn);
                    string tempA = string.Format("{0}", newRowA[myColumn.ToString()]);
                    string tempB = string.Format("{0}", newRowB[myColumn.ToString()]);
                    if (pickedA)
                    {
                        if (pickedB)
                        {
                            if (CompareDifferentTypes(tempA, tempB))
                            {
                                Movements.Add($"Элемент {tempB} > {tempA}, добавляем {tempA} в основную таблицу.");
                                AddRowInTable(newRowA, DataNewTable);
                                pickedA = false;
                                await Task.Delay(1010 - Slider);
                            }
                            else
                            {
                                Movements.Add($"Элемент {tempB} < {tempA}, добавляем {tempB} в основную таблицу.");
                                AddRowInTable(newRowB, DataNewTable);
                                pickedB = false;
                                await Task.Delay(1010 - Slider);
                            }
                        }
                        else
                        {
                            Movements.Add($"Добавление элемента {tempA} в основную таблицу. ");
                            AddRowInTable(newRowA, DataNewTable);
                            pickedA = false;
                            await Task.Delay(1010 - Slider);
                        }
                    }
                    else if (pickedB)
                    {
                        Movements.Add($"Добавление элемента {tempB} в основную таблицу");
                        AddRowInTable(newRowB, DataNewTable);
                        pickedB = false;
                        await Task.Delay(1010 - Slider);
                    }
                }
                DataTableA.Rows.Clear();
                DataTableB.Rows.Clear();
            }
            IsEnable = true;
            ChangeMainTable();
            FileRewriter.RewriteCSV(folderPath, selectedTable, selectedScheme);
        }

        async void NativeOuterSort2()
        {
            Movements.Add("Внешняя сортировка - естественное слияние");
            IsEnable = false;
            while (true)
            {
                _series.Clear();
                _segments = 1;
                DataRow prev = DataNewTable.Rows[0];
                bool flag = true;
                bool flagf = true;
                int seriaCounter = 0;
                string seria = "seria1";
                Movements.Add("\nФайл разделяется на 2 вспомогательных,\nразделяя основной файл на серии\n" +
                              "(уже отсортированные подмассивы).\nНечетные серии в таблицу А, нечётные - B\n");
                AddRowInTable(prev, DataTableA, seria);
                int counter = 0;
                foreach (DataRow cur in DataNewTable.Rows)
                {
                    if (flagf)
                    {
                        flagf = false;
                        continue;
                    }
                    DataColumn myColunm = DataNewTable.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == SelectedColumn);
                    string tempA = string.Format("{0}", prev[myColunm.ToString()]);
                    string tempB = string.Format("{0}", cur[myColunm.ToString()]);
                    if (CompareDifferentTypes(tempB, tempA))
                    {
                        Movements.Add($"\nЭлемент {tempA} > {tempB}\nНачинается новая серия элементов.\n");
                        flag = !flag;
                        _segments++;
                        _series.Add(counter + 1);
                        counter = 0;
                        seriaCounter++;
                        if (seriaCounter == 2)
                        {
                            seria = (seria == "seria1") ? "seria2" : "seria1";
                            seriaCounter = 0;
                        }
                    }
                    if (flag)
                    {
                        Movements.Add($"Добавление строки под номером {DataNewTable.Rows.IndexOf(cur)} в таблицу А\n" +
                                      "Продолжается серия элементов");
                        AddRowInTable(cur, DataTableA, seria);
                        await Task.Delay(1010 - Slider);
                        counter++;
                    }
                    else
                    {
                        Movements.Add($"Добавление строки под номером {DataNewTable.Rows.IndexOf(cur)} в таблицу B\n" +
                                      "Продолжается серия элементов");
                        AddRowInTable(cur, DataTableB, seria);
                        await Task.Delay(1010 - Slider);
                        counter++;
                    }
                    prev = cur;
                }


                if (_segments == 1)
                {
                    Movements.Add("\nПосле разделения на файлы остался один сегмент\n(одна серия), значит сортировка закончена\n");
                    break;
                }
          
                DataNewTable.Rows.Clear();
                DataRow newRowA = DataNewTable.NewRow();
                DataRow newRowB = DataNewTable.NewRow();
                SetNewSeries(DataTableA, _seriesA);
                SetNewSeries(DataTableB, _seriesB);

                string strSeriaA = "seria1";
                string strSeriaB = "seria1";
                bool pickedA = false, pickedB = false;
                int positionA = 0, positionB = 0;
                int seriaA = _seriesA[0]; int seriaB = _seriesB[0];
                int indA = 0; int indB = 0;
                bool endA = false; bool endB = false;
                Movements.Add("\nНачинаем слияние в файл\n");
                while (true)
                {
                    if (endA && endB)
                    {
                        break;
                    }

                    if (seriaA == 0 && seriaB == 0)
                    {
                        indA++; indB++;
                        if (indA <= _seriesA.Count - 1)
                        {
                            seriaA = _seriesA[indA];
                        }
                        if (indB <= _seriesB.Count - 1)
                        {
                            seriaB = _seriesB[indB];
                        }
                    }

                    if (positionA != DataTableA.Rows.Count)
                    {
                        if (seriaA > 0)
                        {
                            if (!pickedA)
                            {
                                DataColumn colunm = DataTableA.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == "Hidden");
                                strSeriaA = string.Format("{0}", DataTableA.Rows[positionA][colunm.ToString()]);
                                ChangeTable(DataTableA, positionA, "current");
                                newRowA = DataTableA.Rows[positionA];
                                positionA += 1;
                                pickedA = true;
                                await Task.Delay(1010 - Slider);
                            }
                        }
                    }
                    else
                    {
                        endA = true;
                    }

                    if (positionB != DataTableB.Rows.Count)
                    {
                        if (seriaB > 0)
                        {
                            if (!pickedB)
                            {
                                DataColumn colunm = DataTableB.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == "Hidden");
                                strSeriaB = string.Format("{0}", DataTableB.Rows[positionB][colunm.ToString()]);
                                ChangeTable(DataTableB, positionB, "current");
                                newRowB = DataTableB.Rows[positionB];
                                positionB += 1;
                                pickedB = true;
                                await Task.Delay(1010 - Slider);
                            }
                        }
                    }
                    else
                    {
                        endB = true;
                    }

                    if (endA && endB && pickedA == false && pickedB == false)
                    {
                        break;
                    }
                    DataColumn myColunm = DataNewTable.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == SelectedColumn);
                    string tempA = string.Format("{0}", DataTableA.Rows[positionA-1][myColunm.ToString()]);
                    string tempB = string.Format("{0}", DataTableB.Rows[positionB-1][myColunm.ToString()]);
                    if (pickedA)
                    {
                        if (pickedB)
                        {
                            if (CompareDifferentTypes(tempA, tempB))
                            {
                                Movements.Add($"Элемент {tempB} > {tempA}, добавляем {tempA} в основную таблицу.");
                                ChangeTable(DataTableA, positionA - 1, strSeriaA);
                                AddRowInTable(DataTableA.Rows[positionA - 1], DataNewTable);
                                pickedA = false;
                                seriaA--;
                                await Task.Delay(1010 - Slider);
                            }
                            else
                            {
                                Movements.Add($"Элемент {tempA} > {tempB}, добавляем {tempB} в основную таблицу.");
                                ChangeTable(DataTableB, positionB - 1, strSeriaB);
                                AddRowInTable(DataTableB.Rows[positionB - 1], DataNewTable);
                                pickedB = false;
                                seriaB--;
                                await Task.Delay(1010 - Slider);
                            }
                        }
                        else
                        {
                            Movements.Add($"Добавляем {tempA} в основную таблицу.");
                            ChangeTable(DataTableA, positionA - 1, strSeriaA);
                            AddRowInTable(DataTableA.Rows[positionA - 1], DataNewTable);
                            pickedA = false;
                            seriaA--;
                            await Task.Delay(1010 - Slider);
                        }
                    }
                    else if (pickedB)
                    {
                        Movements.Add($"Добавляем {tempB} в основную таблицу.");
                        ChangeTable(DataTableB, positionB - 1, strSeriaB);
                        AddRowInTable(DataTableB.Rows[positionB - 1], DataNewTable);
                        pickedB = false;
                        seriaB--;
                        await Task.Delay(1010 - Slider);
                    }
                }
                DataTableA.Rows.Clear();
                DataTableB.Rows.Clear();
            }
            IsEnable = true;
            ChangeMainTable();
            FileRewriter.RewriteCSV(folderPath, selectedTable, selectedScheme);
        }

        private void SetNewSeries(DataTable dataTable, List<int> series)
        {
            series.Clear();
            int columnNumber = 0;
            foreach(Column column in keyTable.Key.Columns)
            {
                if(column.Name == SelectedColumn) break;               
                columnNumber++;
            }
            string prev = dataTable.Rows[0][columnNumber].ToString();
            string cur;
            int count = 0;
            DataTable newTable = new DataTable();
            foreach(Column column in keyTable.Key.Columns)
            {
                newTable.Columns.Add(column.Name);
            }
            string seria = "seria1";
            AddRowInTable(dataTable.Rows[0], newTable, seria);
            for (int i = 1; i < dataTable.Rows.Count; i++)
            {
                cur = dataTable.Rows[i][columnNumber].ToString();
                if (CompareDifferentTypes(cur, prev))
                {
                    series.Add(count + 1);
                    count = 0;
                    prev = cur;
                    seria = (seria == "seria1") ? "seria2" : "seria1";
                    AddRowInTable(dataTable.Rows[i], newTable, seria);
                    if (i == dataTable.Rows.Count - 1)
                    {
                        series.Add(count + 1);
                        seria = (seria == "seria1") ? "seria2" : "seria1";
                    }
                    continue;
                }
                count++;
                AddRowInTable(dataTable.Rows[i], newTable, seria);
                if (i == dataTable.Rows.Count - 1)
                {
                    series.Add(count + 1);
                    seria = (seria == "seria1") ? "seria2" : "seria1";
                }
                prev = cur;
            }
            dataTable.Rows.Clear();
            foreach (DataRow row in newTable.Rows)
            {
                dataTable.Rows.Add(row.ItemArray);
            }
        }

        private void SetNewSeriesNew(DataTable dataTable, List<int> series)
        {
            DataTable temp = new DataTable();
            foreach (Column column in keyTable.Key.Columns)
            {
                temp.Columns.Add(column.Name);
            }
            foreach (DataRow row in dataTable.Rows)
            {
                temp.Rows.Add(row.ItemArray);
            }
            dataTable.Rows.Clear();
            series.Clear();
            DataRow prev = temp.Rows[0];
            bool flag = true;
            bool flagf = true;
            int seriaCounter = 0;
            string seria = "seria1";
            AddRowInTable(prev, dataTable);
            int counter = 0;
            foreach (DataRow cur in temp.Rows)
            {
                if (flagf)
                {
                    flagf = false;
                    continue;
                }
                DataColumn myColunm = temp.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == SelectedColumn);
                string tempA = string.Format("{0}", prev[myColunm.ToString()]);
                string tempB = string.Format("{0}", cur[myColunm.ToString()]);
                if (CompareDifferentTypes(tempB, tempA))
                {
                    flag = !flag;
                    series.Add(counter + 1);
                    counter = 0;
                    seriaCounter++;
                    if (seriaCounter == 2)
                    {
                        seria = (seria == "seria1") ? "seria2" : "seria1";
                        seriaCounter = 0;
                    }
                }
                if (flag)
                {
                    AddRowInTable(cur, dataTable, seria);
                    counter++;
                }
                else
                {
                    AddRowInTable(cur, dataTable, seria);
                    counter++;
                }
                prev = cur;
            }
        }

        async void DoThreeWaySort()
        {
            Movements.Add("Внешняя сортировка: трехпутевое слияние");
            IsEnable = false;
            _iterations = 1;
            while (true)
            {
                //bool flag = true;
                Movements.Add("\nДелим данные на два вспомогательных файла,\n" +
                    "сравнивая предыдущий с текущим чтобы в файлах\nэлементы были от меньшего к большему\n" +
                    "для последующего корректного сравнения");
                //DataRow prev = DataNewTable.Rows[0];
                //Movements.Add($"Сначала добавляем в таблицу A строку номер {DataNewTable.Rows.IndexOf(prev)}");
                //AddRowInTable(prev, DataTableA);
                _segments = 1;
                int counterI = 0;
                bool flag = true;
                int counter1 = 0;
                int seriaCounter = 0;
                string seria = "seria1";
                Movements.Add("\nДелим данные на три вспомогательных файла\n" +
                    $"c итерацией равной {_iterations}\n");
                foreach (DataRow row in DataNewTable.Rows)
                {
                    if (counterI == _iterations)
                    {
                        Movements.Add("Меняем таблицу");
                        flag = !flag;
                        counterI = 0;
                        _segments++;
                        seriaCounter++;
                        if (seriaCounter == 3)
                        {
                            seria = (seria == "seria1") ? "seria2" : "seria1";
                            seriaCounter = 0;
                        }
                    }
                    if (_segments % 3 == 1)
                    {
                        Movements.Add($"Добавляем в таблицу A строку номер {DataNewTable.Rows.IndexOf(row)}");
                        AddRowInTable(row, DataTableA, seria);
                        await Task.Delay(1010 - Slider);
                    }
                    else if(_segments % 3 == 2)
                    {
                        Movements.Add($"Добавляем в таблицу B строку номер {DataNewTable.Rows.IndexOf(row)}");
                        AddRowInTable(row, DataTableB, seria);
                        await Task.Delay(1010 - Slider);
                    }
                    else
                    {
                        Movements.Add($"Добавляем в таблицу С строку номер {DataNewTable.Rows.IndexOf(row)}");
                        AddRowInTable(row, DataTableС, seria);
                        await Task.Delay(1010 - Slider);
                    }
                    counter1++;
                    counterI++;
                }
                flag = true;
                //foreach (DataRow row in DataNewTable.Rows)
                //{
                //    if (flag)
                //    {
                //        flag = false;
                //        continue;
                //    }
                //    DataColumn myColunm = DataNewTable.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == SelectedColumn);
                //    string tempA = string.Format("{0}", prev[myColunm.ToString()]);
                //    string tempB = string.Format("{0}", row[myColunm.ToString()]);
                //    if (CompareDifferentTypes(tempB, tempA))
                //    {
                //        Movements.Add($"{tempA} > {tempB}, следующий элемент записывается в новый файл");
                //        _segments++;
                //        seriaCounter++;
                //        if (seriaCounter == 2)
                //        {
                //            seria = (seria == "seria1") ? "seria2" : "seria1";
                //            seriaCounter = 0;
                //        }
                //    }
                //    if (_segments % 3 == 1)
                //    {
                //        Movements.Add($"Добавляем в таблицу A строку номер {DataNewTable.Rows.IndexOf(row)}");
                //        AddRowInTable(row, DataTableA, seria);
                //        await Task.Delay(1010 - Slider);
                //    }
                //    else if (_segments % 3 == 2)
                //    {
                //        Movements.Add($"Добавляем в таблицу B строку номер {DataNewTable.Rows.IndexOf(row)}");
                //        AddRowInTable(row, DataTableB, seria);
                //        await Task.Delay(1010 - Slider);
                //    }
                //    else
                //    {
                //        Movements.Add($"Добавляем в таблицу C строку номер {DataNewTable.Rows.IndexOf(row)}");
                //        AddRowInTable(row, DataTableС, seria);
                //        await Task.Delay(1010 - Slider);
                //    }
                //    prev = row;
                //}

                if (_segments == 1)
                {
                    Movements.Add($"\nПосле разделения на файлы получили\n всего один сегмент, значит \nсортировка окончена");
                    break;
                }
                Movements.Add($"\nТеперь сливаем таблицу A и B в одну\n");
                DataNewTable.Rows.Clear();
                //if(DataTableA.Rows.Count > 0)
                //{
                //    SetNewSeries(DataTableA, _seriesA);
                //}
                //if(DataTableB.Rows.Count > 0)
                //{
                //    SetNewSeries(DataTableB, _seriesB);
                //}
                //if(DataTableС.Rows.Count > 0)
                //{
                //    SetNewSeries(DataTableС, _seriesC);
                //}
                DataRow newRowA = DataNewTable.NewRow();
                DataRow newRowB = DataNewTable.NewRow();
                DataRow newRowC = DataNewTable.NewRow();
                int counterA = _iterations;
                int counterB = _iterations;
                int counterC = _iterations;
                string strSeriaA = "seria1";
                string strSeriaB = "seria1";
                string strSeriaC = "seria1";
                bool pickedA = false, pickedB = false, pickedC = false, endA = false, endB = false, endC = false;
                int positionA = 0; int positionB = 0; int positionC = 0;
                int currentPA = 0; int currentPB = 0; int currentPC = 0;
                //int seriaA = _seriesA[0]; int seriaB = _seriesB[0]; int seriaC = _seriesC[0];
                int indA = 0; int indB=0; int indC = 0;
                while(true)
                {
                    if (endA && endB && endC && pickedA == false && pickedB == false && pickedC == false)
                    {
                        break;
                    }
                    //if (seriaA == 0 && seriaB == 0 && seriaC ==0)
                    //{
                    //    indA++; indB++; indC++;
                    //    if (indA <= _seriesA.Count - 1)
                    //    {
                    //        seriaA = _seriesA[indA];
                    //    }
                    //    if (indB <= _seriesB.Count - 1)
                    //    {
                    //        seriaB = _seriesB[indB];
                    //    }
                    //    if (indC <= _seriesC.Count - 1)
                    //    {
                    //        seriaC = _seriesC[indC];
                    //    }
                    //}

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
                        //if(seriaA > 0)
                        {
                            if (!pickedA)
                            {
                                DataColumn colunm = DataTableA.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == "Hidden");
                                strSeriaA = string.Format("{0}", DataTableA.Rows[positionA][colunm.ToString()]);
                                ChangeTable(DataTableA, positionA, "current");
                                newRowA = DataTableA.Rows[positionA];
                                positionA += 1;
                                pickedA = true;
                                await Task.Delay(1010 - Slider);
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
                        //if (seriaB > 0)
                        {
                            if (!pickedB)
                            {
                                DataColumn colunm = DataTableB.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == "Hidden");
                                strSeriaB = string.Format("{0}", DataTableB.Rows[positionB][colunm.ToString()]);
                                ChangeTable(DataTableB, positionB, "current");
                                newRowB = DataTableB.Rows[positionB];
                                positionB += 1;
                                pickedB = true;
                                await Task.Delay(1010 - Slider);
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
                        //if (seriaC > 0)
                        {
                            if (!pickedC)
                            {
                                DataColumn colunm = DataTableС.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == "Hidden");
                                strSeriaC = string.Format("{0}", DataTableС.Rows[positionC][colunm.ToString()]);
                                ChangeTable(DataTableС, positionC, "current");
                                newRowC = DataTableС.Rows[positionC];
                                positionC += 1;
                                pickedC = true;
                                await Task.Delay(1010 - Slider);
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
                    string tempA = string.Format("{0}", DataTableA.Rows[positionA-1][myColumn.ToString()]);
                    string tempB = string.Format("{0}", DataTableB.Rows[positionB-1][myColumn.ToString()]);
                    if (pickedA)
                    {
                        if (pickedB)
                        {
                            if (CompareDifferentTypes(tempA, tempB))
                            {
                                if (pickedC)
                                {
                                    string tempC = string.Format("{0}", DataTableС.Rows[positionC - 1][myColumn.ToString()]);
                                    if (CompareDifferentTypes(tempA, tempC))
                                    {
                                        Movements.Add($"{tempA} < {tempC} и {tempA} < {tempB},\nзаписываем {tempA}  в таблицу");
                                        ChangeTable(DataTableA, positionA - 1, strSeriaA);
                                        AddRowInTable(DataTableA.Rows[positionA - 1], DataNewTable);
                                        counterA--;
                                        pickedA = false;
                                        //seriaA--;
                                        await Task.Delay(1010 - Slider);
                                    }
                                    else
                                    {
                                        Movements.Add($"{tempC} < {tempA} и {tempC} < {tempB}, записываем {tempC}  в таблицу");
                                        ChangeTable(DataTableС, positionC - 1, strSeriaC);
                                        AddRowInTable(DataTableС.Rows[positionC - 1], DataNewTable);
                                        counterC--;
                                        //seriaC--;
                                        pickedC = false;
                                        await Task.Delay(1010 - Slider);
                                    }
                                }
                                else
                                {
                                    Movements.Add($"записываем {tempA} <  {tempB}, записываем {tempA}  в таблицу");
                                    ChangeTable(DataTableA, positionA - 1, strSeriaA);
                                    AddRowInTable(DataTableA.Rows[positionA - 1], DataNewTable);
                                    counterA--;
                                    //seriaA--;
                                    pickedA = false;
                                    await Task.Delay(1010 - Slider);
                                }
                            }
                            else
                            {
                                if (pickedC)
                                {
                                    string tempC = string.Format("{0}", DataTableС.Rows[positionC - 1][myColumn.ToString()]);
                                    if (CompareDifferentTypes(tempB, tempC))
                                    {
                                        Movements.Add($"{tempB} < {tempA} и {tempB} < {tempC},\nзаписываем {tempB}  в таблицу");
                                        ChangeTable(DataTableB, positionB - 1, strSeriaB);
                                        AddRowInTable(DataTableB.Rows[positionB - 1], DataNewTable);
                                        counterB--;
                                        //seriaB--;
                                        pickedB = false;
                                        await Task.Delay(1010 - Slider);
                                    }
                                    else
                                    {
                                        Movements.Add($"{tempC} < {tempA} и {tempC} < {tempB},\nзаписываем {tempC}  в таблицу");
                                        ChangeTable(DataTableС, positionC - 1, strSeriaC);
                                        AddRowInTable(DataTableС.Rows[positionC - 1], DataNewTable);
                                        counterC--;
                                        //seriaC--;
                                        pickedC = false;
                                        await Task.Delay(1010 - Slider);
                                    }
                                }
                                else
                                {
                                    Movements.Add($"{tempB} < {tempA},\nзаписываем {tempB}  в таблицу");
                                    ChangeTable(DataTableB, positionB - 1, strSeriaB);
                                    AddRowInTable(DataTableB.Rows[positionB - 1], DataNewTable);
                                    counterB--;
                                    //seriaB--;
                                    pickedB = false;
                                    await Task.Delay(1010 - Slider);
                                }
                            }
                        }
                        else if (pickedC)
                        {
                            string tempC = string.Format("{0}", DataTableС.Rows[positionC - 1][myColumn.ToString()]);
                            if (CompareDifferentTypes(tempA, tempC))
                            {
                                Movements.Add($"{tempA} < {tempC}, записываем {tempA}  в таблицу");
                                ChangeTable(DataTableA, positionA - 1, strSeriaA);
                                AddRowInTable(DataTableA.Rows[positionA - 1], DataNewTable);
                                counterA--;
                                //seriaA--;
                                pickedA = false;
                                await Task.Delay(1010 - Slider);
                            }
                            else
                            {
                                Movements.Add($"{tempC} < {tempA}, записываем {tempC}  в таблицу");
                                ChangeTable(DataTableС, positionC - 1, strSeriaC);
                                AddRowInTable(DataTableС.Rows[positionC - 1], DataNewTable);
                                counterC--;
                                //seriaC--;
                                pickedC = false;
                                await Task.Delay(1010 - Slider);
                            }
                        }
                        else
                        {
                            Movements.Add($"записываем {tempA}  в таблицу");
                            ChangeTable(DataTableA, positionA - 1, strSeriaA);
                            AddRowInTable(DataTableA.Rows[positionA - 1], DataNewTable);
                            counterA--;
                            //seriaA--;
                            pickedA = false;
                            await Task.Delay(1010 - Slider);
                        }
                    }
                    else if (pickedB)
                    {
                        if (pickedC)
                        {
                            string tempC = string.Format("{0}", DataTableС.Rows[positionC - 1][myColumn.ToString()]);
                            if (CompareDifferentTypes(tempB, tempC))
                            {
                                Movements.Add($"{tempB} < {tempC}, записываем {tempB}  в таблицу");
                                ChangeTable(DataTableB, positionB - 1, strSeriaB);
                                AddRowInTable(DataTableB.Rows[positionB - 1], DataNewTable);
                                counterB--;
                                //seriaB--;
                                pickedB = false;
                                await Task.Delay(1010 - Slider);
                            }
                            else
                            {
                                Movements.Add($"{tempC} < {tempB}, записываем {tempC}  в таблицу");
                                ChangeTable(DataTableС, positionC - 1, strSeriaC);
                                AddRowInTable(DataTableС.Rows[positionC - 1], DataNewTable);
                                counterC--;
                                //seriaC--;
                                pickedC = false;
                                await Task.Delay(1010 - Slider);
                            }
                        }
                        else
                        {
                            Movements.Add($"записываем {tempB}  в таблицу");
                            ChangeTable(DataTableB, positionB - 1, strSeriaB);
                            AddRowInTable(DataTableB.Rows[positionB - 1], DataNewTable);
                            counterB--;
                            //seriaB--;
                            pickedB = false;
                            await Task.Delay(1010-Slider);
                        }
                    }
                    else if (pickedC)
                    {
                        string tempC = string.Format("{0}", DataTableС.Rows[positionC - 1][myColumn.ToString()]);
                        Movements.Add($"записываем {tempC}  в таблицу");
                        ChangeTable(DataTableС, positionC - 1, strSeriaC);
                        AddRowInTable(DataTableС.Rows[positionC - 1], DataNewTable);
                        counterC--;
                        //seriaC--;
                        pickedC = false;
                        await Task.Delay(1010 - Slider);
                    }
                    currentPA += positionA;
                    currentPB += positionB;
                    currentPC += positionC;
                }
                _iterations *= 3;
                DataTableA.Rows.Clear();
                DataTableB.Rows.Clear();
                DataTableС.Rows.Clear();
            }
            IsEnable = true;
            ChangeMainTable();
            FileRewriter.RewriteCSV(folderPath, selectedTable, selectedScheme);
        }
    }
   
}
