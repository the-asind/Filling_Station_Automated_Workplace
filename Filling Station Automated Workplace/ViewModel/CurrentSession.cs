namespace Filling_Station_Automated_Workplace.ViewModel;

public static class CurrentSession
{
    public static Receipt CurrentReceipt = new Receipt();

    public static void CreateNewReceipt()
    {
        CurrentReceipt = new Receipt();
    }
    
}