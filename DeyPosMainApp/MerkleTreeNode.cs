using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp
{
    public class MerkleTreeNode
    {
        public MerkleTreeNode Left { get; set; }

        public MerkleTreeNode Right { get; set; }
        
        public string Hash { get; set; }

        public bool IsBlockNode { get; set; }

        public int Index { get; set; }

    }
}
