using Microsoft.Maui.Controls;
using System.Globalization;
using FinanceFrenzy.Models;

namespace FinanceFrenzy.Views
{
    public partial class IncomePage : ContentPage
    {
        private UserInfo currentUser;

        public IncomePage()
        {
            InitializeComponent();
            LoadUserData();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadUserData();
        }

        private void LoadIncome()
        {
            double savedIncome = DatabaseHelper.LoadIncomeData();
            IncomeEntry.Text = savedIncome.ToString();
            incomeLabel.Text = $"Current Income: {savedIncome.ToString("C", new CultureInfo("en-US"))}";
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (!double.TryParse(IncomeEntry.Text, out double newIncome))
            {
                DisplayAlert("Invalid Input", "Please enter a valid income.", "OK");
                return;
            }

            DatabaseHelper.SaveIncomeData(newIncome);
            LoadIncome();

            // Optional: clear entry after save
            IncomeEntry.Text = string.Empty;
        }

        private async void LoadUserData()
        {
            double savedIncome = DatabaseHelper.LoadIncomeData();
            double savedTakeHomePay = DatabaseHelper.LoadTakeHomePay(); 

            TakeHomeLabel.Text = $"Take-Home Pay: {savedTakeHomePay.ToString("C", new CultureInfo("en-US"))}"; // ✅ Show saved take-home pay

            currentUser = await DatabaseHelper.GetUserAsync(1);

            if (currentUser != null)
            {
                StatePicker.SelectedItem = currentUser.State;
                FilingStatusPicker.SelectedItem = currentUser.FilingStatus;
            }
            else
            {
                currentUser = new UserInfo();
            }
            LoadIncome();
        }

        //this calculates the take-home pay based on the user's inputs and saves it to the database
        private async void CalcuateButton_Clicked1(object sender, EventArgs e)
        {


            if (currentUser == null)
            {
                await DisplayAlert("Error", "User data not loaded.", "OK");
                return;
            }

            string state = StatePicker.SelectedItem?.ToString();
            string filingStatus = FilingStatusPicker.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(filingStatus))
            {
                await DisplayAlert("Error", "Please select your state and filing status.", "OK");
                return;
            }

            
            double income = DatabaseHelper.LoadIncomeData();
            if (income == 0)
            {
                await DisplayAlert("Error", "Set Income First.", "OK");
                return;
            }

            double takeHomePay = TaxCalculator.CalculateTakeHomePay(income, state, filingStatus);
            TakeHomeLabel.Text = $"Take-Home Pay: {takeHomePay.ToString("C", new CultureInfo("en-US"))}";

            // Save data back to database
            currentUser.Income = income;
            currentUser.State = state;
            currentUser.FilingStatus = filingStatus;
            currentUser.TakeHomePay = takeHomePay;

            await DatabaseHelper.UpdateUserAsync(currentUser);
            
        
        }


    }
}


