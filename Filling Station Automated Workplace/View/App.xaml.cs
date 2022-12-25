using Filling_Station_Automated_Workplace.Domain;

namespace Filling_Station_Automated_Workplace.View;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public App()
    {
        new DataSecure().EnsureAssetsExists();
    }
}