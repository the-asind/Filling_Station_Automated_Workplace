using System.Data;
using System.IO;
using System.Reflection;
using Filling_Station_Automated_Workplace.Model;
using Filling_Station_Automated_Workplace.ViewModel;

namespace Filling_Station_Automated_Workplace.Data;

public class GoodsData : IGoodsDataProvider
{
    public DataTable GoodsDataTable { get; }

    public GoodsData()
    {
        GoodsDataTable = Deserialize.GetDataTableFromCsvFile("Goods.csv");

        foreach (DataRow row in GoodsDataTable.Rows)
        {
            if (row["Count"].ToString() == "0")
            {
                row.Delete();
            }
        }

        GoodsDataTable.AcceptChanges();
        
        DataColumn?[] primaryKeyColumns = { GoodsDataTable.Columns["ID"] };
        GoodsDataTable.PrimaryKey = primaryKeyColumns!;
    }
}