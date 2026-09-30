using Bubblefy.Business;
using Bubblefy.Data;
using Bubblefy.Model;
using Microsoft.EntityFrameworkCore;

namespace Bubblefy.Service;

public class FaixaService(AppDbContext context, MotorBuscaMusical motorBusca)
{
    public List<Faixa> ObterFaixas() => context.Faixas.AsNoTracking().ToList();

    public List<string> ObterGeneros() => context.Faixas
        .Where(faixa => faixa.Genero != null)
        .Select(faixa => faixa.Genero!)
        .Distinct()
        .OrderBy(genero => genero)
        .ToList();

    public List<string> ObterPaises() => context.Faixas
        .Where(faixa => faixa.Pais != null)
        .Select(faixa => faixa.Pais!)
        .Distinct()
        .OrderBy(pais => pais)
        .ToList();

    public List<Faixa> Buscar(FiltroBusca filtro) => motorBusca.Buscar(ObterFaixas(), filtro);
}