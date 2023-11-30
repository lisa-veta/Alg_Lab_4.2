using System.Collections.Generic;
using System.Windows.Media;

namespace DummyDB.Core
{
    public class Row
    {
        public Dictionary<Column, object> Data { get; set; }
        public SolidColorBrush Color { get; set; } = Brushes.White;
        public Row()
        {
            Data = new Dictionary<Column, object>();
        }
    }
}
