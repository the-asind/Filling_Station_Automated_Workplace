using System;
using System.Data;
using System.IO;
using System.Reflection;
using Filling_Station_Automated_Workplace.View;
using Microsoft.VisualBasic.FileIO;

namespace Filling_Station_Automated_Workplace.Model;

public class Receipt
{
    public NozzlePost RelateNozzlePost = new NozzlePost();
    public Payment PaymentType = new Payment();
    public PositionsInReceipt CommodityItem = new PositionsInReceipt();
    
    private Receipt(int paymentId, int nozzlePostId=-1)
    {
        RelateNozzlePost.Id = nozzlePostId;
        PaymentType.Id = paymentId;
        

    }
 
    
}