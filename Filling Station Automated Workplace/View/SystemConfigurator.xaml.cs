using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Filling_Station_Automated_Workplace.Domain;

namespace Filling_Station_Automated_Workplace.View;

public partial class SystemConfigurator : Window
{
    public SystemConfigurator()
    {
        InitializeComponent();
        var SystemDataTable = Deserialize.GetDataTableFromCsvFile("System.csv");
        SystemConfigurationGrid.ItemsSource = SystemDataTable.DefaultView;
    }
    
    private void AcceptChangesButton_Click(object sender, RoutedEventArgs e)
    {
        // Get the DataTable from the DataGrid's ItemsSource
        var dataTable = (SystemConfigurationGrid.ItemsSource as DataView)?.Table;
        if (dataTable == null) return;
        
        // Check if the ID is not repeated in all lines and that it can only be numbers and not an empty value
        var ids = new List<int>();
        foreach (DataRow row in dataTable.Rows)
        {
            if (row["ID"] == DBNull.Value || !int.TryParse(row["ID"].ToString(), out int id) || id <= 0 || ids.Contains(id))
            {
                MessageBox.Show("Некорректное значение в поле ID", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            ids.Add(id);
        }

        // Check for empty rows and cells and delete empty rows
        for (int i = dataTable.Rows.Count - 1; i >= 0; i--)
        {
            bool isEmptyRow = true;
            for (int j = 0; j < dataTable.Columns.Count; j++)
            {
                if (dataTable.Rows[i][j] != DBNull.Value && !string.IsNullOrWhiteSpace(dataTable.Rows[i][j].ToString()))
                {
                    isEmptyRow = false;
                    break;
                }
            }
            if (isEmptyRow) dataTable.Rows.RemoveAt(i);
        }

        foreach (DataRow row in dataTable.Rows)
        {
            if (row["Name"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Name"].ToString()))
            {
                MessageBox.Show("Некорректное значение в поле Название", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
        }

        // Check if the values in the Reserve and Price columns are valid Doubles
        foreach (DataRow row in dataTable.Rows)
        {
            if (row["Count"] == DBNull.Value || !int.TryParse(row["Count"].ToString(), out _))
            {
                MessageBox.Show("Некорректное значение в поле Остаток", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (row["Price"] == DBNull.Value || !double.TryParse(row["Price"].ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out _))
            {
                MessageBox.Show("Некорректное значение в поле Цена", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
        }
        
        // Serialize the DataTable to the CSV file
        Serialize.WriteDataTableToCsv(dataTable, "System.csv");
        Close();
    }


    public void CancelChangesButton_Click(object sender, RoutedEventArgs e)
            {
                var originalDataTable = Deserialize.GetDataTableFromCsvFile("System.csv");
                SystemConfigurationGrid.ItemsSource = originalDataTable.DefaultView;
                Close();
            }

            private void DeleteRowMenuItem_Click(object sender, RoutedEventArgs e)
            {
                if (SystemConfigurationGrid.SelectedItem != null)
                    try
                    {
                        var selectedItem = SystemConfigurationGrid.SelectedItem;
                        var itemsSource = SystemConfigurationGrid.ItemsSource as IList;
                        itemsSource?.Remove(selectedItem);
                    }
                    catch
                    {
                        MessageBox.Show(
                            "Наведите курсор на строку с данными и повторите действие, чтобы удалить её",
                            "Ошибка удаления строки", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
            }


            private void DataGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
            {
                var dataGrid = (DataGrid)sender;
                if (dataGrid.SelectedCells.Count > 0)
                {
                    var selectedRow = dataGrid.SelectedCells[0].Item;
                    dataGrid.SelectedItem = selectedRow;
                }
            }
        }