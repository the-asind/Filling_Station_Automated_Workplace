using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using Filling_Station_Automated_Workplace.Data;
using Filling_Station_Automated_Workplace.Model;
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

    ObservableCollection<string?> NozzlePostNames
    {
        get
        {
            var nozzlePostNames = new ObservableCollection<string?>();

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
        NozzlePostNames = new ObservableCollection<string?>(_nozzlePostData.NozzlePostDataTable.AsEnumerable()
            .Select(row => row.Field<string>("Name")));
        NozzlelId = count;
    }

    public DataTable NozzlePostDataTable => _nozzlePostData.NozzlePostDataTable;
    public ObservableCollection<string?> NozzlePostNames { get; }

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
        get => _literCount;
        
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
        
        var rows = NozzlePostDataTable.Select("Name = '" + name + "'");
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
        if (Deserialize.GetTankReserveById(SelectedFuelId) >= count) LiterCount = count;
        else throw new ArgumentException();
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
        private set
        {
            OnUserControlActive(this);
            if (_fillUp == value) return;
            _fillUp = value;
            if (value) LiterCountChanged(0);
            OnPropertyChanged(nameof(LiterCount));
            OnPropertyChanged(nameof(Summary));
            OnPropertyChanged(nameof(TextSummary));
        }
    }
    
    private static void OnUserControlActive(NozzlePostViewModel nozzlePostVm)
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

    // ReSharper disable once MemberCanBePrivate.Global
    public int NozzlelId
    {
        get => _nozzlelId;
        init
        {
            if (_nozzlelId == value) return;
            _nozzlelId = value;
            OnPropertyChanged(nameof(SelectedFuelId));
            OnPropertyChanged(nameof(SelectedFuelName));
            OnPropertyChanged(nameof(Summary));
            OnPropertyChanged(nameof(Price));
            OnPropertyChanged(nameof(TextSummary));
            SelectedIdChanged?.Invoke(this, this);
        }
    }
    
    private bool _isNozzlePostBusy;
    public bool IsNozzlePostBusy
    {
        get => _isNozzlePostBusy;
        set
        {
            if (_isNozzlePostBusy == value) return;
            _isNozzlePostBusy = value;
            OnPropertyChanged(nameof(IsNozzlePostBusy));
        }
    }

    public void StartFueling()
    {
        // Initialize the timer
        _timer = new DispatcherTimer();
        _timer.Start();
        _timer.Interval = TimeSpan.FromMilliseconds(100);
        if (FillUp) _timer.Tick += FuelingFillUpTimerTick;   
        else _timer.Tick += FuelingTimerTick;   
    }
    
    private void FuelingTimerTick(object? sender, EventArgs e)
    {
        double increment = _random.NextDouble() * (0.2/LiterCount) + 0.01;

        Progress += increment;

        if (Progress >= 1)
        {
            _timer.Stop();
            IsNozzlePostBusy = false;
            Progress = 0;
        }
    }
    
    private void FuelingFillUpTimerTick(object? sender, EventArgs e)
    {
        double increment = _random.NextDouble() * 0.01 + 0.01;

        Progress += increment;
        LiterCountChanged(_random.Next(0, 1));
        if (Progress >= 1)
        {
            _timer.Stop();
            IsNozzlePostBusy = false;
            Progress = 0;
        }
        
    }
    
    public byte ProgressInPercent => (byte)(Progress * 100);

    private readonly Random _random = new Random();
    private DispatcherTimer _timer = new DispatcherTimer();
    
    private double _progress;
    public double Progress
    {
        get => _progress;
        set
        {
            switch (value)
            {
                case >= 1:
                    _progress = 1;
                    break;
                case < 0:
                    return;
            }

            _progress = value;
            OnPropertyChanged(nameof(ProgressInPercent));
            OnPropertyChanged(nameof(Progress));
        }
    }
}
