using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2VideoUploader.Model
{
    /**
     * Generic class to hold information about video as it is being converted
     */
    public class VideoFileTransformContainer
    {
        public readonly FileInfo inputFileInfo;
        public readonly FileInfo? outputFileInfo;
        public readonly string fileName;

        public string inputFilePath;
        public string? outputFilePath;
        public string? subtitlesFilePath;
        public string? totalSha1;
        public string[]? sha1Array;

        public VideoFileTransformContainer(string inputFilePath, string? outputFilePath = null, string? subtitlesFilePath = null, string? totalSha1 = null, string[]? sha1Array = null)
        {
            this.inputFilePath = inputFilePath;
            this.outputFilePath = outputFilePath;
            this.subtitlesFilePath = subtitlesFilePath;
            this.totalSha1 = totalSha1;
            this.sha1Array = sha1Array;

            inputFileInfo = new FileInfo(inputFilePath);
            if (outputFilePath != null)
            {
                outputFileInfo = new FileInfo(outputFilePath);
            }
            fileName = inputFileInfo.Name;
        }


        public override bool Equals(object? obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            } else
            {
                VideoFileTransformContainer other = (VideoFileTransformContainer)obj;
                return this.fileName == other.fileName;
            }
        }

        public override int GetHashCode()
        {
            return fileName.GetHashCode();
        }

    }
}
