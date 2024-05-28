using IDM_Connector.Core;
using IDM_Connector.Shared;
using Microsoft.Extensions.Logging;

namespace IDM_Connector;

/// <summary>
/// Класс-коннектор для получения данных из удаленного источника (REST API). 
/// </summary>
public class Connector: IConnector
{
    #region init

    private readonly ILogger _logger;

    private IEnumerable<Employee> _employees;
    private IEnumerable<Position> _positions;
    private IEnumerable<Unit> _units;

    public Connector(ILogger logger, 
    ISourceData sourceData, IArchiver archiver, IValidator validator, Settings settings)
    {
        _logger = logger;

        try
        {
            //Получение данных о подразделениях
            var entitiesUnit = sourceData.GetData<Unit>(settings, Options.UNIT_END_URL);
            //Валидация данных
            validator.Validate<Unit>(entitiesUnit);
            //Сохранение данных в память
            _units = entitiesUnit;

            //Получение данных о сотрудниках
            var entitiesEmployee = sourceData.GetData<Employee>(settings, Options.EMPLOYEE_END_URL);
            //Валидация данных
            validator.Validate<Employee>(entitiesEmployee, _units);
            //Сохранение данных в память
            _employees = entitiesEmployee;

            //Получаем данные об должностях
            var entitiesPosition = sourceData.GetData<Position>(settings, Options.POSITION_END_URL);
            //Валидация данных
            validator.Validate<Position>(entitiesPosition);
            //Сохранение данных в память
            _positions = entitiesPosition;

            //Сохранение данных в архив
            archiver.SaveForArchive(settings.ArchivePath, _employees, _positions, _units);
        }
        catch
        {
            //Обнуляем коллекции если получили ошибку
            _employees = Enumerable.Empty<Employee>();
            _units = Enumerable.Empty<Unit>();
            _positions = Enumerable.Empty<Position>();
            throw;
        }
    }

    #endregion

    public void Dispose(){}

    /// <summary>
    /// Получение сотрудников по Id подразделения
    /// </summary>
    /// <param name="unitId"></param>
    /// <returns></returns>
    public IEnumerable<Employee> GetEmployeesByUnit(long unitId)
    {
        if(_employees.Any())
            throw new InvalidDataException($"Нет информации о сотрудниках");
        return _employees.Where(e=>e.UnitId == unitId);
    }

    /// <summary>
    /// Получение всех должностей
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Position> GetPositions()
    {
        if (_positions.Any())
            throw new InvalidDataException($"Нет информации о должностях сотрудников");
        return _positions.ToList();
    }

    /// <summary>
    /// Получение подразделений по Id старшего подразделения
    /// </summary>
    /// <param name="parentId"></param>
    /// <returns></returns>
    public IEnumerable<Unit> GetUnitsByParentId(long parentId)
    {
        if (_units.Any())
            throw new InvalidDataException($"Нет информации о подразделениях к которым относятся сотрудники");
        return _units.Where(e => e.ParentId == parentId);
    }
}