using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2VideoUploader.Model
{
    public class FileProgressEventArgs : EventArgs
    {
        public FileProgressEventArgs(VideoFileTransformContainer video, string? progressString = null)
        {
            Video = video;
            ProgressString = progressString;
        }

        public VideoFileTransformContainer Video { get; }
        public string? ProgressString { get; }
    }
}
