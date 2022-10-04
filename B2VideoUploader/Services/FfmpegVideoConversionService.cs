using B2VideoUploader.Helper;
using B2VideoUploader.Model;
using FFMpegCore;
using FFMpegCore.Arguments;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CustomArgument = B2VideoUploader.scratch.CustomArgument;

namespace B2VideoUploader.Services
{
    public class FfmpegVideoConversionService : IOnProgressConversionEvents
    {
        private readonly CustomLogger customLogger;
        private readonly Config config;
        private static readonly Regex ProgressRegex = new Regex("time=(\\d\\d:\\d\\d:\\d\\d.\\d\\d?)", RegexOptions.Compiled);

        public event EventHandler<FileProgressEventArgs> OnConversionStart;
        public event EventHandler<FileProgressEventArgs> OnConversionProgress;
        public event EventHandler<FileProgressEventArgs> OnConversionComplete;
        public event EventHandler<FileProgressEventArgs> OnConversionError;

        public FfmpegVideoConversionService(CustomLogger customLogger, Config config)
        {
            this.customLogger = customLogger;
            this.config = config;
            GlobalFFOptions.Configure(new FFOptions { BinaryFolder = getFFMPegPath(), TemporaryFilesFolder = getTempVideoStoragePath() });
        }

        private static string getTempVideoStoragePath()
        {
            var tempDir = $"{Path.GetTempPath()}convertedvideos";
            var dirInfo = Directory.CreateDirectory(tempDir);
            return dirInfo.FullName;
        }

        private string? getFFMPegPath()
        {
            var pathIfFFMpegInPath = Util.GetFullPath("ffmpeg.exe");
            var fullPath = Path.GetFullPath(config.FFMpegBinariesLocation);
            return pathIfFFMpegInPath ?? fullPath ?? throw new FileNotFoundException($"ffmpeg executable not found at: (${config.FFMpegBinariesLocation})");
        }

        public async Task<string> createCytubeJson(string inputFilePath, string videoUrl, string? subtitleUrl)
        {
            var mediaAnalysis = await FFProbe.AnalyseAsync(inputFilePath);
            var seconds = mediaAnalysis.Duration.TotalSeconds;
            var fileName = new FileInfo(inputFilePath).Name;
            var quality = calcCorrespondingResolution(mediaAnalysis.VideoStreams.First().Height);

            var titleProp = new JProperty("title", fileName);
            var durationProp = new JProperty("duration", seconds);
            var sourcesProp = new JProperty("sources",
                    new JArray(
                        new JObject(
                            new JProperty("url", videoUrl),
                            new JProperty("contentType", "video/webm"),
                            new JProperty("quality", quality)
                            )
                        )
                    );
            var subtitleProp = !string.IsNullOrEmpty(subtitleUrl) ?
                new JProperty("textTracks",
                    new JArray(
                        new JObject(
                            new JProperty("url", subtitleUrl),
                            new JProperty("contentType", "text/vtt"),
                            new JProperty("name", "English Subtitles"),
                            new JProperty("default", true)
                            )
                        )
                    )
                : null;

            string jsonFilePath = $"{getTempVideoStoragePath()}/{fileName}.json";
            if (subtitleProp != null)
            {
                File.WriteAllText(jsonFilePath,
                    new JObject(titleProp, durationProp, sourcesProp, subtitleProp)
                    .ToString(Newtonsoft.Json.Formatting.Indented)
                    );
            }
            else
            {
                File.WriteAllText(jsonFilePath,
                    new JObject(titleProp, durationProp, sourcesProp)
                    .ToString(Newtonsoft.Json.Formatting.Indented)
                    );
            }
            customLogger.LogInformation($"Wrote json to ({jsonFilePath})");
            return jsonFilePath;
        }

        private int calcCorrespondingResolution(int height)
        {
            //use lower resolution in case heigh is slightly heigher ie (361 px instead of 360)
            int offsetFudge = 4;
            height = height - offsetFudge;
            return height <= 240 ? 240
                : height <= 360 ? 360
                : height <= 480 ? 480
                : height <= 540 ? 540
                : height <= 720 ? 720
                : height <= 1080 ? 1080
                : height <= 1440 ? 1440
                : 2160;
        }

        private async Task WriteSha1File(string videoOutputFilePath)
        {
            await Task.Run(async () =>
            {
                var infoFilePath = $"{Path.GetDirectoryName(videoOutputFilePath)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(videoOutputFilePath)}.sha1";
                var sha1Array = await Util.CreateSha1Array(videoOutputFilePath, config.UploadPartSize);
                await File.WriteAllLinesAsync(infoFilePath, sha1Array);
            });
        }

        private async Task<bool> CheckOutputVideoIntegrity(string outputFilePath)
        {
            var infoFilePath = $"{Path.GetDirectoryName(outputFilePath)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(outputFilePath)}.sha1";
            if (File.Exists(infoFilePath))
            {
                var sha1Array = await Util.CreateSha1Array(outputFilePath, config.UploadPartSize);
                var read = await File.ReadAllLinesAsync(infoFilePath);
                if (read.SequenceEqual(sha1Array))
                {
                    return true;
                }
                File.Delete(infoFilePath);
                return false;
            }
            return false;
        }


        /**
         * todo: have user select subtitle instead of guessing?
         */
        public async Task<VideoFileTransformContainer?> extractSubtitles(VideoFileTransformContainer video)
        {
            string inputFilePath = video.inputFilePath;
            customLogger.LogInformation("Begin extracting subtitles.");

            var fileName = Path.GetFileNameWithoutExtension(inputFilePath);
            var outputPath = $"{getTempVideoStoragePath()}/{fileName}.vtt";

            var userProvidedSubFileName = $"{Path.GetDirectoryName(inputFilePath)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(inputFilePath)}.srt";
            if (File.Exists(userProvidedSubFileName))
            {
                customLogger.LogInformation("Found user provided subtitles");
                await FFMpegArguments.FromFileInput(userProvidedSubFileName)
                    .OutputToFile(outputPath, true).ProcessAsynchronously();
                video.subtitlesFilePath = outputPath;
                return video;
            }

            if (File.Exists(outputPath))
            {
                video.subtitlesFilePath = outputPath;
                return video;
            }
            var mediaAnalysis = await FFProbe.AnalyseAsync(inputFilePath);
            try
            {
                await FFMpegArguments.FromFileInput(inputFilePath)
                    .OutputToFile(outputPath, false, options => options
                        .WithArgument(new CustomArgument("-map 0:m:language:eng"))
                        .WithArgument(new CustomArgument("-map -0:v"))
                        .WithArgument(new CustomArgument("-map -0:a"))
                        ).ProcessAsynchronously();
                video.subtitlesFilePath = outputPath;
                return video;
            }
            catch (Exception e)
            {
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }
                return video;
            }

        }


        /**
         * Convert video to web compatible format.
         * 
         * Currently will convert to a webm container with av1 video and libopus audio
         * 
         * 
         * Todo: use nvidia encode if available?
         * Todo: Check if video is already a webm (may not be enough as we still might want to extract subtitles)
         */
        public async Task<VideoFileTransformContainer> convertVideoToWebFormat(string inputFilePath)
        {
            var fileInfo = new FileInfo(inputFilePath);
            var fileName = fileInfo.Name;
            var outputPath = $"{getTempVideoStoragePath()}/{fileName}.webm";
            var mediaAnalysis = await FFProbe.AnalyseAsync(inputFilePath);
            var container = mediaAnalysis.Format.FormatName;

            var video = new VideoFileTransformContainer(inputFilePath, outputPath);
            customLogger.LogInformation("Check if video has already been converted and is valid.");

            //check if video has already been converted and file exists
            if (File.Exists(outputPath))
            {
                var validateExistingOutput = await CheckOutputVideoIntegrity(outputPath);
                if (validateExistingOutput)
                {
                    customLogger.LogInformation("Converted video already exists");
                    OnConversionComplete?.Invoke(this, new FileProgressEventArgs(video));
                    return video;
                }
            }

            customLogger.LogInformation("Start converting video.");

            //read percentage updates
            var onErrorHandler = (string msg) =>
            {
                Match match = ProgressRegex.Match(msg);
                if (match.Success)
                {
                    TimeSpan obj = TimeSpan.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                    double percentageUpdate = Math.Round(obj.TotalSeconds / mediaAnalysis.Duration.TotalSeconds * 100.0, 2);
                    string statusMessage = $"{percentageUpdate}% encoded";
                    customLogger.LogInformation(statusMessage);
                    OnConversionComplete?.Invoke(this, new FileProgressEventArgs(video, statusMessage));
                }
            };

            var test = FFMpeg.GetVideoCodecs();
            var subtitleStream = mediaAnalysis.PrimarySubtitleStream?.CodecLongName;

            var task = FFMpegArguments.FromFileInput(video.inputFilePath)
                .OutputToFile(outputPath, true, options => options
                .WithVideoCodec(MyCodecs.LibSvtAv1)
                .WithAudioCodec(MyCodecs.LibOpus)
                .WithCustomArgument("-ac 2") //force channel stereo mixing due to https://trac.ffmpeg.org/ticket/5718
                .WithCustomArgument("-sn") //remove subtitles
                .WithCustomArgument("-preset 5")
                .WithCustomArgument("-crf 30")
                .WithCustomArgument("-pix_fmt yuv420p10le") //use 10 bit video
                .WithCustomArgument("-g 240")
                .WithCustomArgument("-svtav1-params tune=0:film-grain=8") //use syntehthic film grain
                .WithFastStart())
                .NotifyOnError(onErrorHandler)
                .CancellableThrough(out var cancel)
                .ProcessAsynchronously();

            var OnApplicationExit = new EventHandler((sender, e) =>
            {
                cancel();
            }); ;

            Application.ApplicationExit += OnApplicationExit;
            await task;
            Application.ApplicationExit -= OnApplicationExit;
            OnConversionProgress.Invoke(this, new FileProgressEventArgs(video, "Writing Sha1"));
            await WriteSha1File(video);
            OnConversionComplete.Invoke(this, new FileProgressEventArgs(video, "Completed Encoding"));
            return video;
        }
    }
}


