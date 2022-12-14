using System;
using System.Data;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Filling_Station_Automated_Workplace.Model;
using Filling_Station_Automated_Workplace.ViewModel;

namespace Filling_Station_Automated_Workplace.View;

/// <summary>
///     Логика взаимодействия для Window1.xaml
/// </summary>
public partial class GoodsSelector
{
    public class GoodsGridData
    {
        public GoodsGridData(int id, string name, int count, double price)
        {
            Id = id;
            Name = name;
            Count = count;
            Price = price;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }

    private void MovingWindow(object sender, RoutedEventArgs e)
    {
        DragMove();
    }


    private void WindowClose(object sender, EventArgs e)
    {
        Close();
    }

    private DataTable _shoppingCartGoodsTable;

    public GoodsSelector()
    {
        InitializeComponent();
        GoodsGrid.ItemsSource = GoodsData.GoodsDataTable.DefaultView;
        _shoppingCartGoodsTable = ShoppingCartItem.Update(CurrentReceipt.Receipt);
        ShoppingCartGrid.ItemsSource = _shoppingCartGoodsTable.DefaultView;
        GoodsGrid.GridLinesVisibility = DataGridGridLinesVisibility.All;
    }

    private void Row_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        try
        {
            if (GoodsGrid.SelectedItem == null)
                return;
            var dr = GoodsGrid.SelectedItem as DataRowView;
            var dr1 = dr?.Row;

            var itemId = int.Parse(Convert.ToString(dr1?.ItemArray[0]) ?? throw new InvalidOperationException());

            CurrentReceipt.Receipt.AddIdToCommodityItem(itemId);
            _shoppingCartGoodsTable = ShoppingCartItem.Update(CurrentReceipt.Receipt);
            ShoppingCartGrid.ItemsSource = _shoppingCartGoodsTable.DefaultView;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Попробуйте снова, иначе обратитесь к техническому специалисту\n" + ex.Message,
                "Возникла ошибка при выборе товарной позиции");
        }
    }
    
    private void SearchGoodsTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var t = (TextBox)sender;
        var filterText = t.Text.ToLower();

        if (filterText == "")
        {
            GoodsGrid.ItemsSource = GoodsData.GoodsDataTable.DefaultView;
            return;
        }

        try
        {
            var dataTableFiltered = GoodsData.GoodsDataTable.AsEnumerable()
                .Where(row => row.Field<string>("Name")!.ToLower().Contains(filterText))
                .OrderByDescending(row => row.Field<string>("Id"))
                .CopyToDataTable();

            GoodsGrid.ItemsSource = dataTableFiltered.DefaultView;
        }
        catch
        {
            // ignored
        }
    }

    private void SearchGoodsTextBox_OnGotFocus(object sender, RoutedEventArgs e)
    {
        var t = (TextBox)sender;
        t.Text = "";
    }

    private void ShoppingCartGrid_OnCellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
    {
        DataGrid dataGrid = sender as DataGrid;
        DataGridColumn column = e.Column;

        string value = (e.EditingElement as TextBox)?.Text;
        int count = int.Parse(value);
        
        // Get the index of the edited row
        int index = dataGrid.ItemContainerGenerator.IndexFromContainer(e.Row);
        
        _shoppingCartGoodsTable = ShoppingCartItem.Update(CurrentReceipt.Receipt);

        DataRow row = _shoppingCartGoodsTable.Rows[index];
        object[] values = row.ItemArray;
        int id = (int)values[0];

        try
        {
            CurrentReceipt.Receipt.ChangeCountById(id, count);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        _shoppingCartGoodsTable = ShoppingCartItem.Update(CurrentReceipt.Receipt);
        ShoppingCartGrid.ItemsSource = _shoppingCartGoodsTable.DefaultView;
        
        
        
        
    }
}

