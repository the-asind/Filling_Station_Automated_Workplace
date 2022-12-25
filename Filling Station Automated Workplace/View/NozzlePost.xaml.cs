using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Filling_Station_Automated_Workplace.ViewModel;

namespace Filling_Station_Automated_Workplace.View;

public partial class NozzlePost
{
    private readonly NozzlePostViewModel _viewModel;

    public NozzlePost(int title, NozzlePostViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
        NozzlePostNumber.Content = title.ToString();
        DataContext = viewModel;
    }

    private void LiterAmountClicked(object sender, RoutedEventArgs e)
    {
        var t = (TextBox)sender;
        t.Text = "";
    }

    private void LiterAmountTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = Regex.IsMatch(e.Text);
    }

    private static readonly Regex Regex = new("[^0-9]+"); //regex that matches disallowed text

    private readonly SolidColorBrush _darkColor = new(Color.FromArgb(0xFF, 0x1B, 0x5E, 0x20));
    private readonly SolidColorBrush _midColor = new(Color.FromArgb(0xFF, 0xA5, 0xD6, 0xA7));
    private readonly SolidColorBrush _lightColor = new(Color.FromArgb(0xFF, 0xC8, 0xE6, 0xC9));

    private void FillUpFullTank_Click(object sender, RoutedEventArgs e)
    {
        switch (FillUpFullTank.IsChecked)
        {
            case true:
                LiterAmount.Background = _midColor;
                LiterAmount.Foreground = _midColor;
                LiterAmount.IsReadOnly = true;
                _viewModel.FillUpFullTank(true);
                LiterAmount.Text = "0";

                break;
            case false:
                LiterAmount.Background = _lightColor;
                LiterAmount.Foreground = _darkColor;
                LiterAmount.IsReadOnly = false;
                _viewModel.FillUpFullTank(false);
                break;
        }
    }

    private void SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox { SelectedItem: DataRowView rowView }) return;

        var selectedString = rowView["Name"].ToString();

        _viewModel.SelectionChanged(selectedString);
    }

    private void LiterAmount_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            var count = int.Parse(LiterAmount.Text);
            try
            {
                _viewModel.LiterCountChanged(count);
            }
            catch (ArgumentException)
            {
                MessagePopup.IsOpen = true;
                LiterAmount.Text = LiterAmount.Text[..^1];
            }
        }
        catch (FormatException)
        {
            //Ignored
        }
    }
}

[ValueConversion(typeof(bool), typeof(bool))]
public class InverseBooleanConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
        CultureInfo culture)
    {
        if (targetType != typeof(bool))
            throw new InvalidOperationException("The target must be a boolean");

        return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    #endregion
}

public class ProgressToEndPointConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double progress) return new Point(0.5, 1 - progress);

        return new Point(0.5, 1);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}