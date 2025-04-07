using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using FinanceFrenzy.Models;

namespace FinanceFrenzy.Views
{
    public partial class SavingsPage : ContentPage
    {
        private ObservableCollection<Saving> savingsList = new();
        private double savingsGoal = 0;

        public SavingsPage()
        {
            InitializeComponent();
        }

        private void LoadIncome()
        {
            double takeHome = DatabaseHelper.LoadTakeHomePay();
            IncomeLabel.Text = $"Take-Home Pay: {takeHome.ToString("C", new CultureInfo("en-US"))}";
            MonthlyPayLabel.Text = $"Monthly Pay: {(takeHome / 12).ToString("C", new CultureInfo("en-US"))}";
        }

        private void LoadSavings()
        {
            var savings = DatabaseHelper.LoadSavings();
            savingsList = new ObservableCollection<Saving>(savings.OrderByDescending(s => s.Date));
            ContributionsListView.ItemsSource = savingsList;
            UpdateSavingsSummary();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadIncome();
            LoadIncome();
            LoadSavings();
            LoadSavedGoal();
        }

        private void LoadSavedGoal()
        {
            double goal = DatabaseHelper.LoadSavingsGoal();
            savingsGoal = goal;
            RecommendedSavingsLabel.Text = $"Recommended Monthly Savings: {goal.ToString("C", new CultureInfo("en-US"))}";
            UpdateSavingsSummary();
        }

        private void CalculateSavingsGoal_Clicked(object sender, EventArgs e)
        {
            if (double.TryParse(PercentEntry.Text, out double percent))
            {
                double monthly = DatabaseHelper.LoadTakeHomePay() / 12;
                savingsGoal = (percent / 100) * monthly;
                RecommendedSavingsLabel.Text = $"Recommended Monthly Savings: {savingsGoal.ToString("C", new CultureInfo("en-US"))}";
                DatabaseHelper.SaveSavingsGoal(savingsGoal);
                UpdateSavingsSummary();
            }
            else
            {
                DisplayAlert("Invalid", "Enter a valid percentage.", "OK");
            }
        }

        private void AddContribution_Clicked(object sender, EventArgs e)
        {
            if (!double.TryParse(ContributionAmountEntry.Text, out double amount))
            {
                DisplayAlert("Invalid", "Please enter a valid amount.", "OK");
                return;
            }

            string tag = ContributionTagEntry.Text;
            DateTime date = ContributionDatePicker.Date;

            var newSaving = new Saving
            {
                Amount = amount,
                Tag = tag,
                Date = date
            };

            DatabaseHelper.SaveSaving(newSaving);
            savingsList.Insert(0, newSaving);

            ContributionAmountEntry.Text = "";
            ContributionTagEntry.Text = "";
            ContributionDatePicker.Date = DateTime.Today;

            UpdateSavingsSummary();
        }

        private void DeleteContribution_Clicked(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            var saving = menuItem?.CommandParameter as Saving;
            if (saving != null)
            {
                DatabaseHelper.DeleteSaving(saving);
                savingsList.Remove(saving);
                UpdateSavingsSummary();
            }
        }

        private void UpdateSavingsSummary()
        {
            double totalSaved = savingsList.Sum(s => s.Amount);
            double remaining = savingsGoal - totalSaved;

            SavingsSummaryLabel.Text = $"Goal: {savingsGoal.ToString("C", new CultureInfo("en-US"))}  |  Total Saved: {totalSaved.ToString("C", new CultureInfo("en-US"))}  |  Remaining: {remaining.ToString("C", new CultureInfo("en-US"))}";
        }
    }
}
