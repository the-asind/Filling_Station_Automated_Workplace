using System.Collections.Generic;
using System.Data;

namespace Filling_Station_Automated_Workplace.ViewModel;

public static class ShoppingCartGoodsTable
{
    public static DataTable Update(Receipt receipt)
    {
        List<PositionsInReceipt> commodityItem = receipt.CommodityItem;
        // Create a new DataTable and set its columns
        DataTable table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Count", typeof(int));

        // Add each item in the List to the DataTable
        foreach (var item in commodityItem)
        {
            table.Rows.Add(item.Id, item.Count);
        }

        return table;
    }
}
