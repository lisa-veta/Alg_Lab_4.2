using DummyDB.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HardLab5
{
    public class FileRewriter
    {
        public static void RewriteCSV(string folderPath, Table selectedTable, TableScheme selectedScheme)
        {
            string pathTable = folderPath + $"\\{selectedScheme.Name}.csv";
            int count = 1;
            StringBuilder newFile = new StringBuilder();
            foreach (var row in selectedTable.Rows)
            {
                foreach (Column column in row.Data.Keys)
                {
                    if (count == selectedScheme.Columns.Count)
                    {
                        newFile.Append($"{row.Data[column]}" + "\n");
                        break;
                    }
                    newFile.Append($"{row.Data[column]}" + ";");
                    count += 1;
                }
                count = 1;
            }
            File.WriteAllText(pathTable, newFile.ToString());
        }

        public static void RewriteJson(string folderPath, TableScheme selectedScheme)
        {
            string jsonNewScheme = JsonSerializer.Serialize(selectedScheme);
            File.WriteAllText(folderPath + $"\\{selectedScheme.Name}.json", jsonNewScheme);
        }

        public static void CreateFiles(string folderPath, TableScheme tableScheme, string newFile)
        {
            string pathScheme = folderPath + $"\\{tableScheme.Name}.json";
            string pathTable = folderPath + $"\\{tableScheme.Name}.csv";
            string jsonNewScheme = JsonSerializer.Serialize<TableScheme>(tableScheme);

            File.WriteAllText(pathScheme, jsonNewScheme);
            File.WriteAllText(pathTable, newFile);
        }

        public static void RewriteFileName(string folderPath, TableScheme selectedScheme, string tableName)
        {
            File.Move(folderPath + $"\\{selectedScheme.Name}.json", folderPath + $"\\{tableName}.json");
            File.Move(folderPath + $"\\{selectedScheme.Name}.csv", folderPath + $"\\{tableName}.csv");
            selectedScheme.Name = tableName;
        }
    }
}
