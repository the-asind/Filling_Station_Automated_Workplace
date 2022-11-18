using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Filling_Station_Automated_Workplace.View;

public partial class NozzlePost : UserControl
{
    public NozzlePost()
    {
        InitializeComponent();
        this.DataContext = this;
    }
    
    
    public void LitreAmountCliked(object sender, RoutedEventArgs e)
    {
            
    }

    public void LiterAmountTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = _regex.IsMatch(e.Text);
    }

    public static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
    public static bool IsTextAllowed(string text)
    {

        return !_regex.IsMatch(text);
    }

    public string Title { get; set; }
}