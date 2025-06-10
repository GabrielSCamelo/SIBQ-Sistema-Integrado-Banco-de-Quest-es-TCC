namespace SIBQ.Models
{
    public class CriarSimuladoViewModel
    {
        public string? Titulo { get; set; }
        public int QuantidadeQuestoes { get; set; }
        public List<int>? DisciplinasSelecionadas { get; set; } = new List<int>();//seleciona disciplinas
        public List<int>? TurmasSelecionadas { get; set; } = new List<int>(); //selecionas as turmas caso seja professor
        public List<Disciplina>? DisciplinasDisponiveis { get; set; } = new List<Disciplina>(); //vai ser enviado a lista de disciplinas
        public List<Turma>? TurmasDisponiveis { get; set; } = new List<Turma>(); // vai ser enviado a lista de turmas disponiveis
        public Simulado? Simulado { get; set; } = new Simulado();
    }
}
