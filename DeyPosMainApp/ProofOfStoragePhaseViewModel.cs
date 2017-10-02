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
using UVCE.ME.IEEE.Apps.DeyPosMainApp.HAT;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Users;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp
{
    public class ProofOfStoragePhaseViewModel : ObservableObject
    {

        ApplicationState ApplicationState { get; set; }
        public ProofOfStoragePhaseViewModel(ApplicationState applicationState)
        {
            ApplicationState = applicationState;
            this.challangedIndexes = new List<int>();
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
                    UpdateMaxBlockIndexNumber();
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
                    UpdateMaxBlockIndexNumber();
                }
            }
        }


        private int maxBlockIndex;

        public int MaxBlockIndex
        {
            get { return this.maxBlockIndex; }
            set
            {
                if (this.maxBlockIndex != value)
                {
                    this.maxBlockIndex = value;
                    RaisePropertyChanged(() => MaxBlockIndex);
                }
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

        public DelegateCommand executePOSCommand;

        public DelegateCommand ExecutePOSCommand
        {
            get
            {
                if (this.executePOSCommand == null)
                {
                    this.executePOSCommand = new DelegateCommand(ExecutePoSCommand, CanExecutePoSCommand);

                }
                return this.executePOSCommand;
            }
        }

        private string commaSepratedBlockIndexes;

        public string CommaSepratedBlockIndexes
        {
            get { return this.commaSepratedBlockIndexes; }
            set
            {
                if (this.commaSepratedBlockIndexes != value)
                {
                    this.commaSepratedBlockIndexes = value;
                    RaisePropertyChanged(() => CommaSepratedBlockIndexes);
                    ExecutePOSCommand.OnCanExecuteChanged();
                }
            }
        }


        private void ExecutePoSCommand(Object data)
        {
            try
            {
                StringBuilder logger = new StringBuilder();

                this.challangedIndexes.Clear();
                string[] stringItems = this.commaSepratedBlockIndexes.Split(',');

                logger.AppendLine("Challanged Indexes for file blocks: ");

                foreach (var item in stringItems)
                {
                    int index = int.Parse(item);
                    this.challangedIndexes.Add(index);

                    logger.Append(index.ToString() + "  ");
                }

                logger.AppendLine("Get the list of file blocks with version information");
                List<FileBlock> fileBlocks = selectedFile.getFileBlocksForUser(SelectedUser, this.challangedIndexes);


                List<FileBlock> cloudRecievedFileBlocks = new List<FileBlock>();

                foreach (FileBlock fileBlock in fileBlocks)
                {
                    logger.AppendLine();
                    logger.AppendLine("File block: "+ fileBlock.Index + " , Version: " + fileBlock.Version + " , known content Hash: " +  fileBlock.ContentHash);
                    string location = SelectedFile.GetCloudLocationForVersion(fileBlock.Version);
                    string hashRecievedFromCloudContent = Utility.ToString(Utility.ComputeHashSumForFile(location + "//" + fileBlock.Index), true);;
                    logger.AppendLine("Getting File block from cloud: " + fileBlock.Index + " , Cloud content Hash: " + fileBlock.ContentHash);
                    logger.AppendLine("Verifying hash :" +  string.Compare(hashRecievedFromCloudContent,fileBlock.ContentHash));

                    // We are creating the empty file block which have hash recieved from cloud
                    cloudRecievedFileBlocks.Add(new FileBlock(fileBlock.FileHashID, fileBlock.Index, fileBlock.Size) { ContentHash = hashRecievedFromCloudContent });
                }


                try
                {
                    // We are creatinng Homomorphic authenticated tree for requested and calculated top node hash code
                    logger.AppendLine("Cloud creatinng Homomorphic authenticated tree for challanged indexes");
                    HATTree<string> HTreeCloud = new HATTree<string>();
                    HTreeCloud.CreateTree(cloudRecievedFileBlocks);

                    logger.AppendLine("Cloud Homomorphic authenticated tree root node hash :" + HTreeCloud.Root.Hash);


                    logger.AppendLine("Locally creatinng Homomorphic authenticated tree for challanged indexes");
                    HATTree<string> HTreeLocal = new HATTree<string>();
                    HTreeLocal.CreateTree(fileBlocks);

                    logger.AppendLine("Locally created Homomorphic authenticated tree root node hash :" + HTreeLocal.Root.Hash);

                    if (HTreeCloud.Root.Hash == HTreeLocal.Root.Hash)
                    {
                        logger.AppendLine("Root hash matched. Content same");
                    }
                    else
                    {
                        logger.AppendLine("Root hash failed to match. Content differs");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }


                Log = logger.ToString();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }


        

        private bool CanExecutePoSCommand(Object data)
        {
            if(string.IsNullOrEmpty(CommaSepratedBlockIndexes))
                return false;
            if (SelectedFile == null || SelectedUser == null)
                return false;

            return true;
        }

        private void UpdateMaxBlockIndexNumber()
        {
            if(this.SelectedFile != null && this.SelectedUser != null)
            {
                if (this.selectedFile.IsOwnerExists(this.selectedUser.Name))
                {
                    MaxBlockIndex = this.SelectedFile.UserFileBlockMapping[this.selectedUser].Count - 1;
                }
                else
                {
                    MaxBlockIndex = -1;
                    //       MessageBox.Show("User does not have ownership for selected file. Please try another file.", "Error");
                }
            }
            else
            {
                MaxBlockIndex = -1;
           //     MessageBox.Show("User or File selection is missing", "Error");
            }
        }

        
    }
}
