using Microsoft.AspNetCore.Identity;

namespace SIBQ.Models
{
    public abstract class Usuario : IdentityUser
    {
        public List<int>? SimuladoCriado { get; set; } = new List<int>();
    }
}
