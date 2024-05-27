using IDM_Connector.Core;
using Microsoft.Extensions.Logging;

namespace IDM_Connector;

/// <summary>
/// Описывает валидацию ответа полученного из источника
/// </summary>

public class Validator
{
    private readonly ILogger _logger;

    public Validator(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Описываются правила валидации для объектов 3 типов (Employee, Position, Unit)
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public Task ValidateData(IEnumerable<object> values, ref IEnumerable<Unit>? units, ref IEnumerable<Position>? positions, ref IEnumerable<Employee>? employees)
    {
        try
        {
            // Правила валидации Employee's
            if (values.GetType() == typeof(IEnumerable<Employee>))
            {
                var baseEmployees = (IEnumerable<Employee>)values;
                foreach (var employee in baseEmployees)
                {
                    // если у сотрудника указано несуществующее подразделение 
                    if (units.Select(u => u.Id).Contains(employee.UnitId))
                    {
                        string errorMessage = $"У сотрудника ID:{employee.Id} указано несуществующее подразделение ID:{employee.UnitId}";
                        _logger.LogError(errorMessage);
                        throw new Exception(errorMessage);
                    }

                    // если у сотрудника отсутствует дата приема на работу
                    if (employee.StartDate == null)
                    {
                        _logger.LogWarning($"У сотрудника ID:{employee.Id} отсутствует дата приема на работу(StartDate)");
                        continue;
                    }
                    //Валидация Employee's успешна
                    employees.ToList().Add(employee);
                }
            }
            // Правила валидации Unit's
            else if (values.GetType() == typeof(IEnumerable<Unit>))
            {
                var baseUnits = (IEnumerable<Unit>)values;
                var unitIds = units.Select(u => u.Id).ToList();

                foreach (var unit in baseUnits)
                {
                    if (unit.ParentId != null)
                        // Если нарушена иерархия подразделений, т.е. отсутствует какое-либо родительское подразделение
                        if (!unitIds.Contains((long)unit.ParentId!))
                        {
                            string errorMessage = $"Нарушена иерархия подразделений. У подразделения ID:{unit.Id} указан несуществующий ParentId:{unit.ParentId}";
                            _logger.LogError(errorMessage);
                            throw new InvalidDataException(errorMessage);
                        }
                }
                //Валидация Unit's успешна
                units = baseUnits;
            }

            // У сущности Position не заданы правила валидации - просто записываем в память
            else if (values.GetType() == typeof(IEnumerable<Position>))
            {
                var baseUnits = (IEnumerable<Position>)values;
                positions = baseUnits;
            }

            else
                throw new ArgumentException($"Неожидаемые входные данные: {values.GetType()}");

            return Task.CompletedTask;
        }
        catch
        {
            // В случае ошибки - обнуляем все коллекции
            employees = Enumerable.Empty<Employee>();
            units = Enumerable.Empty<Unit>();
            positions = Enumerable.Empty<Position>();
            throw;
        }
    }
}
