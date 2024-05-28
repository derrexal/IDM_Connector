using IDM_Connector.Core;
using IDM_Connector.Shared;

namespace IDM_Connector;

/// <summary>
/// Интерфейс описывающий источник данных
/// </summary>
public interface ISourceData
{
    IEnumerable<T> GetData<T>(Settings settings, string endUrl) where T : Entity;
}