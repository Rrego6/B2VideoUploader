using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;
using B2VideoUploader.Model;
using Microsoft.Extensions.Logging;
using static System.Windows.Forms.ListViewItem;

namespace B2VideoUploader.Services
{
    class VideoProcessorService
    {
        private readonly BlackBlazeUploadService b2UploadService;
        private readonly FfmpegVideoConversionService ffmpegVideoConversionService;
        private readonly CustomLogger logger;

        private SemaphoreSlim semaphoreEncoding;
        private SemaphoreSlim semaphoreUpload;

        public VideoProcessorService(BlackBlazeUploadService b2UploadService, FfmpegVideoConversionService ffmpegVideoConversionService, CustomLogger logger)
        {
            this.b2UploadService = b2UploadService;
            this.ffmpegVideoConversionService = ffmpegVideoConversionService;
            this.logger = logger;
            semaphoreEncoding = new SemaphoreSlim(1);
            semaphoreUpload = new SemaphoreSlim(1);
        }

        public Task<string> processVideoAsync(string filePath)
        {
            return Task.Run(async Task<string>? () =>
            {
                return await processVideoAysncHelper(filePath);
            });

        }

        private async Task<string> processVideoAysncHelper(string filePath)
        {
            semaphoreEncoding.Wait();
            string outputVideoFilePath;
            string subtitleFilePathTemp;
            try
            {
                (outputVideoFilePath, subtitleFilePathTemp) = await ffmpegVideoConversionService.convertVideoToWebFormat(filePath);
            }
            finally
            {
                semaphoreEncoding.Release();
            }
            var subtitleFilePath = await ffmpegVideoConversionService.extractSubtitles(filePath);

            semaphoreUpload.Wait();
            string videoUploadUrl;
            try
            {
                videoUploadUrl = await b2UploadService.uploadVideo(filePath);
            }
            finally
            {
                semaphoreUpload.Release();
            }
            string? subtitleUrl = null;
            if (subtitleFilePath != null)
            {
                subtitleUrl = await b2UploadService.uploadFile(subtitleFilePath, "text/vtt");
            }
            var jsonFilePath = await ffmpegVideoConversionService.createCytubeJson(outputVideoFilePath, videoUploadUrl, subtitleUrl);
            var jsonUrl = await b2UploadService.uploadFile(jsonFilePath, "application/json");
            return jsonUrl;
        }
    }
}
