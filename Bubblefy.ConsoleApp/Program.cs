using Bubblefy.Model;
using Bubblefy.Service;
using Microsoft.Extensions.DependencyInjection;

ServiceCollection services = new();
services.AddBubblefyServices();
using ServiceProvider provider = services.BuildServiceProvider();
provider.InitializeBubblefyDatabase();
using IServiceScope scope = provider.CreateScope();
FaixaService faixaService = scope.ServiceProvider.GetRequiredService<FaixaService>();
PlaylistService playlistService = scope.ServiceProvider.GetRequiredService<PlaylistService>();
UsuarioService usuarioService = scope.ServiceProvider.GetRequiredService<UsuarioService>();

Usuario? usuarioLogado = null;
bool continuar = true;

Console.WriteLine("\nOlá, bem vindo a ferramenta Bubblefy");
Console.WriteLine("Crie sua propria bolha!");

while (continuar && usuarioLogado == null)
{
	Console.WriteLine("\n1 - Entrar\n2 - Cadastrar\n3 - Sair");
	Console.Write("Escolha uma opcao: ");
	string opcaoAcesso = Console.ReadLine() ?? string.Empty;

	switch (opcaoAcesso)
	{
		case "1":
			usuarioLogado = Entrar();
			break;
		case "2":
			Cadastrar();
			break;
		case "3":
			continuar = false;
			break;
		default:
			Console.WriteLine("Opcao invalida.");
			break;
	}
}

while (continuar)
{
	Console.WriteLine($"\nBem vindo {usuarioLogado!.Nome}");
	Console.WriteLine("\n1 - Ver todas as faixas\n2 - Criar lista a partir de filtros\n3 - Ver listas criadas\n4 - Sair");
	Console.Write("Escolha uma opcao: ");
	string opcao = Console.ReadLine() ?? string.Empty;

	switch (opcao)
	{
		case "1": MostrarFaixas(faixaService.ObterFaixas()); break;
		case "2": CriarListaComFiltros(); break;
		case "3": VerPlaylists(); break;
		case "4": continuar = false; break;
		default: Console.WriteLine("Opcao invalida."); break;
	}
}

Usuario? Entrar()
{
	Console.Write("Usuario: ");
	string nome = Console.ReadLine() ?? string.Empty;
	Console.Write("Senha: ");
	string senha = LerSenha();

	Usuario? usuario = usuarioService.Autenticar(nome, senha);
	if (usuario == null)
		Console.WriteLine("Usuario ou senha invalidos.");

	return usuario;
}

void Cadastrar()
{
	Console.Write("Usuario: ");
	string nome = Console.ReadLine() ?? string.Empty;
	Console.Write("Senha: ");
	string senha = LerSenha();

	try
	{
		usuarioService.Cadastrar(nome, senha);
		Console.WriteLine("Usuario cadastrado. Agora e so entrar.");
	}
	catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
	{
		Console.WriteLine(ex.Message);
	}
}

string LerSenha()
{
	string senha = string.Empty;

	while (true)
	{
		ConsoleKeyInfo tecla = Console.ReadKey(true);

		if (tecla.Key == ConsoleKey.Enter)
			break;

		if (tecla.Key == ConsoleKey.Backspace && senha.Length > 0)
			senha = senha[..^1];
		else if (!char.IsControl(tecla.KeyChar))
			senha += tecla.KeyChar;
	}

	Console.WriteLine();
	return senha;
}

void CriarListaComFiltros()
{
	Console.WriteLine("\nPreencha os filtros desejados. Deixe em branco para ignorar.");

	FiltroBusca filtro = new()
	{
		Genero = EscolherGenero(),
		Pais = EscolherPais()
	};

	Console.Write("Ano: ");
	filtro.Ano = LerInteiroOpcional();
	Console.Write("BPM medio minimo: ");
	filtro.BpmMediaMinima = LerInteiroOpcional();
	Console.Write("BPM medio maximo: ");
	filtro.BpmMediaMaxima = LerInteiroOpcional();
	Console.Write("Valencia minima (0 a 5): ");
	filtro.ValenciaMinima = LerInteiroOpcional();
	Console.Write("Valencia maxima (0 a 5): ");
	filtro.ValenciaMaxima = LerInteiroOpcional();

	List<Faixa> resultado = faixaService.Buscar(filtro);
	Console.WriteLine("\n" + resultado.Count + " faixa(s) encontrada(s):");
	MostrarFaixas(resultado);

	if (resultado.Count == 0)
		return;

	Console.Write("Nome da lista: ");
	string nome = Console.ReadLine() ?? string.Empty;
	if (string.IsNullOrWhiteSpace(nome))
	{
		Console.WriteLine("Nome invalido.");
		return;
	}

	playlistService.Criar(usuarioLogado!.Id, nome, resultado);
	Console.WriteLine("Lista \"" + nome + "\" criada com " + resultado.Count + " faixa(s).");
}

string? EscolherGenero()
{
	List<string> generos = faixaService.ObterGeneros();
	ExibirOpcoes("Generos disponiveis", "Escolha o Genero", generos);
	return ObterOpcao(generos);
}

string? EscolherPais()
{
	List<string> paises = faixaService.ObterPaises();
	ExibirOpcoes("Paises disponiveis", "Escolha o pais", paises);
	return ObterOpcao(paises);
}

void ExibirOpcoes(string titulo, string instrucao, List<string> opcoes)
{
	Console.WriteLine($"\n{titulo}:");
	for (int i = 0; i < opcoes.Count; i++)
	{
		Console.Write($"{i + 1} - {opcoes[i],-15} ");
		if ((i + 1) % 3 == 0)
			Console.WriteLine();
	}

	Console.WriteLine();
	Console.Write($"{instrucao}: ");
}

string? ObterOpcao(List<string> opcoes)
{
	string escolha = Console.ReadLine() ?? string.Empty;
	if (!int.TryParse(escolha, out int numero) || numero < 1 || numero > opcoes.Count)
		return null;

	return opcoes[numero - 1];
}

void VerPlaylists()
{
	List<Playlist> playlists = playlistService.Listar(usuarioLogado!.Id);
	if (playlists.Count == 0)
	{
		Console.WriteLine("Nenhuma lista criada ainda.");
		return;
	}

	foreach (Playlist playlist in playlists)
	{
		Console.WriteLine("\nLista: " + playlist.Nome + " (" + playlist.Faixas.Count + " faixas)");
		MostrarFaixas(playlist.Faixas);
	}
}

void MostrarFaixas(IEnumerable<Faixa> faixas)
{
	foreach (Faixa faixa in faixas)
		Console.WriteLine($"{faixa.Id} - {faixa.Titulo} - {faixa.Artista} | {faixa.Genero} | {faixa.Pais} | {faixa.Ano} | BPM {faixa.BpmMedia} | Valencia {faixa.Valencia}");
}

int? LerInteiroOpcional()
{
	string texto = Console.ReadLine() ?? string.Empty;
	return int.TryParse(texto, out int valor) ? valor : null;
}
