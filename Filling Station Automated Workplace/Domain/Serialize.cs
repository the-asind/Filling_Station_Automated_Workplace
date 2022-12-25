using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Filling_Station_Automated_Workplace.Data;
using Filling_Station_Automated_Workplace.Model;
using Filling_Station_Automated_Workplace.ViewModel;

namespace Filling_Station_Automated_Workplace.Domain;

public static class Serialize
{
    public static void UpdateGoodsFile(ObservableCollection<ShoppingCartItem> receiptItems)
    {
        var goodsLines = File.ReadAllLines(ConfigurationData.CsvFileDefault+"Goods.csv").ToList();

        // Loop through each item in the ReceiptItems collection
        foreach (var item in receiptItems)
        {
            var line = goodsLines.FirstOrDefault(l => l.StartsWith(item.Id.ToString()));

            if (line != null)
            {
                var fields = line.Split(';');

                fields[2] = (int.Parse(fields[2]) - item.Count).ToString();

                goodsLines[goodsLines.IndexOf(line)] = string.Join(";", fields);
            }
        }

        // Save the updated list of lines back to the Goods.csv file
        File.WriteAllLines(ConfigurationData.CsvFileDefault+"Goods.csv", goodsLines);
    }

    public static void UpdateTanksFile(NozzlePostViewModel selectedNozzlePostInstance)
    {
        string[] lines = File.ReadAllLines(ConfigurationData.CsvFileDefault+"Tanks.csv");
        string[]? fields = lines.FirstOrDefault(line => line.Split(';')[0] == selectedNozzlePostInstance.SelectedFuelId.ToString())?.Split(';');

        // If the row was found, update the "Reserve" field with the new value
        if (fields != null)
        {
            fields[2] = (double.Parse(fields[2], CultureInfo.InvariantCulture) - selectedNozzlePostInstance.LiterCount).ToString(CultureInfo.InvariantCulture);
            lines[Array.IndexOf(lines, lines.FirstOrDefault(line => line.Split(';')[0] == fields[0]))] = string.Join(";", fields);

            // Write the updated contents back to the "Tanks.csv" file
            File.WriteAllLines(ConfigurationData.CsvFileDefault+"Tanks.csv", lines);
        }
    }
    
    public static void WriteDataTableToCsvWithAutoGeneratedId(DataTable dataTable, string filePath)
    {
        // Create a string builder to store the CSV data
        StringBuilder csvBuilder = new StringBuilder();
        csvBuilder.AppendLine(string.Join(";", dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName)));

        // Add the rows to the CSV
        int id = 0;
        foreach (DataRow row in dataTable.Rows)
        {
            row["ID"] = id;
            id++;
            csvBuilder.AppendLine(string.Join(";", row.ItemArray));
        }

        // Write the CSV to the file
        File.WriteAllText(ConfigurationData.CsvFileDefault + filePath, csvBuilder.ToString());
    }


    public static void WriteDataTableToCsv(DataTable dataTable, string filePath)
    {
        // Create a string builder to store the CSV data
        StringBuilder csvBuilder = new StringBuilder();
        csvBuilder.AppendLine(string.Join(";", dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName)));

        foreach (DataRow row in dataTable.Rows)
        {
            csvBuilder.AppendLine(string.Join(";", row.ItemArray));
        }

        // Write the CSV to the file
        File.WriteAllText(ConfigurationData.CsvFileDefault + filePath, csvBuilder.ToString());
    }
    
    public static void SerializeNozzlePostCount(int nozzlePostCount)
    {
        XDocument doc = XDocument.Load(ConfigurationData.CsvFileDefault + "Configuration.xml");
        if (doc.Root != null)
        {
            XElement nozzlePostCountElement = doc.Root.Element("NozzlePostCount") ?? throw new InvalidOperationException();
            nozzlePostCountElement.Value = nozzlePostCount.ToString();
        }
        else
        {
            throw new InvalidOperationException();
        }

        doc.Save(ConfigurationData.CsvFileDefault + "Configuration.xml");
    }


}