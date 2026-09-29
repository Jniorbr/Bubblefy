namespace Bubblefy.Data;


public class Faixa
{
    public int Id { get; set; }
    public string ?Titulo { get; set; }
    public string ?Artista { get; set; }
    public string ?Genero { get; set; }
    public string ?Pais { get; set; }
    public int Ano { get; set; }
    public int BpmMedia { get; set; }
    public int Valencia { get; set; } // 0 = triste, 5 = animado

    public List<Playlist> Playlists { get; set; } = new List<Playlist>();

}

