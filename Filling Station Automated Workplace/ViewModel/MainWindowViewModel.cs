using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows;
using Filling_Station_Automated_Workplace.Data;

namespace Filling_Station_Automated_Workplace.ViewModel;

// ViewModel interface
public interface IMainWindowViewModel
{
    DataTable ConfigurationDataTable
    {
        get
        {
            var ConfigurationData = new ConfigurationData();
            return ConfigurationData.ConfigurationDataTable;
        }
    }

    ObservableCollection<string> MainWindowNames
    {
        get
        {
            var MainWindowNames = new ObservableCollection<string>();

            foreach (DataRow row in ConfigurationDataTable.Rows) MainWindowNames.Add(row["Name"].ToString());

            return MainWindowNames;
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
    }

    public DataTable ConfigurationDataTable => _ConfigurationData.ConfigurationDataTable;
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        var handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private NozzlePostViewModel _selectedNozzlePostInstance;

    public NozzlePostViewModel SelectedNozzlePostInstance
    {
        get => _selectedNozzlePostInstance;
        set
        {
            if (_selectedNozzlePostInstance == value) return;
            _selectedNozzlePostInstance = value;
            
        }
    }

    private void OnNozzlePostUserControlActive(object sender, NozzlePostViewModel e)
    {
        SelectedNozzlePostInstance = e;
        OnPropertyChanged(nameof(SelectedNozzlePostInstance));
    }
}