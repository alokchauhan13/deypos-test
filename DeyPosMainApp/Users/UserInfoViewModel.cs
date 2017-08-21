using Common.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Common;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp.Users
{
    public class UserInfoViewModel : ObservableObject
    {
        public UserInfoViewModel(UserManager userManager)
        {
            ShowViewToCreateUser = false;
            this.userManager = userManager;
        }

        UserManager userManager;

        private string userName;

        public string UserName
        {
            get { return this.userName; }
            set
            {
                if (this.userName != value)
                {
                    this.userName = value;
                    RaisePropertyChanged(() => UserName);
                    if(String.IsNullOrEmpty(this.userName)== false)
                        UserNameHash = Utility.ComputeHashAsString(this.userName);
                }
            }
        }


        private string userNameHash;

        public string UserNameHash
        {
            get { return this.userNameHash; }
            set
            {
                if (this.userNameHash != value)
                {
                    this.userNameHash = value;
                    RaisePropertyChanged(() => UserNameHash);
                }
            }
        }

        private bool showViewToCreateUser;

        public bool ShowViewToCreateUser
        {
            get { return this.showViewToCreateUser; }
            set
            {
                if (this.showViewToCreateUser != value)
                {
                    this.showViewToCreateUser = value;
                    RaisePropertyChanged(() => ShowViewToCreateUser);
                }
            }
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

        private void ExecuteCreateUserCommand(Object data)
        {
            this.userManager.AddUser(UserName);
        }

        private bool CanExecuteCreateUserCommand(Object data)
        {
            return true;
        }
    }
}
