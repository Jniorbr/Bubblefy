using Bubblefy.Data;
using Bubblefy.Model;
using Microsoft.EntityFrameworkCore;

namespace Bubblefy.Service;

public class PlaylistService(AppDbContext context)
{
    public List<Playlist> Listar(int usuarioId) => context.Playlists
        .Where(playlist => playlist.UsuarioId == usuarioId)
        .Include(playlist => playlist.Faixas)
        .AsNoTracking()
        .ToList();

    public Playlist Criar(int usuarioId, string nome, IEnumerable<Faixa> faixas)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);

        List<int> faixaIds = faixas.Select(faixa => faixa.Id).Distinct().ToList();
        if (faixaIds.Count == 0)
            throw new ArgumentException("A playlist precisa ter ao menos uma faixa.", nameof(faixas));

        List<Faixa> faixasPersistidas = context.Faixas
            .Where(faixa => faixaIds.Contains(faixa.Id))
            .ToList();

        Playlist playlist = new() { Nome = nome, UsuarioId = usuarioId, Faixas = faixasPersistidas };
        context.Playlists.Add(playlist);
        context.SaveChanges();
        return playlist;
    }
}