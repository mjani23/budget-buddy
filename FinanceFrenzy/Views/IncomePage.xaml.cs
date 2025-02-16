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
            string savedIncome = DatabaseHelper.LoadIncomeData(); 

            if (double.TryParse(savedIncome, out double incomeAmount))
            {
				incomeLabel.Text = $"Current Income: {incomeAmount.ToString("C", new CultureInfo("en-US"))}";
            }
            else
            {
                incomeLabel.Text = "Current Income: $0.00"; 
            }
        }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        string newIncome = await DisplayPromptAsync("Update Income", "Enter your new income:", 
            initialValue: DatabaseHelper.LoadIncomeData(), keyboard: Keyboard.Numeric);

            if (IsNumeric(newIncome))
            {
                DatabaseHelper.SaveIncomeData(newIncome);  // Save new income
                LoadIncome();  
            }
            else
            {
                await DisplayAlert("Invalid Input", "Please enter a valid numeric income.", "OK");
            }
    }
	
	private bool IsNumeric(string input)
        {
            return decimal.TryParse(input, NumberStyles.Currency, CultureInfo.InvariantCulture, out _);

        }
    
	
}