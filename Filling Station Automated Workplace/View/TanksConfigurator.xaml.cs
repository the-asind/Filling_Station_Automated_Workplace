using System.Data;
using System.Windows;
using Filling_Station_Automated_Workplace.Model;

namespace Filling_Station_Automated_Workplace.Data;

public partial class TanksConfigurator : Window
{
    
    DataTable dataTable = new DataTable();
    
    public TanksConfigurator()
    {
        InitializeComponent();
        dataTable = Deserialize.GetDataTableFromCsvFile("Tanks.csv");
        dataGrid.ItemsSource = dataTable.DefaultView;
    }
}