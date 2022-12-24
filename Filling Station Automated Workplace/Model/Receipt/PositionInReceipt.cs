using Filling_Station_Automated_Workplace.Data;
using Filling_Station_Automated_Workplace.Domain;
using Filling_Station_Automated_Workplace.Model;

namespace Filling_Station_Automated_Workplace.ViewModel;

public class PositionInReceipt
{
    public int Id { get; set; }
    public int Count { get; set; }

    public double TotalCost =>
        GoodsModel.GetPriceById(Id) * Count;
}