namespace FinanceFrenzy.Views;

public partial class Settings : ContentPage
{
	public Settings()
	{
		InitializeComponent();
		
	}

	private async void btnGoBack_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync($"//dashboard");
    }
}
