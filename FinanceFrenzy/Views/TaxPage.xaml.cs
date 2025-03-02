namespace FinanceFrenzy.Views;

public partial class TaxPage : ContentPage
{
	public TaxPage()
	{
		InitializeComponent();
	}

    private async void btnGoBack_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync($"//dashboard");
    }
}