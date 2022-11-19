using System;
using System.Data;
using Microsoft.VisualBasic.FileIO;

namespace Filling_Station_Automated_Workplace.Model;

public class Goods
{
    public static readonly DataTable DataTable = GetDataTabletFromCsvFile(System.Reflection.Assembly.GetExecutingAssembly().Location + @"\Assets\Goods.csv");

    private Goods()
    {
        
        
        
    }

    private static DataTable GetDataTabletFromCsvFile(string csvFilePath)
    {
        var csvData = new DataTable();
        try
        {
            using TextFieldParser csvReader = new TextFieldParser(csvFilePath);
            csvReader.SetDelimiters(new string[] { ";" });
            csvReader.HasFieldsEnclosedInQuotes = true;
            var colFields = csvReader.ReadFields();
            if (colFields != null)
                foreach (var column in colFields)
                {
                    DataColumn dateColumn = new DataColumn(column);
                    dateColumn.AllowDBNull = true;
                    csvData.Columns.Add(dateColumn);
                }

            while (!csvReader.EndOfData)
            {
                //Making empty value as null
                if ((csvReader.ReadFields() ?? Array.Empty<string>()) is not { } fieldData) continue;
                for (var i = 0; i < fieldData.Length; i++)
                {
                    if (fieldData[i] == "")
                    {
                        fieldData[i] = null!;
                    }
                }

                csvData.Rows.Add(fieldData);
            }
        }
        catch (Exception ex)
        {
            // ignored
        }

        return csvData;
    }
}