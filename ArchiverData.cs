using IDM_Connector.Core;
using System.Globalization;
using System.IO.Compression;
using System.Text.Json;

namespace IDM_Connector;

public class ArchiverData: IArchiver
{
    /// <summary>
    /// Сохранение данных в архив. 
    /// Название архива в формате: текущая дата и время (ISO 8601) в часовом поясе Москвы (GMT+3). 
    /// </summary>
    /// <param name="path">Путь по которому будет сохранен архив</param>
    /// <returns></returns>
    public void SaveForArchive(string path, IEnumerable<Employee>? employees, IEnumerable<Position>? positions, IEnumerable<Unit>? units)
    {
        // Если получили пустые коллекции для сохранения в архив
        if (units is null)
            throw new ArgumentNullException($"В процессе сохранения данных в архив произошла ошибка. Необходимых данных для сохранения нет: {nameof(units)}");
        if (positions is null)
            throw new ArgumentNullException($"В процессе сохранения данных в архив произошла ошибка. Необходимых данных для сохранения нет: {nameof(positions)}");
        if (employees is null)
            throw new ArgumentNullException($"В процессе сохранения данных в архив произошла ошибка. Необходимых данных для сохранения нет: {nameof(employees)}");

        // Сериализуем данные для дальнейшего сохранения в файл
        var unitsJson = JsonSerializer.Serialize(units);
        var positionsJson = JsonSerializer.Serialize(positions);
        var employeesJson = JsonSerializer.Serialize(employees);

        using (var zipStream = new MemoryStream())
        {
            using (ZipArchive zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                AddEntry("employees.json", employeesJson, zip);
                AddEntry("positions.json", positionsJson, zip);
                AddEntry("units.json", unitsJson, zip);
            }

            // описываем формат времени который будет содержаться в названии архива
            var timeZoneMoscow = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            var todayDateTimeMoscow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneMoscow).ToString("o", CultureInfo.InvariantCulture);
            //TODO: сделать защиту от дурака: проверить существование архиваа и если он есть изменить название (Согласовать новое название)
            string pathToSaveArchive = $"{path}data_{todayDateTimeMoscow}.zip";
            File.WriteAllBytes(pathToSaveArchive, zipStream.ToArray());
        }
    }

    /// <summary>
    /// Вспомогательная задача для функции сохранения данных в архив.
    /// Создает файл с содержимимым в формате .json
    /// </summary>
    /// <param name="fileName">Имя файла формата .json</param> 
    /// <param name="fileContent">Содержимое файла</param>
    /// <param name="archive">Архив в который будут сохранены файлы</param>
    private void AddEntry(string fileName, string fileContent, ZipArchive archive)
    {
        var employeesEntry = archive.CreateEntry(fileName);
        using (StreamWriter sw = new StreamWriter(employeesEntry.Open()))
            sw.Write(fileContent);
    }
}