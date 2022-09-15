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
using CustomArgument = B2VideoUploader.Model.CustomArgument;

namespace B2VideoUploader.Helper
{
    public class FfmpegVideoConversionService
    {
        private readonly CustomLogger customLogger;
        private readonly Config config;
        private static readonly Regex ProgressRegex = new Regex("time=(\\d\\d:\\d\\d:\\d\\d.\\d\\d?)", RegexOptions.Compiled);



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
                            new JProperty("contentType", "video/mp4"),
                            new JProperty("quality", quality)
                            )
                        )
                    );
            var subtitleProp = !String.IsNullOrEmpty(subtitleUrl) ?
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
            } else
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
            return height <= 240  ? 240
                : height <= 360 ? 360
                : height <= 480 ? 480
                : height <= 540 ? 540
                : height <= 720 ? 720
                : height <= 1080 ? 1080
                : height <= 1440 ? 1440
                : 2160;
        }

        private async Task WriteSha1File(string outputFilePath)
        {
            var infoFilePath = $"{Path.GetDirectoryName(outputFilePath)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(outputFilePath)}.sha1";
            var sha1Array = await Util.CreateSha1Array(outputFilePath, config.UploadPartSize);
            await File.WriteAllLinesAsync(infoFilePath, sha1Array);
        }

        private async Task<bool> CheckOutputVideoIntegrity(string outputFilePath)
        {
            var infoFilePath = $"{Path.GetDirectoryName(outputFilePath)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(outputFilePath)}.sha1";
            if (File.Exists(infoFilePath))
            {
                var sha1Array = await Util.CreateSha1Array(outputFilePath, config.UploadPartSize);
                var read = await File.ReadAllLinesAsync(infoFilePath);
                if(Enumerable.SequenceEqual(read, sha1Array))
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
        public async Task<string?> extractSubtitles(string inputFilePath)
        {
            customLogger.LogInformation("Begin extracting subtitles.");

            var fileName = Path.GetFileNameWithoutExtension(inputFilePath);
            var outputPath = $"{getTempVideoStoragePath()}/{fileName}.vtt";

            var userProvidedSubFileName =  $"{Path.GetDirectoryName(inputFilePath)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(inputFilePath)}.srt";
            if (File.Exists(userProvidedSubFileName))
            {
                customLogger.LogInformation("Found user provided subtitles");
                await FFMpegArguments.FromFileInput(userProvidedSubFileName)
                    .OutputToFile(outputPath, true).ProcessAsynchronously();
                return outputPath;
            }

            if (File.Exists(outputPath))
            {
                return outputPath;
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
                return outputPath;
            }
            catch(Exception e)
            {
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }
                return null;
            }

            }


        /**
         * Convert video to web compatible format.
         * 
         * Currently will convert to h264 in a single-pass. Ideally, vp9 should be used a two pass mode.
         * 
         * Todo: Dont reencode valid files
         * Todo: switch to vp9??
         * Todo: use nvidia encode if available?
         */


        public async Task<(string, string)> convertVideoToWebFormat(string inputFilePath, Action<double> onPercentageProgress )
        {
          /*
           * todo:
           * AnalyzeInputFile and return if valid for web
           * Analyze output file and return if valid for 
           */
            var fileName = Path.GetFileNameWithoutExtension(inputFilePath);
            var outputPath = $"{getTempVideoStoragePath()}/{fileName}.mp4";
            var mediaAnalysis = await FFProbe.AnalyseAsync(inputFilePath);
            var container = mediaAnalysis.Format.FormatName;

            customLogger.LogInformation("Check if video has already been converted and is valid.");

            //check if video has already been converted and file exists
            if (File.Exists(outputPath))
            {
                var validateExistingOutput = await CheckOutputVideoIntegrity(outputPath);
                if (validateExistingOutput)
                {
                    customLogger.LogInformation("Converted video already exists");
                    return (outputPath, container);
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
                    double obj2 = Math.Round(obj.TotalSeconds / mediaAnalysis.Duration.TotalSeconds * 100.0, 2);
                    onPercentageProgress!(obj2);
                }
            };
            

            var currentVideoCodec = mediaAnalysis.PrimaryVideoStream.GetCodecInfo();
            var currentAudioCodec = mediaAnalysis.PrimaryAudioStream.GetCodecInfo();
            var subtitleStream = mediaAnalysis.PrimarySubtitleStream?.CodecLongName;
            var task = FFMpegArguments.FromFileInput(inputFilePath)
                .OutputToFile(outputPath, true, options => options
                .WithVideoCodec(VideoCodec.LibX264)
                .WithConstantRateFactor(20) //https://trac.ffmpeg.org/wiki/Encode/H.264#crf
                .WithAudioCodec(AudioCodec.Aac)
                .WithFastStart())
                .NotifyOnError(onErrorHandler)
                .CancellableThrough(out var cancel)
                .ProcessAsynchronously();

            await WriteSha1File(outputPath);
            return (outputPath, container);
        }
    }
}
