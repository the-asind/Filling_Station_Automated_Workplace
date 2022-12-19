using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Filling_Station_Automated_Workplace.Model;
using Filling_Station_Automated_Workplace.ViewModel;

namespace Filling_Station_Automated_Workplace.View;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer;
    private DateTime _sessionTime = new(0, 0);

    private DataTable? _shoppingCartGoodsTable;

    //Добавим информацию в таблицу
    private void grid_Loaded(object sender, RoutedEventArgs e)
    {
        _shoppingCartGoodsTable = ShoppingCartItem.Update(CurrentReceipt.Receipt);
        GoodsMainMenuGrid.ItemsSource = _shoppingCartGoodsTable.DefaultView;
    }

    private readonly MainWindowViewModel _viewModel;
    
    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        PickedNozzle.DataContext = _viewModel;
        CreateNozzlePosts(2);

        //  DispatcherTimer setup
        _timer = new DispatcherTimer();
        _timer.Tick += _timer_Tick;
        _timer.Interval = new TimeSpan(0, 0, 1);
        _timer.Start();
    }

    private void CreateNozzlePosts(int count)
    {
        NozzleList.Children.Clear();
        
        // Create a new instance of the NozzlePostViewModel window
        var dataProvider = new ConcreteNozzlePostViewModel();
        
        for (int i = 1; i <= count; i++)
        {
            NozzlePostViewModel viewModel = new NozzlePostViewModel(i, dataProvider);

            NozzlePost nozzlePostControl = new NozzlePost(i, viewModel);
            nozzlePostControl.DataContext = viewModel;

            NozzleList.Children.Add(nozzlePostControl);
        }
    }
    
    private void _timer_Tick(object sender, EventArgs e)
    {
        // Updating the Label which displays the current second
        DateTimeFoot.Text = DateTime.Now.ToString("D");
        CurrentTime.Text = DateTime.Now.ToString("H:mm:ss");

        // updating onsession timer
        _sessionTime = _sessionTime.AddSeconds(1);
        ShiftTime.Text = $"Смена открыта: {_sessionTime.ToString("H:mm")}";

        // Forcing the CommandManager to raise the RequerySuggested event
        CommandManager.InvalidateRequerySuggested();
    }

    private void LitreAmountCliked(object sender, RoutedEventArgs e)
    {
    }

    private void LiterAmountTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = _regex.IsMatch(e.Text);
    }

    private static readonly Regex _regex = new("[^0-9.]+"); //regex that matches disallowed text

    private static bool IsTextAllowed(string text)
    {
        return !_regex.IsMatch(text);
    }

    private void AddGoodsButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Create a new instance of the GoodsSelectorViewModel window
        var dataProvider = new ConcreteGoodsSelectorViewModel();
        var goodsSelectorViewModel = new GoodsSelectorViewModel(dataProvider);

        var goodsSelector = new GoodsSelector(goodsSelectorViewModel);
        goodsSelector.Topmost = true;
        goodsSelector.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        // Show the window
        goodsSelector.ShowDialog();

        _shoppingCartGoodsTable = ShoppingCartItem.Update(CurrentReceipt.Receipt);
        GoodsMainMenuGrid.ItemsSource = _shoppingCartGoodsTable.DefaultView;

        SetGoodsSummaryTextBlockValue(CurrentReceipt.Receipt.GetGoodsSummary());
    }

    public void SetGoodsSummaryTextBlockValue(double value)
    {
        var rounded = Math.Round(value, 2);
        var culture = CultureInfo.InvariantCulture;
        var sum = rounded.ToString("#.##", culture);

        GoodsSummaryTextBlock.Text = string.Concat("Сумма: ", sum);
    }

    private void MainWindow_OnClosed(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }
}