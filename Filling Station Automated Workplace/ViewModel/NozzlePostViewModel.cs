using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using Filling_Station_Automated_Workplace.Data;
using GalaSoft.MvvmLight.Messaging;

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
public sealed class NozzlePostViewModel : INotifyPropertyChanged, INozzlePostViewModel
{
    private readonly INozzlePostDataProvider _nozzlePostData;

    public NozzlePostViewModel(int count, INozzlePostDataProvider nozzlePostData)
    {
        _nozzlePostData = nozzlePostData;
        NozzlePostNames = new ObservableCollection<string>(_nozzlePostData.NozzlePostDataTable.AsEnumerable()
            .Select(row => row.Field<string>("Name"))!);
        NozzlelId = count;
    }

    public DataTable NozzlePostDataTable => _nozzlePostData.NozzlePostDataTable;
    public ObservableCollection<string> NozzlePostNames { get; }

    public double Price
    {
        get
        {
            var row = NozzlePostDataTable.Rows.Find(SelectedFuelId);
            if (row != null)
                return double.Parse(row["Price"].ToString()!,
                    NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture);

            return 0;
        }
    }

    public string TextPrice => Price.ToString("C2");

    public double Summary => Price * LiterCount + 0;

    public string TextSummary => (Price * LiterCount).ToString("C2");


    private int _literCount;

    public int LiterCount
    {
        get => FillUp ? LitersFillProgress : _literCount;
        
        set
        {
            if (_literCount == value) return;
            _literCount = value;
            OnUserControlActive(this);
            OnPropertyChanged(nameof(SelectedFuelId));
            OnPropertyChanged(nameof(Price));
            OnPropertyChanged(nameof(TextPrice));
            OnPropertyChanged(nameof(Summary));
            OnPropertyChanged(nameof(TextSummary));
        }
    }

    public void SelectionChanged(string? name)
    {
        SelectedFuelName = name;
        OnUserControlActive(this);
        
        DataRow[] rows = NozzlePostDataTable.Select("Name = '" + name + "'");
        if (rows.Length > 0)
        {
            int id = int.Parse(rows[0]["id"].ToString() ?? throw new InvalidOperationException("invalid id in Tanks.csv!"));
            SelectedFuelId = id;
        }
        else
        {
            throw new ArgumentException("id of " + name + " doesn't exist");
        }
    }

    public void LiterCountChanged(int count)
    {
        LiterCount = count;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        var handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void FillUpFullTank(bool value)
    {
        FillUp = value;
        Messenger.Default.Send(new FillUpChangedMessage());
    }

    private bool _fillUp;

    public bool FillUp
    {
        get => _fillUp;
        set
        {
            OnUserControlActive(this);
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
            OnPropertyChanged(nameof(SelectedFuelId));
            OnPropertyChanged(nameof(Summary));
            OnPropertyChanged(nameof(TextPrice));
            OnPropertyChanged(nameof(TextSummary));
        }
    }
    
    public static void OnUserControlActive(NozzlePostViewModel nozzlePostVm)
    {
        SelectedIdChanged?.Invoke(null, nozzlePostVm);
    }
    
    public static event EventHandler<NozzlePostViewModel>? SelectedIdChanged;

    private int _selectedFuelId;

    public int SelectedFuelId
    {
        get => _selectedFuelId;
        set
        {
            if (_selectedFuelId != value)
            {
                _selectedFuelId = value;
                
                OnPropertyChanged(nameof(SelectedFuelName));
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(TextPrice));
                SelectedIdChanged?.Invoke(this, this);
                OnPropertyChanged(nameof(TextSummary));
            }
        }
    }
    
    private string? _selectedFuelName;

    public string? SelectedFuelName
    {
        get => _selectedFuelName;
        set
        {
            if (_selectedFuelName != value)
            {
                _selectedFuelName = value;
                OnPropertyChanged(nameof(SelectedFuelId));
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(TextSummary));
            }
        }
    }
    
    private int _nozzlelId;

    public int NozzlelId
    {
        get => _nozzlelId;
        set
        {
            if (_nozzlelId != value)
            {
                _nozzlelId = value;
                OnPropertyChanged(nameof(SelectedFuelId));
                OnPropertyChanged(nameof(SelectedFuelName));
                OnPropertyChanged(nameof(Summary));
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(TextSummary));
                SelectedIdChanged?.Invoke(this, this);
            }
        }
    }
}