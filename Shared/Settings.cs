namespace IDM_Connector.Shared
{
    /// <summary>
    /// Класс предпологает хранение настроек необходимых для использования библиотеки. 
    /// </summary>
    public class Settings
    {
        public string BaseUrl { get; set; }
        public string Login { get; set; } 
        public string Password { get; set; } 
        public string ArchivePath { get; set; }
    }
}
