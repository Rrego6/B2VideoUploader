using B2VideoUploader.Helper;
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
        private static readonly string bucket_id_key = "secrets:bucket_id";
        private static readonly string application_id_key = "secrets:application_id";
        private static readonly string application_key_key = "secrets:application_key";
        private static readonly string min_large_upload_size_key = "B2UploadConfiguration:min_large_upload_size";
        private static readonly string upload_part_size_key = "B2UploadConfiguration:upload_part_size";
        private static readonly string request_timeout_time_key = "B2UploadConfiguration:request_timeout_time";
        private static readonly string ffmpeg_binary_location_key = "B2UploadConfiguration:ffmpeg_binary_location";
        private static readonly string settings_ini_location = "settings.ini";


        public string BucketId;
        public string ApplicationId;
        public string ApplicationKey;
        public long MinLargeUploadSize;
        public long UploadPartSize;
        public int RequestTimeoutTime;
        public string FFMpegBinariesLocation;

        public Config(IConfiguration config)
        {
            BucketId = config.GetValue<string>(bucket_id_key);
            ApplicationId = config.GetValue<string>(application_id_key);
            ApplicationKey = config.GetValue<string>(application_key_key);
            MinLargeUploadSize = config.GetValue<long>(min_large_upload_size_key) * 1000 * 1000; //convert from MB to B
            UploadPartSize = config.GetValue<long>(upload_part_size_key) * 1000 * 1000; //convert from MB to B
            RequestTimeoutTime = config.GetValue<int>(request_timeout_time_key);
            FFMpegBinariesLocation = config.GetValue<string>(ffmpeg_binary_location_key);
        }

        public void OverWriteSecrets(string ApplicationId, string ApplicationKey, string BucketId)
        {
            this.ApplicationKey = ApplicationKey;
            this.ApplicationId = ApplicationId;
            this.BucketId = BucketId;

            var ini = new IniFile(settings_ini_location);
            ini.Write(bucket_id_key, BucketId);
            ini.Write(application_id_key, ApplicationId);
            ini.Write(application_key_key, ApplicationKey);
        }
    }
}
