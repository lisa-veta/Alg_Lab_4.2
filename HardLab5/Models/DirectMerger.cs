using DummyDB.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HardLab5.Models
{
    public class DirectMerger : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        //private int _slider = 500;
        //public int Slider
        //{
        //    get { return _slider; }
        //    set
        //    {
        //        _slider = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private bool _isEnable = true;
        //public bool IsEnable
        //{
        //    get { return _isEnable; }
        //    set
        //    {
        //        _isEnable = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public DirectMerger(bool isEnable, int slider)
        //{
        //    _isEnable = isEnable;
        //    _slider = slider;
        //}

        private void AddRowInTable(DataTable dataTable, DataRow newRow)
        {
            dataTable.Rows.Add(newRow.ItemArray);
        }

        public async void Sort(string selectedColumn, int _segments, int _iterations, DataTable dataNewTable, DataTable dataTableA, DataTable dataTableB, KeyValuePair<TableScheme, Table> keyTable)
        {
            //IsEnable = false;
            while (true)
            {
                _segments = 1;
                int counter = 0;
                bool flag = true;
                int counter1 = 0;
                foreach (DataRow row in dataNewTable.Rows)
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
                        AddRowInTable(dataTableA, row);
                        counter++;
                        await Task.Delay(1010 );
                    }
                    else
                    {
                        AddRowInTable(dataTableB, row);
                        counter++;
                        await Task.Delay(1010);
                    }
                    counter1++;

                }
                flag = true;

                if (_segments == 1)
                {
                    break;
                }

                dataNewTable.Rows.Clear();
                DataRow newRowA = dataNewTable.NewRow();
                DataRow newRowB = dataNewTable.NewRow();
                int counterA = _iterations;
                int counterB = _iterations;
                bool pickedA = false, pickedB = false, endA = false, endB = false;
                int positionA = 0;
                int positionB = 0;
                int currentPA = 0;
                int currentPB = _iterations;
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

                    if (positionA != dataTableA.Rows.Count)
                    {
                        if (counterA > 0)
                        {
                            if (!pickedA)
                            {
                                newRowA = dataTableA.Rows[positionA];
                                positionA += 1;
                                pickedA = true;
                            }
                        }
                    }
                    else
                    {
                        endA = true;
                    }

                    if (positionB != dataTableB.Rows.Count)
                    {
                        if (counterB > 0)
                        {
                            if (!pickedB)
                            {
                                newRowB = dataTableB.Rows[positionB];
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
                            DataColumn myColunm = dataNewTable.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName == selectedColumn);
                            int tempA = int.Parse(string.Format("{0}", newRowA[myColunm.ToString()]));
                            int tempB = int.Parse(string.Format("{0}", newRowB[myColunm.ToString()]));
                            if (tempA < tempB)
                            {
                                AddRowInTable(dataNewTable, newRowA);
                                counterA--;
                                pickedA = false;

                                await Task.Delay(1010);
                            }
                            else
                            {
                                AddRowInTable(dataNewTable, newRowB);
                                counterB--;
                                pickedB = false;
                            }
                        }
                        else
                        {
                            AddRowInTable(dataNewTable, newRowA);
                            counterA--;
                            pickedA = false;

                            await Task.Delay(1010);
                        }
                    }
                    else if (pickedB)
                    {
                        AddRowInTable(dataNewTable, newRowB);
                        counterB--;
                        pickedB = false;

                        await Task.Delay(1010);
                    }

                    currentPA += positionA;
                    currentPB += positionB;
                }
                _iterations *= 2; // увеличиваем размер серии в 2 раза
                dataTableA.Rows.Clear();
                dataTableB.Rows.Clear();
            }
            //IsEnable = true;
        }
    }
}
