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
        GoodsDataTable = DataSerializer.GetDataTableFromCsvFile(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Assets\Goods.csv");

        DataColumn[] primaryKeyColumns = { GoodsDataTable.Columns["ID"] };
        GoodsDataTable.PrimaryKey = primaryKeyColumns;
    }
}