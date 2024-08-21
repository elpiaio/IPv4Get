using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPv4Get.DTOs
{
    public struct IpRange
    {
        [Index(0)]
        public uint IpFrom { get; set; }

        [Index(1)]
        public uint IpTo { get; set; }

        [Index(2)]
        public string CountryCode { get; set; }

        [Index(3)]
        public string CountryName { get; set; }

        [Index(4)]
        public string RegionName { get; set; }

        [Index(5)]
        public string CityName { get; set; }

        //[Index(6)]
        //public double Latitude { get; set; }

        //[Index(7)]
        //public double Longitude { get; set; }

        //[Index(8)]
        //public string ZipCode { get; set; }

        //[Index(9)]
        //public string TimeZone { get; set; }
    }
}
