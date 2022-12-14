using System.Collections.Generic;
using System.Data;
using System.Linq;
using Filling_Station_Automated_Workplace.Model;

namespace Filling_Station_Automated_Workplace.ViewModel;

public class Receipt
{
    public NozzlePost RelateNozzlePost;
    public Payment PaymentType;
    public List<PositionsInReceipt> CommodityItem;
    
    public Receipt()
    {
        this.RelateNozzlePost = new NozzlePost();
        this.PaymentType = new Payment();
        this.CommodityItem = new List<PositionsInReceipt>();
        this.CommodityItem.Add(new PositionsInReceipt { Id = 23, Count = 1 });
    }
    
    public void AddIdToCommodityItem(int id)
    {
        // Check if the id exists in the CommodityItem list
        var position = this.CommodityItem.FirstOrDefault(x => x.Id == id);
        if (position is null)
        {
            // Add a new position with the specified id and count of 1
            this.CommodityItem.Add(new PositionsInReceipt { Id = id, Count = 1 });
        }
        else
        {
            // Increase the count of the existing position
            position.Count++;
        }

    }
    
}