using System.Data;
using Filling_Station_Automated_Workplace.Domain;
using Filling_Station_Automated_Workplace.ViewModel;

namespace Filling_Station_Automated_Workplace.Data;

public class GoodsData : IGoodsDataProvider
{
    public DataTable GoodsDataTable { get; }

    public GoodsData()
    {
        GoodsDataTable = Deserialize.GetDataTableFromCsvFile("Goods.csv");

        GoodsDataTable.AcceptChanges();

        DataColumn?[] primaryKeyColumns = { GoodsDataTable.Columns["ID"] };
        GoodsDataTable.PrimaryKey = primaryKeyColumns!;
    }
}