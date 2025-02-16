namespace FinanceFrenzy;
using FinanceFrenzy.Models;

public partial class App : Application
{
	public App()
    {
        InitializeComponent();
        DatabaseHelper.InitializeDatabase(); // Make sure the database is initialized
        MainPage = new AppShell(); // Use AppShell to manage navigation
    }

    

}