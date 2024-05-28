using IDM_Connector.Core;
using Microsoft.Extensions.Logging;

namespace IDM_Connector;

/// <summary>
/// Описывает валидацию ответа полученного из источника
/// </summary>

public class Validator: IValidator
{
    private readonly ILogger _logger;

    public Validator(ILogger<Validator> logger)
    {
        _logger = logger;
    }

    //TODO:вынести на 2 метода (валидация и сохранение в память)
    public void ValidateEmployeesData(IEnumerable<Employee> employees, IEnumerable<Unit> units)
    {
        var newEmployees = employees.ToList();
        // Правила валидации Employee's
        foreach (var employee in employees)
        {
            // если у сотрудника указано несуществующее подразделение 
            if (units.Any(s => s.Id == employee.UnitId))
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
            newEmployees.Add(employee);
        }
        employees = newEmployees;
    }

    public void ValidatePositionsData(IEnumerable<Position>? positions)
    {
        
    }

    public void ValidateUnitsData(IEnumerable<Unit> units)
    {
        // Правила валидации Unit's
        foreach (var unit in units)
        {
            // Если нарушена иерархия подразделений, т.е. отсутствует какое-либо родительское подразделение
            if (unit.ParentId.HasValue && units.Any(s => s.Id == unit.ParentId.Value))
            {
                string errorMessage = $"Нарушена иерархия подразделений. У подразделения ID:{unit.Id} указан несуществующий ParentId:{unit.ParentId}";
                _logger.LogError(errorMessage);
                throw new InvalidDataException(errorMessage);
            }
        }
    }

    public void Validate<T>(IEnumerable<T> entities, IEnumerable<Unit>? units=null) where T: Entity
    {
        var baseEmployees = entities as IEnumerable<Employee>;
        if (baseEmployees is not null)
        {
            if (units is null)
                throw new ArgumentNullException($"Необходимо передать параметр {nameof(units)}");                    
            ValidateEmployeesData(baseEmployees, units);
            return;
            
        }
            
        var baseUnits = entities as IEnumerable<Unit>;
        if (baseUnits is not null)
        {
            ValidateUnitsData(baseUnits);
            return;
        }

        var basePositions = entities as IEnumerable<Position>;
        if (basePositions is not null)
        {
            ValidatePositionsData(basePositions);
            return;
        }

        throw new InvalidDataException("Ошибка валидации. Функция валидации пройдена до конца и не вернула результат");
    }
}