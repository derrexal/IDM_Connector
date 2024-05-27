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
    public Task SaveForArchive(string path, IEnumerable<Employee>? employees, IEnumerable<Position>? positions, IEnumerable<Unit>? units)
    {
        try
        {
            if (units is null || positions is null || employees is null)
                // А каких именно? почему бы не помочь с отладкой?
                throw new ArgumentNullException($"В процессе сохранения данных в архив произошла ошибка. Необходимых данных для сохранения нет");

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

                // А точно в любой ОС "TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time")" найдёт такую штуку? 
                // описываем формат времени который будет содержаться в названии архива
                var timeZoneMoscow = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
                var todayDateTimeMoscow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneMoscow).ToString("o", CultureInfo.InvariantCulture);
                
                // Проверка на существование такого файла?
                string pathToSaveArchive = $"{path}data_{todayDateTimeMoscow}.zip";
                // Почему не async?
                File.WriteAllBytes(pathToSaveArchive, zipStream.ToArray());

                return Task.CompletedTask;
            }
        }
        catch { throw; }

    }

    /// <summary>
    /// Вспомогательная задача для функции сохранения данных в архив.
    /// Создает файл с содержимимым в формате .json
    /// </summary>
    /// <param name="fileName">Имя файла формата .json</param> 
    /// <param name="fileContent">Содержимое файла</param>
    /// <param name="archive">Архив в который будут сохранены файлы</param>
    private Task AddEntry(string fileName, string fileContent, ZipArchive archive)
    {
        try
        {
            var employeesEntry = archive.CreateEntry(fileName);
            using (StreamWriter sw = new StreamWriter(employeesEntry.Open()))
                // Зачем-то это Task, но async-await не используем...
                sw.Write(fileContent);
            return Task.CompletedTask;
        }
        // Ну ты понял.
        catch { throw; }
    }
}