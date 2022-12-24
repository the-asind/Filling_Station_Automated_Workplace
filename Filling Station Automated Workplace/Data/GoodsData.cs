using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Filling_Station_Automated_Workplace.Domain;
using Filling_Station_Automated_Workplace.Model;
using Filling_Station_Automated_Workplace.ViewModel;

namespace Filling_Station_Automated_Workplace.Data;

public class GoodsData : IGoodsDataProvider
{
    public DataTable GoodsDataTable { get; }

    public GoodsData()
    {
        GoodsDataTable = Deserialize.GetDataTableFromCsvFile("Goods.csv");

        var rowsToDelete = GoodsDataTable.Rows.Cast<DataRow>().Where(row => row["Count"].ToString() == "0").ToList();

        foreach (var row in rowsToDelete)
        {
            row.Delete();
        }

        GoodsDataTable.AcceptChanges();
        
        DataColumn?[] primaryKeyColumns = { GoodsDataTable.Columns["ID"] };
        GoodsDataTable.PrimaryKey = primaryKeyColumns!;
    }
}