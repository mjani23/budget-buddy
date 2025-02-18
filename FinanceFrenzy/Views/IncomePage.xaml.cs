using Microsoft.Maui.Controls;
using System.Globalization;
using FinanceFrenzy.Models;

namespace FinanceFrenzy.Views;

public partial class IncomePage : ContentPage
{
    public IncomePage()
    {
        InitializeComponent();
        LoadIncome();
    }

    private void LoadIncome()
    {
        double savedIncome = DatabaseHelper.LoadIncomeData();  // Load as double

        incomeLabel.Text = $"Current Income: {savedIncome.ToString("C", new CultureInfo("en-US"))}";
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        string newIncomeString = await DisplayPromptAsync("Update Income", "Enter your new income:", 
            initialValue: DatabaseHelper.LoadIncomeData().ToString(), keyboard: Keyboard.Numeric);

        if (double.TryParse(newIncomeString, NumberStyles.Currency, CultureInfo.InvariantCulture, out double newIncome))
        {
            DatabaseHelper.SaveIncomeData(newIncome);  // Save as double
            LoadIncome();  // Refresh display
        }
        else
        {
            await DisplayAlert("Invalid Input", "Please enter a valid numeric income.", "OK");
        }
    }
}
