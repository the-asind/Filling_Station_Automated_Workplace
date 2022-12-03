using System;
using System.Collections.Generic;

namespace Filling_Station_Automated_Workplace.Model;

public struct PositionsInReceipt
{
    public List<CommodityItem> Commodity = new List<CommodityItem>();

    public PositionsInReceipt()
    {
    }

    public struct CommodityItem
    {
        private uint count = 1;

        public CommodityItem()
        {
        }

        public uint Id { get; set; } = 0;

        public uint Count {
            get => count;
            set
            {
                if (value <= 1) throw new ArgumentOutOfRangeException(nameof(value));
                count = value;
            }
        }
    
    }
}

