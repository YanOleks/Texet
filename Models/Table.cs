using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Texet.Models;

internal class Table
{
    public string FileName { get; set; }
    public DataTable Data { get; set; }    

    public Table()
    {
        FileName = "table.csv";       
        Data = new DataTable();
    }

    public static Table Load()
    {
        string appDataPath = FileSystem.AppDataDirectory;
        string filename = System.IO.Path.Combine(appDataPath, "table.csv");

        if (!File.Exists(filename))
            throw new FileNotFoundException("Unable to find file on local storage.", filename);

        return
            new()
            {
                Data = ReadCSV(filename),
                FileName = Path.GetFileName(filename),
            };
    }
    public void Save() =>
        File.WriteAllText(System.IO.Path.Combine(FileSystem.AppDataDirectory, FileName), ConvertToCSVFormat());

    private string ConvertToCSVFormat()
    {
        if (Data.Rows.Count == 0 || Data.Columns.Count == 0) return "";
        StringBuilder sb = new StringBuilder();
        foreach (DataRow row in Data.Rows)
        {
            for (int i = 0; i < Data.Columns.Count; i++)
            {
                sb.Append($"\"{row[i]}\",");
            }
            sb.Remove(sb.Length - 1,1);
            sb.Append('\n');
        }
        return sb.ToString();
    }
    public static DataTable ReadCSV(string filename)
    {
        DataTable table = new DataTable();
        string[] lines = File.ReadAllLines(filename);

        if (lines.Length > 0)
        {
            int columnCount = lines[0].Split(',').Length;
            for (int i = 0; i < columnCount; i++)
            {
                table.Columns.Add($"Column{i + 1}");
            }

            foreach (string line in lines)
            {
                string[] data = ParseCSVLine(line);
                table.Rows.Add(data);
            }
        }

        return table;
    }

    private static string[] ParseCSVLine(string line)
    {
        var values = new List<string>();
        bool inQuotes = false;
        StringBuilder currentValue = new StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    currentValue.Append(c);
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                values.Add(currentValue.ToString());
                currentValue.Clear();
            }
            else
            {
                currentValue.Append(c);
            }
        }

        values.Add(currentValue.ToString());

        return values.ToArray();
    }



    public void AddColumn()
    {
        string columnName = $"Column{Data.Columns.Count + 1}";
        Data.Columns.Add(columnName);

        foreach (DataRow row in Data.Rows)
        {
            row[columnName] = string.Empty;
        }
    }

    public void AddRow()
    {
        DataRow newRow = Data.NewRow();

        foreach (DataColumn column in Data.Columns)
        {
            newRow[column.ColumnName] = string.Empty;
        }

        Data.Rows.Add(newRow);
    }

    public void RemoveColumn()
    {
        if (Data.Columns.Count > 0)
        {
            Data.Columns.RemoveAt(Data.Columns.Count - 1);
        }
    }

    public void RemoveRow()
    {
        if (Data.Rows.Count > 0)
        {
            Data.Rows.RemoveAt(Data.Rows.Count - 1);
        }
    }

    public void UpdateCell(int row, int column, string value)
    {
        if (row < Data.Rows.Count && column < Data.Columns.Count)
        {
            Data.Rows[row][column] = value;
        }
    }
}
