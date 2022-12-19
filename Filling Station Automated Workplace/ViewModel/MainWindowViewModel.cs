using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
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
        NozzlePostViewModel.UserControlActive += OnNozzlePostUserControlActive;
        _ConfigurationData = dataProvider;
    }

    public DataTable ConfigurationDataTable => _ConfigurationData.ConfigurationDataTable;
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        var handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private int _selectedNozzlePostId;

    public int SelectedNozzlePostId
    {
        get => _selectedNozzlePostId;
        set
        {
            if (_selectedNozzlePostId == value) return;
            _selectedNozzlePostId = value;
            
        }
    }
    
    private void OnNozzlePostSelectedIdChanged(object sender, int e)
    {
        SelectedNozzlePostId = e;
    }
    
    private int _activeUserControlId;
    public int ActiveUserControlId
    {
        get => _activeUserControlId;
        set
        {
            if (_activeUserControlId != value)
            {
                _activeUserControlId = value;
                OnPropertyChanged(nameof(ActiveUserControlId));
            }
        }
    }

    private void OnNozzlePostUserControlActive(object sender, int e)
    {
        ActiveUserControlId = e;
    }
}