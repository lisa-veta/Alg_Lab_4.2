using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HardLab5.Models
{
    class RowToBrushConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataRowView dataRowView = value as DataRowView;
            string status = (string)dataRowView[0];
            switch (status)
            {
                case "seria1":
                    return Brushes.LightBlue;
                case "seria2":
                    return Brushes.LightYellow;
                case "current":
                    return Brushes.LightCoral;
            }
            if (targetType != typeof(Brush))
            {
                return null;
            }
            return Brushes.White;
        }
    }
}
