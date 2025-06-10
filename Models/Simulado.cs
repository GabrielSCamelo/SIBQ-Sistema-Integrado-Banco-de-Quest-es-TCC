using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIBQ.Models
{
    public class Simulado
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Today;
        public DateTime DataExpiracao { get; set; } = DateTime.Today.AddDays(30);
        public string? CriadorId { get; set; } // Id do aluno ou professor que criou o simulado
        public virtual Usuario? Criador { get; set; }
        public bool? CriadoPorAluno { get; set; }
        [Range(5, 25, ErrorMessage = "A quantidade de questões deve estar entre 5 e 25.")]
        public int? QuantidadeQuestoes { get; set; }
        public List<int>? disciplinas { get; set; } = new List<int>();
        public List<Simulado_Questao>? simulado_Questaos { get; set; }
        public List<Simulado_Aluno>? simulado_Alunos { get; set; }
        public List<int>? Turmas { get; set; } = new List<int>();

        [NotMapped]
        public bool Ativo
        {
            get
            {
                return (DateTime.Today - DataCriacao).TotalDays <= 30;
            }
        }
        [NotMapped]
        public bool FoiRealizado { get; set; }
    }
}
