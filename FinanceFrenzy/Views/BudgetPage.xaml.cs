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
        var budgetCategories = DatabaseHelper.LoadBudgetCategories();
        BudgetListView.ItemsSource = BudgetCategories;
    }

	protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadIncome(); 
        BudgetCategories.Clear();
        foreach (var category in DatabaseHelper.LoadBudgetCategories())
        {
            BudgetCategories.Add(category);
        }
    }

    //loads takehome pay and monthly pay
    private void LoadIncome()
    {
        double savedTakeHomePay = DatabaseHelper.LoadTakeHomePay();  // Load take-home pay

        
        // Format properly with CultureInfo
        IncomeLabel.Text = $"Take-Home Pay: {savedTakeHomePay.ToString("C", new CultureInfo("en-US"))}";
        MonthlyPayLabel.Text = $"Monthly Pay: {(savedTakeHomePay / 12).ToString("C", new CultureInfo("en-US"))}";
    
    }

    //this adds a new budget category to the listview and database
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

        var newCategory = new BudgetCategory { Category = categoryName, Amount = budgetAmount };
        BudgetCategories.Add(newCategory);
        DatabaseHelper.SaveBudgetCategory(newCategory); // Save to DB

        CategoryEntry.Text = string.Empty;
        BudgetAmountEntry.Text = string.Empty;
    }

    //this deletes a budget category from the listview and database
    private void DeleteClicked(object sender, EventArgs e)
    {
        var menuItem = sender as MenuItem;
        if (menuItem?.CommandParameter is BudgetCategory category)
        {
            DatabaseHelper.DeleteBudgetCategory(category); 
            BudgetCategories.Remove(category);
        }
    }

}
