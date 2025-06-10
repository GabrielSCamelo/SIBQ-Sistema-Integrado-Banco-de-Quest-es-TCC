namespace SIBQ.Models
{
    public class Turma
    {
        public int Id { get; set; }
        public int DisciplinaId { get; set; }
        public virtual Disciplina Disciplina { get; set; } = null!;
        public required string Periodo { get; set; }
        public required string ProfessorId { get; set; }
        public virtual Professor Professor { get; set; } = null!;
        public required List<Aluno_Turma>? aluno_Turmas { get; set; }
    }
}
