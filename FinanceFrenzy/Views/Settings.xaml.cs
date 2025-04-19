using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using FinanceFrenzy.Models;
using System.Globalization;
using System.Text;

namespace FinanceFrenzy.Views;

public partial class Settings : ContentPage
{
    private const string ThemePreferenceKey = "AppTheme";

    public Settings()
    {
        InitializeComponent();
        LoadSavedTheme();
    }

    private void LoadSavedTheme()
    {
        string savedTheme = Preferences.Get(ThemePreferenceKey, "Light");
        Application.Current.UserAppTheme = savedTheme == "Dark" ? AppTheme.Dark : AppTheme.Light;
        DarkModeSwitch.IsToggled = savedTheme == "Dark";
    }

    private void DarkModeSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        var theme = e.Value ? "Dark" : "Light";
        Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
        Preferences.Set(ThemePreferenceKey, theme);
    }

    private async void btnGoBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"//dashboard");
    }

    private async void ExportAllButton_Clicked(object sender, EventArgs e)
    {
        string path = Path.Combine(FileSystem.AppDataDirectory, "FullFinanceReport.txt");

        var sb = new StringBuilder();
        sb.AppendLine("Finance Frenzy - Full Report");
        sb.AppendLine($"Generated on: {DateTime.Now:MMMM dd, yyyy hh:mm tt}\n");

        sb.AppendLine($"Income: {DatabaseHelper.LoadIncomeData():C}");
        sb.AppendLine($"Take-Home Pay: {DatabaseHelper.LoadTakeHomePay():C}\n");

        sb.AppendLine("Budget Categories:");
        foreach (var cat in DatabaseHelper.LoadBudgetCategories())
            sb.AppendLine($"- {cat.Category}: {cat.Amount:C}");
        sb.AppendLine();

        sb.AppendLine("Expenses:");
        foreach (var exp in DatabaseHelper.LoadExpenses())
            sb.AppendLine($"{exp.Category} | {exp.Amount:C} | {exp.Date:yyyy-MM-dd} | {exp.Tag}");
        sb.AppendLine();

        sb.AppendLine("Savings:");
        foreach (var save in DatabaseHelper.LoadSavings())
            sb.AppendLine($"{save.Amount:C} | {save.Date:yyyy-MM-dd} | {save.Tag}");
        sb.AppendLine();

        sb.AppendLine($"Savings Goal: {DatabaseHelper.LoadSavingsGoal()}\n");

        File.WriteAllText(path, sb.ToString());

#if WINDOWS || MACCATALYST
        string email = await DisplayPromptAsync("Send Report", "Enter email address:");
        if (!string.IsNullOrWhiteSpace(email))
        {
            var message = new EmailMessage
            {
                Subject = "Finance Frenzy Report",
                Body = "Attached is your full financial report.",
                To = new List<string> { email }
            };
            message.Attachments.Add(new EmailAttachment(path));

            try
            {
                await Email.Default.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Email failed: {ex.Message}", "OK");
            }
        }
#else
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Share Full Report",
            File = new ShareFile(path)
        });
#endif
    }

    private async void ClearAllButton_Clicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Clear All Data", "This will delete all saved income, expenses, savings, and budgets. Continue?", "Yes", "Cancel");
        if (confirm)
        {
            DatabaseHelper.DeleteAllExpenses();
            DatabaseHelper.DeleteAllSavings();
            DatabaseHelper.DeleteAllBudgets();
            Preferences.Clear(); // clears app settings like dark mode + theme prefs
            await DisplayAlert("Data Cleared", "All app data has been reset.", "OK");
        }
    }
}
