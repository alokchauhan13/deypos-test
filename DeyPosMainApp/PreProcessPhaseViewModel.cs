using Common.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Common;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp
{
    public class PreProcessPhaseViewModel : ObservableObject
    {

        ApplicationState ApplicationState { get; set; }
        public PreProcessPhaseViewModel(ApplicationState applicationState)
        {
            ApplicationState = applicationState;
        }


        private string sha512Hash;

        public string SHA256Hash
        {
            get { return this.sha512Hash; }
            set
            {
                if (this.sha512Hash != value)
                {
                    this.sha512Hash = value;
                    RaisePropertyChanged(() => SHA256Hash);
                }
            }
        }



        #region Upload Command

        public DelegateCommand runCommand;

        public DelegateCommand RunCommand
        {
            get
            {
                if (this.runCommand == null)
                {
                    this.runCommand = new DelegateCommand(ExecuteRunCommand, CanExecuteRunCommand);

                }
                return this.runCommand;
            }
        }

        private void ExecuteRunCommand(Object data)
        {
            try
            {
                ApplicationState.FileManager.CurrentSelectedFile.CheckSumByteArray = Utility.ComputeHashSum(ApplicationState.FileManager.CurrentSelectedFile.FileSourcePath);
                ApplicationState.FileManager.CurrentSelectedFile.FileContentHashID = Utility.ToHex(ApplicationState.FileManager.CurrentSelectedFile.CheckSumByteArray, true);

                SHA256Hash = "HEX Values: " + "\r\n";

                SHA256Hash = SHA256Hash + ApplicationState.FileManager.CurrentSelectedFile.CombinedHash;

                SHA256Hash = SHA256Hash + "\r\n" + "Actual Byte Array :";

                foreach (var item in ApplicationState.FileManager.CurrentSelectedFile.CheckSumByteArray)
                {

                    SHA256Hash = SHA256Hash +  item.ToString() + " ";
                }

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


        #endregion

    }
}
