using System;
using System.Collections.Generic;
using System.Linq;
using Filling_Station_Automated_Workplace.Data;
using Filling_Station_Automated_Workplace.Model;

namespace Filling_Station_Automated_Workplace.ViewModel;

public class Receipt
{
    public NozzlePostViewModel? RelateNozzlePost;
    public Payment PaymentType;
    public List<PositionInReceipt> CommodityItem;

    public Receipt()
    {
        PaymentType = new Payment();
        CommodityItem = new List<PositionInReceipt>();
    }

    public void AddIdToCommodityItem(int id)
    {
        // Check if the id exists in the CommodityItem list
        var position = CommodityItem.FirstOrDefault(x => x.Id == id);
        if (position is null)
            CommodityItem.Add(new PositionInReceipt { Id = id, Count = 1 });
        else
            // Increase the count of the existing position
            position.Count++;
    }

    public void RemoveIdFromCommodityItem(int id)
    {
        // Check if the id exists in the CommodityItem list
        var position = CommodityItem.FirstOrDefault(x => x.Id == id);
        if (position != null)
            // Remove the position from the CommodityItem list
            CommodityItem.Remove(position);
    }
    
    public void ClearCommodityItem()
    {
        CommodityItem.Clear();
    }
    
    public void ChangeCountById(int id, int count)
    {
        // Check if the id exists in the CommodityItem list
        var position = CommodityItem.FirstOrDefault(x => x.Id == id);
        if (position is null)
        {
            // Add a new position with the specified id and count of 1
            CommodityItem.Add(new PositionInReceipt { Id = id, Count = 1 });
        }
        else
        {
            if (count == 0) RemoveIdFromCommodityItem(id);
            // Change the count of the existing position
            if (GoodsModel.GetRemainingById(id) >= count)
                position.Count = count;
            else
                throw new ArgumentException(
                    "Количество товаров в корзине не может превышать доступное количество этих товаров в базе. Пожалуйста, введите допустимое количество и повторите попытку.");
        }
    }

    public double GetGoodsSummary()
    {
        var sum = CommodityItem.Sum(x => x.TotalCost);
        return sum;
    }
    
    public string TextGoodsSummary => (CommodityItem.Sum(x => x.TotalCost)).ToString("C2");
}