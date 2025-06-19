using Microsoft.AspNetCore.Mvc.Rendering;

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

        public List<SelectListItem> Alternativas { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "A", Text = "A" },
            new SelectListItem { Value = "B", Text = "B" },
            new SelectListItem { Value = "C", Text = "C" },
            new SelectListItem { Value = "D", Text = "D" },
            new SelectListItem { Value = "E", Text = "E" }
        };

    }
}