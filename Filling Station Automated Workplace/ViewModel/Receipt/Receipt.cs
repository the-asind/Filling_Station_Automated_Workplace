using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Filling_Station_Automated_Workplace.Model;

namespace Filling_Station_Automated_Workplace.ViewModel;

public class Receipt
{
    public NozzlePost RelateNozzlePost;
    public Payment PaymentType;
    public List<PositionInReceipt> CommodityItem;
    
    public Receipt()
    {
        this.RelateNozzlePost = new NozzlePost();
        this.PaymentType = new Payment();
        this.CommodityItem = new List<PositionInReceipt>();
    }
    
    public void AddIdToCommodityItem(int id)
    {
        // Check if the id exists in the CommodityItem list
        var position = this.CommodityItem.FirstOrDefault(x => x.Id == id);
        if (position is null)
        {
            this.CommodityItem.Add(new PositionInReceipt { Id = id, Count = 1});
        }
        else
        {
            // Increase the count of the existing position
            position.Count++;
        }
    }
    
    public void RemoveIdFromCommodityItem(int id)
    {
        // Check if the id exists in the CommodityItem list
        var position = this.CommodityItem.FirstOrDefault(x => x.Id == id);
        if (position != null)
        {
            // Remove the position from the CommodityItem list
            this.CommodityItem.Remove(position);
        }
    }

    
    public void ChangeCountById(int id, int count)
    {
        // Check if the id exists in the CommodityItem list
        var position = this.CommodityItem.FirstOrDefault(x => x.Id == id);
        if (position is null)
        {
            // Add a new position with the specified id and count of 1
            this.CommodityItem.Add(new PositionInReceipt { Id = id, Count = 1 });
        }
        else
        {
            // Increase the count of the existing position
            if (GoodsData.GetRemainingById(id) >= count)
            {
                position.Count = count;
            }
            else
                throw new ArgumentException(
                    "Количество товаров в корзине не может превышать доступное количество этих товаров в базе. Пожалуйста, введите допустимое количество и повторите попытку.");
        }
    }

    public double GetGoodsSummary()
    {
        double sum = CommodityItem.Sum(x => x.TotalCost);
        return sum;
    }
    
}