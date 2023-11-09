using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using DummyDB.Core;
using HardLab5.ViewModels;

namespace HardLab5
{
    class ViewModelNewTable : BaseViewModel
    {
        public ObservableCollection<ViewModelNewTable> Items { get; } = new ObservableCollection<ViewModelNewTable>();

        public string folderPath;
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
        public ICommand CreateColumns => new DelegateCommand(param =>
        {
            Items.Add(new ViewModelNewTable());
        });
        public ICommand RemoveColumn => new DelegateCommand(param =>
        {
            if(Items.Count > 0)
            {
                Items.RemoveAt(Items.Count - 1);
            }
            return;
        });

        public ICommand CreateTable => new DelegateCommand(param =>
        {
            if (TableName == null || TableName == "" || Items.Count == 0 || CheckData())
            {
                MessageBox.Show("Данные не заполнены до конца");
                return;
            }
            if (CheckEqualsNames())
            {
                System.Windows.MessageBox.Show("Найдены столбцы с повторяющимися именами");
                return;
            }
            CreateNewTable();
        });

        private void CreateNewTable() 
        {
            string name = TableName;
            List<Column> columns = GetList();
            TableScheme tableScheme = new TableScheme
            {
                Name = name,
                Columns = columns
            };
            string newFile = ColumnAdder.AddColumnInNewTable(columns);
            FileRewriter.CreateFiles(folderPath, tableScheme, newFile);
            System.Windows.MessageBox.Show("Успешно!");
            Items.Clear();
            TableName = null;
        }

        private bool CheckEqualsNames()
        {
            for(int i = 0; i < Items.Count()-1; i++)
            {
                for(int j = i+1; j < Items.Count(); j++)
                {
                    if (Items[i].ColumnName == Items[j].ColumnName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private List<Column> GetList()
        {
            List<Column> columns = new List<Column>();
            foreach (var item in Items)
            {
                Column column = new Column();
                column.Name = item.ColumnName;
                column.Type = item.Type;
                column.IsPrimary = item.Primary;
                columns.Add(column);
            }
            return columns;
        }

        private bool CheckData()
        {
            foreach (var item in Items)
            {
                if (item.ColumnName == null || item.ColumnName == "" || item.Type == null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
