using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Filling_Station_Automated_Workplace.Data;
using Microsoft.VisualBasic.FileIO;

namespace Filling_Station_Automated_Workplace.Domain;

public static class Deserialize
{
    public static readonly string CsvFileDefault =
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Assets\";

    public static DataTable GetDataTableFromCsvFile(string csvFilePath)
    {
        var csvData = new DataTable();
        try
        {
            using var csvReader = new TextFieldParser(string.Concat(CsvFileDefault, csvFilePath));
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
        catch 
        {
            // ignored
        }

        return csvData;
    }

    public static double GetTankReserveById(int id)
    {
        // Read the data from Tanks.csv
        var csvData = File.ReadAllText(string.Concat(CsvFileDefault, "Tanks.csv"));

        var lines = csvData.Split('\n');

        // Find the line with the matching id
        foreach (var line in lines)
        {
            if (line == lines[0]) continue;

            var fields = line.Split(';');
            var lineId = int.Parse(fields[0]);
            if (lineId == id) return double.Parse(fields[2], CultureInfo.GetCultureInfo("en-US"));
        }

        // If no matching id was found, return 0
        return 0;
    }

    public static UsersData.Users DeserializeUsersData()
    {
        var fileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Assets\Users.xml";
        var serializer = new XmlSerializer(typeof(UsersData.Users));

        using var fs = new FileStream(fileName, FileMode.Open);
        var users = (UsersData.Users)serializer.Deserialize(fs)!;

        return users;
    }

    public static ConfigurationData DeserializeConfiguration()
    {
        var fileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Assets\Configuration.xml";
        var xmlSerializer = new XmlSerializer(typeof(ConfigurationData));

        using (var stream = new StreamReader(fileName))
        {
            return (ConfigurationData)xmlSerializer.Deserialize(stream)! ?? throw new InvalidOperationException();
        }
    }
}