using Common.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp
{
    public class FileInformation : ObservableObject
    {


        private int index;

        public int Index
        {
            get { return this.index; }
            set
            {
                if (this.index != value)
                {
                    this.index = value;
                    RaisePropertyChanged(() => Index);
                }
            }
        }


        private string blockName;

        public string BlockName
        {
            get { return this.blockName; }
            set
            {
                if (this.blockName != value)
                {
                    this.blockName = value;
                    RaisePropertyChanged(() => BlockName);
                }
            }
        }

        private string md5Hash;

        public string MD5Hash
        {
            get { return this.md5Hash; }
            set
            {
                if (this.md5Hash != value)
                {
                    this.md5Hash = value;
                    RaisePropertyChanged(() => MD5Hash);
                }
            }
        }


        private string blockLocation;

        public string BlockLocation
        {
            get { return this.blockLocation; }
            set
            {
                if (this.blockLocation != value)
                {
                    this.blockLocation = value;
                    RaisePropertyChanged(() => BlockLocation);
                }
            }
        }

        private long size;

        public long Size
        {
            get { return this.size; }
            set
            {
                if (this.size != value)
                {
                    this.size = value;
                    RaisePropertyChanged(() => Size);
                }
            }
        }



    }
}
