using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2VideoUploader.Helper
{
    class VideoProcessorService
    {
        private readonly BlackBlazeUploadService b2UploadService;
        private readonly FfmpegVideoConversionService ffmpegVideoConversionService;
        private readonly CustomLogger logger;

        Queue<object> videoConversionJobQueue;
        Queue<object> videoUploadJobQueue;

        public interface IOnProgressUpload
        {
            event EventHandler OnProgressUpload;
        }

        public interface IOnProgressConversion
        {
            event EventHandler OnProgressConversion;
        }

        public VideoProcessorService(BlackBlazeUploadService b2UploadService, FfmpegVideoConversionService ffmpegVideoConversionService, CustomLogger logger)
        {
            this.b2UploadService = b2UploadService;
            this.ffmpegVideoConversionService = ffmpegVideoConversionService;
            this.logger = logger;
            videoConversionJobQueue = new Queue<object>();
        }

        public void ProcessVideo(string filePath)
        {

        }


    }
}
