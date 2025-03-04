using Microsoft.Maui.Controls;
using System.Globalization;
using FinanceFrenzy.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace FinanceFrenzy.Views;

// Represents a group of expenses under a single category
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
    private List<string> ExistingCategories = new();

    public ExpensesPage()
    {
        InitializeComponent();
        LoadIncome();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
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

        CategoryPicker.ItemsSource = null; 
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

        string categoryToUse = CategoryPicker.SelectedItem.ToString();
        string expenseAmountText = ExpenseAmountEntry.Text;
        string tag = ExpenseTagEntry.Text;

        if (!double.TryParse(expenseAmountText, out double expenseAmount))
        {
            DisplayAlert("Error", "Please enter a valid numeric amount.", "OK");
            return;
        }

        var newExpense = new Expense { Category = categoryToUse, Amount = expenseAmount, Tag = tag };
        DatabaseHelper.SaveExpense(newExpense);

        LoadExpenses();
        ExpenseAmountEntry.Text = string.Empty;
        ExpenseTagEntry.Text = string.Empty;
    }

    private void DeleteExpense_Clicked(object sender, EventArgs e)
    {
        var menuItem = sender as MenuItem;
        var expense = menuItem?.CommandParameter as Expense;

        if (expense != null)
        {
            DatabaseHelper.DeleteExpense(expense);
            LoadExpenses();
        }
    }
}
