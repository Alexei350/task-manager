namespace TaskManager.Models.Request.Base;

public class GetPagedModel
{
    /// <summary>
    /// Página atual
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Tamanho da página
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Filtro da busca
    /// </summary>
    public string Filter { get; set; }
}