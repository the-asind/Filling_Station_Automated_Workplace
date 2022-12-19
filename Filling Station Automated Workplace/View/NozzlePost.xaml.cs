using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Filling_Station_Automated_Workplace.ViewModel;

namespace Filling_Station_Automated_Workplace.View;


public partial class NozzlePost : UserControl
{
    private readonly NozzlePostViewModel _viewModel;
    
    public NozzlePost(int title, NozzlePostViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
        NozzlePostNumber.Content = title.ToString();
        TotalNozzleAmount.DataContext = viewModel;
        PriceForLiter.DataContext = viewModel;
    }
    
    public void LitreAmountClicked(object sender, RoutedEventArgs e)
    {
        var t = (TextBox)sender;
        t.Text = "";
    }

    public void LiterAmountTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = _regex.IsMatch(e.Text);
    }

    public static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
    public static bool IsTextAllowed(string text)
    {

        return !_regex.IsMatch(text);
    }
    
    private SolidColorBrush _darkColor =  new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0x5E, 0x20));
    private SolidColorBrush _midColor =  new SolidColorBrush(Color.FromArgb(0xFF, 0xA5, 0xD6, 0xA7));
    private SolidColorBrush _lightColor =  new SolidColorBrush(Color.FromArgb(0xFF, 0xC8, 0xE6, 0xC9));
    
    private void FillUpFullTank_Click(object sender, RoutedEventArgs e)
    {
        switch (FillUpFullTank.IsChecked)
        {
            case true:
                LiterAmount.Background = _midColor;
                LiterAmount.Foreground = _midColor;
                LiterAmount.IsReadOnly = true;
                
                break;
            case false:
                LiterAmount.Background = _lightColor;
                LiterAmount.Foreground = _darkColor;
                LiterAmount.IsReadOnly = false;
                
                break;
        }
    }

    private void SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int id =  NozzleComboBox.SelectedIndex;
        _viewModel.SelectionChanged(id);
    }

    private void LiterAmount_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            var count = int.Parse(LiterAmount.Text);
            _viewModel.LiterCountChanged(count);
        }
        catch (Exception exception)
        {
            //Ignored
        }
    }
}