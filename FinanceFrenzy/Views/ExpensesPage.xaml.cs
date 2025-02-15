namespace FinanceFrenzy.Views;

public partial class ExpensesPage : ContentPage
{
	public ExpensesPage()
	{
		InitializeComponent();
	}

    private async void btnGoBack_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync($"//dashboard");
    }
}