using IDM_Connector.Core;

namespace IDM_Connector;

/// <summary>
/// Интерфейс описывающий архиватор
/// </summary>
public interface IArchiver
{
    Task SaveForArchive(string path, IEnumerable<Employee>? employees, IEnumerable<Position>? positions, IEnumerable<Unit>? units);
}
