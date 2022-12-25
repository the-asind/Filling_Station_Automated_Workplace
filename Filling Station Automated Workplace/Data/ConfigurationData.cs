using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Filling_Station_Automated_Workplace.Domain;
using Filling_Station_Automated_Workplace.Model;

namespace Filling_Station_Automated_Workplace.Data;

public class ConfigurationData
{
    public static readonly string CsvFileDefault = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Assets\";

    public ConfigurationData()
    {
        NozzlePostCount = Deserialize.GetNozzlePostCountFromXml();
    }
    
    public void UpdateConfigurationData()
    {
        NozzlePostCount = Deserialize.GetNozzlePostCountFromXml();
    }

    public int NozzlePostCount { get; set; }

}