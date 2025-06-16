using System.ComponentModel.DataAnnotations;

namespace SIBQ.Models
{
    public class Aluno : Usuario
    {
        public List<Aluno_Turma>? Turmas { get; set; }
        public List<Simulado_Aluno>? Simulados { get; set; }

        [Required(ErrorMessage = "O campo Matrícula é obrigatório.")]
        [MaxLength(50, ErrorMessage = "A matrícula deve ter no máximo 50 caracteres.")]
        public string? Matricula { get; set; }
    }
}
