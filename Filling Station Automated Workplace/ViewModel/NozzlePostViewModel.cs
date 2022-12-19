using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Security.AccessControl;
using Filling_Station_Automated_Workplace.Data;
using Filling_Station_Automated_Workplace.View;

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
            ObservableCollection<string> nozzlePostNames = new ObservableCollection<string>();

            foreach (DataRow row in NozzlePostDataTable.Rows)
            {
                nozzlePostNames.Add(row["Name"].ToString());
            }

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
public class NozzlePostViewModel : INozzlePostViewModel
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
            DataRow? row = NozzlePostDataTable.Rows.Find(SelectedId);
            if (row != null)
            {
                return double.Parse(row["Price"].ToString()!,
                    NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture);
            }

            return 0;
        }
    }

    public string TextPrice => string.Concat(Price.ToString("C2"), " \nза литр");


    public string Summary
    {
        get
        {
            return (Price * LiterCount).ToString();
        }
    }
    

    public int SelectedId = 0;
    public int LiterCount { get; set; }

    public void SelectionChanged(int id)
    {
        SelectedId = id;
        
    }

    public void LiterCountChanged(int count)
    {
        LiterCount = count;
    }
}
