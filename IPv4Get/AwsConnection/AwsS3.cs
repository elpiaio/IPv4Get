using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPv4Get.AwsConnection
{

    internal class AwsS3
    {
        private static string AwsAccesKeyId = Environment.GetEnvironmentVariable("AWS-S3-ID", EnvironmentVariableTarget.Machine);
        private static string AwsAccesKey = Environment.GetEnvironmentVariable("AWS-S3-ACCESKEY", EnvironmentVariableTarget.Machine);
        private static string BucketName = Environment.GetEnvironmentVariable("AWS-S3-BUCKETNAME", EnvironmentVariableTarget.Machine);

        private static AmazonS3Client _s3Client = new AmazonS3Client(AwsAccesKeyId, AwsAccesKey, Amazon.RegionEndpoint.USEast1);


        public static void UploadFile(MemoryStream fileStream)
        {
            var transferUtility = new TransferUtility(_s3Client);
            string fileKey = $"MeuAquivo.cvs";

            var uploadRequest = new TransferUtilityUploadRequest
            {
                BucketName = Environment.GetEnvironmentVariable("AWS-Bucket"),
                Key = fileKey,
                InputStream = fileStream,
                CannedACL = S3CannedACL.Private // Defina as permissões para o arquivo (público)
            };

            // Faz o upload do arquivo
            transferUtility.UploadAsync(uploadRequest).Wait();
        }
    }
}
