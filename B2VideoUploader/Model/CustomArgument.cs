using FFMpegCore.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2VideoUploader.Model
{
    public class CustomArgument : IArgument
    {
        private readonly string text;

        public CustomArgument(string text)
        {
            this.text = text;
        }
        public string Text => text;
    }
}
