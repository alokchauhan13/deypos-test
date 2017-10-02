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
        public static byte[] ComputeHashSumForFile(string fileLocation)
        {
            using (var sha = SHA256.Create())
            {
                using (var stream = File.OpenRead(fileLocation))
                {
                    return sha.ComputeHash(stream);
                }
            }
        }


        public static string ComputeHashAsString(string data)
        {
            using (var sha = SHA256.Create())
            {
                using (var stream = new MemoryStream())
                {
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(data);
                    writer.Flush();
                    stream.Position = 0;
                    byte[] byteArray = sha.ComputeHash(stream);
                    return ToString(byteArray, true);
                }
            }
        }

        public static string ToString(byte[] bytes, bool upperCase)
        {
            return BitConverter.ToString(bytes);
        }

        public static byte[] ToByteArray(string data)
        {
            String[] arr = data.Split('-');
            byte[] array = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++) array[i] = Convert.ToByte(arr[i], 16);
            return array;
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
