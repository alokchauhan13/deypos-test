using Common.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Common;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Users;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp
{
    public class UploadPhaseViewModel : ObservableObject
    {


        ApplicationState ApplicationState { get; set; }
        public UploadPhaseViewModel(ApplicationState applicationState)
        {
            ApplicationState = applicationState;
        }



        private string uploadFileLogs;

        public string UploadFileLogs
        {
            get { return this.uploadFileLogs; }
            set
            {
                if (this.uploadFileLogs != value)
                {
                    this.uploadFileLogs = value;
                    RaisePropertyChanged(() => UploadFileLogs);
                }
            }
        }


        public DelegateCommand uploadFileCommand;
        private object tmpfileBlockDirectory;

        public DelegateCommand UploadFileCommand
        {
            get
            {
                if (this.uploadFileCommand == null)
                {
                    this.uploadFileCommand = new DelegateCommand(ExecuteRunCommand, CanExecuteRunCommand);

                }
                return this.uploadFileCommand;
            }
        }

        private void ExecuteRunCommand(Object data)
        {
            UploadFileLogs = string.Empty;
            StringBuilder logString = new StringBuilder();
            try
            {
                logString.AppendLine("Trying finding the file on cloud...");

                bool fileFlound = false;
                bool ownerFound = false;

                foreach(var item in ApplicationState.FileManager.DataFiles)
                {
                    if (ApplicationState.FileManager.CurrentSelectedFile.CombinedHash == item.CombinedHash)
                    {
                        fileFlound = true;
                    }

                    logString.AppendLine(item.FileName + " : "+ item.FileContentHashID );
                    logString.AppendLine("Owners: ");

                    foreach (var owner in item.Owners)
                    {
                        if(fileFlound == true)
                        {
                            if(ApplicationState.UserManager.CurrentUser.Name == owner.Name)
                            {
                                ownerFound = true;
                            }
                        }
                        logString.Append(owner.Name + ", ");
                    }
                }

                if(fileFlound== true)
                {
                    logString.AppendLine("File found...");
                }
                else
                {
                    logString.AppendLine("File Not found...Uploading File...");
                    ApplicationState.FileManager.CurrentSelectedFile.Version = 1;

                    if (Directory.Exists(ApplicationState.FileManager.CurrentSelectedFile.CloudLocation) == true)
                    {
                        Directory.Delete(ApplicationState.FileManager.CurrentSelectedFile.CloudLocation, true);
                    }
                    Directory.CreateDirectory(ApplicationState.FileManager.CurrentSelectedFile.LatestVersionCloudLocation);
                    Utility.SplitFile(ApplicationState.FileManager.CurrentSelectedFile.FileSourcePath, 1024 * 512, 
                        ApplicationState.FileManager.CurrentSelectedFile.LatestVersionCloudLocation);

                    ApplicationState.FileManager.AddFile(ApplicationState.FileManager.CurrentSelectedFile);

                }

                if (ownerFound == true)
                {
                    logString.AppendLine("File owner found...");
                }
                else
                {
                    logString.AppendLine("File owner Not found...Adding Owner...");
                    ApplicationState.FileManager.AddUser(ApplicationState.FileManager.CurrentSelectedFile.CombinedHash,
                                                        ApplicationState.UserManager.CurrentUser);

                    logString.AppendLine("Adding file blocks version information for new user...");
                    ApplicationState.FileManager.CurrentSelectedFile.UpdateUserFileBlock(ApplicationState.UserManager.CurrentUser,
                                                                                                    ApplicationState.FileManager.CurrentSelectedFile.FileBlocks.ToList());
                }

                logString.AppendLine(string.Format("Owner List for file {0}...", ApplicationState.FileManager.CurrentSelectedFile.FileName));
                foreach (User user in ApplicationState.FileManager.CurrentSelectedFile.Owners)
                {
                    logString.AppendLine("\r\n"+ user.Name + ". Following is version and hash for each block");
                    foreach (var item in ApplicationState.FileManager.CurrentSelectedFile.UserFileBlockMapping[user])
                    {
                        logString.AppendLine(item.Index + ":" + item.Version + " : " + item.ContentHash);
                    }
                }

                UploadFileLogs = logString.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private bool CanExecuteRunCommand(Object data)
        {
            return ApplicationState.FileManager.CurrentSelectedFile != null;
        }


    }
}
