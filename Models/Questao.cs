namespace SIBQ.Models
{
    public class Questao
    {
        public int Id { get; set; }
        public string? Titulo { get; set; } // Título da questão
        public string? Enunciado { get; set; } // Enunciado da questão
        public int? DisciplinaId { get; set; } // Id da disciplina associada
        public virtual Disciplina? Disciplina { get; set; } // Disciplina associada à questão
        public string? OpcaoA { get; set; } // Opção A da questão
        public string? OpcaoB { get; set; } // Opção B da questão
        public string? OpcaoC { get; set; } // Opção C da questão
        public string? OpcaoD { get; set; } // Opção D da questão
        public string? OpcaoE { get; set; } // Opção E da questão
        public char? OpcaoCorretaIndex { get; set; } // Índice da opção correta ('A', 'B', 'C', 'D', 'E')
        public List<Simulado_Questao>? simulado_Questaos { get; set; }
    }
}
