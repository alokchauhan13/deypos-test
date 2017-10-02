using Common.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Users;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp.DataFile
{
    public class FileManager : ObservableObject
    {
        public FileManager(string cloudLocation)
        {
            DataFiles = new ObservableCollection<FileProperties>();
            CloudLocation = cloudLocation;
        }

        private FileProperties currentSelectedFile;

        public FileProperties CurrentSelectedFile
        {
            get { return this.currentSelectedFile; }
            set
            {
                if (this.currentSelectedFile != value)
                {
                    this.currentSelectedFile = value;
                    RaisePropertyChanged(() => CurrentSelectedFile);
                }
            }
        }

        public string CloudLocation { get; set; }

        public string TmpCloudLocation { get { return CloudLocation + "\\Tmp"; } }
        public ObservableCollection<FileProperties> DataFiles { get; private set; }

        public FileProperties CreateFileInfo(string filePath, User owner)
        {
            FileProperties prop = new FileProperties(filePath);
            prop.CloudLocation = this.CloudLocation;
            // Adding owners of the files.
            prop.Owners.Add(owner);
            return prop;
        }

        private bool IsOwnerExistsForFile(string fileName, string user)
        {
            foreach (var item in this.DataFiles)
            {
                foreach (var fileOwner in item.Owners)
                {
                    if(fileOwner.Name == user && item.FileName == fileName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsFileExists(string fileHash)
        {
            foreach (var item in this.DataFiles)
            {
                if(item.CombinedHash == fileHash)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddUser(string fileHash, User user)
        {
            foreach (FileProperties file in this.DataFiles)
            {
                if (file.CombinedHash == fileHash)
                {
                    bool ownerFound = false;
                    foreach (var item in file.Owners)
                    {
                        if(item.Name == user.Name)
                        {
                            ownerFound = true;
                        }
                    }
                    if(ownerFound == false)
                        file.Owners.Add(user);
                }
            }
        }

        private FileProperties GetFileProperties(string fileHash, string owner)
        {
            foreach (var item in this.DataFiles)
            {
                foreach (var fileOwner in item.Owners)
                {
                    if (fileOwner.Name == owner && item.CombinedHash == fileHash)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        internal void AddFile(FileProperties file)
        {
            if(IsFileExists(file.CombinedHash) == false)
            {
                this.DataFiles.Add(file);
            }
            else
            {
                MessageBox.Show("File already exists. Nothing to Add");
            }
        }
    }
}
