using Common.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Common;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.DataFile;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Users;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp
{
    public class UpdatePhaseViewModel : ObservableObject
    {

        ApplicationState ApplicationState { get; set; }
        public UpdatePhaseViewModel(ApplicationState applicationState)
        {
            ApplicationState = applicationState;
            UpdatedFileLocation = @".\Data\SampleData\Updated\big.txt";
        }


        public ObservableCollection<User> Users
        {
            get { return this.ApplicationState.UserManager.Users; }
        }

        public List<int> challangedIndexes;

        private User selectedUser;

        public User SelectedUser
        {
            get { return this.selectedUser; }
            set
            {
                if (this.selectedUser != value)
                {
                    this.selectedUser = value;
                    RaisePropertyChanged(() => SelectedUser);
                }
            }

        }


        public ObservableCollection<FileProperties> Files
        {
            get { return this.ApplicationState.FileManager.DataFiles; }
        }


        private FileProperties selectedFile;

        public FileProperties SelectedFile
        {
            get { return this.selectedFile; }
            set
            {
                if (this.selectedFile != value)
                {
                    this.selectedFile = value;
                    RaisePropertyChanged(() => SelectedFile);
                }
            }
        }


        private string updatedFileLocation;

        public string UpdatedFileLocation
        {
            get { return this.updatedFileLocation; }
            set
            {
                if (this.updatedFileLocation != value)
                {
                    this.updatedFileLocation = value;
                    RaisePropertyChanged(() => UpdatedFileLocation);
                    UpdateFileCommand.OnCanExecuteChanged();
                }
            }
        }


        public DelegateCommand updateFileCommand;

        public DelegateCommand UpdateFileCommand
        {
            get
            {
                if (this.updateFileCommand == null)
                {
                    this.updateFileCommand = new DelegateCommand(ExecuteUpdateFileCommand, CanExecuteUpdateFileCommand);

                }
                return this.updateFileCommand;
            }
        }

        private string log;

        public string Log
        {
            get { return this.log; }
            set
            {
                if (this.log != value)
                {
                    this.log = value;
                    RaisePropertyChanged(() => Log);
                }
            }
        }

        private void ExecuteUpdateFileCommand(Object data)
        {
            try
            {             
                ExecuteCreateBlockCommand(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }




        private bool CanExecuteUpdateFileCommand(Object data)
        {
            if (string.IsNullOrEmpty(UpdatedFileLocation))
                return false;
            if (SelectedFile == null || SelectedUser == null)
                return false;

            return true;
        }

        private void ExecuteCreateBlockCommand(Object data)
        {
            try
            {
                StringBuilder logger = new StringBuilder();
                logger.AppendLine("Start creating the blocks for updated file. If no change found no version update will be performed.");

                bool createBlocks = true;
                string tmpfileBlockDirectory = ApplicationState.FileManager.TmpCloudLocation + "\\" + SelectedFile.CombinedHash;

                logger.AppendLine("Creating temporary location to create file blocks");

                Directory.Delete(tmpfileBlockDirectory, true);
                if (Directory.Exists(tmpfileBlockDirectory) == false)
                {
                    Directory.CreateDirectory(tmpfileBlockDirectory);
                }


                if (createBlocks == true)
                {
                    Utility.SplitFile(UpdatedFileLocation, 1024 * 512, tmpfileBlockDirectory);
                    logger.AppendLine("File blocks created...");
                }

                List<FileBlock> fileBlocks = ReadFileBlocks(tmpfileBlockDirectory);

                bool contentChanged = false;

                logger.AppendLine("If any changes found from previous version, version number will be updated.");
                logger.AppendLine();

                foreach (var file  in fileBlocks)
                {
                    foreach(var oldFile in SelectedFile.UserFileBlockMapping[selectedUser])
                    {
                        if(file.Index == oldFile.Index && file.ContentHash != oldFile.ContentHash)
                        {
                            file.Version = oldFile.Version + 1;
                            contentChanged = true;
                            logger.AppendLine("Content change detected for Index : " + file.Index + ", New block hash: " + file.ContentHash);
                        }                        
                    }
                }

                if (contentChanged)
                {
                    logger.AppendLine();
                    logger.AppendLine("Updating file version information.");
                    SelectedFile.UpdateUserFileBlock(SelectedUser, fileBlocks);
                    SelectedFile.Version = SelectedFile.Version + 1;

                    Directory.CreateDirectory(SelectedFile.LatestVersionCloudLocation);

                    logger.AppendLine("Uploading latest file for selected user on cloud.");
                    Utility.SplitFile(UpdatedFileLocation, 1024 * 512, SelectedFile.LatestVersionCloudLocation);

                    logger.AppendLine("Latest file blocks for selected user can be found on: " + SelectedFile.LatestVersionCloudLocation);

                    logger.AppendLine("This update can be verified via POS step");

                }

                Log = logger.ToString();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private List<FileBlock> ReadFileBlocks(string directoryName)
        {
            List<FileBlock> fileBlocks = new List<FileBlock>();

            foreach (var file in Directory.GetFiles(directoryName))
            {
                try
                {
                    FileInfo f = new FileInfo(file);
                    FileBlock fileBlock = new FileBlock(SelectedFile.FileContentHashID,
                                                        int.Parse(Path.GetFileNameWithoutExtension(file)),
                                                        f.Length);


                    fileBlock.ContentHash = Utility.ToString(Utility.ComputeHashSumForFile(file), true);

                    fileBlocks.Add(fileBlock);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return fileBlocks;
        }



    }
}
