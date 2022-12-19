using System;
using System.Data;
using Microsoft.VisualBasic.FileIO;

namespace Filling_Station_Automated_Workplace.Model;

public static class DataSerializer
{
    public static DataTable GetDataTableFromCsvFile(string csvFilePath)
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
}