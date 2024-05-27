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
        ((IDisposable)_httpClient).Dispose();
    }

    /// <summary>
    /// Получение данных обо всех сущностях(Employee, Position, Unit) из источника
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <param name="login"></param>
    /// <param name="password"></param>
    /// <param name="archivePath"></param>
    /// <returns></returns>
    public IEnumerable<object> GetData(string login, string password, string endUrl)
    {
        var authenticationHeaderValue = new AuthenticationHeaderValue("basic", GetAuthenticationString(login, password));
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _httpClient.BaseAddress!.ToString() + endUrl);
            request.Headers.Authorization = authenticationHeaderValue;

            using var response = _httpClient.Send(request);
            response.EnsureSuccessStatusCode();

            IEnumerable<object>? entities = null;
            if (endUrl == Settings.UNIT_END_URL)
                entities = response.Content.ReadFromJsonAsync<IEnumerable<Unit>>().Result;
            else if (endUrl == Settings.POSITION_END_URL)
                entities = response.Content.ReadFromJsonAsync<IEnumerable<Position>>().Result;
            else if (endUrl == Settings.EMPLOYEE_END_URL)
                entities = response.Content.ReadFromJsonAsync<IEnumerable<Employee>>().Result;

            if (entities is null)
                throw new Exception($"Не были получены данные по адресу:{_httpClient.BaseAddress!.ToString() + endUrl}");

            return entities;
        }
        catch { throw; }
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
