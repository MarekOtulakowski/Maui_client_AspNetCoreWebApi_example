using MauiClientApp.Models;
using MauiClientApp.Services;

namespace MauiClientApp;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        loadingIndicator.IsRunning = true;
        loadingIndicator.IsVisible = true;

        string username = usernameEntry.Text;
        string password = passwordEntry.Text;

        var isAuthenticated = await AuthService.LoginAsync(username, password);

        loadingIndicator.IsRunning = false;
        loadingIndicator.IsVisible = false;

        if (isAuthenticated)
        {
            Application.Current.MainPage = new MainPage();
        }
        else
        {
            await DisplayAlert("Login Failed", "Invalid username or password or service in unavailable.", "OK");
        }
    }

    private void OnPasswordToggleClicked(object sender, EventArgs e)
    {
        passwordEntry.IsPassword = !passwordEntry.IsPassword;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var savedToken = await AuthService.GetTokenAsync();
        if (savedToken != null)
        {
            AuthService._token = savedToken;
            Application.Current.MainPage = new MainPage();
        }
        else
        {
            UserModel user = await AuthService.GetCurrentUserAsync();
            if (user != null)
            {
                usernameEntry.Text = user.Username;
                passwordEntry.Text = user.Password;
            }
        }
    }
}
