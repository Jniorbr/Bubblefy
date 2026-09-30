using Bubblefy.Model;

namespace Bubblefy.Business;

public class MotorBuscaMusical
{
    public List<Faixa> Buscar(IEnumerable<Faixa> faixas, FiltroBusca filtro)
    {
        return faixas.Where(f =>
            (filtro.Genero == null || f.Genero == filtro.Genero) &&
            (filtro.Pais == null || f.Pais == filtro.Pais) &&
            (filtro.Ano == null || f.Ano == filtro.Ano) &&
            (filtro.BpmMediaMinima == null || f.BpmMedia >= filtro.BpmMediaMinima) &&
            (filtro.BpmMediaMaxima == null || f.BpmMedia <= filtro.BpmMediaMaxima) &&
            (filtro.ValenciaMinima == null || f.Valencia >= filtro.ValenciaMinima) &&
            (filtro.ValenciaMaxima == null || f.Valencia <= filtro.ValenciaMaxima)
        ).ToList();
    }
}