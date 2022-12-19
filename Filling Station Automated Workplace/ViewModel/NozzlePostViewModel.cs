using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using Filling_Station_Automated_Workplace.Data;

namespace Filling_Station_Automated_Workplace.ViewModel;

// ViewModel interface
public interface INozzlePostViewModel
{
    DataTable NozzlePostDataTable
    {
        get
        {
            var nozzlePostData = new NozzlePostData();
            return nozzlePostData.NozzlePostDataTable;
        }
    }

    ObservableCollection<string> NozzlePostNames
    {
        get
        {
            var nozzlePostNames = new ObservableCollection<string>();

            foreach (DataRow row in NozzlePostDataTable.Rows) nozzlePostNames.Add(row["Name"].ToString());

            return nozzlePostNames;
        }
    }
}

public class ConcreteNozzlePostViewModel : INozzlePostDataProvider
{
    private readonly NozzlePostData _nozzlePostData;

    public DataTable NozzlePostDataTable => _nozzlePostData.NozzlePostDataTable;

    public ConcreteNozzlePostViewModel()
    {
        _nozzlePostData = new NozzlePostData();
    }
}

// Data interface
public interface INozzlePostDataProvider
{
    DataTable NozzlePostDataTable { get; }
}

// ViewModel class
public class NozzlePostViewModel : INotifyPropertyChanged, INozzlePostViewModel
{
    private readonly INozzlePostDataProvider _nozzlePostData;

    public NozzlePostViewModel(int count, INozzlePostDataProvider nozzlePostData)
    {
        _nozzlePostData = nozzlePostData;
        NozzlePostNames = new ObservableCollection<string>(_nozzlePostData.NozzlePostDataTable.AsEnumerable()
            .Select(row => row.Field<string>("Name"))!);
    }

    public DataTable NozzlePostDataTable => _nozzlePostData.NozzlePostDataTable;
    public ObservableCollection<string> NozzlePostNames { get; }

    public double Price
    {
        get
        {
            var row = NozzlePostDataTable.Rows.Find(SelectedId);
            if (row != null)
                return double.Parse(row["Price"].ToString()!,
                    NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture);

            return 0;
        }
    }

    public string TextPrice => string.Concat(Price.ToString("C2"), " ");

    public double Summary => Price * LiterCount;

    public string TextSummary => (Price * LiterCount).ToString("C2");

    private int _literCount;

    public int LiterCount
    {
        get => FillUp ? LitersFillProgress : _literCount;
        
        set
        {
            if (_literCount == value) return;
            _literCount = value;
            OnPropertyChanged(nameof(SelectedId));
            OnPropertyChanged(nameof(Price));
            OnPropertyChanged(nameof(TextPrice));
            OnPropertyChanged(nameof(Summary));
            OnPropertyChanged(nameof(TextSummary));
        }
    }

    public void SelectionChanged(int id)
    {
        SelectedId = id;
    }

    public void LiterCountChanged(int count)
    {
        LiterCount = count;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        var handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void FillUpFullTank(bool value)
    {
        FillUp = value;
    }

    private bool _fillUp;

    public bool FillUp
    {
        get => _fillUp;
        set
        {
            OnUserControlActive(_selectedId);
            if (_fillUp == value) return;
            _fillUp = value;
            OnPropertyChanged(nameof(LiterCount));
            OnPropertyChanged(nameof(LitersFillProgress));
            OnPropertyChanged(nameof(Summary));
            OnPropertyChanged(nameof(TextSummary));
        }
    }
    
    private int _litersFillProgress;

    public int LitersFillProgress
    {
        get => _litersFillProgress;
        set
        {
            if (_litersFillProgress == value) return;
            _litersFillProgress = value;
            OnPropertyChanged(nameof(LiterCount));
            OnPropertyChanged(nameof(SelectedId));
            OnPropertyChanged(nameof(Summary));
            
        }
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

    public static event EventHandler<int> UserControlActive;
    
    public static void OnUserControlActive(int userControlId)
    {
        UserControlActive?.Invoke(null, userControlId);
    }
    
    private int _selectedId;

    public int SelectedId
    {
        get => _selectedId;
        set
        {
            if (_selectedId != value)
            {
                _selectedId = value;
                OnPropertyChanged(nameof(SelectedId));
                OnPropertyChanged(nameof(_selectedId));
            }
        }
    }
}