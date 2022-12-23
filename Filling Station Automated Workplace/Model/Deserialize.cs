using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Filling_Station_Automated_Workplace.Data;
using Microsoft.VisualBasic.FileIO;

namespace Filling_Station_Automated_Workplace.Model;

public static class Deserialize
{
    public static DataTable GetDataTableFromCsvFile(string csvFilePath)
    {
        var csvData = new DataTable();
        var csvFileDefault = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Assets\";
        try
        {
            using var csvReader = new TextFieldParser(String.Concat(csvFileDefault, csvFilePath));
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

    public static UsersData.Users DeserializeUsersData()
    {
        var fileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Assets\Users.xml";
        XmlSerializer serializer = new XmlSerializer(typeof(UsersData.Users));
        UsersData.Users users;

        using (FileStream fs = new FileStream(fileName, FileMode.Open))
        {
            users = (UsersData.Users)serializer.Deserialize(fs)!;
        }

        return users;
    }
}