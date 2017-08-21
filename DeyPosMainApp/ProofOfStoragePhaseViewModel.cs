using Common.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp
{
    public class ProofOfStoragePhaseViewModel : ObservableObject
    {

        ApplicationState ApplicationState { get; set; }
        public ProofOfStoragePhaseViewModel(ApplicationState applicationState)
        {
            ApplicationState = applicationState;
        }

    }
}
