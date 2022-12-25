using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Filling_Station_Automated_Workplace.Domain;

namespace Filling_Station_Automated_Workplace.Data;

[XmlRoot("Configuration")]
public class ConfigurationData
{

    [XmlElement("NozzlePostCount")] public int NozzlePostCount { get; set; }

    [XmlArray("PaymentTypes")]
    [XmlArrayItem("PaymentType")]
    public List<PaymentTypeData>? PaymentTypes { get; set; }
}

public class PaymentTypeData
{
    [XmlElement("Name")] public string? Name { get; set; }

    [XmlElement("IsActive")] public bool IsActive { get; set; }

    [XmlElement("IsCash")] public bool IsCash { get; set; }
}