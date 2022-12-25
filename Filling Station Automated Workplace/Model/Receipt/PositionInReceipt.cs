using Filling_Station_Automated_Workplace.Domain;

namespace Filling_Station_Automated_Workplace.Model
{
    public class PositionInReceipt
    {
        public int Id { get; init; }
        public int Count { get; set; }

        public string? Name
        {
            get
            {
                (string? name, _) = GoodsModel.GetNameAndPriceById(Id);
                return name;
            }
        }
    
        public string Price
        {
            get
            {
                (_, double price) = GoodsModel.GetNameAndPriceById(Id);
                return price.ToString("C2");
            }
        }
        

        public double TotalCost =>
            GoodsModel.GetPriceById(Id) * Count;
    }
}