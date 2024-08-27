using Amazon.S3;
using Amazon.S3.Transfer;
using System;
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
            if (fileStream == null || fileStream.Length == 0)
            {
                throw new ArgumentException("The file stream is null or empty.");
            }

            fileStream.Position = 0;

            var transferUtility = new TransferUtility(_s3Client);
            string fileKey = "MeuArquivo.csv";

            var uploadRequest = new TransferUtilityUploadRequest
            {
                BucketName = BucketName,
                Key = fileKey,
                InputStream = fileStream,
                CannedACL = S3CannedACL.Private
            };

            transferUtility.UploadAsync(uploadRequest).Wait();
        }
    }
}