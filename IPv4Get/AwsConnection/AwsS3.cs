using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using CsvHelper.Configuration;
using CsvHelper;
using IPv4Get.DTOs;
using System;
using System.Globalization;
using System.IO;

namespace IPv4Get.AwsConnection
{
    internal class AwsS3
    {
        private static string AwsAccesKeyUserId = Environment.GetEnvironmentVariable("AWS-S3-USER-ID", EnvironmentVariableTarget.Machine);
        private static string AwsAccesKey = Environment.GetEnvironmentVariable("AWS-S3-ACCESKEY", EnvironmentVariableTarget.Machine);
        private static string BucketName = Environment.GetEnvironmentVariable("AWS-S3-BUCKETNAME", EnvironmentVariableTarget.Machine);

        private static AmazonS3Client _s3Client = new AmazonS3Client(AwsAccesKeyUserId, AwsAccesKey, Amazon.RegionEndpoint.USEast2);

        public static void UploadFile(MemoryStream fileStream)
        {
            try
            {
                if (fileStream == null || fileStream.Length == 0)
                {
                    throw new ArgumentException("The file stream is null or empty.");
                }

                fileStream.Position = 0;

                var transferUtility = new TransferUtility(_s3Client);
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd-HH:mm");
                string fileKey = $"ipsFolder/IP2LOCATION-LITE-{currentDate}.csv";

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = BucketName,
                    Key = fileKey,
                    InputStream = fileStream,
                    CannedACL = S3CannedACL.Private
                };

                transferUtility.UploadAsync(uploadRequest).Wait();
                Console.WriteLine("sucess");
            }
            catch
            {
                throw new Exception();
            }
        }

        public static async Task<IEnumerable<IpRange>> GetLastFile()
        {
            try
            {
                var listObjectsRequest = new ListObjectsV2Request
                {
                    BucketName = BucketName,
                    Prefix = "ipsFolder/",
                };

                var listObjectsResponse = await _s3Client.ListObjectsV2Async(listObjectsRequest);

                var recentCsvFile = listObjectsResponse.S3Objects
                    .Where(o => o.Key.EndsWith(".csv"))
                    .OrderByDescending(o => o.LastModified)
                    .FirstOrDefault();

                if (recentCsvFile == null)
                {
                    throw new Exception("No CSV files found in the specified bucket and folder.");
                }

                string mostRecentFileKey = recentCsvFile.Key;
                Console.WriteLine($"Most recent CSV file: {mostRecentFileKey}");

                // Get the file content from S3
                var fileRequest = new GetObjectRequest
                {
                    BucketName = BucketName,
                    Key = mostRecentFileKey
                };

                using var fileResponse = await _s3Client.GetObjectAsync(fileRequest);
                using var fileStream = fileResponse.ResponseStream;
                using var reader = new StreamReader(fileStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                // Map CSV data to IpRange objects
                var ipRanges = csv.GetRecords<IpRange>().ToList();
                return ipRanges;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving and processing the most recent CSV file.", ex);
            }
        }
    }
}