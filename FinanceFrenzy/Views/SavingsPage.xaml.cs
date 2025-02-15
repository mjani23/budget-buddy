namespace FinanceFrenzy.Views;

public partial class SavingsPage : ContentPage
{
	public SavingsPage()
	{
		InitializeComponent();
	}

   	private async void btnGoBack_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync($"//dashboard");
    }

}