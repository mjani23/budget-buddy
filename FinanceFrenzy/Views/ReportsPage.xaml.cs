namespace FinanceFrenzy.Views;

public partial class ReportsPage : ContentPage
{
	public ReportsPage()
	{
		InitializeComponent();
	}

    private async void btnGoBack_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync($"//dashboard");
    }
}