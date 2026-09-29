using Bubblefy.Data;

List<Faixa> faixas = DadosSemente.ObterFaixas();
MotorBuscaMusical motor = new MotorBuscaMusical(faixas);
List<Playlist> playlists = new List<Playlist>();

bool continuar = true;

while (continuar)
{
    Console.WriteLine("\nOlá, bem vindo a ferramenta Bubblefy");
    Console.WriteLine("Crie sua propria bolha!\n");
    Console.WriteLine("Insira seu usuario:");
    string usuario = Console.ReadLine();
    //Console.Clear();
    Console.WriteLine($"Bem vindo {usuario} ");


    Console.WriteLine("\n1 - Ver todas as faixas\n2 - Criar lista a partir de filtros\n3 - Ver listas criadas\n4 - Sair");
    Console.Write("Escolha uma opcao: ");
    string opcao = Console.ReadLine();

    switch (opcao)
    {
        case "1": MostrarFaixas(faixas); break;
        case "2": CriarListaComFiltros(); break;
        case "3": VerPlaylists(); break;
        case "4": continuar = false; break;
        default: Console.WriteLine("Opcao invalida."); break;
    }
}

void CriarListaComFiltros()
{
    Console.WriteLine("\nPreencha os filtros desejados. Deixe em branco para ignorar.");

    FiltroBusca filtro = new FiltroBusca();

    filtro.Genero = EscolherGenero();

    filtro.Pais = EscolherPais();

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

    List<Faixa> resultado = motor.Buscar(filtro);

    Console.WriteLine("\n" + resultado.Count + " faixa(s) encontrada(s):");
    MostrarFaixas(resultado);

    if (resultado.Count == 0)
        return;

    Console.Write("Nome da lista: ");
    string nome = Console.ReadLine();
    playlists.Add(new Playlist { Nome = nome, Faixas = resultado });

    Console.WriteLine("Lista \"" + nome + "\" criada com " + resultado.Count + " faixa(s).");
}

string EscolherGenero()
{
    List<string> genero = faixas.Select(f => f.Genero).Distinct().OrderBy(p => p).ToList();

    Console.WriteLine("\nGeneros disponiveis:");
    for (int i = 0; i < genero.Count; i++)
    {
        Console.Write($"{i + 1} - {genero[i],-15} ");
        if ((i + 1) % 3 == 0)
        {
            Console.WriteLine();
        }

    }
    Console.WriteLine();
    Console.Write("Escolha o Genero: ");
    string opcao = Console.ReadLine()!;

    if (!int.TryParse(opcao, out int numero) || numero == 0 || numero > genero.Count)
        return null;

    return genero[numero - 1];
}

string EscolherPais()
{
    List<string> paises = faixas.Select(f => f.Pais).Distinct().OrderBy(p => p).ToList();

    Console.WriteLine("\nPaises disponiveis:");
    for (int i = 0; i < paises.Count; i++)
    {
        Console.Write($"{i + 1} - {paises[i], -15} ");
        if ((i + 1) % 3 == 0)
        {
            Console.WriteLine();
        }
    }
    Console.WriteLine();
    Console.Write("Escolha o pais: ");
    string opcao = Console.ReadLine()!;

    if (!int.TryParse(opcao, out int numero) || numero == 0 || numero > paises.Count)
        return null;

    return paises[numero - 1];
}



void VerPlaylists()
{
    if (playlists.Count == 0)
    {
        Console.WriteLine("Nenhuma lista criada ainda.");
        return;
    }

    foreach (Playlist p in playlists)
    {
        Console.WriteLine("\nLista: " + p.Nome + " (" + p.Faixas.Count + " faixas)");
        MostrarFaixas(p.Faixas);
    }
}

void MostrarFaixas(List<Faixa> lista)
{
    foreach (Faixa f in lista)
        Console.WriteLine($"{f.Id} - {f.Titulo} - {f.Artista} | {f.Genero} | {f.Pais} | {f.Ano} | BPM {f.BpmMedia} | Valencia {f.Valencia}");
}

string LerTextoOpcional()
{
    string texto = Console.ReadLine();
    return string.IsNullOrWhiteSpace(texto) ? null : texto;
}

int? LerInteiroOpcional()
{
    string texto = Console.ReadLine();
    return int.TryParse(texto, out int valor) ? valor : null;
}
