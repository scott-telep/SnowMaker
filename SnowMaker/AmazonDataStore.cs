using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace SnowMaker
{
    public class AmazonDataStore : IOptimisticDataStore
    {
        const string SeedValue = "1";

        readonly string bucket;

        public AmazonDataStore(string bucket)
        {
            this.bucket = bucket;
        }

        public string GetData(string blockName)
        {
            try
            {
                return ReadObjectData(bucket, blockName);
            }
            catch (Exception e)
            {
                WriteObjectData(bucket, blockName, SeedValue);
                return SeedValue;
            }
        }

        public bool TryOptimisticWrite(string blockName, string data)
        {
            WriteObjectData(bucket, blockName, data);
            return true;
        }

        static string ReadObjectData(string bucketName, string keyName)
        {
            string responseBody = "";

            using (var client = new AmazonS3Client(Amazon.RegionEndpoint.USWest2))
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                using (GetObjectResponse response = client.GetObject(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string title = response.Metadata["x-amz-meta-title"];
                    Console.WriteLine("The object's title is {0}", title);

                    responseBody = reader.ReadToEnd();
                }
            }
            return responseBody;
        }

        static void WriteObjectData(string bucketName, string keyName, string data)
        {
           
            // Create the data to write to the stream.
            UTF8Encoding uniEncoding = new UTF8Encoding();
            byte[] firstString = uniEncoding.GetBytes(SeedValue);
            // 3. Upload data from a type of System.IO.Stream.
            TransferUtility fileTransferUtility = new
                    TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USEast1));
            using (MemoryStream memStream = new MemoryStream(firstString))
            {
                fileTransferUtility.Upload(memStream, bucketName, keyName);
            }
            Console.WriteLine("Upload 3 completed");
                
        }
    }
}
