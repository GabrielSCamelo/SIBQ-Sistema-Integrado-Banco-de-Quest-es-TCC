using SIBQ.Models;

public class Simulado_Aluno_Resposta
{
    public int SimuladoAlunoId { get; set; }
    public virtual Simulado_Aluno? SimuladoAluno { get; set; }

    public int QuestaoId { get; set; }
    public virtual Questao? Questao { get; set; }

    public char? Resposta { get; set; }
}