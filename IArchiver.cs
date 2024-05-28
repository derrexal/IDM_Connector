using IDM_Connector.Core;

namespace IDM_Connector;

/// <summary>
/// Интерфейс описывающий архиватор
/// </summary>
public interface IArchiver
{
    void SaveForArchive(string path, IEnumerable<Employee>? employees, IEnumerable<Position>? positions, IEnumerable<Unit>? units);
}
