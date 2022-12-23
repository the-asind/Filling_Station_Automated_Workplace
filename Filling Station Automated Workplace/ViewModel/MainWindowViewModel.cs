using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight.Messaging;
using Filling_Station_Automated_Workplace.Data;
using Filling_Station_Automated_Workplace.Model;

namespace Filling_Station_Automated_Workplace.ViewModel;

// ViewModel interface
public interface IMainWindowViewModel
{
    DataTable ConfigurationDataTable
    {
        get
        {
            var configurationData = new ConfigurationData();
            if (configurationData == null) throw new ArgumentNullException(nameof(configurationData));
            return configurationData.ConfigurationDataTable;
        }
    }

    ObservableCollection<string?> MainWindowNames
    {
        get
        {
            var mainWindowNames = new ObservableCollection<string?>();

            foreach (DataRow row in ConfigurationDataTable.Rows) mainWindowNames.Add(row["Name"].ToString());

            return mainWindowNames;
        }
    }
}

public class ConcreteMainWindowViewModel : IConfigurationDataProvider
{
    private readonly ConfigurationData _ConfigurationData;

    public DataTable ConfigurationDataTable => _ConfigurationData.ConfigurationDataTable;

    public ConcreteMainWindowViewModel()
    {
        _ConfigurationData = new ConfigurationData();
    }
}

// Data interface
public interface IConfigurationDataProvider
{
    DataTable ConfigurationDataTable { get; }
}

// ViewModel class
public class MainWindowViewModel : INotifyPropertyChanged, IMainWindowViewModel
{
    private readonly IConfigurationDataProvider _ConfigurationData;

    public MainWindowViewModel()
    {
        var dataProvider = new ConcreteMainWindowViewModel();
        NozzlePostViewModel.SelectedIdChanged += OnNozzlePostUserControlActive;
        _ConfigurationData = dataProvider;
        ReceiptItems = new ObservableCollection<ShoppingCartItem>();
        Messenger.Default.Register<FillUpChangedMessage>(this, OnFillUpChangedMessageReceived);
    }
    
    private void OnFillUpChangedMessageReceived(FillUpChangedMessage message)
    {
        OnPropertyChanged(nameof(TotalCostText));
    }
    

    public DataTable ConfigurationDataTable => _ConfigurationData.ConfigurationDataTable;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        var handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private NozzlePostViewModel? _selectedNozzlePostInstance;

    public NozzlePostViewModel? SelectedNozzlePostInstance
    {
        get => _selectedNozzlePostInstance;
        set
        {
            if (_selectedNozzlePostInstance == value) return;
            _selectedNozzlePostInstance = value;
            CurrentReceipt.Receipt.RelateNozzlePost = value;
            
            OnPropertyChanged(nameof(TotalCostText));
        }
    }

    private void OnNozzlePostUserControlActive(object sender, NozzlePostViewModel e)
    {
        SelectedNozzlePostInstance = e;
        OnPropertyChanged(nameof(SelectedNozzlePostInstance));
        
        OnPropertyChanged(nameof(ReceiptItems));
        OnPropertyChanged(nameof(TotalCostText));
    }

    public string TotalCostText
    {
        get
        {
            if (SelectedNozzlePostInstance != null)
                if (!SelectedNozzlePostInstance.FillUp)
                    return (SelectedNozzlePostInstance.Summary + GoodsSummary).ToString("C2");
                else
                {
                    return "Н/Д";
                }
            return GoodsSummary.ToString("C2");
        }
    }

    

public void SetGoodsSummary(double getGoodsSummary)
    {
        GoodsSummary = getGoodsSummary;
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
        }
    }

    public string TextGoodsSummary => _goodsSummary.ToString("C2");
    
    public void UpdateReceiptItems(Receipt receipt)
    {
        ReceiptItems = new ObservableCollection<ShoppingCartItem>(ShoppingCartItem.IUpdate(receipt));
        OnPropertyChanged(nameof(ReceiptItems));
        OnPropertyChanged(nameof(GoodsSummary));
        OnPropertyChanged(nameof(TotalCostText));
    }



    public ObservableCollection<ShoppingCartItem> ReceiptItems { get; set; }
    
    
}

public class FillUpChangedMessage
{
}
