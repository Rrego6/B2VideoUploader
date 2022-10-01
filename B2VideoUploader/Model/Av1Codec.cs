using FFMpegCore;
using FFMpegCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2VideoUploader.Model
{
    public class Av1Codec
    {
        public static Codec LibSvtAv1 => FFMpeg.GetCodec("libsvtav1");
    }
}
