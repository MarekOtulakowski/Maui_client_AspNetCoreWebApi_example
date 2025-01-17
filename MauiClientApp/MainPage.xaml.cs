using MauiClientApp.Models;
using MauiClientApp.Services;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace MauiClientApp
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient;
        private const string WeatherApiUrl = ConstantValues.ApiUrl + "/api/WeatherForecast";

        public ObservableCollection<WeatherForecast> WeatherForecasts { get; set; }

        public MainPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            WeatherForecasts = new ObservableCollection<WeatherForecast>();
            weatherListView.ItemsSource = WeatherForecasts;
        }

        private async void OnGetWeatherForecastClicked(object sender, EventArgs e)
        {
            await AuthService.RefreshTokenAsync();

            var token = await AuthService.GetTokenAsync();

            if (token == null || string.IsNullOrEmpty(token.AccessToken))
            {
                await DisplayAlert("Error", "You need to log in first", "OK");
                Application.Current.MainPage = new LoginPage();
                return;
            }

            if (token.Expiration <= DateTime.UtcNow)
            {
                await DisplayAlert("Session Expired", "Your session has expired. Please log in again.", "OK");
                Application.Current.MainPage = new LoginPage();
                return;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(WeatherApiUrl);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    bool isTokenRefreshed = await AuthService.RefreshTokenAsync();
                    if (isTokenRefreshed)
                    {
                        token = await AuthService.GetTokenAsync();
                        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
                        response = await _httpClient.GetAsync(WeatherApiUrl);
                    }
                    else
                    {
                        await DisplayAlert("Session Expired", "Unable to refresh token. Please log in again.", "OK");
                        await AuthService.DeleteTokenAsync();
                        Application.Current.MainPage = new LoginPage();
                        return;
                    }
                }

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var forecasts = JsonSerializer.Deserialize<List<WeatherForecast>>(responseString);

                    WeatherForecasts.Clear();
                    foreach (var forecast in forecasts)
                    {
                        WeatherForecasts.Add(forecast);
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Failed to fetch weather data.", "OK");
                }
            }
            catch
            {
                await DisplayAlert("Error", "Service is unavailable, please try again later.", "OK");
            }
        }
    }
}
