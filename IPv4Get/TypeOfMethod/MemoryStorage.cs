using CsvHelper.Configuration;
using CsvHelper;
using IPv4Get.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPv4Get.TypeOfMethod
{
    public class MemoryStorage
    {
        public static async Task MemoryStorageInit()
        {
            /*
            string token = "2F8jiHl1R9L5aDrviytmCFGUFzicmsag6j7aMoGCpOyKPLoZENKMwM8GOp8637Xi";
            string databaseCode = "DB3LITECSV";

            string downloadUrl = $"https://www.ip2location.com/download/?token={token}&file={databaseCode}";

            string zipFilePath = await DownloadFileAsync(downloadUrl, "ips.zip");
            string extractedCsvPath = "";

            if (zipFilePath != "") extractedCsvPath = ExtractCsvFromZip(zipFilePath);
            */

            string extractedCsvPath = "C:\\Users\\thiago\\Documents\\trabalho\\IpGet\\IPv4Get\\bin\\Debug\\net8.0\\extracted\\IP2LOCATION-LITE-DB3.CSV";

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
