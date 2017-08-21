using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.DataFile;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Users;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp
{
    public class ApplicationState
    {
        public ApplicationState(string cloudLocation)
        {
            UserManager = new UserManager();
            FileManager = new FileManager(cloudLocation);
        }

        public UserManager UserManager { get; private set; }

        public FileManager FileManager { get; private set; }

    }
}
