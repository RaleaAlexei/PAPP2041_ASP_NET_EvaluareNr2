using Microsoft.AspNetCore.Identity;

namespace Library.Models
{
    public class Utilizator : IdentityUser
    {
        public string FullName { get; set; }
    }
}
