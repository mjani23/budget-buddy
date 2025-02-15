namespace FinanceFrenzy.Views;

public partial class BudgetPage : ContentPage
{
	public BudgetPage()
	{
		InitializeComponent();
	}

    private async void btnGoBack_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync($"//dashboard");
    }
}