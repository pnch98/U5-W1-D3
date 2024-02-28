using System.ComponentModel.DataAnnotations;

namespace Scarpe_Co.Models
{
    public class Utente
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        [ScaffoldColumn(false)]
        public bool IsAdmin { get; set; }
        public Utente() { }
        public Utente(string username, string password, bool isAdmin)
        {
            Username = username;
            Password = password;
            IsAdmin = isAdmin;
        }
        public Utente(int id, string username, string password, bool isAdmin)
        {
            Id = id;
            Username = username;
            Password = password;
            IsAdmin = isAdmin;
        }
    }
}