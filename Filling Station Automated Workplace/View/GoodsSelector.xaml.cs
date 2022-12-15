using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
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
    
    private void WindowClose(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private DataTable _shoppingCartGoodsTable;

    public GoodsSelector()
    {
        InitializeComponent();
        GoodsGrid.ItemsSource = GoodsData.GoodsDataTable.DefaultView;
        _shoppingCartGoodsTable = ShoppingCartItem.Update(CurrentReceipt.Receipt);
        _shoppingCartGoodsTable.AcceptChanges();
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
            // Ignored
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
        var dataGrid = sender as DataGrid;
        var column = e.Column;
        var count = new int();
        try
        {
            var value = (e.EditingElement as TextBox)?.Text;
            count = int.Parse(value);
        }
        catch (Exception exception)
        {
            MessageBox.Show(string.Concat("Ошибка в введённом количестве!\n",exception.Message));
            return;
        }

        // Get the index of the edited row
        var index = dataGrid.ItemContainerGenerator.IndexFromContainer(e.Row);

        _shoppingCartGoodsTable = ShoppingCartItem.Update(CurrentReceipt.Receipt);

        var row = _shoppingCartGoodsTable.Rows[index];
        object[] values = row.ItemArray;
        var id = (int)values[0];

        try
        {
            CurrentReceipt.Receipt.ChangeCountById(id, count);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return;
        }

        _shoppingCartGoodsTable = ShoppingCartItem.Update(CurrentReceipt.Receipt);
        ShoppingCartGrid.ItemsSource = _shoppingCartGoodsTable.DefaultView;
    }

    private void ShoppingCartGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        // Check if the double-click did not occur in the Count column
        var cell = ShoppingCartGrid.CurrentCell.Column;

        // Check if the double-click did not occur in the Count column
        if (cell != null && cell != ShoppingCartGrid.Columns[2])
        {
            try
            {
                if (ShoppingCartGrid.SelectedItem == null)
                    return;
                var dr = ShoppingCartGrid.SelectedItem as DataRowView;
                var dr1 = dr?.Row;
                
                var itemId = int.Parse(Convert.ToString(dr1?.ItemArray[0]) ?? throw new InvalidOperationException());

                CurrentReceipt.Receipt.RemoveIdFromCommodityItem(itemId);
                _shoppingCartGoodsTable = ShoppingCartItem.Update(CurrentReceipt.Receipt);
                ShoppingCartGrid.ItemsSource = _shoppingCartGoodsTable.DefaultView;
            }
            catch (Exception ex)
            {
                // Ignored
            }
        }
    }
}