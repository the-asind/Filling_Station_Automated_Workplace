using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Data;
using Filling_Station_Automated_Workplace.Data;
using Filling_Station_Automated_Workplace.Domain;
using Filling_Station_Automated_Workplace.Model;
using GalaSoft.MvvmLight.Messaging;

namespace Filling_Station_Automated_Workplace.ViewModel;

// ViewModel interface
public interface IMainWindowViewModel
{
    int NozzlePostCount
    {
        get
        {
            var configurationData = new ConfigurationData();
            if (configurationData == null) throw new ArgumentNullException(nameof(configurationData));
            return configurationData.NozzlePostCount;
        }
    }
}

public class ConcreteMainWindowViewModel : IConfigurationDataProvider
{
    private ConfigurationData _configurationData;

    public int NozzlePostCount => _configurationData.NozzlePostCount;

    public ConcreteMainWindowViewModel()
    {
        _configurationData = new ConfigurationData();
    }
}

// Data interface
public interface IConfigurationDataProvider
{
    int NozzlePostCount { get; }
    
}

// ViewModel class
public sealed class MainWindowViewModel : INotifyPropertyChanged, IMainWindowViewModel
{
    private ConfigurationData _configurationData;

    public MainWindowViewModel()
    {
        var dataProvider = new ConfigurationData();
        NozzlePostViewModel.SelectedIdChanged += OnNozzlePostUserControlActive;
        _configurationData = dataProvider;
        ReceiptItems = new ObservableCollection<ShoppingCartItem>();
        IsWindowFree = true;
        IsPaymentReady = true;
        Messenger.Default.Register<FillUpChangedMessage>(this, OnFillUpChangedMessageReceived);
        Messenger.Default.Register<FillUpEndedMessage>(this, OnFillUpEndedMessageReceived);
        Messenger.Default.Register<UpdateUserMessage>(this, UpdateUserReceived);
    }

    private void UpdateUserReceived(UpdateUserMessage obj)
    {
        OnPropertyChanged(nameof(UserLoginName));
        OnPropertyChanged(nameof(UserAccessLevel));
    }

    private void OnFillUpEndedMessageReceived(FillUpEndedMessage message)
    {
        IsPaymentReady = true;
        OnPropertyChanged(nameof(IsPaymentReady));
        OnPropertyChanged(nameof(TotalCostText));
        OnPropertyChanged(nameof(FinishPaymentType));
    }
    
    private void OnFillUpChangedMessageReceived(FillUpChangedMessage message)
    {
        OnPropertyChanged(nameof(TotalCostText));
        OnPropertyChanged(nameof(FinishPaymentType));
    }
    
    public int NozzlePostCount => _configurationData.NozzlePostCount;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        var handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void UpdateNozzlePostCount()
    {
        _configurationData = new ConfigurationData();
        OnPropertyChanged(nameof(NozzlePostCount));
    }


    private NozzlePostViewModel? _selectedNozzlePostInstance;

    public NozzlePostViewModel? SelectedNozzlePostInstance
    {
        get => _selectedNozzlePostInstance;
        set
        {
            if (_selectedNozzlePostInstance == value) return;
            if (value is { IsNozzlePostBusy: true }) return;
            _selectedNozzlePostInstance = value;
            CurrentSession.CurrentReceipt.RelateNozzlePost = value;
            
            IsPaymentReady = true;
            OnPropertyChanged(nameof(IsPaymentReady));

            OnPropertyChanged(nameof(TotalCostText));
        }
    }

    private void OnNozzlePostUserControlActive(object? sender, NozzlePostViewModel? e)
    {
        SelectedNozzlePostInstance = e;
        OnPropertyChanged(nameof(SelectedNozzlePostInstance));

        OnPropertyChanged(nameof(ReceiptItems));
        OnPropertyChanged(nameof(TotalCostText));
        OnPropertyChanged(nameof(FinishPaymentType));
    }

    public string TotalCostText
    {
        get
        {
            if (SelectedNozzlePostInstance != null)
                if (!SelectedNozzlePostInstance.FillUp)
                    return (SelectedNozzlePostInstance.Summary + GoodsSummary).ToString("C2");
                else
                    return "Н/Д";
            return GoodsSummary.ToString("C2");
        }
    }
    
    public void SetGoodsSummary(double getGoodsSummary)
    {
        GoodsSummary = getGoodsSummary;
        IsPaymentReady = true;
        OnPropertyChanged(nameof(IsPaymentReady));
    }

    private double _goodsSummary;

    public double GoodsSummary
    {
        get => _goodsSummary;

        set
        {
            if (Math.Abs(_goodsSummary - value) < 0.001) return;
            _goodsSummary = value;
            OnPropertyChanged(nameof(TextGoodsSummary));
            OnPropertyChanged(nameof(ReceiptItems));
            OnPropertyChanged(nameof(TotalCostText));
            OnPropertyChanged(nameof(FinishPaymentType));
        }
    }
    
    public bool FinishPaymentType
    {
        get
        {
            if (SelectedNozzlePostInstance == null) return true;
            return !SelectedNozzlePostInstance.FillUp;
            // 1 - ОПЛАТА ; 0 - ПУСК
        }
    }

    public string TextGoodsSummary => _goodsSummary.ToString("C2");

    public bool IsWindowFree { get; set; }
    public bool IsPaymentReady { get; set; }
    
    public void UpdateReceiptItems(Receipt receipt)
    {
        ReceiptItems = new ObservableCollection<ShoppingCartItem>(ShoppingCartItem.IUpdate(receipt));
        OnPropertyChanged(nameof(ReceiptItems));
        OnPropertyChanged(nameof(GoodsSummary));
        OnPropertyChanged(nameof(TextGoodsSummary));
        OnPropertyChanged(nameof(TotalCostText));
        OnPropertyChanged(nameof(FinishPaymentType));
    }
    
    public ObservableCollection<ShoppingCartItem> ReceiptItems { get; set; }

    public void FinishPayment()
    {
        if (FinishPaymentType || SelectedNozzlePostInstance == null)
        {
            Serialize.UpdateGoodsFile(ReceiptItems);
            
            CurrentSession.CreateNewReceipt();
            UpdateReceiptItems(CurrentSession.CurrentReceipt);
            SetGoodsSummary(CurrentSession.CurrentReceipt.GetGoodsSummary());

            if (SelectedNozzlePostInstance != null)
            {
                if (!SelectedNozzlePostInstance.IsAlreadyFilledOut)
                {
                    Serialize.UpdateTanksFile(SelectedNozzlePostInstance);
                    SelectedNozzlePostInstance.StartFueling();
                    SelectedNozzlePostInstance.IsNozzlePostBusy = true;
                    SelectedNozzlePostInstance = null;
                }
                else
                {
                    SelectedNozzlePostInstance.IsAlreadyFilledOut = false;
                    Serialize.UpdateTanksFile(SelectedNozzlePostInstance);
                    SelectedNozzlePostInstance = null;
                }
            }

            IsWindowFree = true;
            OnPropertyChanged(nameof(IsWindowFree));
            IsPaymentReady = false;
            OnPropertyChanged(nameof(IsPaymentReady));
        }
        else 
        {
            Serialize.UpdateTanksFile(SelectedNozzlePostInstance);
            SelectedNozzlePostInstance.StartFueling();
            SelectedNozzlePostInstance.IsNozzlePostBusy = true;
            IsWindowFree = false;
            OnPropertyChanged(nameof(IsWindowFree));
            IsPaymentReady = false;
            OnPropertyChanged(nameof(IsPaymentReady));
        }
        
        OnPropertyChanged(nameof(SelectedNozzlePostInstance));
    }
    
    public string? UserLoginName
    {
        get => User.FullName;
        set
        {
            if (User.FullName == value) return;
            User.FullName = value;
            OnPropertyChanged(nameof(UserLoginName));
        }
    }
    
    public bool UserAccessLevel
    {
        get => User.IsAdmin;
        set
        {
            if (User.IsAdmin == value) return;
            User.IsAdmin = value;
            OnPropertyChanged(nameof(UserAccessLevel));
        }
    }
}

public class FillUpChangedMessage { }
public class FillUpEndedMessage { }
public class UpdateUserMessage { }
