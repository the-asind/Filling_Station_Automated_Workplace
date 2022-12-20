using System;
using System.Windows;
using System.Windows.Controls;
using Filling_Station_Automated_Workplace.ViewModel;

namespace Filling_Station_Automated_Workplace.View;

public partial class Login : Window
{
    private LoginViewModel _viewModel;
    
    public Login()
    {
        InitializeComponent();
        _viewModel = new LoginViewModel();
    }

    private void ClearTextBox(object sender, RoutedEventArgs e)
    {
        var t = (TextBox)sender;
        t.Text = "";
    }

    private void TryToEnter(object sender, RoutedEventArgs e)
    {
        try
        {
            _viewModel.TryToEnter(LoginBox.Text, PasswordBox.Text);
            Close();
        }
        catch (ArgumentException)
        {
            ErrorBox.Text = "Неверное имя пользователя или пароль!";
        }
        
    }

}