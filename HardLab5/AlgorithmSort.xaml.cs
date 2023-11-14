using HardLab5.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HardLab5
{
    /// <summary>
    /// Логика взаимодействия для AlgorithmSort.xaml
    /// </summary>
    public partial class AlgorithmSort : Window
    {
        public AlgorithmSort()
        {
            InitializeComponent();
        }

        public ViewModelSorting ViewModelSorting
        {
            get => default;
            set
            {
            }
        }
    }
}
