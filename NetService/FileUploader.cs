using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ArsVisual.NotifyComponents.MsgBox;
using System.Windows;
using System.Text.Json;

namespace ArsVisual.NetService
{
    public class FileUploader
    {
       

        async public void UploadFileAsync(string filePath, string email, string password)
        {
            try
            {
               
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new ArgumentException("Путь к файлу не может быть пустым", nameof(filePath));
                }

                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException("Email не может быть пустым", nameof(email));
                }

                if (string.IsNullOrEmpty(password))
                {
                    throw new ArgumentException("Пароль не может быть пустым", nameof(password));
                }

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Указанный файл не найден", filePath);
                }

                using var formContent = new MultipartFormDataContent();
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                formContent.Add(fileContent, "file", Path.GetFileName(filePath));

             
                FileInfo._httpClient.DefaultRequestHeaders.Clear();
                FileInfo._httpClient.DefaultRequestHeaders.Add("email", email);
                FileInfo._httpClient.DefaultRequestHeaders.Add("password", password);

               
                var response = await FileInfo._httpClient.PostAsync("api/files/upload-by-api", formContent);

              
                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new UnauthorizedAccessException("Неверные учетные данные (email или пароль)");
                    }

                    throw new HttpRequestException($"Ошибка загрузки: {response.StatusCode} - {errorMessage}");
                }


                string responseBody = await response.Content.ReadAsStringAsync();
                var responseJson = JsonSerializer.Deserialize<UploadResponce>(
                    responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

            }
            catch (Exception ex)
            {
                NotifyBox.Show($"Произошла ошибка при загрузке файла: {ex.Message}","Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
               
            }
        }
    }
}
