using Common.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Common;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.DataFile;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.HAT;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp
{
    public class AuthenticatedTreeViewModel : ObservableObject
    {

        ApplicationState ApplicationState { get; set; }
        public AuthenticatedTreeViewModel(ApplicationState  applicationState)
        {
            ApplicationState = applicationState;
        }

        private string merkleTextLog;

        public string MerkleTextLog
        {
            get { return this.merkleTextLog; }
            set
            {
                if (this.merkleTextLog != value)
                {
                    this.merkleTextLog = value;
                    RaisePropertyChanged(() => MerkleTextLog);
                }
            }
        }


        private string homomorphicTextLog;

        public string HomomorphicTextLog
        {
            get { return this.homomorphicTextLog; }
            set
            {
                if (this.homomorphicTextLog != value)
                {
                    this.homomorphicTextLog = value;
                    RaisePropertyChanged(() => HomomorphicTextLog);
                }
            }
        }



        public DelegateCommand createBlockCommand;

        public DelegateCommand CreateBlockCommand
        {
            get
            {
                if (this.createBlockCommand == null)
                {
                    this.createBlockCommand = new DelegateCommand(ExecuteCreateBlockCommand, CanExecuteRunCommand);

                }
                return this.createBlockCommand;
            }
        }




        public DelegateCommand createAuthneticatedTreeCommand;

        public DelegateCommand CreateAuthneticatedTreeCommand
        {
            get
            {
                if (this.createAuthneticatedTreeCommand == null)
                {
                    this.createAuthneticatedTreeCommand = new DelegateCommand(ExecuteCreateAuthneticatedTreeCommand, CanExecuteRunCommand);

                }
                return this.createAuthneticatedTreeCommand;
            }
        }


        private void ExecuteCreateBlockCommand(Object data)
        {
            try
            {
                if(Directory.Exists(ApplicationState.FileManager.CloudLocation) == false)
                {
                    Directory.CreateDirectory(ApplicationState.FileManager.CloudLocation);
                }


                if (Directory.Exists(ApplicationState.FileManager.TmpCloudLocation) == false)
                {
                    Directory.CreateDirectory(ApplicationState.FileManager.TmpCloudLocation);
                }

                bool createBlocks = true;
                string fileBlockDirectory = ApplicationState.FileManager.CloudLocation + "\\" + ApplicationState.FileManager.CurrentSelectedFile.CombinedHash;

                string tmpfileBlockDirectory = ApplicationState.FileManager.TmpCloudLocation + "\\" + ApplicationState.FileManager.CurrentSelectedFile.CombinedHash;

                ApplicationState.FileManager.CurrentSelectedFile.CloudLocation = fileBlockDirectory;
                ApplicationState.FileManager.CurrentSelectedFile.TmpCloudLocation = tmpfileBlockDirectory;

                if (Directory.Exists(fileBlockDirectory) == false)
                {
                    Directory.CreateDirectory(fileBlockDirectory);
                }
                

                if (Directory.Exists(tmpfileBlockDirectory) == false)
                {
                    Directory.CreateDirectory(tmpfileBlockDirectory);
                }
                else
                {
                    if(MessageBox.Show("Tmp cloud Directory already exists: " + tmpfileBlockDirectory + ". Do you want to again create file blocks", "Warning", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                    {
                        Directory.Delete(tmpfileBlockDirectory, true);
                        if (Directory.Exists(tmpfileBlockDirectory) == false)
                        {
                            Directory.CreateDirectory(tmpfileBlockDirectory);
                        }
                    }
                    else
                    {
                        createBlocks = false;
                    }
                }

                if(createBlocks == true)
                {
                    Utility.SplitFile(ApplicationState.FileManager.CurrentSelectedFile.FileSourcePath, 1024 * 512, tmpfileBlockDirectory);
                }

                ReadFileBlocks(tmpfileBlockDirectory);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void ReadFileBlocks(string directoryName)
        {
            ApplicationState.FileManager.CurrentSelectedFile.FileBlocks.Clear();

            foreach (var item in Directory.GetFiles(directoryName))
            {
                try
                {
                    FileInfo f = new FileInfo(item);
                    FileBlock fileBlock = new FileBlock(ApplicationState.FileManager.CurrentSelectedFile.FileContentHashID,
                                                        ApplicationState.UserManager.CurrentUser.Name,
                                                        int.Parse(Path.GetFileNameWithoutExtension(item)),
                                                        f.Length);


                    fileBlock.ContentHash = Utility.ToHex(Utility.ComputeHashSum(item),true);

                    ApplicationState.FileManager.CurrentSelectedFile.FileBlocks.Add(fileBlock);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void ExecuteCreateAuthneticatedTreeCommand(Object data)
        {
            CreateMerkleTree();
            CreateHomomorphicTree();
        }

        private void CreateHomomorphicTree()
        {
            try
            {
                HATTree<string> HTree = new HATTree<string>();

                HTree.CreateTree(ApplicationState.FileManager.CurrentSelectedFile.FileBlocks.ToList());
                List<HATNode<string>> nodes = HTree.PreOrderTraversal();

                StringBuilder compleString = new StringBuilder();
                foreach (HATNode<string> item in nodes)
                {
                    string logText = item.Index + "\t" + item.LeafNodesCount + "\t " + item.Version + "\t" + item.Tag +"\r\n";
                    compleString.Append(logText);
                }

                HomomorphicTextLog = compleString.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateMerkleTree()
        {
            try
            {
                MerkleTree MTree = new MerkleTree();
                MTree.CreateTree(ApplicationState.FileManager.CurrentSelectedFile.FileBlocks.ToList());
                List<MerkleTreeNode> nodes = MTree.PreOrderTraversal();
                StringBuilder compleString = new StringBuilder();
                foreach (MerkleTreeNode item in nodes)
                {
                    string logText = item.Index + "," + item.IsBlockNode + ", " + item.Hash + "\r\n";
                    compleString.Append(logText);
                }

                MerkleTextLog = compleString.ToString();

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
