using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace Filling_Station_Automated_Workplace.Data;

public class UsersData
{
    [XmlRoot("Users")]
    public class Users
    {
        [XmlElement("User")]
        public List<User> UsersList { get; set; }
    }

    public class User
    {
        [XmlElement("Login")]
        public string Login { get; set; }

        [XmlElement("Password")]
        public string Password { get; set; }
        
        [XmlElement("FullName")]
        public string FullName { get; set; }

        [XmlElement("AccessLevel")]
        public string AccessLevel { get; set; }
    }

}
