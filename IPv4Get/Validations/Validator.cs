using IPv4Get.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPv4Get.Validations
{
    internal class Validator
    {
        public static void sizeValidator(IEnumerable<IpRange> ipRanges, IEnumerable<IpRange> oldIpRanges)
        {
            int oldCount = oldIpRanges.Count();
            int newCount = ipRanges.Count();

            decimal difference = Math.Abs((decimal)(oldCount - newCount) / oldCount);

            if (difference > 0.20m)
            {
                throw new InvalidOperationException("The difference between the old and new IP ranges exceeds 20%.");
            }
            else
            {
                Console.WriteLine("Validation passed: The difference between the old and new IP ranges is within 20%.");
            }
        }

        public static void IpRangeAndDataValidator(IEnumerable<IpRange> ipRanges, IEnumerable<IpRange> oldIpRanges)
        {
            var ipsChecking = ipRanges.Where(ip => ip.IpFrom > ip.IpTo).ToList();
            if (ipsChecking.Count() > 0) throw new InvalidOperationException($"{ipsChecking.Count} ranges de ip vieram invalidos");
            Console.WriteLine("Ranges de ip validados");

            int count = 0;
            foreach (var ipRange in ipRanges)
            {
                var oldIpRange = oldIpRanges.Where(ip => ip.CountryCode == ipRange.CountryCode).FirstOrDefault();
                if(oldIpRange.CountryName != ipRange.CountryName) throw new InvalidOperationException("Os dados do pais estão vindo diferentes");
                count++;
            }
        }
    }
}
