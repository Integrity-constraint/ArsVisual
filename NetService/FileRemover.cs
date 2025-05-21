using ArsVisual.NotifyComponents.MsgBox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ArsVisual.NetService
{
   public class FileRemover
    {
       // private static readonly HttpClient _httpClient = new HttpClient
       // {
       //     BaseAddress = new Uri("https://digitalhive74.ru/")
       // };

        public static async Task<bool> DeleteFileAsync(string fileId, string email, string password)
        {
            try
            {
              
                if (string.IsNullOrEmpty(fileId))
                {
                    throw new ArgumentException("Идентификатор файла не может быть пустым", nameof(fileId));
                }

                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException("Email не может быть пустым", nameof(email));
                }

                if (string.IsNullOrEmpty(password))
                {
                    throw new ArgumentException("Пароль не может быть пустым", nameof(password));
                }

             
                string apiUrl = $"api/files/delete-by-api?fileId={fileId}";
                var request = new HttpRequestMessage(HttpMethod.Delete, apiUrl);

              
                request.Headers.Add("email", email);
                request.Headers.Add("password", password);

             
                var response = await FileInfo._httpClient.SendAsync(request);

              
                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new UnauthorizedAccessException("Неверные учетные данные (email или пароль)");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        throw new FileNotFoundException("Файл не найден", fileId);
                    }

                    throw new HttpRequestException($"Ошибка при удалении файла: {response.StatusCode} - {errorMessage}");
                }

            
                string responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<DeleteResponse>(
                    responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

             
                NotifyBox.Show($"Файл {result?.FileName} удалён","Уведомление", MessageBoxButton.OK);
                return true;
            }
            catch (Exception ex)
            {
                NotifyBox.Show($"Произошла ошибка при удалении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK);
                
                return false;
            }
        }

     
        private class DeleteResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string FileId { get; set; }
            public string FileName { get; set; }
        }

       
      
    }
}
