using System.Globalization;
using Microsoft.Maui.Controls;
using FinanceFrenzy.Models;

namespace FinanceFrenzy.Views;


public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();
        LoadDashboardData();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDashboardData(); 
    }

    private void LoadDashboardData()
    {
        double income = DatabaseHelper.LoadIncomeData();
        double takeHome = DatabaseHelper.LoadTakeHomePay();
        double monthlyPay = takeHome / 12;

        double goalAmount = DatabaseHelper.LoadSavingsGoal(); 
        double totalSaved = DatabaseHelper.LoadSavings().Sum(s => s.Amount);
        double remainingGoal = Math.Max(0, goalAmount - totalSaved);

        double totalExpenses = DatabaseHelper.LoadExpenses().Sum(c => c.Amount);

        IncomeLabel.Text = income.ToString("C", new CultureInfo("en-US"));
        TakeHomeLabel.Text = takeHome.ToString("C", new CultureInfo("en-US"));
        ExpensesLabel.Text = totalExpenses.ToString("C", new CultureInfo("en-US"));
        SavingsLabel.Text = remainingGoal.ToString("C", new CultureInfo("en-US"));
    }
}