namespace SIBQ.Models
{
    public class QuestaoViewModel
    {
        public Questao? Questao { get; set; }
        public List<Disciplina>? Disciplinas { get; set; }
        public List<Questao>? Questaos { get; set; }
        public string? titulo { get; set; }
        public int? PaginaAtual { get; set; }
        public int? TotalPaginas { get; set; }
        public int? DisciplinaId { get; set; }
    }
}
