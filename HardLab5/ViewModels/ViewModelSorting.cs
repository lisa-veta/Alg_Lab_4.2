using DummyDB.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

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

        private int _iterations = 1;
        private int _segments;

        public ICommand Start => new DelegateCommand(param =>
        {
            //CreateTwoTables();
            DataTable dataTable = new DataTable();
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable.Columns.Add(column.Name);
            }
            DataTableA = dataTable;
            DataTable dataTable1 = new DataTable();
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable1.Columns.Add(column.Name);
            }
            DataTableB = dataTable1;
            Sort();
        });

        private void CreateTwoTables()
        {
            //СreateDataTable(1, 2, true);
            //СreateDataTable(0, 2, false);
            Thread.Sleep(1000);
        }


        private void СreateDataTable(int temp, int step, bool flag)
        {
            DataTable dataTable = new DataTable();
            selectedScheme = keyTable.Key;
            selectedTable = keyTable.Value;
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable.Columns.Add(column.Name);
            }

            for (int i = temp; i < keyTable.Value.Rows.Count; i += step)
            {
                DataRow newRow = dataTable.NewRow();
                foreach (var element in keyTable.Value.Rows[i].Data)
                {
                    newRow[element.Key.Name] = element.Value;
                }
                dataTable.Rows.Add(newRow);
            }
            if (flag)
            {
                DataTableA = dataTable;
            }
            else
            {
                DataTableB = dataTable;
            }
        }

        private void AddRowInTable(DataRow newRow, char table)
        {
            if(table == 'A')
            {
                //DataRow newRow = DataTableA.NewRow();
                //foreach (var element in keyTable.Value.Rows[i].Data)
                //{
                //    newRow[element.Key.Name] = element.Value;
                //}
                //DataTableA.Rows.Add(newRow);
                DataTableA.Rows.Add(newRow.ItemArray);
            }
            else
            {
                //DataRow newRow = DataTableA.NewRow();
                //foreach (var element in keyTable.Value.Rows[i].Data)
                //{
                //    newRow[element.Key.Name] = element.Value;
                //}
                //DataTableA.Rows.Add(newRow);
                DataTableB.Rows.Add(newRow.ItemArray);
            }
        }

         void Sort()
        {
            while (true)
            {
                SplitToFiles();
                // суть сортировки заключается в распределении на
                // отсортированные последовательности.
                // если после распределения на 2 вспомогательных файла
                // выясняется, что последовательность одна, значит файл
                // отсортирован, завершаем работу.
                
                if (_segments == 1)
                {
                    break;
                }
                MergePairs();
            }
        }

         void SplitToFiles() // разделение на 2 вспом. файла
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
                    AddRowInTable(row, 'A');
                    counter++;
                }
                else
                {
                    AddRowInTable(row, 'B');
                    counter++;
                }
                counter1++;
            }
            flag = true;
        }

        private void MergePairs() // слияние отсорт. последовательностей обратно в файл
        {
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
            DataTable dataTable = new DataTable();
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable.Columns.Add(column.Name);
            }
            //DataTableB = dataTable;
            while (true)
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
                        //int tempA = newRowA.Field<int>(myColunm);
                        int tempA = int.Parse(string.Format("{0}", newRowA[myColunm.ToString()]));
                        int tempB = int.Parse(string.Format("{0}", newRowB[myColunm.ToString()]));
                        //int tempB = newRowB.Field<int>(myColunm);
                        //foreach (var rows in keyTable.Value.Rows[currentPA].Data)
                        //{
                        //    if(rows.Key.Name == SelectedColumn)
                        //    {
                        //        tempA = int.Parse(string.Format("{0}", rows.Value));
                        //    }
                        //}
                        //foreach (var rows in keyTable.Value.Rows[currentPB].Data)
                        //{
                        //    if (rows.Key.Name == SelectedColumn)
                        //    {
                        //        tempB = int.Parse(string.Format("{0}", rows.Value));
                        //    }
                        //}
                        if (tempA < tempB)
                        {
                            dataTable.Rows.Add(newRowA.ItemArray);
                            counterA--;
                            pickedA = false;
                        }
                        else
                        {
                            dataTable.Rows.Add(newRowB.ItemArray);
                            counterB--;
                            pickedB = false;
                        }
                    }
                    else
                    {
                        dataTable.Rows.Add(newRowA.ItemArray);
                        counterA--;
                        pickedA = false;
                    }
                }
                else if (pickedB)
                {
                    dataTable.Rows.Add(newRowB.ItemArray);
                    counterB--;
                    pickedB = false;
                }
                currentPA += positionA;
                currentPB += positionB;
            }
            _iterations *= 2; // увеличиваем размер серии в 2 раза
            DataNewTable = dataTable;
            DataTableA.Rows.Clear();
            DataTableB.Rows.Clear();
        }


        private void MergePairs1() // слияние отсорт. последовательностей обратно в файл
        {
            DataNewTable.Rows.Clear();
            int counterA = _iterations;
            int counterB = _iterations;
            bool pickedA = false, pickedB = false, endA = false, endB = false;
            int positionA = 0;
            int positionB = 0;
            DataRow newRowA = DataTableA.Rows[0];
            DataRow newRowB = DataTableA.Rows[0];
            DataTable dataTable = new DataTable();
            foreach (Column column in keyTable.Key.Columns)
            {
                dataTable.Columns.Add(column.Name);
            }
            while (!endA || !endB)
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
                        int tempA = 0;
                        int tempB = 0;
                        foreach (var rows in keyTable.Value.Rows[positionA-1].Data)
                        {
                            if (rows.Key.Name == SelectedColumn)
                            {
                                tempA = int.Parse(string.Format("{0}", rows.Value));
                            }
                        }
                        foreach (var rows in keyTable.Value.Rows[positionB-1].Data)
                        {
                            if (rows.Key.Name == SelectedColumn)
                            {
                                tempB = int.Parse(string.Format("{0}", rows.Value)); ;
                            }
                        }
                        if (tempA < tempB)
                        {
                            dataTable.Rows.Add(newRowA.ItemArray);
                            counterA--;
                            pickedA = false;
                        }
                        else
                        {

                            dataTable.Rows.Add(newRowB.ItemArray);
                            counterB--;
                            pickedB = false;
                        }
                    }
                    else
                    {
                        dataTable.Rows.Add(newRowA.ItemArray);
                        counterA--;
                        pickedA = false;
                    }
                }
                else if (pickedB)
                {
                    dataTable.Rows.Add(newRowB.ItemArray);
                    counterB--;
                    pickedB = false;
                }

            }
            _iterations *= 2; // увеличиваем размер серии в 2 раза
            DataNewTable = dataTable;
            DataTableA.Rows.Clear();
            DataTableB.Rows.Clear();
        }
    }
}
