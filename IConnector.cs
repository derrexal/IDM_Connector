using IDM_Connector.Core;

namespace IDM_Connector;

/// <summary>
/// Интерфейс описывающий http-коннектор
/// </summary>
public interface IConnector : IDisposable
{
    IEnumerable<Employee> GetEmployeesByUnit(long unitId);
    IEnumerable<Position> GetPositions();
    IEnumerable<Unit> GetUnitsByParentId(long parentId);
}
