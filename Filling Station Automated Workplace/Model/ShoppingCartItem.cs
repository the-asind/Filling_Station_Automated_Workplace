using System.Collections.Generic;
using System.Data;
using System.Linq;
using Filling_Station_Automated_Workplace.Domain;

namespace Filling_Station_Automated_Workplace.Model;

public class ShoppingCartItem
{
    public int Id { get; private init; }
    public int Count { get; private init; }

    public static DataTable Update(Receipt receipt)
    {
        var commodityItem = receipt.CommodityItem;
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Count", typeof(int));
        table.Columns.Add("TotalCost", typeof(double));

        // Add each item in the List to the DataTable
        foreach (var item in commodityItem)
        {
            var (goodsName, _) = GoodsModel.GetNameAndPriceById(item.Id);

            table.Rows.Add(item.Id, goodsName, item.Count, item.TotalCost);
        }

        return table;
    }

    public static IEnumerable<ShoppingCartItem> UpdateCart(Receipt receipt)
    {
        var commodityItem = receipt.CommodityItem;

        // Create a ShoppingCartItem object for each item in the List and add it to the collection

        return commodityItem.Select(item => new ShoppingCartItem { Id = item.Id, Count = item.Count }).ToList();
    }
}