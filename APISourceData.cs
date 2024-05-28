using IDM_Connector.Core;
using IDM_Connector.Shared;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace IDM_Connector;

/// <summary>
/// Класс описывающий работу с удаленным источником кадровых данных через REST API
/// </summary>
public class APISourceData: ISourceData, IDisposable
{
    private readonly HttpClient _httpClient;
    
    public APISourceData(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    /// <summary>
    /// Получение данных обо всех сущностях(Employee, Position, Unit) из источника
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <param name="login"></param>
    /// <param name="password"></param>
    /// <param name="archivePath"></param>
    /// <returns></returns>
    public IEnumerable<T>? GetData<T>(Settings settings, string endUrl) where T : Entity
    {
        //Проверяем, можем ли работать с этим URL (на случай добавления новой функциональности)
        CheckEndUrl(endUrl);
        
        //Формируем запрос
        var url = settings.BaseUrl + endUrl;
        var authenticationHeaderValue = new AuthenticationHeaderValue("basic", GetAuthenticationString(settings.Login, settings.Password));
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = authenticationHeaderValue;

        using var response = _httpClient.Send(request);
        response.EnsureSuccessStatusCode();
        if(string.IsNullOrEmpty(response.Content.ToString()))
            throw new Exception($"Не были получены данные по адресу:{url}");

        return response.Content.ReadFromJsonAsync<IEnumerable<T>>().Result;
    }

    /// <summary>
    /// Проверка на возможность работы с передаваемым URL
    /// </summary>
    /// <param name="endUrl"></param>
    /// <exception cref="ArgumentException"></exception>
    private void CheckEndUrl(string endUrl)
    {
        switch (endUrl)
        {
            case Options.UNIT_END_URL:
                break;
            case Options.POSITION_END_URL:
                break;
            case Options.EMPLOYEE_END_URL:
                break;
            default:
                throw new ArgumentException($"Не допустимый формат данных. {nameof(endUrl)}={endUrl}");
        }
    }

    /// <summary>
    /// Возвращает строку авторизации для подключения по типу Basic
    /// </summary>
    /// <param name="login"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    private string GetAuthenticationString(string login, string password)
    {
        var authenticationString = $"{login}:{password}";
        var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));
        return base64EncodedAuthenticationString;
    }
}