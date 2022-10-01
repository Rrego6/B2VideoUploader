using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2VideoUploader.Model
{
    public interface IOnProgressConversionEvents
    {
        event EventHandler<FileProgressEventArgs> OnConversionStart;
        event EventHandler<FileProgressEventArgs> OnConversionProgress;
        event EventHandler<FileProgressEventArgs> OnConversionComplete;
        event EventHandler<FileProgressEventArgs> OnConversionError;
    }
}
