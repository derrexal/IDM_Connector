namespace IDM_Connector;

/// <summary>
/// Интерфейс описывающий источник данных
/// </summary>
public interface ISourceData
{
    public IEnumerable<object> GetData(string login, string password, string endUrl);
}