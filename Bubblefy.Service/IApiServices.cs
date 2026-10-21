using Bubblefy.Model;

namespace Bubblefy.Service;

public interface IUsuarioService
{
    Usuario Cadastrar(string nome, string senha);
    Usuario? Autenticar(string nome, string senha);
    Usuario? ObterPorId(int id);
}

public interface IFaixaService
{
    List<Faixa> ObterFaixas();
    Faixa? ObterPorId(int id);
    List<string> ObterGeneros();
    List<string> ObterPaises();
    List<Faixa> Buscar(FiltroBusca filtro);
}

public interface IPlaylistService
{
    List<Playlist> Listar(int usuarioId);
    Playlist? ObterPorId(int usuarioId, int playlistId);
    Playlist Criar(int usuarioId, string nome, IEnumerable<int> faixaIds);
}

public interface ITokenService
{
    string GenerateToken(Usuario usuario);
}
