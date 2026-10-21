using Bubblefy.Data;
using Bubblefy.Model;
using Microsoft.EntityFrameworkCore;

namespace Bubblefy.Service;

public class PlaylistService(AppDbContext context) : IPlaylistService
{
    public List<Playlist> Listar(int usuarioId) => context.Playlists
        .Where(playlist => playlist.UsuarioId == usuarioId)
        .Include(playlist => playlist.Faixas)
        .AsNoTracking()
        .ToList();

    public Playlist? ObterPorId(int usuarioId, int playlistId) => context.Playlists
        .Where(playlist => playlist.UsuarioId == usuarioId && playlist.Id == playlistId)
        .Include(playlist => playlist.Faixas)
        .AsNoTracking()
        .FirstOrDefault();

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

    public Playlist Criar(int usuarioId, string nome, IEnumerable<int> faixaIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        if (nome.Length > 100)
            throw new ArgumentException("O nome da playlist deve ter ate 100 caracteres.", nameof(nome));
        ArgumentNullException.ThrowIfNull(faixaIds);

        List<int> ids = faixaIds.Distinct().ToList();
        if (ids.Count == 0)
            throw new ArgumentException("A playlist precisa ter ao menos uma faixa.", nameof(faixaIds));

        List<Faixa> faixas = context.Faixas
            .Where(faixa => ids.Contains(faixa.Id))
            .ToList();
        if (faixas.Count != ids.Count)
            throw new ArgumentException("Uma ou mais faixas informadas nao existem.", nameof(faixaIds));

        return Criar(usuarioId, nome, faixas);
    }
}