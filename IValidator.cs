using IDM_Connector.Core;

namespace IDM_Connector;

/// <summary>
/// Интерфейс описывающий валидатор
/// </summary>
public interface IValidator
{
    void ValidateUnitsData(IEnumerable<Unit> units);
    void ValidatePositionsData(IEnumerable<Position> positions);
    void ValidateEmployeesData(IEnumerable<Employee> employees, IEnumerable<Unit> units);
    void Validate<T>(IEnumerable<T> entities, IEnumerable<Unit>? units=null) where T : Entity;

}
