using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Filling_Station_Automated_Workplace.Data;
using Filling_Station_Automated_Workplace.Domain;

namespace Filling_Station_Automated_Workplace.View;

public partial class SystemConfigurator
{
    private ObservableCollection<UsersData.User> UsersCollection { get; set; }
    private ObservableCollection<PaymentTypeData.PaymentType> PaymentTypesCollection { get; set; }
    
    public SystemConfigurator()
    {
        InitializeComponent();
        
        UsersCollection = new ObservableCollection<UsersData.User>(Deserialize.DeserializeUsersData().UsersList);
        var users = Deserialize.DeserializeUsersData();
        UserChangeGrid.ItemsSource = users.UsersList;
        
        PaymentTypesCollection = new ObservableCollection<PaymentTypeData.PaymentType>(Deserialize.DeserializePaymentType().PaymentTypeList);
        var paymentTypes = Deserialize.DeserializePaymentType();
        PaymentChangeGrid.ItemsSource = paymentTypes.PaymentTypeList;
    }
    
    private void AcceptChangesButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void ChangeUsersButton_Clicked(object sender, RoutedEventArgs e)
    {
        UserChangePopup.IsOpen = true;
    }

    private void ChangePostButton_Clicked(object sender, RoutedEventArgs e)
    {
        NozzleCountPopup.IsOpen = true;
    }

    private void ChangePaymentButton_Clicked(object sender, RoutedEventArgs e)
    {
        PaymentChangePopup.IsOpen = true;
    }
    private void SetNewPostCountButton_Clicked(object sender, RoutedEventArgs e)
    {
        try
        {
            Serialize.SerializeNozzlePostCount(int.Parse(NewPostCountTextBox.Text));
            NozzleCountPopup.IsOpen = false;
        }
        catch (InvalidOperationException)
        {
            MessageBox.Show("Файл конфигурации не найден либо бит", "Ошибка сериализации", MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }
        catch (FormatException)
        {
            MessageBox.Show("Значение не является числом", "Неверное число", MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }
    }
    
    private void NewPostCountClicked(object sender, RoutedEventArgs e)
    {
        var t = (TextBox)sender;
        t.Text = "";
    }

    private void NewPostCountInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = Regex.IsMatch(e.Text);
    }
    
    private void HandleCanExecute(object sender, CanExecuteRoutedEventArgs e) {

        if ( e.Command == ApplicationCommands.Cut ||
             e.Command == ApplicationCommands.Copy ||
             e.Command == ApplicationCommands.Paste ) {

            e.CanExecute = false;
            e.Handled = true;
        }

    }

    private static readonly Regex Regex = new("[^0-9]+"); //regex that matches disallowed text

    private void DeleteRowMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (UserChangeGrid.SelectedItem != null)
            try
            {
                var selectedItem = UserChangeGrid.SelectedItem;
                var itemsSource = UserChangeGrid.ItemsSource as IList;
                itemsSource?.Remove(selectedItem);
            }
            catch
            {
                MessageBox.Show(
                    "Наведите курсор на строку с данными и повторите действие, чтобы удалить её",
                    "Ошибка удаления строки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
    }


    private void ChangeGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var dataGrid = (DataGrid)sender;
        if (dataGrid.SelectedCells.Count > 0)
        {
            var selectedRow = dataGrid.SelectedCells[0].Item;
            dataGrid.SelectedItem = selectedRow;
        }
    }
    
    private void UserChangeDisagreeButton_Clicked(object sender, RoutedEventArgs e)
    {
        UserChangePopup.IsOpen = false;
        UsersCollection = new ObservableCollection<UsersData.User>(Deserialize.DeserializeUsersData().UsersList);
        var users = Deserialize.DeserializeUsersData();
        UserChangeGrid.ItemsSource = users.UsersList;
    }
    
    private void UserChangeAcceptButton_Clicked(object sender, RoutedEventArgs e)
    {
        // Find rows with empty Login, Password, and FullName fields
        var emptyRows = UserChangeGrid.Items.OfType<UsersData.User>()
            .Where(user => string.IsNullOrWhiteSpace(user.Login) && string.IsNullOrWhiteSpace(user.Password) && string.IsNullOrWhiteSpace(user.FullName))
            .ToList();

        // Remove the fully empty rows from the DataGrid
        foreach (var row in emptyRows)
        {
            UserChangeGrid.Items.Remove(row);
        }
        
        // Check that at least one row exists in the data table
        if (UserChangeGrid.Items.OfType<UsersData.User>().All(user => user.AccessLevel != "admin"))
        {
            var errorOutput = new string(String.Concat("Вы не можете не иметь ни одного пользователя,",
                "являющегося администратором."));
            MessageBox.Show(errorOutput, "Отсутствие администраторов", MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        // Find rows with empty or whitespace-only Login, Password, or FullName fields
        var invalidRows = UserChangeGrid.Items.OfType<UsersData.User>()
            .Where(user => string.IsNullOrWhiteSpace(user.Login) || string.IsNullOrWhiteSpace(user.Password) || string.IsNullOrWhiteSpace(user.FullName))
            .ToList();

        if (invalidRows.Any())
        {
            MessageBox.Show(
                "Заполните все строковые поля",
                "Ошибка проверки целостности строк", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            // If the list is valid, serialize the updated list of users to the Users.xml file
            Serialize.SerializeUsers(UserChangeGrid.Items.OfType<UsersData.User>().ToList());
            UserChangePopup.IsOpen = false;
        }
    }


    private void PaymentChangeAcceptButton_Clicked(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void PaymentChangeDisagreeButton_Clicked(object sender, RoutedEventArgs e)
    {
        PaymentTypesCollection = new ObservableCollection<PaymentTypeData.PaymentType>(Deserialize.DeserializePaymentType().PaymentTypeList);
        var Payments = Deserialize.DeserializePaymentType();
        PaymentChangeGrid.ItemsSource = Payments.PaymentTypeList;
    }
}

public class AccessLevelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string accessLevel)
        {
            return accessLevel == "admin";
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isAdmin)
        {
            return isAdmin ? "admin" : "user";
        }

        return "user";
    }
}

public class BoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string isActive)
        {
            return isActive == "true";
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isActive)
        {
            return isActive ? "true" : "false";
        }

        return "false";
    }
}
