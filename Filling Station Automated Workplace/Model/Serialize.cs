using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using Filling_Station_Automated_Workplace.ViewModel;

namespace Filling_Station_Automated_Workplace.Model;

public static class Serialize
{
    public static void UpdateGoodsFile(ObservableCollection<ShoppingCartItem> receiptItems)
    {
        var goodsLines = File.ReadAllLines(Deserialize.csvFileDefault+"Goods.csv").ToList();

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
        File.WriteAllLines(Deserialize.csvFileDefault+"Goods.csv", goodsLines);
    }

    public static void UpdateTanksFile(NozzlePostViewModel selectedNozzlePostInstance)
    {
        // Read the contents of the "Tanks.csv" file into a string array
        string[] lines = File.ReadAllLines(Deserialize.csvFileDefault+"Tanks.csv");

        // Find the row with the corresponding "Id" (SelectedFuelId)
        string[]? fields = lines.FirstOrDefault(line => line.Split(';')[0] == selectedNozzlePostInstance.SelectedFuelId.ToString())?.Split(';');

        // If the row was found, update the "Reserve" field with the new value
        if (fields != null)
        {
            fields[2] = (double.Parse(fields[2], CultureInfo.InvariantCulture) - selectedNozzlePostInstance.LiterCount).ToString(CultureInfo.InvariantCulture);

            // Replace the old row with the updated row
            lines[Array.IndexOf(lines, lines.FirstOrDefault(line => line.Split(';')[0] == fields[0]))] = string.Join(";", fields);

            // Write the updated contents back to the "Tanks.csv" file
            File.WriteAllLines(Deserialize.csvFileDefault+"Tanks.csv", lines);
        }
    }

}