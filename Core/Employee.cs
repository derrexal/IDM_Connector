namespace IDM_Connector.Core;

public class Employee
{
    public long Id { get; set; }

    // Табельный номер
    public string PersonnelNumber { get; set; }
    public string FullName { get; set; }
    public WorkStatus Status { get; set; }

    // Для EFCore ещё делается свойство Position типа Position, которое сразу не тянется, а только после Include.
    // Должность
    public long PositionId { get; set; }

    // Здесь так же, как с PositionId
    // Подразделение
    public long UnitId { get; set; }

    // Основная или по совместительству
    public bool? IsMainJob { get; set; }

    // Может отсутствовать?
    // Дата приема на работу
    public DateTime? StartDate { get; set; }
}

public enum WorkStatus
{
    Work = 0,
    Dismissed = 1
}