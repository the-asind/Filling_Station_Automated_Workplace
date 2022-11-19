using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Filling_Station_Automated_Workplace.Model;

namespace Filling_Station_Automated_Workplace.View
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class GoodsSelector : Window
    {
        public class GoodsGridd
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Count { get; set; }
            public double Price { get; set; }

        }
        
        void Window_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }
        public GoodsSelector()
        {
            InitializeComponent();
            
            GoodsGrid.ItemsSource = Goods.DataTable.DefaultView;
            ShoppingCartGrid.ItemsSource = _shoppingCartGoodsTable.DefaultView;
            GoodsGrid.GridLinesVisibility = DataGridGridLinesVisibility.All;
        }
        
        DataTable _shoppingCartGoodsTable = new DataTable();

        private void Row_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (GoodsGrid.SelectedItem == null)
                    return;
                DataRowView? dr = GoodsGrid.SelectedItem as DataRowView;
                DataRow? dr1 = dr?.Row;
                MessageBox.Show(Convert.ToString(dr1?.ItemArray[0]));
                
                _shoppingCartGoodsTable.Rows.Add(Convert.ToString(dr1?.ItemArray[0]));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Попробуйте снова, иначе, обратитесь к техническому специалисту\n"+ex.Message, "Возникла ошибка при выборе товарной позиции");
            }

        }

        

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void AcceptButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

    
}
