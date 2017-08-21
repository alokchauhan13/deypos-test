using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Common;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Users;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp.DataFile
{
    public class FileProperties
    {
        public FileProperties(string filePath)
        {
            FileSourcePath = filePath;
            FileName = Path.GetFileName(filePath);
            FileNameHashId = Utility.ComputeHashAsString(FileName);
            FileBlocks = new ObservableCollection<FileBlock>();
            Owners = new ObservableCollection<User>();
        }
        public string FileSourcePath { get; private set; }

        public string FileName { get; private set; }

        public string FileNameHashId { get; private set; }
        public string FileContentHashID { get; set; }

        public string CombinedHash { get { return FileNameHashId + "." + FileContentHashID; } }

        public byte[] CheckSumByteArray { get; set; }

        public string CloudLocation { get; set; }

        public string TmpCloudLocation { get; set; }
              

        public ObservableCollection<FileBlock> FileBlocks { get; private set; }

        public ObservableCollection<User> Owners { get; private set; }
    }
}
