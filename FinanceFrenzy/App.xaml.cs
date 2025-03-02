namespace FinanceFrenzy;
using FinanceFrenzy.Models;

public partial class App : Application
{
	public App()
    {
        InitializeComponent();
        DatabaseHelper.InitializeDatabase(); 
        MainPage = new AppShell(); 
    }

    

}