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

bool continuar = true;

while (continuar)
{
	Console.WriteLine("\nOlá, bem vindo a ferramenta Bubblefy");
	Console.WriteLine("Crie sua propria bolha!\n");
	Console.WriteLine("Insira seu usuario:");
	string usuario = Console.ReadLine() ?? string.Empty;
	Console.WriteLine($"Bem vindo {usuario} ");

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

	playlistService.Criar(nome, resultado);
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
	List<Playlist> playlists = playlistService.Listar();
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
