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
    private readonly Receipt _receipt = new();

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
        _shoppingCartGoodsTable = ShoppingCartGoodsTable.Update();
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

            _receipt.AddIdToCommodityItem(itemId);
            _shoppingCartGoodsTable = ShoppingCartGoodsTable.Update(_receipt);
            ShoppingCartGrid.ItemsSource = _shoppingCartGoodsTable.DefaultView;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Попробуйте снова, иначе обратитесь к техническому специалисту\n" + ex.Message,
                "Возникла ошибка при выборе товарной позиции");
        }
    }

    private void AcceptButton_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
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
}