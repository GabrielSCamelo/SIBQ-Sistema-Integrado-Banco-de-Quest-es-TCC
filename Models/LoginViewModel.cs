using System.ComponentModel.DataAnnotations;

namespace SIBQ.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O campo E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo E-mail não é um endereço de e-mail válido.")] //Email Institucional
        public required string Nickname { get; set; }

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
