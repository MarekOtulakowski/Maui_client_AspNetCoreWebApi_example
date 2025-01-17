using System.Text;
using MauiClientApp.Models;
using System.Text.Json;
using SQLite;
using System.IO;

namespace MauiClientApp.Services
{
    public static class AuthService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public static TokenModel _token;
        private const string ApiUrl = ConstantValues.ApiUrl + "/api/auth";
        private static SQLiteAsyncConnection _db;

        public static async void InitializeDatabase(SQLiteAsyncConnection db)
        {
            _db = db;
            await _db.CreateTableAsync<TokenModel>();
            await _db.CreateTableAsync<UserModel>();
        }

        public static async Task<bool> LoginAsync(string username, string password)
        {
            var content = new StringContent(JsonSerializer.Serialize(new { username, password }), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(ApiUrl + "/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    _token = JsonSerializer.Deserialize<TokenModel>(responseString);

                    if (_token != null)
                    {
                        await SaveTokenAsync(_token);

                        var user = new UserModel
                        {
                            Username = username,
                            Password = password
                        };
                        await SaveUserAsync(user);

                        StartTokenRefreshTimer();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public static async Task<UserModel> GetCurrentUserAsync()
        {
            return await _db.Table<UserModel>().FirstOrDefaultAsync();
        }


        private static async Task SaveUserAsync(UserModel user)
        {
            await _db.DeleteAllAsync<UserModel>();
            await _db.InsertAsync(user);
        }

        private static async Task SaveTokenAsync(TokenModel token)
        {
            await _db.DeleteAllAsync<TokenModel>(); 
            await _db.InsertAsync(token); 
        }

        public static async Task DeleteTokenAsync()
        {
            await _db.DeleteAllAsync<TokenModel>();
        }

        public static async Task<TokenModel> GetTokenAsync()
        {
            var token = await _db.Table<TokenModel>().FirstOrDefaultAsync();
            return token;
        }

        private static void StartTokenRefreshTimer()
        {
            Device.StartTimer(TimeSpan.FromMinutes(30), () =>
            {
                Task.Run(async () => await RefreshTokenAsync());
                return true;
            });
        }

        public static async Task<bool> RefreshTokenAsync()
        {
            if (_token == null) return false;

            var user = await _db.Table<UserModel>().FirstOrDefaultAsync();
            if (user == null || string.IsNullOrEmpty(user.Username))
            {
                Console.WriteLine("Nie znaleziono użytkownika w bazie danych.");
                return false;
            }

            var content = new StringContent(JsonSerializer.Serialize(new
            {
                username = user.Username,
                refreshToken = _token.RefreshToken
            }), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(ApiUrl + "/refresh-token", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    _token = JsonSerializer.Deserialize<TokenModel>(responseString);

                    await SaveTokenAsync(_token);

                    Console.WriteLine("Token został pomyślnie odświeżony.");
                    return true;
                }

                Console.WriteLine($"Nie udało się odświeżyć tokenu. StatusCode: {response.StatusCode}");
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}
