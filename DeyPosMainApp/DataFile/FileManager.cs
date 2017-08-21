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

        private bool IsFileExists(string fileName)
        {
            foreach (var item in this.DataFiles)
            {
                if(item.FileName == fileName)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddUser(string fileName, User user)
        {
            foreach (var item in this.DataFiles)
            {
                if (item.FileName == fileName)
                {
                    item.Owners.Add(user);
                }
            }
        }

        private FileProperties GetFileProperties(string fileName, string owner)
        {
            foreach (var item in this.DataFiles)
            {
                foreach (var fileOwner in item.Owners)
                {
                    if (fileOwner.Name == owner && item.FileName == fileName)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        internal void AddFile(FileProperties file)
        {
            if(IsFileExists(file.FileName) == false)
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
