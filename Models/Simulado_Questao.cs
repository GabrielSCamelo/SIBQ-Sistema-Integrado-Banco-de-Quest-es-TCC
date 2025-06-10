namespace SIBQ.Models
{
    public class Simulado_Questao
    {
        public virtual Questao? Questao { get; set; }
        public int QuestaoId { get; set; }
        public virtual Simulado? Simulado { get; set; }
        public int SimuladoId { get; set; }
    }
}
