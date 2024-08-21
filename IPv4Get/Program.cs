using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using IPv4Get.DTOs;
using IPv4Get.TypeOfMethod;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Manipulador de IPs");
        Console.WriteLine("Digite 1 para pegar salvar o csv em fisicamente! \n\nDigite 2 para pegar o csv e validar em memoria!");

        var input = Console.ReadLine();

        switch (input.Trim())
        {
            case "1":
                Console.WriteLine("valido");
                await PhysicalStorage.PhysicalStorageInit();
                break;
            case "2":
                Console.WriteLine("valido");
                await MemoryStorage.MemoryStorageInit();
                break;
            default:
                Console.WriteLine("Invalido");
                break;
        }

    }
}
