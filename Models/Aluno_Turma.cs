namespace SIBQ.Models
{
    public class Aluno_Turma
    {
        public required string AlunoId { get; set; }
        public virtual Aluno? aluno { get; set; }//um aluno pode ter varias turmas
        public int TurmaId { get; set; }
        public virtual Turma? Turma { get; set; }//uma turma pode ter varios alunos
    }
}
