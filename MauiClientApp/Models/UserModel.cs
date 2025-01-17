using SQLite;

namespace MauiClientApp.Models
{
    public class UserModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; } 
    }
}
