namespace Bubblefy.Data;

public class Playlist
{
    public int Id { get; set; }
    public string ?Nome { get; set; }
    public List<Faixa> Faixas { get; set; } = new List<Faixa>();
}
