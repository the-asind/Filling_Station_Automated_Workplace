using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Filling_Station_Automated_Workplace.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
   
    public partial class MainWindow : Window
    {
        DispatcherTimer _timer;
        DateTime _sessionTime = new DateTime(0, 0);

        class MyTable
        {
            public MyTable(int Id, string Name, int Count, double Price)
            {
                this.Id = Id;
                this.Name = Name;
                this.Count = Count;
                this.Price = Price;
            }
            public int Id { get; set; }
            public string Name { get; set; }
            public int Count { get; set; }
            public double Price { get; set; }
        }

        //Добавим информацию в таблицу
        private void grid_Loaded(object sender, RoutedEventArgs e)
        {
            List<MyTable> result = new List<MyTable>();
            result.Add(new MyTable(1001, "Масло М-5 5л", 1, 215.5));
            result.Add(new MyTable(3010, "Сигареты Парламент тонкий", 1, 150));
            result.Add(new MyTable(3041, "Боржоми 0,5л", 2, 99.99));
            result.Add(new MyTable(3123, "Киндер Сюрприз", 1, 99.90));
            result.Add(new MyTable(3012, "Шоколад Алёнка 100гр", 3, 119.50));
            result.Add(new MyTable(2165, "Аптечка автомобильная Аполо ", 1, 999));
            grid.ItemsSource = result;
        }

        public MainWindow()
        {
            InitializeComponent();

            //  DispatcherTimer setup
            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();


        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current second
            dateTime.Text = DateTime.Now.ToString("D");
            CurrentTime.Text = DateTime.Now.ToString("H:mm:ss");

            // updating onsession timer
            _sessionTime = _sessionTime.AddSeconds(1);
            shiftTime.Text = String.Format("Смена открыта: {0}", _sessionTime.ToString("H:mm"));

            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

        private void LitreAmountCliked(object sender, RoutedEventArgs e)
        {
            
        }

        private void LiterAmountTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private static readonly Regex _regex = new Regex("[^0-9.]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {

            return !_regex.IsMatch(text);
        }
    }
}