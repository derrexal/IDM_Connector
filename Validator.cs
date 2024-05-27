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
    /// 
    // Очень странный интерфейс... Лучше потом голосом обсудим.
    // Вообще с валидацией можно сделать гораздо проще через аттрибуты...
    public Task ValidateData(IEnumerable<object> values, ref IEnumerable<Unit>? units, ref IEnumerable<Position>? positions, ref IEnumerable<Employee>? employees)
    {
        try
        {
            // Вообще тут три типа - можно красивый свитч написать
            
            // Это делается вот так
            // var baseEmployees = values as IEnumerable<Employee>;
            // if (baseEmployees != null)
            // {
            //     
            // }
            
            // Правила валидации Employee's
            if (values.GetType() == typeof(IEnumerable<Employee>))
            {
                var baseEmployees = (IEnumerable<Employee>)values;
                foreach (var employee in baseEmployees)
                {
                    // Это делается вот так
                    //if (units.Any(s => s.Id == employee.UnitId))
                    
                    // если у сотрудника указано несуществующее подразделение 
                    if (units.Select(u => u.Id).Contains(employee.UnitId))
                    {
                        // Если я правильно понимаю, то ты бы хотел собрать все ошибки - собери сперва все ошибки.
                        // Потом проверь пуст ли список ошибок - если пуст, то всё ок. Если нет - то выдай
                        // (лучше какой-нибудь ValidatingException, и поле в нём со списком ошибок) исключение со всем списком.
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
                    
                    // Это прям ошибка... ToList возвращает новый список - ты добавил в новый список элемент и никуда этот новый список не сохранил.
                    //Валидация Employee's успешна
                    employees.ToList().Add(employee);
                }
            }
            // Правила валидации Unit's
            else if (values.GetType() == typeof(IEnumerable<Unit>))
            {
                var baseUnits = (IEnumerable<Unit>)values;
                var unitIds = units.Select(u => u.Id).ToList(); // смотри аналогично в employees

                foreach (var unit in baseUnits)
                {
                    // Это делается вот так
                    //if (unit.ParentId.HasValue && units.Any(s => s.Id == unit.ParentId.Value))
                    
                    if (unit.ParentId != null)
                        // Если нарушена иерархия подразделений, т.е. отсутствует какое-либо родительское подразделение
                        if (!unitIds.Contains((long)unit.ParentId!))
                        {
                            string errorMessage = $"Нарушена иерархия подразделений. У подразделения ID:{unit.Id} указан несуществующий ParentId:{unit.ParentId}";
                            _logger.LogError(errorMessage);
                            throw new InvalidDataException(errorMessage);
                        }
                }
                // Точно не Add, как в случае с employees? :)
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
            // Не понятно зачем вообще здесь try-catch - все эксепшены ты сам бросаешь
            // В случае ошибки - обнуляем все коллекции
            employees = Enumerable.Empty<Employee>();
            units = Enumerable.Empty<Unit>();
            positions = Enumerable.Empty<Position>();
            throw;
        }
    }
}
