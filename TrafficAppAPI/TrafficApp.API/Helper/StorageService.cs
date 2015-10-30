using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace TrafficApp.API.Helper
{
    public class StorageService
    {
        private IAmazonS3 client = null;

        public StorageService()
        {
            string accessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
            string secretKey = ConfigurationManager.AppSettings["AWSSecretKey"];
            if (this.client == null)
            {
                this.client = Amazon.AWSClientFactory.CreateAmazonS3Client(accessKey, secretKey, RegionEndpoint.USWest2);
            }
        }

        public bool UploadFile(string awsBucketName, string key, Stream stream)
        {
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                BucketName = awsBucketName,
                CannedACL = S3CannedACL.PublicRead,
                Key = key
            };

            TransferUtility fileTransferUtility = new TransferUtility(this.client);
            fileTransferUtility.Upload(uploadRequest);
            return true;
        }
    }
}