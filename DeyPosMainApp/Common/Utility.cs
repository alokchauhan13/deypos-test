using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp.Common
{
    public class Utility
    {
        public static byte[] ComputeHashSum(string fileName)
        {
            using (var md5 = SHA256.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return md5.ComputeHash(stream);
                }
            }
        }


        public static string ComputeHashAsString(string data)
        {
            using (var md5 = SHA256.Create())
            {
                using (var stream = new MemoryStream())
                {
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(data);
                    writer.Flush();
                    stream.Position = 0;
                    byte[] byteArray = md5.ComputeHash(stream);
                    return ToHex(byteArray, true);
                }
            }
        }

        public static string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }

        public static void SplitFile(string inputFile, int chunkSize, string path)
        {
            const int BUFFER_SIZE = 20 * 1024;
            byte[] buffer = new byte[BUFFER_SIZE];

            using (Stream input = File.OpenRead(inputFile))
            {
                int index = 0;
                while (input.Position < input.Length)
                {
                    using (Stream output = File.Create(path + "\\" + index))
                    {
                        int remaining = chunkSize, bytesRead;
                        while (remaining > 0 && (bytesRead = input.Read(buffer, 0,
                                Math.Min(remaining, BUFFER_SIZE))) > 0)
                        {
                            output.Write(buffer, 0, bytesRead);
                            remaining -= bytesRead;
                        }
                    }
                    index++;
                    Thread.Sleep(100);
                }
            }
        }
    }
}
