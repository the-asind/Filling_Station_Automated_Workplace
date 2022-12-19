using System.Collections.ObjectModel;
using System.Data;
using Filling_Station_Automated_Workplace.Data;

namespace Filling_Station_Automated_Workplace.ViewModel;

// ViewModel interface
public interface IGoodsSelectorViewModel
{
    DataTable GoodsDataTable { get; }
}

public class ConcreteGoodsSelectorViewModel : IGoodsDataProvider
{
    private readonly GoodsData _goodsData;

    public DataTable GoodsDataTable => _goodsData.GoodsDataTable;

    public ConcreteGoodsSelectorViewModel()
    {
        _goodsData = new GoodsData();
    }
}


// Data interface
public interface IGoodsDataProvider
{
    DataTable GoodsDataTable { get; }
}

// ViewModel class
public class GoodsSelectorViewModel : IGoodsSelectorViewModel
{
    private readonly IGoodsDataProvider _goodsData;

    public GoodsSelectorViewModel(IGoodsDataProvider goodsData)
    {
        this._goodsData = goodsData;
    }

    public DataTable GoodsDataTable => _goodsData.GoodsDataTable;

    // Define a method to filter the GoodsDataTable by a given text
    public DataTable GetFilteredGoodsDataTable(string filterText)
    {
        if (string.IsNullOrEmpty(filterText))
        {
            return GoodsDataTable;
        }

        try
        {
            var dataTableFiltered = GoodsDataTable.AsEnumerable()
                .Where(row => row.Field<string>("Name")!.ToLower().Contains(filterText.ToLower()))
                .OrderByDescending(row => row.Field<string>("Id"))
                .CopyToDataTable();

            return dataTableFiltered;
        }
        catch
        {
            // Return empty DataTable to say View that it should leave the previous filtered output
            return new DataTable();
        }
    }
}