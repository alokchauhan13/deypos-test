using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Common;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.DataFile;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp
{

    public class MerkleTree
    {

        public MerkleTreeNode Root { get; set; }

        int count = 0;

        public List<string> Logs { get; set; }

        List<MerkleTreeNode> AllNodes { get; set; }

        List<MerkleTreeNode> AllNodesTemp { get; set; }

        public MerkleTree()
        {
            Root = null;
            AllNodes = new List<MerkleTreeNode>();
            AllNodesTemp = new List<MerkleTreeNode>();
        }



        public void CreateTree(List<FileBlock> fileBlocks)
        {
            fileBlocks = fileBlocks.OrderBy(x => x.Index).ToList();

            for (int i = 0; i < fileBlocks.Count; i = i + 2)
            {
                MerkleTreeNode left = new MerkleTreeNode();
                left.Hash = fileBlocks[i].ContentHash;
                left.IsBlockNode = true;
                left.Index = i;

                MerkleTreeNode right = new MerkleTreeNode();
                if (i + 1 < fileBlocks.Count)
                {
                    right.Hash = fileBlocks[i + 1].ContentHash;
                    right.IsBlockNode = true;
                    right.Index = i + 1;
                }

                MerkleTreeNode parent = new MerkleTreeNode();

                parent.Left = left;
                parent.Right = right;
                parent.IsBlockNode = false;
                parent.Index = -1;

                if (i + 1 < fileBlocks.Count)
                {
                    parent.Hash = Utility.ComputeHashAsString(left.Hash + right.Hash);

                }
                else
                {
                    parent.Hash = Utility.ComputeHashAsString(left.Hash);
                }

                AllNodes.Add(parent);
            }

            CreateTree();
        }


        private void CreateTree()
        {
            if (AllNodes.Count <= 1)
            {
                Root = AllNodes[0];
                return;
            }
            else
            {
                AllNodesTemp = new List<MerkleTreeNode>();

                for (int i = 0; i < AllNodes.Count; i = i + 2)
                {


                    MerkleTreeNode parent = new MerkleTreeNode();
                    parent.Left = AllNodes[i];
                    if (i + 1 < AllNodes.Count)
                    {
                        parent.Right = AllNodes[i + 1];
                    }
                    parent.IsBlockNode = false;
                    parent.Index = -1;

                    if (i + 1 < AllNodes.Count)
                    {
                        parent.Hash = Utility.ComputeHashAsString(AllNodes[i].Hash + AllNodes[i + 1].Hash);
                    }
                    else
                    {
                        parent.Hash = Utility.ComputeHashAsString(AllNodes[i].Hash);
                    }


                    AllNodesTemp.Add(parent);
                }

                AllNodes = AllNodesTemp;
                CreateTree();
            }
        }

        List<MerkleTreeNode> preOrderedNodes;

        public List<MerkleTreeNode> PreOrderTraversal()
        {
            preOrderedNodes = new List<MerkleTreeNode>();
            PreOrder(Root);
            return preOrderedNodes;
        }

        private void PreOrder(MerkleTreeNode node)
        {
            if (node == null)
                return;
            preOrderedNodes.Add(node);
            PreOrder(node.Left);
            PreOrder(node.Right);
        }

    }

}
