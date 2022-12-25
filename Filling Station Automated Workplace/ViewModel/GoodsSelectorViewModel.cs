using System;
using System.Data;
using Filling_Station_Automated_Workplace.Data;
using Filling_Station_Automated_Workplace.Model;

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
        _goodsData = goodsData;
    }

    private DataTable? ShoppingCartDataTable { get; } = new();

    public DataTable GoodsDataTable => _goodsData.GoodsDataTable;

    //TODO: закончить перенос логики из GoodsSelectorView во ViewModel, чтоб по-человечески было.
    public void UpdateShoppingCartData(int index, int count)
    {
        var row = ShoppingCartDataTable?.Rows[index];
        var values = row?.ItemArray;
        var id = (int)(values?[0] ?? 0);

        try
        {
            CurrentSession.CurrentReceipt.ChangeCountById(id, count);
        }
        catch (ArgumentException)
        {
            // Ignored
        }
    }

    // Define a method to filter the GoodsDataTable by a given text
    public DataTable GetFilteredGoodsDataTable(string filterText)
    {
        if (string.IsNullOrEmpty(filterText)) return GoodsDataTable;

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