using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2VideoUploader.Model
{
    public interface IOnProgressUploadEvents
    {
        event EventHandler<FileProgressEventArgs> OnUploadStart;
        event EventHandler<FileProgressEventArgs> OnUploadProgress;
        event EventHandler<FileProgressEventArgs> OnUploadComplete;
        event EventHandler<FileProgressEventArgs> OnUploadError;
    }
}
