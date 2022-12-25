using System;
using System.Linq;
using Filling_Station_Automated_Workplace.Data;
using Filling_Station_Automated_Workplace.Domain;
using Filling_Station_Automated_Workplace.Model;
using GalaSoft.MvvmLight.Messaging;

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
        //TODO: реализовать использование безопасного механизма хранения пароля, например, хэширование или шифрование.
        if (_usersData.UsersList.Any(user => user.Login == login && user.Password == password))
        {
            var user = _usersData.UsersList.Single(user => user.Login == login && user.Password == password);
            User.FullName = user.FullName;
            User.IsAdmin = user.AccessLevel == "admin";
            Messenger.Default.Send(new UpdateUserMessage());
            return;
        }

        throw new ArgumentException(login);
    }
}
