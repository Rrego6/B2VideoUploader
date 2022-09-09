using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2VideoUploader.Model
{
    public class Config
    {
        private static readonly string bucket_id_key = "secret:bucket_id";
        private static readonly string application_id_key = "secret:application_id";
        private static readonly string application_key_key = "secret:application_key";
        private static readonly string min_large_upload_size_key = "B2UploadConfiguration:min_large_upload_size";
        private static readonly string upload_part_size = "B2UploadConfiguration:upload_part_size";
        private static readonly string request_timeout_time = "B2UploadConfiguration:request_timeout_time";

        public readonly string BucketId;
        public readonly string ApplicationId;
        public readonly string ApplicationKey;
        public readonly long MinLargeUploadSize;
        public readonly long UploadPartSize;
        public readonly int RequestTimeoutTime;
        public readonly string FFMpegBinariesLocation;

        public Config(IConfiguration config)
        {
            BucketId = config.GetValue<string>(bucket_id_key);
            ApplicationId = config.GetValue<string>(application_id_key);
            ApplicationKey = config.GetValue<string>(application_key_key);
            MinLargeUploadSize = config.GetValue<long>(min_large_upload_size_key) * 1000 * 1000; //convert from MB to B
            UploadPartSize = config.GetValue<long>(upload_part_size) * 1000 * 1000; //convert from MB to B
            RequestTimeoutTime = config.GetValue<int>(request_timeout_time);
            FFMpegBinariesLocation = config.GetValue<string>(FFMpegBinariesLocation);
        }
    }
}
