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

    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    private IEnumerable<Employee> _employees;
    private IEnumerable<Position> _positions;
    private IEnumerable<Unit> _units;

    // Это кошмар, а не конструктор... При такой реализации и DI не прикрутишь.
    public Connector(HttpClient httpClient, ILogger logger, 
        ISourceData sourceData, IArchiver archiver, IValidator validator,
        string baseUrl,string login, string password, string archivePath)
    {
        // Зачем ты просишь передавать тебе httpClient? Тем более, что изменяешь его
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(baseUrl);
        _logger = logger;

        // мб из конструктора вынести в поле?
        string[] endUrls = [Settings.UNIT_END_URL, Settings.POSITION_END_URL, Settings.EMPLOYEE_END_URL];
        
        // Очень плохая практика выносить логику, тем более такую сложную и потенциально долгую в конструктор.
        // Он точно не для этого.
        try
        {
            foreach (string endUrl in endUrls)
            {
                //Получение данных
                var entities = sourceData.GetData(login, password, endUrl);

                //Валидация данных и сохранение в памяти
                validator.ValidateData(entities, ref _units, ref _positions, ref _employees);
            }
            //Сохранение данных в архив
            archiver.SaveForArchive(archivePath, _employees, _positions, _units);
        }
        // И снова не понятно зачем это
        catch { throw; }
    }

    #endregion

    // То есть ты при создании экземпляра уже всё запросил и держишь httpClient открытым всё это время... Зачем?
    public void Dispose()
    {
        _httpClient.Dispose();
    }

    /// <summary>
    /// Получение сотрудников по Id подразделения
    /// </summary>
    /// <param name="unitId"></param>
    /// <returns></returns>
    public IEnumerable<Employee> GetEmployeesByUnit(long unitId)
    {
        try
        {
            if(_employees is null)
                throw new InvalidDataException($"Нет информации о сотрудниках");
            return _employees.Where(e=>e.UnitId == unitId);
        }
        // Ой всё...
        catch { throw; }
    }

    /// <summary>
    /// Получение всех должностей
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Position> GetPositions()
    {
        try
        {
            if (_positions is null)
                throw new InvalidDataException($"Нет информации о должностях сотрудников");
            return _positions.ToList();
        }
        catch { throw; }
    }

    /// <summary>
    /// Получение подразделений по Id старшего подразделения
    /// </summary>
    /// <param name="parentId"></param>
    /// <returns></returns>
    public IEnumerable<Unit> GetUnitsByParentId(long parentId)
    {
        try
        {
            if (_units is null)
                throw new InvalidDataException($"Нет информации о подразделениях к которым относятся сотрудники");
            return _units.Where(e => e.ParentId == parentId);
        }
        catch { throw; }
    }
}