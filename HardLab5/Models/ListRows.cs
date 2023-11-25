using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HardLab5.Models
{
    public class ListRows
    {
        public List<DataRow> ListOfRows {get; set;}
        public SolidColorBrush Color { get; set; } = Brushes.White;
    }
}
