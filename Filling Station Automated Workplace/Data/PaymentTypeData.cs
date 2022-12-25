using System.Collections.Generic;
using System.Xml.Serialization;

namespace Filling_Station_Automated_Workplace.Data;


public class PaymentTypeData
{
    [XmlRoot("PaymentTypes")]
    public class PaymentTypes
    {
        [XmlElement("PaymentType")]
        public List<PaymentType> PaymentTypeList { get; set; } = new List<PaymentType>();
    }


    public class PaymentType
    {
        [XmlElement("Name")] public string? Name { get; set; }

        [XmlElement("IsActive")] public bool IsActive { get; set; }

        [XmlElement("IsCash")] public bool IsCash { get; set; }
    }
}

