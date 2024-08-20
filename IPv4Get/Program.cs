using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using System.Globalization;

class Program
{
    static async Task Main(string[] args)
    {
        string token = "CqrjUtOHTcE5tug9ZxmOWteYZF3dgxI6AcKPhUQrhsfm4zM9Xfu9R1xrIv7iJXLe";
        string databaseCode = "DB3LITECSV";

        string downloadUrl = $"https://www.ip2location.com/download/?token={token}&file={databaseCode}";

        string filePath = await DownloadFileAsync(downloadUrl, "ips.csv");

        if (File.Exists(filePath))
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<dynamic>(); 
                foreach (var record in records)
                {
                    Console.WriteLine(record); 
                }
            }
        }
    }

    static async Task<string> DownloadFileAsync(string url, string fileName)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var filePath = Path.Combine("../../../", fileName);
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await response.Content.CopyToAsync(fs);
                    Console.WriteLine($"Arquivo baixado com sucesso: {filePath}");
                    return filePath;
                }
            }
            else
            {
                Console.WriteLine($"Erro ao baixar o arquivo: {response.StatusCode}");
                return null;
            }
        }
    }
}
