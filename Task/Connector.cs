//public class Employee
//{
//    public long Id { get; set; }

//    // Табельный номер
//    public string PersonnelNumber { get; set; }
//    public string FullName { get; set; }
//    public WorkStatus Status { get; set; }

//    // Должность
//    public long PositionId { get; set; }

//    // Подразделение
//    public long UnitId { get; set; }

//    // Основная или по совместительству
//    public bool? IsMainJob { get; set; }

//    // Дата приема на работу
//    public DateTime? StartDate { get; set; }
//}

//public enum WorkStatus
//{
//    Work = 0,
//    Dismissed = 1
//}

//public class Unit
//{
//    public long Id { get; set; }
//    public long? ParentId { get; set; }
//    public string FullName { get; set; }
//}

//public class Position
//{
//    public long Id { get; set; }
//    public string FullName { get; set; }
//}

//public interface IConnector : IDisposable
//{
//    IEnumerable<Employee> GetEmployeesByUnit(long unitId);
//    IEnumerable<Position> GetPositions();
//    IEnumerable<Unit> GetUnitsByParentId(long parentId);
//}

//// Требуется реализовать интерфейс,
//// получить данные из сервиса (baseUrl + /units, /positions, /employees),
//// преобразовать их в нужный формат,
//// валидировать
//public class Connector : IConnector
//{
//    public Connector(ILogger logger, string baseUrl, string login, string password, string archivePath)
//    {
//    }
//}
