using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Threading.Tasks;
using ArsVisual.NotifyComponents.MsgBox;
using System.Windows;

namespace ArsVisual.NetService
{
    public class FileInfo
    {
        public string fileName { get; set; }
        public string idApi { get; set; }
        public long fileSize { get; set; }
        public string fileType { get; set; }
        public DateTime lastModified { get; set; }
      
        public static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://digitalhive74.ru/")
        };

        public static async Task<List<FileInfo>> GetFilesByFormatAsync(string email, string password, string format)
        {
            try
            {
              
                if (string.IsNullOrEmpty(email))
                    throw new ArgumentException("Email не может быть пустым", nameof(email));
                if (string.IsNullOrEmpty(password))
                    throw new ArgumentException("Пароль не может быть пустым", nameof(password));

                string apiUrl = "api/files/get-files-by-format";
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

                request.Headers.Add("email", email);
                request.Headers.Add("password", password);

              
                if (!string.IsNullOrEmpty(format))
                {
                    var queryParams = System.Web.HttpUtility.ParseQueryString(string.Empty);
                    queryParams["format"] = format;
                    apiUrl += "?" + queryParams.ToString();
                    request.RequestUri = new Uri(_httpClient.BaseAddress + apiUrl);
                }

                var response = await _httpClient.SendAsync(request);

         
                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        throw new UnauthorizedAccessException("Неверные учетные данные (email или пароль)");
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        throw new Exception("Пользователь не найден");

                    throw new HttpRequestException($"Ошибка при получении списка файлов: {response.StatusCode} - {errorMessage}");
                }

                // Десериализация ответа
                string responseBody = await response.Content.ReadAsStringAsync();
                var responseJson = JsonSerializer.Deserialize<Dictionary<string, List<FileInfo>>>(
                    responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                var files = responseJson?.GetValueOrDefault("files") ?? new List<FileInfo>();
               
                return files;
            }
            catch (Exception ex)
            {
                NotifyBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
