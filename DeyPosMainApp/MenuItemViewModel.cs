using Common.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Common;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Users;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp
{
    public class MenuItemViewModel : ObservableObject
    {

        public ApplicationState ApplicationState { get; private set; }
        public MenuItemViewModel(ApplicationState applicationState)
        {
            ApplicationState = applicationState;
        }

        public DelegateCommand createUserCommand;

        public DelegateCommand CreateUserCommand
        {
            get
            {
                if (this.createUserCommand == null)
                {
                    this.createUserCommand = new DelegateCommand(ExecuteCreateUserCommand, CanExecuteCreateUserCommand);

                }
                return this.createUserCommand;
            }
        }


        public DelegateCommand helpCommand;

        public DelegateCommand HelpCommand
        {
            get
            {
                if (this.helpCommand == null)
                {
                    this.helpCommand = new DelegateCommand(ExecuteHelpCommand, CanExecuteHelpCommand);

                }
                return this.helpCommand;
            }
        }

        private void ExecuteCreateUserCommand(Object data)
        {
            GenericWindowDialogViewModel vm = new GenericWindowDialogViewModel(new UserInfoViewModel(this.ApplicationState.UserManager) { ShowViewToCreateUser = true},"Create User");

            GenericWindowDialog windowDialog = new GenericWindowDialog(vm);
            if(windowDialog.ShowDialog() == true)
            {

            }
            else
            {

            }
        }

        private bool CanExecuteCreateUserCommand(Object data)
        {
            return true;
        }

        private void ExecuteHelpCommand(Object data)
        {
            GenericWindowDialogViewModel vm = new GenericWindowDialogViewModel(new HelpViewModel(),"Help");

            GenericWindowDialog windowDialog = new GenericWindowDialog(vm);
            if (windowDialog.ShowDialog() == true)
            {

            }
            else
            {

            }
        }

        private bool CanExecuteHelpCommand(Object data)
        {
            return true;
        }

    }
}
