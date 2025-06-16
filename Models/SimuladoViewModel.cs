namespace SIBQ.Models
{
    public class SimuladoViewModel
    {
        public List<Simulado>? simulados { get; set; } = new List<Simulado>();
        public string? DataCriacao { get; set; }
        public string? DataExpiracao { get; set; }
        public Dictionary<string, char>? Respostas { get; set; }
        public Simulado? Simulado { get; set; } = new Simulado();
    }
}
