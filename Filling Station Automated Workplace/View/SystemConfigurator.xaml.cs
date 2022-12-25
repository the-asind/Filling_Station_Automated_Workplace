using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Filling_Station_Automated_Workplace.Domain;

namespace Filling_Station_Automated_Workplace.View;

public partial class SystemConfigurator : Window
{
    public SystemConfigurator()
    {
        InitializeComponent();
    }
    
    private void AcceptChangesButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void ChangeUsersButton_Clicked(object sender, RoutedEventArgs e)
    {
        
    }

    private void ChangePostButton_Clicked(object sender, RoutedEventArgs e)
    {
        MessagePopup.IsOpen = true;
    }

    private void ChangePaymentButton_Clicked(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NewPostCountTextBox_Changed(object sender, TextChangedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void SetNewPostCountButton_Clicked(object sender, RoutedEventArgs e)
    {
        try
        {
            Serialize.SerializeNozzlePostCount(int.Parse(NewPostCountTextBox.Text));
            MessagePopup.IsOpen = false;
        }
        catch (InvalidOperationException)
        {
            MessageBox.Show("Файл конфигурации не найден либо бит", "Ошибка сериализации", MessageBoxButton.OK,
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

}