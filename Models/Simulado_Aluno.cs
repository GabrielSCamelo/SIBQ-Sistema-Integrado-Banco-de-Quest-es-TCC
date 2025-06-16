namespace SIBQ.Models
{
    public class Simulado_Aluno
    {
        public int SimuladoId { get; set; }
        public virtual Simulado? Simulado { get; set; }
        public required string AlunoId { get; set; }
        public virtual Aluno? Aluno { get; set; }
        public List<Simulado_Aluno_Resposta>? Respostas { get; set; } = new List<Simulado_Aluno_Resposta>();
    }
}
