using HardLab5.ViewModels;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
namespace HardLab5
{
    class ViewModelNewDB : BaseViewModel
    {
        public string folderPath;
        public WindowDB WindowDB { get; set; }

        public string folderPathDB = "";

        private string _newName;
        public string NewName
        {
            get { return _newName; }
            set
            {
                _newName = value;
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
        public ICommand CreateNewFile => new DelegateCommand(param =>
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();

            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                folderPathDB = openFolderDialog.SelectedPath;
            }
            folderPathDB += "\\" + NewName;
            Message = "Путь к папке:\n" + folderPathDB;
        });

        public ICommand CreateDB => new DelegateCommand(param =>
        {
            if (folderPathDB == "" || NewName == "" || NewName == null)
            {
                MessageBox.Show("Вы не ввели имя БД или не выбрали путь!");
                return;
            }
            CreateNewDB();
        });

        public ICommand UndoAction => new DelegateCommand(param =>
        {
            folderPath = null;
            ClearData();
        });

        private void CreateNewDB()
        {
            Directory.CreateDirectory(folderPathDB);
            MessageBox.Show("Папка для новой БД успешно создана! " + Message + "\n");
            NewName = "";
            string folderName = folderPathDB.Split('\\')[folderPathDB.Split('\\').Length - 1];
            ((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Items.Clear();
            ((MainWindow)System.Windows.Application.Current.MainWindow).folderTree.Header = folderName;
            folderPath = folderPathDB;
            MainViewModel.folderPath = folderPath;
            ClearData();
            WindowDB.Close();
        }

        private void ClearData()
        {
            folderPathDB = null;
            NewName = null;
            Message = null;
        }
    }
}
