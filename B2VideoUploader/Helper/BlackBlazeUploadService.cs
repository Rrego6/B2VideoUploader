using B2VideoUploader.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace B2VideoUploader.Helper
{
    public class BlackBlazeUploadService
    {

        private readonly BlackBlazeB2Api b2Api;
        private readonly ILogger logger;
        private readonly Config config;
        JObject authResponse;
         
        public BlackBlazeUploadService(BlackBlazeB2Api b2Api, CustomLogger logger, Config config)
        {
            this.b2Api = b2Api;
            this.logger = logger;
            this.config = config;
        }

        private async Task initService()
        {
            authResponse = await b2Api.blackBlazeLogin(config.ApplicationId, config.ApplicationKey);
        }

        public async Task<string> uploadFile(string filePath, string contentType, Action<string>? progressCallback = null)
        {
            if (authResponse == null)
            {
                await initService();
            }

            FileInfo fileInfo = new FileInfo(filePath);
            long localFileSize = fileInfo.Length;
            if (localFileSize < config.MinLargeUploadSize)
            {
                return await BlackBlazeUploadSmallFile(filePath, config.BucketId, contentType, progressCallback);
            } else
            {
                return await BlackBlazeUploadLargeFile(filePath, config.BucketId, contentType, progressCallback);
            }
        }

        public async Task<string> uploadVideo(string filePath, Action<string>? progressCallback = null)
        {
            if (authResponse == null)
            {
                await initService();
            }

            FileInfo fileInfo = new FileInfo(filePath);
            long localFileSize = fileInfo.Length;
            if (localFileSize < config.MinLargeUploadSize)
            {
                return await BlackBlazeUploadSmallVideo(filePath, config.BucketId, progressCallback);
            }
            else
            {
                return await BlackBlazeUploadLargeVideo(filePath, config.BucketId, progressCallback);
            }
        }

        private async Task<string> BlackBlazeUploadSmallVideo(string filePath, string bucketID, Action<string>? progressCallback = null)
        {

            return await BlackBlazeUploadSmallFile(filePath, bucketID, "video/mp4", progressCallback);
        }
        
        private async Task<string> BlackBlazeUploadSmallFile(string filePath, string bucketID, string contentType, Action<string>? progressCallback = null)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            string fileName = fileInfo.Name;
            long contentLength = fileInfo.Length;
            logger.LogInformation($"Begin Uploading of Small Video ({fileName})");

            string apiUrl = (string)authResponse["apiUrl"];
            string accountAuthorizationToken = (string)authResponse["authorizationToken"];
            string accountId = (string)authResponse["accountId"];


            var bucketInfoResponse = await b2Api.b2ListBuckets(apiUrl, accountAuthorizationToken, accountId, bucketID);

            var getUploadUrlResponse = await b2Api.b2GetUploadUrl(apiUrl, accountAuthorizationToken, bucketID);
            var uploadUrl = (string)getUploadUrlResponse["uploadUrl"];
            var uploadAuthorizationToken = (string)getUploadUrlResponse["authorizationToken"];

            byte[] dataChunk = Util.getDataChunk(filePath, 0, contentLength);
            string sha1CheckSum = Util.calcSha1(dataChunk, (int)contentLength);

            var getUploadFileResponse = await b2Api.b2UploadFile(uploadUrl, uploadAuthorizationToken, fileName, contentType, (int)contentLength, sha1CheckSum, dataChunk);
            logger.LogInformation($"Completed Uploading of Small Video ({fileName})");

            var downloadBaseUrl = authResponse["downloadUrl"];
            var bucketName = bucketInfoResponse["buckets"][0]["bucketName"];
            var encodedFileName = Util.urlEncode(fileName);
            var downloadUrl = $"{downloadBaseUrl}/file/{bucketName}/{encodedFileName}";
            return downloadUrl;
        }

        private async Task<string> BlackBlazeUploadLargeFile(string filePath, string bucketId, string contentType, Action<string>? progressCallback = null)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            string apiUrl = (string)authResponse["apiUrl"];
            string accountAuthorizationToken = (string)authResponse["authorizationToken"];
            string fileName = fileInfo.Name;
            logger.LogInformation($"Begin Uploading of Large Video ({fileName})");
            progressCallback($"Begin Uploading of Large Video ({fileName})");
            string accountId = (string)authResponse["accountId"];
            var bucketInfoResponse = await b2Api.b2ListBuckets(apiUrl, accountAuthorizationToken, accountId, bucketId);

            logger.LogInformation($"Calculating whole sha1 of file ({fileName})");
            progressCallback($"Calculating whole sha1 of file ({fileName})");

            //check if file already exists
            //if file exists, get sha1 of whole file, then check sha1
            var wholeFileSha1 = Util.calcSha1(filePath);

            var downloadBaseUrl = authResponse["downloadUrl"];
            var bucketName = bucketInfoResponse["buckets"][0]["bucketName"];
            var encodedFileName = Util.urlEncode(fileName);
            var downloadUrl = $"{downloadBaseUrl}/file/{bucketName}/{encodedFileName}";

            var isUploadedAlready = await checkExistingUploadedFile(fileInfo, apiUrl, accountAuthorizationToken, bucketId, wholeFileSha1, progressCallback);
            if (isUploadedAlready)
            {
                return downloadUrl;
            }

            long localFileSize = fileInfo.Length;
            long totalBytesSent = 0;

            long minimumPartSize = config.UploadPartSize;
            var numParts = localFileSize / config.UploadPartSize + 1;
            var sha1Array = await Util.ReadSha1File(filePath);
            int partNum = 1;

            var b2StartLargeFileResponse = await b2Api.b2StartLargeFile(apiUrl, accountAuthorizationToken, bucketId, fileName, contentType, wholeFileSha1);
            string fileId = (string)b2StartLargeFileResponse["fileId"];

            var b2GetUploadPartUrl = b2Api.b2GetUploadPartUrl(apiUrl, accountAuthorizationToken, fileId);

            while (totalBytesSent < localFileSize)
            {
                var bytesToSend = config.UploadPartSize;
                if ((localFileSize - totalBytesSent) < minimumPartSize)
                {
                    bytesToSend = (localFileSize - totalBytesSent);
                }

                byte[] dataChunk = Util.getDataChunk(filePath, totalBytesSent, bytesToSend);
                string sha1CheckSum = sha1Array[partNum - 1];

                var uploadUrlResponse = await b2GetUploadPartUrl;
                var uploadUrl = (string)uploadUrlResponse["uploadUrl"];
                var uploadAuthorizationToken = (string)uploadUrlResponse["authorizationToken"];

                var uploadPartResponse = await b2Api.b2UploadPart(uploadUrl, uploadAuthorizationToken, sha1CheckSum, dataChunk, partNum);
                logger.LogInformation($"Uploaded {partNum}/{numParts} for {fileName}");
                progressCallback($"Uploaded {partNum}/{numParts} for {fileName}");
                partNum++;
                totalBytesSent = totalBytesSent + bytesToSend;
            }
            await b2Api.b2FinishLargeFile(apiUrl, accountAuthorizationToken, fileId, sha1Array);
            logger.LogInformation($"Completed Uploading of Large Video ({fileName})");
            progressCallback($"Completed Uploading of Large Video ({fileName})");

            return downloadUrl;
        }

        private async Task<bool> checkExistingUploadedFile(FileInfo fileInfo, string apiUrl, string accountAuthorizationToken, string bucketId, string wholeFileSha1, Action<string>? progressCallback = null)
        {
            string fileName = fileInfo.Name;
            //check if file alredy exists
            logger.LogInformation($"Check if file already uploaded ({fileName})");
            if (progressCallback != null)
            {
                progressCallback($"Check if file already uploaded ({fileName})");
            }
            try
            {
                var existingFileResponse = await b2Api.b2ListFileVersions(apiUrl, accountAuthorizationToken, bucketId, fileName);
                var largeFileObject = existingFileResponse["files"][0];
                var largeFileSha1 = (string)largeFileObject["fileInfo"]["large_file_sha1"];
                var action = (string)largeFileObject["action"];
                var contentLength = (long)largeFileObject["contentLength"];

                if (wholeFileSha1.Equals(largeFileSha1) && "upload".Equals(action) && contentLength == fileInfo.Length)
                {
                    return true;

                }
            }
            catch (Exception e) { }
            return false;
        }

        /**
         */

        private async Task<string?> handlePartiallyCompleteUpload(string apiUrl, string accountAuthorizationToken, string bucketId, FileInfo fileInfo, string contentType, string wholeFileSha1)
        {

            string filePath = fileInfo.FullName;
            string fileName = fileInfo.Name;

            long localFileSize = fileInfo.Length;
            long totalBytesSent = 0;

            long minimumPartSize = config.UploadPartSize;
            var numParts = localFileSize / config.UploadPartSize + 1;
            var sha1Array = await Util.ReadSha1File(filePath);
            int partNum = 1;

            // get file id of large file
            // b2_list_parts
            // check if parts are the same amount
            // check each part sha and skip

            // get existing unfinished file
            // listUnfinishedLargeFiles
            try
            {
                var unfinishedFileResponse = await b2Api.b2ListUnFinishedLargeFiles(apiUrl, accountAuthorizationToken, bucketId, fileName);
                var unfinishedLargeFileId = (string)unfinishedFileResponse["files"][0]["fileId"]; //grab the first file
                if (String.IsNullOrEmpty(unfinishedLargeFileId))
                {
                    return null;
                }

                var listUnfinishedParts = await b2Api.b2ListParts(apiUrl, accountAuthorizationToken, bucketId, unfinishedLargeFileId, numParts);
                var parts = listUnfinishedParts["parts"].ToArray();
                foreach (var part in parts)
                {
                    var storedsha1 = part["contentSha1"];

                }
                return null;
                /*
                var b2StartLargeFileResponse = await b2Api.b2StartLargeFile(apiUrl, accountAuthorizationToken, bucketId, fileName, contentType, wholeFileSha1);
                string fileId = (string)b2StartLargeFileResponse["fileId"];

                var b2GetUploadPartUrl = b2Api.b2GetUploadPartUrl(apiUrl, accountAuthorizationToken, fileId);

                while (totalBytesSent < localFileSize)
                {
                    var bytesToSend = config.UploadPartSize;
                    if ((localFileSize - totalBytesSent) < minimumPartSize)
                    {
                        bytesToSend = (localFileSize - totalBytesSent);
                    }

                    byte[] dataChunk = Util.getDataChunk(filePath, totalBytesSent, bytesToSend);
                    string sha1CheckSum = sha1Array[partNum - 1];

                    var uploadUrlResponse = await b2GetUploadPartUrl;
                    var uploadUrl = (string)uploadUrlResponse["uploadUrl"];
                    var uploadAuthorizationToken = (string)uploadUrlResponse["authorizationToken"];

                    var uploadPartResponse = await b2Api.b2UploadPart(uploadUrl, uploadAuthorizationToken, sha1CheckSum, dataChunk, partNum);
                    logger.LogInformation($"Completed {partNum}/{numParts} for {fileName}");
                    progressCallback($"Completed {partNum}/{numParts} for {fileName}");
                    partNum++;
                    totalBytesSent = totalBytesSent + bytesToSend;
                }
                await b2Api.b2FinishLargeFile(apiUrl, accountAuthorizationToken, fileId, sha1Array);
                logger.LogInformation($"Completed Uploading of Large Video ({fileName})");
                progressCallback($"Completed Uploading of Large Video ({fileName})");

                return downloadUrl;
                */

            }
            catch (Exception e) { return null; }






        }

        private async Task<string> BlackBlazeUploadLargeVideo(string filePath, string bucketId, Action<string>? progressCallback = null)
        {
            return await BlackBlazeUploadLargeFile(filePath, bucketId, "video/mp4", progressCallback);
        }

        private IEnumerable<Task<JObject>> b2GetUploadPartUrlList(string apiUrl, string authorizationToken, string fileId, int numParts)
        {
            return Enumerable.Range(0, numParts).Select(_ => b2Api.b2GetUploadPartUrl(apiUrl, authorizationToken, fileId));
        }
    }
}
