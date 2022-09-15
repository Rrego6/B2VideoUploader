using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace B2VideoUploader.Helper
{
    public static class Util
    {

        public static async Task WriteSha1File(string videoOutputFilePath, long uploadPartSize)
        {
            var infoFilePath = $"{Path.GetDirectoryName(videoOutputFilePath)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(videoOutputFilePath)}.sha1";
            var sha1Array = await Util.CreateSha1Array(videoOutputFilePath, uploadPartSize);
            await File.WriteAllLinesAsync(infoFilePath, sha1Array);
        }

        public static async Task<string[]> ReadSha1File(string videoOutputFilePath)
        {
            var infoFilePath = $"{Path.GetDirectoryName(videoOutputFilePath)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(videoOutputFilePath)}.sha1";
            return await File.ReadAllLinesAsync(infoFilePath);
        }

        public static Task<string[]> CreateSha1Array(string filePath, long uploadPartSize)
        {
            return Task.Run(() =>
            {
                SHA1 sha1 = SHA1.Create();
                FileInfo fileInfo = new FileInfo(filePath);
                string fileName = fileInfo.Name;

                long localFileSize = fileInfo.Length;
                long minimumPartSize = uploadPartSize;
                var numParts = localFileSize / uploadPartSize + 1;
                long totalBytesSent = 0;
                var sha1Array = new String[numParts];

                int partNum = 1;
                while (totalBytesSent < localFileSize)
                {
                    var bytesToSend = uploadPartSize;
                    if ((localFileSize - totalBytesSent) < minimumPartSize)
                    {
                        bytesToSend = (localFileSize - totalBytesSent);
                    }

                    byte[] dataChunk = Util.getDataChunk(filePath, totalBytesSent, bytesToSend);
                    string sha1CheckSum = Util.calcSha1(dataChunk, (int)bytesToSend);

                    //store sha1 for validation later
                    sha1Array[partNum - 1] = sha1CheckSum;

                    partNum++;
                    totalBytesSent = totalBytesSent + bytesToSend;
                }
                return sha1Array;
            });
        }

        public static string urlEncode(string str)
        {
            if (str == "/")
            {
                return str;
            }
            return Uri.EscapeDataString(str);
        }

        public static byte[] getDataChunk(string filePath, long initByte, long bytesToRead)
        {
            FileStream f = File.OpenRead(filePath);
            byte[] data = new byte[bytesToRead];

            f.Seek(initByte, SeekOrigin.Begin);
            f.Read(data, 0, (int)bytesToRead);
            f.Close();
            return data;
        }

        public static string calcSha1(byte[] data, int dataLength)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] hashData = sha1.ComputeHash(data, 0, dataLength);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashData)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static string calcSha1(string filePath)
        {
            SHA1 sha1 = SHA1.Create();
            var fileStream = File.OpenRead(filePath);
            byte[] hashData = sha1.ComputeHash(fileStream);
            fileStream.Close();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashData)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }


        public static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }
    }
}
