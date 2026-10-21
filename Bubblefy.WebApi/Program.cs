using System.Security.Claims;
using Bubblefy.Service;
using Bubblefy.WebApi.Contracts;
using Bubblefy.WebApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddBubblefyServices();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = TokenService.GetSigningKey(builder.Configuration),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseAuthentication();
app.UseAuthorization();

app.Services.InitializeBubblefyDatabase();

app.MapPost("/api/auth/register", (CredentialsRequest request, IUsuarioService users) =>
        TypedResults.Created("/api/users/me", users.Cadastrar(request.Nome, request.Senha).ToResponse()))
    .WithName("RegisterUser")
    .WithSummary("Cadastra um usuario")
    .Produces<UserResponse>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status409Conflict);

app.MapPost("/api/auth/login", Results<Ok<AuthTokenResponse>, UnauthorizedHttpResult> (
        CredentialsRequest request,
        IUsuarioService users,
        ITokenService tokens) =>
    {
        var user = users.Autenticar(request.Nome, request.Senha);
        return user is null
            ? TypedResults.Unauthorized()
            : TypedResults.Ok(new AuthTokenResponse(tokens.GenerateToken(user), "Bearer"));
    })
    .WithName("LoginUser")
    .WithSummary("Autentica um usuario e retorna JWT")
    .Produces<AuthTokenResponse>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

app.MapGet("/api/users/me", Results<Ok<UserResponse>, NotFound> (
        ClaimsPrincipal principal,
        IUsuarioService users,
        CancellationToken cancellationToken) =>
    {
        int userId = GetUserId(principal);
        var user = users.ObterPorId(userId);
        return user is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(user.ToResponse());
    })
    .RequireAuthorization()
    .WithName("GetCurrentUser")
    .WithSummary("Retorna o usuario autenticado")
    .Produces<UserResponse>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);

app.MapGet("/api/tracks", (IFaixaService tracks, CancellationToken cancellationToken) =>
        tracks.ObterFaixas().Select(track => track.ToResponse()).ToList())
    .WithName("GetTracks")
    .WithSummary("Lista as faixas")
    .Produces<List<TrackResponse>>(StatusCodes.Status200OK);

app.MapGet("/api/tracks/search", (
        string? genero,
        string? pais,
        int? ano,
        int? bpmMediaMinima,
        int? bpmMediaMaxima,
        int? valenciaMinima,
        int? valenciaMaxima,
        IFaixaService tracks,
        CancellationToken cancellationToken) =>
    {
        var filter = new TrackSearchRequest
        {
            Genero = genero,
            Pais = pais,
            Ano = ano,
            BpmMediaMinima = bpmMediaMinima,
            BpmMediaMaxima = bpmMediaMaxima,
            ValenciaMinima = valenciaMinima,
            ValenciaMaxima = valenciaMaxima
        };
        return tracks.Buscar(filter.ToFiltro())
            .Select(track => track.ToResponse())
            .ToList();
    })
    .WithName("SearchTracks")
    .WithSummary("Busca faixas por filtros")
    .Produces<List<TrackResponse>>(StatusCodes.Status200OK);

app.MapGet("/api/tracks/genres", (IFaixaService tracks, CancellationToken cancellationToken) =>
        tracks.ObterGeneros())
    .WithName("GetTrackGenres")
    .WithSummary("Lista os generos disponiveis")
    .Produces<List<string>>(StatusCodes.Status200OK);

app.MapGet("/api/tracks/countries", (IFaixaService tracks, CancellationToken cancellationToken) =>
        tracks.ObterPaises())
    .WithName("GetTrackCountries")
    .WithSummary("Lista os paises disponiveis")
    .Produces<List<string>>(StatusCodes.Status200OK);

app.MapGet("/api/tracks/{id:int}", Results<Ok<TrackResponse>, NotFound> (
        int id,
        IFaixaService tracks,
        CancellationToken cancellationToken) =>
    {
        var track = tracks.ObterPorId(id);
        return track is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(track.ToResponse());
    })
    .WithName("GetTrackById")
    .WithSummary("Busca uma faixa por ID")
    .Produces<TrackResponse>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

app.MapGet("/api/playlists", (
        ClaimsPrincipal principal,
        IPlaylistService playlists,
        CancellationToken cancellationToken) =>
        playlists.Listar(GetUserId(principal)).Select(playlist => playlist.ToResponse()).ToList())
    .RequireAuthorization()
    .WithName("GetPlaylists")
    .WithSummary("Lista as playlists do usuario autenticado")
    .Produces<List<PlaylistResponse>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

app.MapGet("/api/playlists/{id:int}", Results<Ok<PlaylistResponse>, NotFound> (
        int id,
        ClaimsPrincipal principal,
        IPlaylistService playlists,
        CancellationToken cancellationToken) =>
    {
        var playlist = playlists.ObterPorId(GetUserId(principal), id);
        return playlist is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(playlist.ToResponse());
    })
    .RequireAuthorization()
    .WithName("GetPlaylistById")
    .WithSummary("Busca uma playlist por ID")
    .Produces<PlaylistResponse>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/playlists", (
        CreatePlaylistRequest request,
        ClaimsPrincipal principal,
        IPlaylistService playlists) =>
    {
        var playlist = playlists.Criar(GetUserId(principal), request.Nome, request.FaixaIds);
        return TypedResults.Created($"/api/playlists/{playlist.Id}", playlist.ToResponse());
    })
    .RequireAuthorization()
    .WithName("CreatePlaylist")
    .WithSummary("Cria uma playlist a partir de faixas existentes")
    .Produces<PlaylistResponse>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized);

app.Run();

static int GetUserId(ClaimsPrincipal principal) =>
    int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!,
        System.Globalization.CultureInfo.InvariantCulture);