using ArsVisual.NotifyComponents.MsgBox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;

namespace ArsVisual.NetService
{
   public class FileLoader
    {
        private static readonly HttpClient _httpClient = new HttpClient();
     
        private const string ApiUrl = "https://digitalhive74.ru/api/files/get-by-id-api/";

        public static async Task<string> GetFileFromCloud(string FileId, string filePath)
        {
            try
            {
                string requestUrl = $"{ApiUrl}{FileId}";
                using HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Ошибка при загрузке файла: {response.StatusCode}");
                    return null;
                }

                string fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "downloaded_file";
                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

    
                filePath = Path.Combine(filePath, fileName);

                await File.WriteAllBytesAsync(filePath, fileBytes);

                App.NotifyUser($"{filePath}", "Проект выгружен", "folder.gif", 4000, PopupAnimation.Slide);
                return filePath;
            }
            catch (Exception ex)
            {
                NotifyBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
