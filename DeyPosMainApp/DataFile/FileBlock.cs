using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp.DataFile
{
    public class FileBlock
    {
        public FileBlock(string fileId, int index, long size)
        {
            FileHashID = fileId;
            Size = size;
            Index = index;
            Version = 1;
        }

        public string FileHashID { get; private set; }

        public int Version { get; set; }

        public long Size { get; private set; }

        public int Index { get; private set; }

        public string ContentHash { get; set; }

        public string UserName { get; set; }
    }
}
