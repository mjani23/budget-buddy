using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace FinanceFrenzy.Views;

public partial class Settings : ContentPage
{
    private const string ThemePreferenceKey = "AppTheme";

    public Settings()
    {
        InitializeComponent();
        LoadSavedTheme();
    }

    private void LoadSavedTheme()
    {
        string savedTheme = Preferences.Get(ThemePreferenceKey, "Light");

        if (savedTheme == "Dark")
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
            DarkModeSwitch.IsToggled = true;
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            DarkModeSwitch.IsToggled = false;
        }
    }

    private void DarkModeSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
            Preferences.Set(ThemePreferenceKey, "Dark");
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            Preferences.Set(ThemePreferenceKey, "Light");
        }
    }

    private async void btnGoBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"//dashboard");
    }
}
