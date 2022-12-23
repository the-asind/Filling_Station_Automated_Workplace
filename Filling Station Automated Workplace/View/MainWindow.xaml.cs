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
using System.Windows.Media.Effects;

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
        //GoodsMainMenuGrid.ItemsSource = _shoppingCartGoodsTable.DefaultView;
    }

    private readonly MainWindowViewModel _viewModel;
    
    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        GoodsMainMenuGrid.DataContext = _viewModel;
        PickedNozzle.DataContext = _viewModel;
        PickedNozzleFuel.DataContext = _viewModel;
        PickedNozzleSummaryPrice.DataContext = _viewModel;
        GoodsSummaryTextBlock.DataContext = _viewModel;
        TotalAmountInfo.DataContext = _viewModel;
        
        
        var blur = new BlurEffect
        {
            Radius = 10
        };
        Effect = blur;

        var login = new Login();
        login.Topmost = true;
        login.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        login.Show();

        IsHitTestVisible = false;

        login.Closed += (sender, args) =>
        {
            // Remove the Blur effect from the main window
            Effect = null;
            IsHitTestVisible = true;
        };


        CreateNozzlePosts(12);

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

    private void AddGoodsButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Create a new instance of the GoodsSelectorViewModel window
        var dataProvider = new ConcreteGoodsSelectorViewModel();
        var goodsSelectorViewModel = new GoodsSelectorViewModel(dataProvider);

        var goodsSelector = new GoodsSelector(goodsSelectorViewModel, _viewModel)
        {
            Topmost = true,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };
        // Show the window
        goodsSelector.Show();
        
        
    }

    private void MainWindow_OnClosed(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }
}