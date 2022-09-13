using B2VideoUploader.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace B2VideoUploader.Helper
{
    public class BlackBlazeB2Api
    {
        private readonly HttpClient httpClient;
        private readonly CustomLogger logger;

        public static readonly string apiUrl_prop = "apiUrl";
        public static readonly string authorizationToken_prop = "apiUrl";

        HttpStatusCode[] httpStatusCodesWorthRetrying = {
           HttpStatusCode.RequestTimeout, // 408
           HttpStatusCode.InternalServerError, // 500
           HttpStatusCode.BadGateway, // 502
           HttpStatusCode.ServiceUnavailable, // 503
           HttpStatusCode.GatewayTimeout // 504
        };

        private static readonly string LoginApiUrl = "https://api.backblazeb2.com/b2api/v2/b2_authorize_account";

        public BlackBlazeB2Api(HttpClient httpClient, CustomLogger logger, Config config)
        {
            this.httpClient = httpClient;

            httpClient.Timeout = TimeSpan.FromSeconds(config.RequestTimeoutTime);
            this.logger = logger;
        }

        public async Task<JObject> blackBlazeLogin(string applicationKeyId, string applicationKey)
        {
            String credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(applicationKeyId + ":" + applicationKey));

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, LoginApiUrl);
            requestMessage.Headers.TryAddWithoutValidation("Authorization", $"Basic {credentials}");
            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);
            var responseString = await responseMessage.Content.ReadAsStringAsync();
            logger.LogTrace(responseString);

            return JObject.Parse(responseString);
        }

        public async Task<JObject> b2StartLargeFile(string apiUrl, string authorizationToken, string bucketId, string fileName, string contentType)
        {
            var endpoint = apiUrl + "/b2api/v2/b2_start_large_file";

            var data =
            new
            {
                fileName = fileName,
                bucketId = bucketId,
                contentType = contentType
            };

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
            requestMessage.Headers.TryAddWithoutValidation("Authorization", authorizationToken);
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(data, Formatting.None));
            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);
            var responseString = await responseMessage.Content.ReadAsStringAsync();
            logger.LogTrace(responseString);

            return JObject.Parse(responseString);
        }

        public async Task<JObject> b2GetUploadPartUrl(string apiUrl, string authorizationToken, string fileId)
        {
            var endpoint = apiUrl + "/b2api/v2/b2_get_upload_part_url";

            var data = new
            {
                fileId = fileId

            };

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
            requestMessage.Headers.TryAddWithoutValidation("Authorization", authorizationToken);
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(data, Formatting.None));
            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);
            var responseString = await responseMessage.Content.ReadAsStringAsync();

            logger.LogTrace(responseString);

            return JObject.Parse(responseString);
        }


        public async Task<JObject> b2FinishLargeFile(string apiUrl, string authorizationToken, string fileId, IEnumerable<string> partSha1Array)
        {
            var endpoint = apiUrl + "/b2api/v2/b2_finish_large_file";
            var data = new
            {
                fileId = fileId,
                partSha1Array = partSha1Array
            };

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
            requestMessage.Headers.TryAddWithoutValidation("Authorization", authorizationToken);
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(data, Formatting.None));
            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);
            var responseString = await responseMessage.Content.ReadAsStringAsync();

            logger.LogTrace(responseString);

            return JObject.Parse(responseString);
        }

        public async Task<JObject> b2ListUnfinishedLargeFiles(string apiUrl, string authorizationToken, string bucketId)
        {
            var endpoint = apiUrl + "b2api/v2/b2_list_unfinished_large_files";
            var data = new
            {
                buckedId = bucketId
            };

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
            requestMessage.Headers.TryAddWithoutValidation("Authorization", authorizationToken);
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(data, Formatting.None));
            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);
            var responseString = await responseMessage.Content.ReadAsStringAsync();

            logger.LogTrace(responseString);

            return JObject.Parse(responseString);
        }

        public async Task<JObject> b2UploadPart(string uploadUrl, string uploadAuthorizationToken, string sha1CheckSum, byte[] data, int partNumber)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, uploadUrl);
            requestMessage.Headers.TryAddWithoutValidation("Authorization", uploadAuthorizationToken);
            requestMessage.Headers.TryAddWithoutValidation("X-Bz-Part-Number", partNumber.ToString());
            requestMessage.Headers.TryAddWithoutValidation("X-Bz-Content-Sha1", sha1CheckSum);
            requestMessage.Content = new StreamContent(new MemoryStream(data));
            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);
            var responseString = await responseMessage.Content.ReadAsStringAsync();
            return JObject.Parse(responseString);
        }

        public async Task<JObject> b2GetUploadUrl(string apiUrl, string accountAuthorizationToken, string bucketId)
        {
            var endpoint = apiUrl + "/b2api/v2/b2_get_upload_url";

            var data = new
            {
                bucketId = bucketId,
            };

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
            requestMessage.Headers.TryAddWithoutValidation("Authorization", accountAuthorizationToken);
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(data, Formatting.None));
            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);
            var responseString = await responseMessage.Content.ReadAsStringAsync();
            logger.LogTrace(responseString);

            return JObject.Parse(responseString);
        }

        public async Task<JObject> b2UploadFile(string uploadUrl, string uploadAuthorizationToken, string fileName, string contentType, int contentLength, string sha1CheckSum, byte[] data)
        {
            var encdodedFileName = Util.urlEncode(fileName);

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, uploadUrl);
            requestMessage.Headers.TryAddWithoutValidation("Authorization", uploadAuthorizationToken);
            requestMessage.Headers.TryAddWithoutValidation("X-Bz-File-Name", encdodedFileName);
            requestMessage.Headers.TryAddWithoutValidation("X-Bz-Content-Sha1", sha1CheckSum);
            requestMessage.Content = new StreamContent(new MemoryStream(data));
            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(contentType) ? "b2/x-auto" : contentType);
            requestMessage.Content.Headers.ContentLength = contentLength;

            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);
            var responseString = await responseMessage.Content.ReadAsStringAsync();

            logger.LogTrace(responseString);
            
            return JObject.Parse(responseString);
        }

        public async Task<JObject> b2ListBuckets(string apiUrl, string authorizationToken, string accountId, string? bucketId = null, string? bucketName = null)
        {
            var endpoint = apiUrl + "/b2api/v2/b2_list_buckets";

            var data = new Dictionary<string, string>()
            {
                { "accountId", accountId }
            };

            if (bucketId != null)
            {
                data.Add("bucketId", bucketId);
            }

            if(bucketName != null)
            {
                data.Add("bucketName", bucketName);
            }

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
            requestMessage.Headers.TryAddWithoutValidation("Authorization", authorizationToken);
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(data, Formatting.None));
            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);
            var responseString = await responseMessage.Content.ReadAsStringAsync();

            logger.LogTrace(responseString);

            return JObject.Parse(responseString);
        }


    }
}
