namespace Bubblefy.Data;

public class MotorBuscaMusical
{
    private readonly List<Faixa> _faixas;

    public MotorBuscaMusical(List<Faixa> faixas)
    {
        _faixas = faixas;
    }

    public List<Faixa> Buscar(FiltroBusca filtro)
    {
        return _faixas.Where(f =>
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
