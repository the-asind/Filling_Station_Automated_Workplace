using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Data;
using Microsoft.VisualBasic.FileIO;

namespace Filling_Station_Automated_Workplace.Model;

public static class GoodsData
{
    public static DataTable GoodsDataTable { get; } = GetDataTabletFromCsvFile(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Assets\Goods.csv");

    static GoodsData()
    {
        DataColumn[] primaryKeyColumns = { GoodsDataTable.Columns["ID"] };
        GoodsDataTable.PrimaryKey = primaryKeyColumns;
    }


    private static DataTable GetDataTabletFromCsvFile(string csvFilePath)
    {
        var csvData = new DataTable();
        try
        {
            using var csvReader = new TextFieldParser(csvFilePath);
            csvReader.SetDelimiters(";");
            csvReader.HasFieldsEnclosedInQuotes = true;
            var colFields = csvReader.ReadFields();
            if (colFields != null)
                foreach (var column in colFields)
                {
                    var dateColumn = new DataColumn(column);
                    dateColumn.AllowDBNull = true;
                    csvData.Columns.Add(dateColumn);
                }

            while (!csvReader.EndOfData)
            {
                //Making empty value as null
                if ((csvReader.ReadFields() ?? Array.Empty<string>()) is not { } fieldData) continue;
                for (var i = 0; i < fieldData.Length; i++)
                    if (fieldData[i] == "")
                        fieldData[i] = null!;

                csvData.Rows.Add(fieldData);
            }
        }
        catch (Exception ex)
        {
            // ignored
        }

        return csvData;
    }

    public static (string?, double?) GetNameAndPriceById(int id)
    {
        // Find the row in the DataTable with the matching Id
        var row = GoodsDataTable.Rows.Find(id);

        // Return the goods data from the row, or null if no matching row was found
        return row != null
            ? (row["Name"].ToString(),
                double.Parse(row["Price"].ToString(), 
                    NumberStyles.AllowDecimalPoint, 
                    CultureInfo.InvariantCulture))
            : throw new ValueUnavailableException($"Не удалось найти {id}");
    }
    
    public static double GetPriceById(int id)
    {
        // Find the row in the DataTable with the matching Id
        var row = GoodsDataTable.Rows.Find(id);

        // Return the goods data from the row, or null if no matching row was found
        return row != null
            ? (double.Parse(row["Price"].ToString(), 
                    NumberStyles.AllowDecimalPoint, 
                    CultureInfo.InvariantCulture))
            : throw new ValueUnavailableException($"Не удалось найти {id}");
    }
    
    public static int GetRemainingById(int id)
    {
        // Find the row in the DataTable with the matching Id
        var row = GoodsDataTable.Rows.Find(id);

        // Return the goods data from the row, or null if no matching row was found
        return row != null
            ? int.Parse(row["Count"].ToString())
            : throw new ValueUnavailableException($"Не удалось найти {id}");
    }
}