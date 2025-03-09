using Microsoft.Maui.Controls;
using System.Globalization;
using FinanceFrenzy.Models;
using System.Collections.ObjectModel;

namespace FinanceFrenzy.Views;

public partial class BudgetPage : ContentPage
{
    private ObservableCollection<BudgetCategory> BudgetCategories = new();

    public BudgetPage()
    {
        InitializeComponent();
        LoadIncome();
        LoadBudgetCategories();
        BudgetListView.ItemsSource = BudgetCategories;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadIncome();
        LoadBudgetCategories();
    }

    private void LoadIncome()
    {
        double savedTakeHomePay = DatabaseHelper.LoadTakeHomePay();
        IncomeLabel.Text = $"Take-Home Pay: {savedTakeHomePay.ToString("C", new CultureInfo("en-US"))}";
        MonthlyPayLabel.Text = $"Monthly Pay: {(savedTakeHomePay / 12).ToString("C", new CultureInfo("en-US"))}";
    }

    private void LoadBudgetCategories()
    {
        BudgetCategories.Clear();
        foreach (var category in DatabaseHelper.LoadBudgetCategories())
        {
            category.CanBeDeleted = category.Category != "Miscellaneous"; // Prevent deletion of Miscellaneous
            BudgetCategories.Add(category);
        }
    }

    private void AddCategoryButton_Clicked(object sender, EventArgs e)
    {
        string categoryName = CategoryEntry.Text;
        string budgetAmountText = BudgetAmountEntry.Text;

        if (string.IsNullOrWhiteSpace(categoryName) || string.IsNullOrWhiteSpace(budgetAmountText))
        {
            DisplayAlert("Error", "Please enter a category name and amount.", "OK");
            return;
        }

        if (!double.TryParse(budgetAmountText, out double budgetAmount))
        {
            DisplayAlert("Error", "Please enter a valid numeric budget amount.", "OK");
            return;
        }

        var existingCategory = BudgetCategories.FirstOrDefault(c => c.Category == categoryName);
        if (existingCategory != null)
        {
            existingCategory.Amount = budgetAmount;  // Update the amount if category exists
            DatabaseHelper.SaveBudgetCategory(existingCategory);
        }
        else
        {
            var newCategory = new BudgetCategory { Category = categoryName, Amount = budgetAmount };
            BudgetCategories.Add(newCategory);
            DatabaseHelper.SaveBudgetCategory(newCategory);
        }

        LoadBudgetCategories();
        CategoryEntry.Text = string.Empty;
        BudgetAmountEntry.Text = string.Empty;
    }

    private void DeleteClicked(object sender, EventArgs e)
    {
        var menuItem = sender as MenuItem;
        if (menuItem?.CommandParameter is BudgetCategory category && category.CanBeDeleted)
        {
            DatabaseHelper.DeleteBudgetCategory(category);
            LoadBudgetCategories();
        }
        else
        {
            DisplayAlert("Error", "You cannot delete the 'Miscellaneous' category.", "OK");
        }
    }
}
