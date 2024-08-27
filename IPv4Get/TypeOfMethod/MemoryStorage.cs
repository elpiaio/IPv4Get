using CsvHelper.Configuration;
using CsvHelper;
using IPv4Get.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPv4Get.Validations;
using IPv4Get.AwsConnection;
using System.IO.Compression;

namespace IPv4Get.TypeOfMethod
{
    public class MemoryStorage
    {
        public static async Task MemoryStorageInit()
        {
            var oldIpRanges = await AwsS3.GetLastFile();
            if (oldIpRanges.Count() < 1) throw new Exception("OLD IPS IS NUll");
            
            string token = "";
            string databaseCode = "DB3LITECSV";
            string downloadUrl = $"https://www.ip2location.com/download/?token={token}&file={databaseCode}";

            string zipFilePath = await DownloadFileAsync(downloadUrl, "ips.zip"); //revisar aqui
            var extractedCsvPath = ExtractCsvFromZip(zipFilePath);
            
            if (File.Exists(extractedCsvPath))
            {
                var ipRanges = ReadIpRanges(extractedCsvPath);
                WriteIpRangesToCsv(extractedCsvPath, ipRanges);
                Validator.sizeValidator(ipRanges, oldIpRanges);
                Validator.IpRangeAndDataValidator(ipRanges, oldIpRanges);
            }

            //TransformarArquivo em memorystream e fazer upload pra aws
            var file = File.ReadAllBytes(extractedCsvPath);
            AwsS3.UploadFile(new MemoryStream(file));
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
    }
}