using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using FinanceFrenzy.Models;
using System.Globalization;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FinanceFrenzy.Views;

public class ExpenseGroup : ObservableCollection<Expense>
{
    public string Category { get; set; }
    public double TotalAmount => this.Sum(e => e.Amount);
    public double BudgetedAmount { get; set; }
    public double RemainingBudget => BudgetedAmount - TotalAmount;

    public ExpenseGroup(string category, double budgetedAmount, IEnumerable<Expense> expenses) : base(expenses)
    {
        Category = category;
        BudgetedAmount = budgetedAmount;
    }
}

public partial class ExpensesPage : ContentPage
{
    private ObservableCollection<ExpenseGroup> ExpensesGrouped = new();
    private List<string?> ExistingCategories = new();

    public ExpensesPage()
    {
        InitializeComponent();
        LoadIncome();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadIncome();
        LoadCategories(); 
        LoadExpenses();   
    }

    private void LoadIncome()
    {
        double savedTakeHomePay = DatabaseHelper.LoadTakeHomePay();
        IncomeLabel.Text = $"Take-Home Pay: {savedTakeHomePay.ToString("C", new CultureInfo("en-US"))}";
        MonthlyPayLabel.Text = $"Monthly Pay: {(savedTakeHomePay / 12).ToString("C", new CultureInfo("en-US"))}";
    }

    private void LoadCategories()
    {
        ExistingCategories = DatabaseHelper.LoadBudgetCategories()
            .Select(b => b.Category)
            .ToList();

        CategoryPicker.ItemsSource = ExistingCategories;
    }

    private void LoadExpenses()
    {
        ExpensesGrouped.Clear();

        var budgetCategories = DatabaseHelper.LoadBudgetCategories();
        var expenses = DatabaseHelper.LoadExpenses();

        var groupedExpenses = expenses.GroupBy(e => e.Category)
            .Select(g =>
            {
                var budget = budgetCategories.FirstOrDefault(b => b.Category == g.Key)?.Amount ?? 0;
                return new ExpenseGroup(g.Key, budget, g);
            });

        foreach (var group in groupedExpenses)
        {
            ExpensesGrouped.Add(group);
        }

        ExpensesListView.ItemsSource = ExpensesGrouped;
    }

    private void expenseAdd_Clicked(object sender, EventArgs e)
    {
        if (CategoryPicker.SelectedItem == null)
        {
            DisplayAlert("Error", "Please select a category.", "OK");
            return;
        }

        string? category = CategoryPicker.SelectedItem.ToString();
        string amountText = ExpenseAmountEntry.Text;
        string tag = ExpenseTagEntry.Text;
        DateTime date = ExpenseDatePicker.Date;

        if (!double.TryParse(amountText, out double amount))
        {
            DisplayAlert("Error", "Please enter a valid numeric amount.", "OK");
            return;
        }

        var newExpense = new Expense
        {
            Category = category,
            Amount = amount,
            Tag = tag,
            Date = date
        };

        DatabaseHelper.SaveExpense(newExpense);
        LoadExpenses();

        ExpenseAmountEntry.Text = "";
        ExpenseTagEntry.Text = "";
    }

    private void DeleteExpense_Clicked(object sender, EventArgs e)
    {
        if (sender is MenuItem menuItem && menuItem.CommandParameter is Expense expense)
        {
            DatabaseHelper.DeleteExpense(expense);
            LoadExpenses();
        }
    }

    private async void NewMonthButton_Clicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Clear All Expenses", "Are you sure you want to clear all current expenses?", "Yes", "Cancel");
        if (confirm)
        {
            DatabaseHelper.DeleteAllExpenses();
            LoadExpenses();
        }
    }

    private async void ExportButton_Clicked(object sender, EventArgs e)
    {
        var expenses = DatabaseHelper.LoadExpenses();
        string reportPath = Path.Combine(FileSystem.AppDataDirectory, "MonthlyExpensesReport.txt");

        var lines = new List<string>
        {
            "Monthly Expenses Report",
            $"Generated on: {DateTime.Now:MMMM dd, yyyy hh:mm tt}",
            "",
            "Category      | Amount   | Date       | Tag",
            "---------------------------------------------"
        };

        foreach (var exp in expenses)
        {
            lines.Add($"{exp.Category,-13} ${exp.Amount,8:F2} {exp.Date:yyyy-MM-dd}   {exp.Tag}");
        }

        File.WriteAllLines(reportPath, lines);

#if WINDOWS || MACCATALYST
        string email = await DisplayPromptAsync("Send Report", "Enter email address to send the report to:");
        if (!string.IsNullOrWhiteSpace(email))
        {
            var message = new EmailMessage
            {
                Subject = "Your Monthly Expense Report",
                Body = "Attached is your monthly expense report.",
                To = new List<string> { email }
            };

            message.Attachments.Add(new EmailAttachment(reportPath));

            try
            {
                await Email.Default.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to send email: {ex.Message}", "OK");
            }
        }
#else
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Share Monthly Expense Report",
            File = new ShareFile(reportPath)
        });
#endif
    }
}
