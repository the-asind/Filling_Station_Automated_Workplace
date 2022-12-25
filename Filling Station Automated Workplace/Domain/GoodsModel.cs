using System;
using System.Globalization;
using System.Windows.Data;
using Filling_Station_Automated_Workplace.Data;

namespace Filling_Station_Automated_Workplace.Domain;

public static class GoodsModel
{
    public static (string?, double) GetNameAndPriceById(int id)
    {
        // Create an instance of the GoodsData class
        var goodsData = new GoodsData();

        // Find the row in the GoodsDataTable with the corresponding Id
        var row = goodsData.GoodsDataTable.Rows.Find(id);

        // Return the goods data from the row, or null if no matching row was found
        return row != null
            ? (row["Name"].ToString(),
                double.Parse(row["Price"].ToString() ?? throw new InvalidOperationException(), 
                    NumberStyles.AllowDecimalPoint, 
                    CultureInfo.InvariantCulture))
            : throw new ValueUnavailableException($"Не удалось найти {id}");
    }

    public static double GetPriceById(int id)
    {
        // Create an instance of the GoodsData class
        var goodsData = new GoodsData();
        
        // Find the row in the DataTable with the matching Id
        var row = goodsData.GoodsDataTable.Rows.Find(id);

        // Return the goods goodsData from the row, or null if no matching row was found
        return row != null
            ? (double.Parse(row["Price"].ToString() ?? throw new InvalidOperationException(), 
                NumberStyles.AllowDecimalPoint, 
                CultureInfo.InvariantCulture))
            : throw new ValueUnavailableException($"Не удалось найти {id}");
    }
    
    public static int GetRemainingById(int id)
    {
        // Create an instance of the GoodsData class
        var goodsData = new GoodsData();
        
        // Find the row in the DataTable with the matching Id
        var row = goodsData.GoodsDataTable.Rows.Find(id);

        // Return the goods goodsData from the row, or null if no matching row was found
        return row != null
            ? int.Parse(row["Count"].ToString() ?? throw new InvalidOperationException())
            : throw new ValueUnavailableException($"Не удалось найти {id}");
    }
}