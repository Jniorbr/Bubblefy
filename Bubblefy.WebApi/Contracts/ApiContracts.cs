using System.ComponentModel.DataAnnotations;
using Bubblefy.Model;

namespace Bubblefy.WebApi.Contracts;

/// <summary>Credentials used to register or authenticate a Bubblefy user.</summary>
public sealed record CredentialsRequest
{
    [Required, MaxLength(50)]
    public string Nome { get; init; } = string.Empty;

    [Required]
    public string Senha { get; init; } = string.Empty;
}

/// <summary>Public details for a Bubblefy user.</summary>
public sealed record UserResponse(int Id, string Nome);

/// <summary>JWT bearer token returned after successful authentication.</summary>
public sealed record AuthTokenResponse(string AccessToken, string TokenType);

/// <summary>Details for a musical track.</summary>
public sealed record TrackResponse(
    int Id,
    string? Titulo,
    string? Artista,
    string? Genero,
    string? Pais,
    int Ano,
    int BpmMedia,
    int Valencia);

/// <summary>Filters used to search the available musical tracks.</summary>
public sealed record TrackSearchRequest
{
    public string? Genero { get; init; }
    public string? Pais { get; init; }
    public int? Ano { get; init; }
    public int? BpmMediaMinima { get; init; }
    public int? BpmMediaMaxima { get; init; }
    public int? ValenciaMinima { get; init; }
    public int? ValenciaMaxima { get; init; }

    public FiltroBusca ToFiltro() => new()
    {
        Genero = Genero,
        Pais = Pais,
        Ano = Ano,
        BpmMediaMinima = BpmMediaMinima,
        BpmMediaMaxima = BpmMediaMaxima,
        ValenciaMinima = ValenciaMinima,
        ValenciaMaxima = ValenciaMaxima
    };
}

/// <summary>Payload for creating a playlist from existing track IDs.</summary>
public sealed record CreatePlaylistRequest
{
    [Required, MaxLength(100)]
    public string Nome { get; init; } = string.Empty;

    [Required, MinLength(1)]
    public List<int> FaixaIds { get; init; } = [];
}

/// <summary>Playlist details, including the tracks it contains.</summary>
public sealed record PlaylistResponse(
    int Id,
    string? Nome,
    IReadOnlyList<TrackResponse> Faixas);

internal static class ApiContractMappings
{
    public static UserResponse ToResponse(this Usuario usuario) => new(usuario.Id, usuario.Nome);

    public static TrackResponse ToResponse(this Faixa faixa) => new(
        faixa.Id,
        faixa.Titulo,
        faixa.Artista,
        faixa.Genero,
        faixa.Pais,
        faixa.Ano,
        faixa.BpmMedia,
        faixa.Valencia);

    public static PlaylistResponse ToResponse(this Playlist playlist) => new(
        playlist.Id,
        playlist.Nome,
        playlist.Faixas.Select(faixa => faixa.ToResponse()).ToList());
}
