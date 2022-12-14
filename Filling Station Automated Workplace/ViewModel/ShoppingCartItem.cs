using System.Collections.Generic;
using System.Data;
using Filling_Station_Automated_Workplace.Model;

namespace Filling_Station_Automated_Workplace.ViewModel;

public static class ShoppingCartItem
{
    public static DataTable Update(Receipt receipt)
    {
        List<PositionsInReceipt> commodityItem = receipt.CommodityItem;
        DataTable table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Count", typeof(int));
        table.Columns.Add("Price", typeof(double));
        
        // Add each item in the List to the DataTable
        foreach (var item in commodityItem)
        {
            var (goodsName, goodsPrice) = GoodsData.GetNameAndPriceById(item.Id);
            var summary = goodsPrice * item.Count;
            
            table.Rows.Add(item.Id, goodsName, item.Count, summary);
            
        }

        return table;
    }
}
