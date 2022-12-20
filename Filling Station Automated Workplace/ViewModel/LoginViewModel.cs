using System;
using System.Collections.Generic;
using System.Linq;
using Filling_Station_Automated_Workplace.Data;
using Filling_Station_Automated_Workplace.Model;

namespace Filling_Station_Automated_Workplace.ViewModel;

public class LoginViewModel
{
    private readonly UsersData.Users _usersData;

    public LoginViewModel()
    {
        _usersData = Deserialize.DeserializeUsersData();
    }

    public void TryToEnter(string login, string password)
    {
        if (_usersData.UsersList.Any(user => user.Login == login && user.Password == password))
        {
            return;
        }

        throw new ArgumentException(login);
    }
}
