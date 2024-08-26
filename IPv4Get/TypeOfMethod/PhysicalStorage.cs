using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using IPv4Get.DTOs;

namespace IPv4Get.TypeOfMethod
{
    public class PhysicalStorage
    {
        public static async Task PhysicalStorageInit() 
        {
            string fileName = " IP2LOCATION-LITE-2024-08.csv";
            string token = "0DkhpZw80xHUSgy3avbA2CEg9DpcQFfOwXMuP5dT9EC979iXUY74JUpcMXlQjm6D";
            string databaseCode = "DB3LITECSV";

            string downloadUrl = $"https://www.ip2location.com/download/?token={token}&file={databaseCode}";

            string zipFilePath = await DownloadFileAsync(downloadUrl, "ips.zip");
            string extractedCsvPath = "";

            if (zipFilePath != "") extractedCsvPath = ExtractCsvFromZip(zipFilePath);

            if (File.Exists(extractedCsvPath))
            {
                var ipRanges = ReadIpRanges(extractedCsvPath);
                WriteIpRangesToCsv(extractedCsvPath, ipRanges);
                Console.WriteLine("Arquivo CSV atualizado com sucesso.");
                int index = 0;
                foreach (var ipRange in ipRanges)
                {
                    Console.WriteLine($"IP From: {ipRange.IpFrom}, IP To: {ipRange.IpTo},CountryCode: {ipRange.CountryCode}, Country: {ipRange.CountryName}, RegionName: {ipRange.RegionName}, City: {ipRange.CityName}");
                    if (index > 20) return;

                    index++;
                }
            }
        }

        static async Task<string> DownloadFileAsync(string url, string fileName)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
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
                catch (Exception ex) 
                {
                    Console.WriteLine(ex);
                    return "";
                }
            }
        }

        static string ExtractCsvFromZip(string zipFilePath)
        {
            try
            {
                string extractPath = Path.Combine(Directory.GetCurrentDirectory(), "extracted");
                Directory.CreateDirectory(extractPath);

                ZipFile.ExtractToDirectory(zipFilePath, extractPath, overwriteFiles: true);
                Console.WriteLine($"Arquivo extrai­do para: {extractPath}");

                string csvFilePath = Directory.GetFiles(extractPath, "*.csv")[0];
                return csvFilePath;
            }
            catch (InvalidDataException ex)
            {
                Console.WriteLine($"Erro ao extrair o arquivo ZIP: {ex.Message}");
                return "erro";
            }
        }
        public static IEnumerable<IpRange> ReadIpRanges(string csvFilePath)
        {
            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<IpRangeMap>();
                return csv.GetRecords<IpRange>().ToList();
            }
        }

        public sealed class IpRangeMap : ClassMap<IpRange>
        {
            public IpRangeMap()
            {
                Map(m => m.IpFrom).Index(0);
                Map(m => m.IpTo).Index(1);
                Map(m => m.CountryCode).Index(2);
                Map(m => m.CountryName).Index(3);
                Map(m => m.RegionName).Index(4);
                Map(m => m.CityName).Index(5);
            }
        }
        public static void WriteIpRangesToCsv(string csvFilePath, IEnumerable<IpRange> ipRanges)
        {
            using (var writer = new StreamWriter(csvFilePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<IpRangeMap>();
                csv.WriteHeader<IpRange>();
                csv.NextRecord();
                csv.WriteRecords(ipRanges);
            }
        }
    }
}
