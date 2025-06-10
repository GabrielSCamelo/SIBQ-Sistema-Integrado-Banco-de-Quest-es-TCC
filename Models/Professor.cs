namespace SIBQ.Models
{
    public class Professor : Usuario
    {
        public List<Turma>? Turmas { get; set; } = new List<Turma>();
        public string? IdFuncional { get; set; }
    }
}
