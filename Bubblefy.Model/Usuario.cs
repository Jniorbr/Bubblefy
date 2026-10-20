namespace Bubblefy.Model;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public List<Playlist> Playlists { get; set; } = new();
}