using System.Collections.Generic;

namespace DummyDB.Core
{
    public class Table
    {
        public TableScheme Scheme { get; set; }
        public List<Row> Rows { get; set; }
    }
}
