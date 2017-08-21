using Common.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.DataFile;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp.Users
{
    public class UserManager : ObservableObject
    {
        public UserManager()
        {
            Users = new ObservableCollection<User>();
        }

        public ObservableCollection<User> Users { get; private set; }

        private User currentUser;

        public User CurrentUser
        {
            get { return this.currentUser; }
            set
            {
                if (this.currentUser != value)
                {
                    this.currentUser = value;
                    RaisePropertyChanged(() => CurrentUser);
                }
            }
        }



        public void AddUser(string name)
        {
            foreach (var item in this.Users)
            {
                if(item.Name == name)
                {
                    // User found.
                    MessageBox.Show("User: " + name + " already exists.","New User", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            Users.Add(new User(name));          
        }

        public User getUser(string name)
        {
            foreach (var item in this.Users)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
