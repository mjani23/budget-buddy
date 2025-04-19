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

    
}
