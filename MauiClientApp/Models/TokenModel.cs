using SQLite;
using System.Text.Json.Serialization;

namespace MauiClientApp.Models
{
    public class TokenModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [JsonPropertyName("token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("refreshTokenExpiry")]
        public DateTime Expiration { get; set; }
    }
}
