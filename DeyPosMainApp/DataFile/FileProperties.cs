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
            FileCreationTime = File.GetCreationTime(filePath).ToFileTimeUtc().ToString();
            FileNameHashId = Utility.ComputeHashAsString(FileName);
            FileBlocks = new ObservableCollection<FileBlock>();
            Owners = new ObservableCollection<User>();
            UserFileBlockMapping = new Dictionary<User, List<FileBlock>>();

            Version = 0;
        }
        public string FileSourcePath { get; private set; }

        public string FileName { get; private set; }

        public string FileNameHashId { get; private set; }

        public string FileContentHashID { get; set; }

        public string FileCreationTime { get; set; }

        public int Version { get; set; }

        public string CombinedHash { get {

                return Utility.ComputeHashAsString(FileNameHashId + "." + FileCreationTime);
                    
                     } }

        public string LatestVersionCloudLocation
        {
            get
            {
                return CloudLocation + "//" + Version;
            }
        }

        public string GetCloudLocationForVersion(int version)
        {
            return CloudLocation + "//" + version;
        }

        public string CloudLocation { get; set; }

        public string TmpCloudLocation { get; set; }
              

        public ObservableCollection<FileBlock> FileBlocks { get; private set; }

        public ObservableCollection<User> Owners { get; private set; }

        public Dictionary<User, List<FileBlock>> UserFileBlockMapping { get; private set; }


        public void UpdateUserFileBlock(User user, List<FileBlock> fileBlocks)
        {
            if (UserFileBlockMapping.ContainsKey(user))
            {
                UserFileBlockMapping[user] = fileBlocks;
            }
            else
            {
                UserFileBlockMapping.Add(user, fileBlocks);
            }
        }

        public bool IsOwnerExists(String userName)
        {
            foreach (var user in this.Owners)
            {
                if(user.Name == userName)
                {
                    return true;
                }
            }

            return false;
        }

        internal List<FileBlock> getFileBlocksForUser(User selectedUser, List<int> challangedIndexes)
        {
            List<FileBlock> fileBlocks = UserFileBlockMapping[selectedUser];

            List<FileBlock> selectedFileBlocks = new List<FileBlock>();

            foreach (var index in challangedIndexes)
            {
                foreach (var block in fileBlocks)
                {
                    if(block.Index == index)
                    {
                        selectedFileBlocks.Add(block);
                    }
                }
            }

            return selectedFileBlocks;
        }
    }
}
