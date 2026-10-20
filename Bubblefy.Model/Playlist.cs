namespace Bubblefy.Model;

public class Playlist
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public List<Faixa> Faixas { get; set; } = new();
}