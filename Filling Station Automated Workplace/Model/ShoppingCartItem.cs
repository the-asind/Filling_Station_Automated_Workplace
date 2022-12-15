using System.Collections.Generic;
using System.Data;
using Filling_Station_Automated_Workplace.Model;
using Filling_Station_Automated_Workplace.ViewModel;

namespace Filling_Station_Automated_Workplace.Model;

public static class ShoppingCartItem
{
    public static DataTable Update(Receipt receipt)
    {
        List<PositionInReceipt> commodityItem = receipt.CommodityItem;
        DataTable table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Count", typeof(int));
        table.Columns.Add("TotalCost", typeof(double));
        
        // Add each item in the List to the DataTable
        foreach (var item in commodityItem)
        {
            var (goodsName, goodsPrice) = GoodsData.GetNameAndPriceById(item.Id);
            
            table.Rows.Add(item.Id, goodsName, item.Count, item.TotalCost);
            
        }

        return table;
    }
}
