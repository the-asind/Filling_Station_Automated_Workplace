using Filling_Station_Automated_Workplace.Domain;

namespace Filling_Station_Automated_Workplace.Model;

public class PositionInReceipt
{
    public int Id { get; set; }
    public int Count { get; set; }

    public double TotalCost =>
        GoodsModel.GetPriceById(Id) * Count;
}