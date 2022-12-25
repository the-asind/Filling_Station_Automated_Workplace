using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using Filling_Station_Automated_Workplace.Model;
using Filling_Station_Automated_Workplace.ViewModel;

namespace Filling_Station_Automated_Workplace.View;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private DateTime _sessionTime = new(0, 0);

    //Добавим информацию в таблицу
    private void grid_Loaded(object sender, RoutedEventArgs e)
    {
        ShoppingCartItem.Update(CurrentSession.CurrentReceipt);
        //GoodsMainMenuGrid.ItemsSource = _shoppingCartGoodsTable.DefaultView;
    }

    private readonly MainWindowViewModel _viewModel;
    private readonly BlurEffect _blur;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        DataContext = _viewModel;

        _blur = new BlurEffect
        {
            Radius = 10
        };
        Effect = _blur;

        var login = new Login
        {
            Topmost = true,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };
        login.Show();

        IsHitTestVisible = false;

        login.Closed += (_, _) =>
        {
            // Remove the Blur effect from the main window
            Effect = null;
            IsHitTestVisible = true;
        };


        CreateNozzlePosts(_viewModel.NozzlePostCount);

        //  DispatcherTimer setup
        var timer = new DispatcherTimer();
        timer.Tick += _timer_Tick;
        timer.Interval = new TimeSpan(0, 0, 1);
        timer.Start();
    }

    private void CreateNozzlePosts(int count)
    {
        NozzleList.Children.Clear();

        // Create a new instance of the NozzlePostViewModel window
        var dataProvider = new ConcreteNozzlePostViewModel();

        for (var i = 1; i <= count; i++)
        {
            var viewModel = new NozzlePostViewModel(i, dataProvider);

            var nozzlePostControl = new NozzlePost(i, viewModel)
            {
                DataContext = viewModel
            };

            NozzleList.Children.Add(nozzlePostControl);
        }
    }

    private void _timer_Tick(object? sender, EventArgs e)
    {
        // Updating the Label which displays the current second
        DateTimeFoot.Text = DateTime.Now.ToString("D");
        CurrentTime.Text = DateTime.Now.ToString("H:mm:ss");

        // updating onsession timer
        _sessionTime = _sessionTime.AddSeconds(1);
        ShiftTime.Text = $"Смена открыта: {_sessionTime:H:mm}";

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

    private void FinishPaymentButton_OnClick(object sender, RoutedEventArgs e)
    {
        _viewModel.FinishPayment();
    }

    private void FuelInfoButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (User.IsAdmin)
        {
            var tanksConfigurator = new TanksConfigurator
            {
                Topmost = true,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            // Show the window
            Effect = _blur;
            tanksConfigurator.ShowDialog();

            Effect = null;
            // Clear the parent container that holds the NozzlePost controls
            NozzleList.Children.Clear();

            // Set the DataContext of the NozzleList to null to release the references to the NozzlePostViewModel instances
            NozzleList.DataContext = null;

            CreateNozzlePosts(_viewModel.NozzlePostCount);

            _viewModel.UpdateReceiptItems(CurrentSession.CurrentReceipt);
        }
        else
        {
            _viewModel.UpdateTanksReservesInfo();
            ReservesUserOnlyPopup.IsOpen = true;
        }
    }

    private void GoodsInfoButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (User.IsAdmin)
        {
            var goodsConfigurator = new GoodsConfigurator
            {
                Topmost = true,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            // Show the window
            Effect = _blur;
            goodsConfigurator.ShowDialog();

            Effect = null;
            _viewModel.UpdateReceiptItems(CurrentSession.CurrentReceipt);
        }
        else
        {
            AccessDeniedPopup.IsOpen = true;
        }
    }

    private void SystemButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (User.IsAdmin)
        {
            var systemConfigurator = new SystemConfigurator
            {
                Topmost = true,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            // Show the window
            Effect = _blur;
            systemConfigurator.ShowDialog();

            Effect = null;

            NozzleList.Children.Clear();
            NozzleList.DataContext = null;
            _viewModel.UpdateNozzlePostCount();
            CreateNozzlePosts(_viewModel.NozzlePostCount);

            _viewModel.UpdateReceiptItems(CurrentSession.CurrentReceipt);
        }
        else
        {
            AccessDeniedPopup.IsOpen = true;
        }
    }

    private void ShiftButton_OnClick(object sender, RoutedEventArgs e)
    {
        var shiftInfo = new ShiftInfo
        {
            Topmost = true,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };
        // Show the window
        Effect = _blur;
        shiftInfo.ShowDialog();

        Effect = null;
    }
}

public class FinishPaymentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return boolValue ? "ОПЛАТА" : "ПУСК";
        throw new ArgumentException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public class UserAccessLevelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return boolValue ? "Администратор" : "Оператор";
        throw new ArgumentException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}