using System.Data;
using Filling_Station_Automated_Workplace.Domain;
using Filling_Station_Automated_Workplace.ViewModel;

namespace Filling_Station_Automated_Workplace.Data;

public class NozzlePostData : INozzlePostDataProvider
{
    public DataTable NozzlePostDataTable { get; }

    public NozzlePostData()
    {
        NozzlePostDataTable = Deserialize.GetDataTableFromCsvFile("Tanks.csv");

        DataColumn[] primaryKeyColumns = { NozzlePostDataTable.Columns["ID"] ?? new DataColumn() };
        NozzlePostDataTable.PrimaryKey = primaryKeyColumns;
    }
}