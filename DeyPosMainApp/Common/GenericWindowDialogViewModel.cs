using Common.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Users;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp.Common
{
    public class GenericWindowDialogViewModel : ObservableObject
    {      
        public string Title { get; set; }
        public GenericWindowDialogViewModel(Object viewModelData, string title)
        {
            this.Title = title;
            ViewModelData = viewModelData;
        }
        public Object ViewModelData { get; private set; }
    }
}
