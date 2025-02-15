namespace FinanceFrenzy.Views;

public partial class IncomePage : ContentPage
{
	public IncomePage()
	{
		InitializeComponent();
	}

    private async void btnGoBack_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync($"//dashboard");
    }
}