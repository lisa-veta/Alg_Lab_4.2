﻿using DummyDB.Core;
using HardLab5.Models;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public System.Windows.Controls.DataGrid DataGrid { get; set; }
        public System.Windows.Controls.DataGrid DataGrid1 { get; set; }
        public System.Windows.Controls.DataGrid DataGrid2 { get; set; }
        public System.Windows.Controls.DataGrid DataGrid3 { get; set; }

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
        public DataTable DataTableC
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
                case "многопутевое слияние":
                    break;
                default:
                    MessageBox.Show("Выберите сортировку");
                    break;
            }
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
            IsEnable = false;
            while (true)
            {
                _segments = 1;
                int counter = 0;
                bool flag = true;
                int counter1 = 0;
                foreach (DataRow row in DataNewTable.Rows)
                {
                    // если достигли количества элементов в последовательности -
                    // меняем флаг для след. файла и обнуляем счетчик количества
                    if (counter == _iterations)
                    {
                        flag = !flag;
                        counter = 0;
                        _segments++;
                    }
                    if (flag)
                    {
                       
                        AddRowInTable(row, DataTableA);
                        counter++;
                        await Task.Delay(1010 - Slider);
                    }
                    else
                    {
                        AddRowInTable(row, DataTableB);
                        counter++;
                        await Task.Delay(1010 - Slider);
                    }
                    counter1++;

                }
                flag = true;
                
                if (_segments == 1)
                {
                    break;
                }

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

                    if (endA && endB && pickedA == false && pickedB == false)
                    {
                        break;
                    }
                    if (pickedA)
                    {
                        if (pickedB)
                        {
                            DataColumn myColunm = DataNewTable.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == SelectedColumn);
                            string tempA = string.Format("{0}", newRowA[myColunm.ToString()]);
                            string tempB = string.Format("{0}", newRowB[myColunm.ToString()]);
                            if (CompareDifferentTypes(tempA, tempB))
                            {
                                AddRowInTable(newRowA, DataNewTable);
                                counterA--;
                                pickedA = false;

                                await Task.Delay(1010 - Slider);
                            }
                            else
                            {
                                AddRowInTable(newRowB, DataNewTable);
                                counterB--;
                                pickedB = false;
                            }
                        }
                        else
                        {
                            AddRowInTable(newRowA, DataNewTable);
                            counterA--;
                            pickedA = false;

                            await Task.Delay(1010 - Slider);
                        }
                    }
                    else if (pickedB)
                    {
                        AddRowInTable(newRowB, DataNewTable);
                        counterB--;
                        pickedB = false;

                        await Task.Delay(1010 - Slider);
                    }

                    currentPA += positionA;
                    currentPB += positionB;
                }
                _iterations *= 2; // увеличиваем размер серии в 2 раза
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
            DataTable dataTable1 = new DataTable();
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable1.Columns.Add(column.Name);
            }
            DataTableB.Rows.Clear();
            DataTableB = dataTable1;
            NativeOuterSort();
        }
        
        //iterations - сколько элементов должно быть в каждом сегменте данных
        private List<int> _series = new List<int>();

        async void NativeOuterSort()
        {
            IsEnable = false;
            while (true)
            {
                _segments = 1;
                DataRow prev = DataNewTable.Rows[0];
                bool flag = true;
                bool flagf = true;
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
                        flag = !flag;
                        _segments++;
                        _series.Add(counter + 1);
                        counter = 0;
                    }
                    if (flag)
                    {
                        AddRowInTable(cur, DataTableA);
                        await Task.Delay(1010 - Slider);
                        counter++;
                    }
                    else
                    {
                        AddRowInTable(cur, DataTableB);
                        await Task.Delay(1010 - Slider);
                        counter++;
                    }
                    prev = cur;
                }

                if (_segments == 1)
                {
                    break;
                }

                DataNewTable.Rows.Clear();
                DataRow newRowA = DataNewTable.NewRow();
                DataRow newRowB = DataNewTable.NewRow();
                DataRow newRowC = DataNewTable.NewRow();

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
                    string tempC = string.Format("{0}", newRowC[myColumn.ToString()]);
                    if (pickedA)
                    {
                        if (pickedB)
                        {
                            if (CompareDifferentTypes(tempA, tempB))
                            {
                                AddRowInTable(newRowA, DataNewTable);
                                pickedA = false;
                                await Task.Delay(1010 - Slider);
                            }
                            else
                            {
                                AddRowInTable(newRowB, DataNewTable);
                                pickedB = false;
                                await Task.Delay(1010 - Slider);
                            }
                        }
                        else
                        {
                            AddRowInTable(newRowA, DataNewTable);
                            pickedA = false;
                            await Task.Delay(1010 - Slider);
                        }
                    }
                    else if (pickedB)
                    {
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
        //async void NativeOuterSort()
        //{
        //    IsEnable = false;
        //    while (true)
        //    {
        //        bool flag = true;
        //        int counter = 0; //элементы в серии
        //        DataColumn myColumn = DataNewTable.Columns.Cast<DataColumn>()
        //            .SingleOrDefault(col => col.ColumnName == SelectedColumn);
        //        for (int i = 0; i < DataNewTable.Rows.Count - 1; i++)
        //        {
        //            var tempFlag = flag;
        //            DataRow row1 = DataNewTable.Rows[i];
        //            DataRow row2 = DataNewTable.Rows[i + 1];
        //            string line1 = string.Format("{0}", row1[myColumn.ToString()]);
        //            string line2 = string.Format("{0}", row2[myColumn.ToString()]);
        //            if (CompareDifferentTypes(line1, line2)) //если 1<2 true
        //            {
        //                counter++;
        //            }
        //            else
        //            {
        //                tempFlag = !tempFlag;
        //                _series.Add(counter + 1);
        //                counter = 0;
        //            }

        //            if (flag)
        //            {
        //                AddRowInTable(row1, DataTableA);
        //            }
        //            else
        //            {
        //                AddRowInTable(row1, DataTableB);
        //            }

        //            flag = tempFlag;
        //        }
        //        _series.Add(counter + 1); //распределение по таблицам завершено

        //        //слияние таблиц
        //        DataNewTable.Rows.Clear();
        //        DataRow newRowA = DataTableA.NewRow();
        //        DataRow newRowB = DataTableB.NewRow();

        //        var indexA = 0;
        //        var indexB = 1;
        //        var counterA = 0;
        //        var counterB = 0;
        //        int indForA = 0;
        //        int indForB = 0;

        //        newRowA = DataTableA.Rows[indForA]; //строка из а
        //        newRowB = DataTableB.Rows[indForB]; //строка из б
        //        string elementA = string.Format("{0}", newRowA[myColumn.ToString()]);
        //        string elementB = string.Format("{0}", newRowB[myColumn.ToString()]);

        //        while (DataTableA.Rows[indForA] != null || DataTableB.Rows[indForB] != null)
        //        {
        //            if (counterA == _series[indexA] && counterB == _series[indexB])
        //            {
        //                counterA = 0;
        //                counterB = 0;
        //                indexA += 2;
        //                indexB += 2;
        //                continue;
        //            }

        //            if (indexA == _series.Count || counterA == _series[indexA])
        //            {
        //                newRowB = DataTableB.Rows[indForB];
        //                AddRowInTable(newRowB, DataNewTable);
        //                indForB++;
        //                counterB++;
        //                continue;
        //            }

        //            if (indexB == _series.Count || counterB == _series[indexB])
        //            {
        //                newRowA = DataTableA.Rows[indForA];
        //                AddRowInTable(newRowA, DataNewTable);
        //                indForA++;
        //                counterA++;
        //                continue;
        //            }

        //            if (CompareDifferentTypes(elementA, elementB))
        //            {
        //                newRowA = DataTableA.Rows[indForA];
        //                AddRowInTable(newRowA, DataNewTable);
        //                indForA++;
        //                counterA++;
        //            }
        //            else
        //            {
        //                newRowB = DataTableB.Rows[indForB];
        //                AddRowInTable(newRowB, DataNewTable);
        //                indForB++;
        //                counterB++;
        //            }
        //        }
        //    }
        //}
    }
}
