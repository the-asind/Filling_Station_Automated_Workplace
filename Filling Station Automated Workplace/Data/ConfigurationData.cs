using System.Data;
using System.IO;
using System.Reflection;
using Filling_Station_Automated_Workplace.Model;

namespace Filling_Station_Automated_Workplace.Data;

public class ConfigurationData
{
    public DataTable ConfigurationDataTable { get; }

    public ConfigurationData()
    {
        /*ConfigurationDataTable = DataSerializer.GetDataTableFromCsvFile(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Assets\Configuration.xml");
*/
    }
}