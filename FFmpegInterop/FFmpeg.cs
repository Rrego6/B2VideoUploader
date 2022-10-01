using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FFmpegInterop
{
    public class FFmpeg
    {
        private readonly string FFmpegDirectory;
        private readonly string FFmpegPath;
        private readonly string FFprobePath;
        protected FFmpeg(string FFmpegDirectory)
        {
            FFmpegDirectory = FFmpegDirectory;
        }

        public delegate void OnProgressHandler(string progress);

        public async Task callFFmpeg(string cmd, OnProgressHandler onProgressHandler)
        {
            using (Process ffMpegProcess = new Process())
            {
                ffMpegProcess.StartInfo.FileName = FFmpegPath;
                ffMpegProcess.StartInfo.Arguments = cmd;
            }
        }

        public async Task<JsonDocument> FFmpegAnalyse(string inputFilePath)
        {
            return null;
        } 


    }
}