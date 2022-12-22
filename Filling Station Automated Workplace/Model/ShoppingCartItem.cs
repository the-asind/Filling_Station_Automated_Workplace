using System.Collections.Generic;
using System.Data;
using Filling_Station_Automated_Workplace.Model;
using Filling_Station_Automated_Workplace.ViewModel;

namespace Filling_Station_Automated_Workplace.Model;

public class ShoppingCartItem
{
    public string? Name { get; set; }
    public int Id { get; set; }
    public int Count { get; set; }
    public double TotalCost { get; set; }
    
    public static DataTable? Update(Receipt receipt)
    {
        List<PositionInReceipt> commodityItem = receipt.CommodityItem;
        DataTable? table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Count", typeof(int));
        table.Columns.Add("TotalCost", typeof(double));
        
        // Add each item in the List to the DataTable
        foreach (var item in commodityItem)
        {
            var (goodsName, goodsPrice) = GoodsModel.GetNameAndPriceById(item.Id);
            
            table.Rows.Add(item.Id, goodsName, item.Count, item.TotalCost);
            
        }

        return table;
    }
    
    public static IEnumerable<ShoppingCartItem> IUpdate(Receipt receipt)
    {
        List<PositionInReceipt> commodityItem = receipt.CommodityItem;
        List<ShoppingCartItem> shoppingCartItems = new List<ShoppingCartItem>();

        // Create a ShoppingCartItem object for each item in the List and add it to the collection
        foreach (var item in commodityItem)
        {
            var (goodsName, goodsPrice) = GoodsModel.GetNameAndPriceById(item.Id);

            var shoppingCartItem = new ShoppingCartItem
            {
                Name = goodsName,
                Id = item.Id,
                Count = item.Count,
                TotalCost = item.TotalCost
            };

            shoppingCartItems.Add(shoppingCartItem);
        }

        return shoppingCartItems;
    }
}
