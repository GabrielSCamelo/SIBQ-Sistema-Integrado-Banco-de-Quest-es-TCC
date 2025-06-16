using SIBQ.Models;

public class Simulado_Aluno_Resposta
{
    public int SimuladoId { get; set; }
    public string AlunoId { get; set; }
    public int QuestaoId { get; set; }
    public virtual Questao? Questao { get; set; }
    public char? Resposta { get; set; }
    public virtual Simulado_Aluno? Simulado_Aluno { get; set; }
}