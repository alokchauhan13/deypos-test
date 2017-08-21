using Common.UI;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.DataFile;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp
{
    public class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            SourceFileToLoad = @".\Data\SampleData\big.txt";
            TargetBlobLocation = @".\Data\SampleBlob";
            ApplicationState = new ApplicationState(TargetBlobLocation);
            AuthenticatedTreeViewModel = new AuthenticatedTreeViewModel(ApplicationState);
            PreProcessPhaseViewModel = new PreProcessPhaseViewModel(ApplicationState);
            ProofOfStoragePhaseViewModel = new ProofOfStoragePhaseViewModel(ApplicationState);
            UpdatePhaseViewModel = new UpdatePhaseViewModel(ApplicationState);
            UploadPhaseViewModel = new UploadPhaseViewModel(ApplicationState);
            MenuItemViewModel = new MenuItemViewModel(ApplicationState);
            HomomorphicExampleViewModel = new HomomorphicExampleViewModel();
        }

        #region View Models


        public AuthenticatedTreeViewModel AuthenticatedTreeViewModel { get; set; }

        public PreProcessPhaseViewModel PreProcessPhaseViewModel { get; set; }

        public ProofOfStoragePhaseViewModel ProofOfStoragePhaseViewModel { get; set; }

        public UpdatePhaseViewModel UpdatePhaseViewModel { get; set; }

        public UploadPhaseViewModel UploadPhaseViewModel { get; set; }

        public HomomorphicExampleViewModel HomomorphicExampleViewModel { get; set; }



        #endregion

        #region Properties

        public ApplicationState ApplicationState { get; set; }

        public string SourceFileToLoad { get; set; }

        public string TargetBlobLocation { get; set; }

        public MenuItemViewModel MenuItemViewModel { get; set; }


        private ObservableObject selectedPhase;

        public ObservableObject SelectedPhase
        {
            get { return this.selectedPhase; }
            set
            {
                if (this.selectedPhase != value)
                {
                    this.selectedPhase = value;
                    RaisePropertyChanged(() => SelectedPhase);
                }
            }
        }

        public ObservableCollection<FileBlock> FileBlocks
        {
            get
            {
                return ApplicationState.FileManager.CurrentSelectedFile.FileBlocks;
            }
        }

        #endregion

        #region Commands

        #region Load File Command

        public DelegateCommand loadFileCommand;

        public DelegateCommand LoadFileCommand
        {
            get
            {
                if (this.loadFileCommand == null)
                {
                    this.loadFileCommand = new DelegateCommand(ExecuteLoadFileCommand, CanExecuteLoadFileCommand);

                }
                return this.loadFileCommand;
            }
        }

        private void ExecuteLoadFileCommand(Object data)
        {
            MessageBox.Show("Execute Load : Source :" + SourceFileToLoad + ", Target Blob: " + TargetBlobLocation);
            ApplicationState.FileManager.CloudLocation = TargetBlobLocation;

            FileProperties fileInfo = ApplicationState.FileManager.CreateFileInfo(SourceFileToLoad, ApplicationState.UserManager.CurrentUser);
            ApplicationState.FileManager.CurrentSelectedFile = fileInfo;

            AuthenticatedTreeViewModel = new AuthenticatedTreeViewModel(ApplicationState);
            PreProcessPhaseViewModel = new PreProcessPhaseViewModel(ApplicationState);
            ProofOfStoragePhaseViewModel = new ProofOfStoragePhaseViewModel(ApplicationState);
            UpdatePhaseViewModel = new UpdatePhaseViewModel(ApplicationState);
            UploadPhaseViewModel = new UploadPhaseViewModel(ApplicationState);
            RaisePropertyChanged(() => FileBlocks);
            RaisePropertyChanged(() => string.Empty);
        }

        private bool CanExecuteLoadFileCommand(Object data)
        {
            return true;
        }

        #endregion

        #region Create Authenticated Tree

        public DelegateCommand createAuthenticatedCommand;

        public DelegateCommand CreateAuthenticatedCommand
        {
            get
            {
                if (this.createAuthenticatedCommand == null)
                {
                    this.createAuthenticatedCommand = new DelegateCommand(ExecuteCreateAuthenticatedCommand, CanExecuteCreateAuthenticatedCommand);

                }
                return this.createAuthenticatedCommand;
            }
        }

        private void ExecuteCreateAuthenticatedCommand(Object data)
        {
            SelectedPhase = AuthenticatedTreeViewModel;
        }

        private bool CanExecuteCreateAuthenticatedCommand(Object data)
        {
            return true;
        }

        #endregion

        #region PreProcess Command


        public DelegateCommand preProcessCommand;

        public DelegateCommand PreProcessCommand
        {
            get
            {
                if (this.preProcessCommand == null)
                {
                    this.preProcessCommand = new DelegateCommand(ExecutePreProcessCommand, CanExecutePreProcessCommand);

                }
                return this.preProcessCommand;
            }
        }

        private void ExecutePreProcessCommand(Object data)
        {
            SelectedPhase = PreProcessPhaseViewModel;
        }

        private bool CanExecutePreProcessCommand(Object data)
        {
            return true;
        }

        #endregion

        #region Upload And Deduplication Command

        public DelegateCommand uploadAndDeduplicateCommand;

        public DelegateCommand UploadAndDeduplicateCommand
        {
            get
            {
                if (this.uploadAndDeduplicateCommand == null)
                {
                    this.uploadAndDeduplicateCommand = new DelegateCommand(ExecuteUploadCommand, CanExecuteUploadCommand);

                }
                return this.uploadAndDeduplicateCommand;
            }
        }

        private void ExecuteUploadCommand(Object data)
        {
            SelectedPhase = UploadPhaseViewModel;
        }

        private bool CanExecuteUploadCommand(Object data)
        {
            return true;
        }


        #endregion

        #region Update Command

        public DelegateCommand updateCommand;

        public DelegateCommand UpdateCommand
        {
            get
            {
                if (this.updateCommand == null)
                {
                    this.updateCommand = new DelegateCommand(ExecuteUpdateCommand, CanExecuteUpdateCommand);

                }
                return this.updateCommand;
            }
        }

        private void ExecuteUpdateCommand(Object data)
        {
            SelectedPhase = UpdatePhaseViewModel;
        }

        private bool CanExecuteUpdateCommand(Object data)
        {
            return true;
        }


        #endregion

        #region Proof Of Storage Command

        public DelegateCommand proofOfStorageCommand;

        public DelegateCommand ProofOfStorageCommand
        {
            get
            {
                if (this.proofOfStorageCommand == null)
                {
                    this.proofOfStorageCommand = new DelegateCommand(ExecuteProofOfStorageCommand, CanExecuteProofOfStorageCommand);

                }
                return this.proofOfStorageCommand;
            }
        }

        private void ExecuteProofOfStorageCommand(Object data)
        {
            SelectedPhase = ProofOfStoragePhaseViewModel;
        }

        private bool CanExecuteProofOfStorageCommand(Object data)
        {
            return true;
        }


        #endregion

        #region Homomorophic Command

        public DelegateCommand homomorphicCommand;

        public DelegateCommand HomomorphicCommand
        {
            get
            {
                if (this.homomorphicCommand == null)
                {
                    this.homomorphicCommand = new DelegateCommand(ExecuteHomomorphicCommand, CanExecuteHomomorphicCommand);

                }
                return this.homomorphicCommand;
            }
        }

        private void ExecuteHomomorphicCommand(Object data)
        {
            SelectedPhase = HomomorphicExampleViewModel;
        }

        private bool CanExecuteHomomorphicCommand(Object data)
        {
            return true;
        }


        #endregion

        #endregion
    }
}
