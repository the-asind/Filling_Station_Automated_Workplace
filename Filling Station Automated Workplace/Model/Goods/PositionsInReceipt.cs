using System.Collections.Generic;

namespace Filling_Station_Automated_Workplace.Model;

public struct PositionsInReceipt
{
    public List<CommodityItem> Id = new List<CommodityItem>();

    public PositionsInReceipt()
    {
    }

    public struct CommodityItem
    {
        public int Id { get; set; }
        public int Count { get; set; }
    
    }
}

