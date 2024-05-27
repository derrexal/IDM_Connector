using IDM_Connector.Core;

namespace IDM_Connector;

/// <summary>
/// Интерфейс описывающий валидатор
/// </summary>
public interface IValidator
{
    public Task ValidateData(IEnumerable<object> values, ref IEnumerable<Unit>? units, ref IEnumerable<Position>? positions, ref IEnumerable<Employee>? employees);
}
