using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Common;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.DataFile;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp.HAT
{
    class HATTree<T>
    {

        List<HATNode<T>> AllNodes { get; set; }

        public HATTree()
        {
            AllNodes = new List<HATNode<T>>();
        }

        public HATNode<T> Root
        {
            get;
            set;
        }


        public void CreateTree(List<FileBlock> fileBlocks)
        {
            AllNodes.Clear();

            List<HATNode<T>> AllParentsNodeTemp = new List<HATNode<T>>();

            ServerBob server = new ServerBob();
            AliceUser user = new AliceUser();

            server.User = user;
            user.Server = server;
            server.setKeysWithUser();
            


            fileBlocks = fileBlocks.OrderBy(x => x.Index).ToList();

            for (int i = 0; i < fileBlocks.Count; i = i + 2)
            {
                HATNode<T> left = new HATNode<T>();
                left.Hash = fileBlocks[i].ContentHash;
                left.Tag = user.EncryptMessage(left.Hash);
                left.IsBlockNode = true;
                left.FileBlockIndex = i;
                left.Version = 1;
                AllNodes.Add(left);

                HATNode<T> right = new HATNode<T>();
                if (i + 1 < fileBlocks.Count)
                {
                    right.Hash = fileBlocks[i + 1].ContentHash;
                    right.Tag = user.EncryptMessage(right.Hash);
                    right.IsBlockNode = true;
                    right.FileBlockIndex = i + 1;
                    right.Version = 1;
                    AllNodes.Add(right);
                }

                HATNode<T> parent = new HATNode<T>();

                parent.Left = left;
                parent.Right = right;
                parent.IsBlockNode = false;
                parent.FileBlockIndex = -1;

                if (i + 1 < fileBlocks.Count)
                {
                    parent.Hash = Utility.ComputeHashAsString(left.Hash + right.Hash);
                    
                }
                else
                {
                    parent.Hash = Utility.ComputeHashAsString(left.Hash);
                }
                
                AllNodes.Add(parent);
                AllParentsNodeTemp.Add(parent);
            }
            CreateTree(AllParentsNodeTemp);
            SetNodeIndexes();
            SetLeafNodeCount();
        }

        private void CreateTree(List<HATNode<T>> nodes )
        {
            if (nodes.Count <= 1)
            {
                Root = nodes[0];
                return;
            }
            else
            {
                List<HATNode<T>>  AllNodesTemp = new List<HATNode<T>>();

                for (int i = 0; i < nodes.Count; i = i + 2)
                {


                    HATNode<T> parent = new HATNode<T>();
                    parent.Left = nodes[i];

                    if (i + 1 < nodes.Count)
                    {
                        parent.Right = nodes[i + 1];
                    }

                    parent.IsBlockNode = false;
                    parent.FileBlockIndex = -1;
                    parent.Version = 0;

                    if (i + 1 < nodes.Count)
                    {
                        parent.Hash = Utility.ComputeHashAsString(nodes[i].Hash + nodes[i + 1].Hash);
                    }
                    else
                    {
                        parent.Hash = Utility.ComputeHashAsString(nodes[i].Hash);
                    }

                    AllNodes.Add(parent);
                    AllNodesTemp.Add(parent);
                }

                CreateTree(AllNodesTemp);
            }
        }



        List<HATNode<T>> preOrderedNodes;

        public List<HATNode<T>> PreOrderTraversal()
        {
            preOrderedNodes = new List<HATNode<T>>();
            PreOrder(Root);
            return preOrderedNodes;
        }

        private void PreOrder(HATNode<T> node)
        {
            if (node == null)
                return;
            preOrderedNodes.Add(node);
            PreOrder(node.Left);
            PreOrder(node.Right);
        }


        private void SetNodeIndexes()
        {
            List<HATNode<T>> preOrderedNodes = PreOrderTraversal();
            int index = 1;
            foreach (HATNode<T> item in preOrderedNodes)
            {
                item.Index = index++;
                if (item.IsBlockNode == true)
                {
                    item.Version = 1;
                }
            }
        }

        private void SetLeafNodeCount()
        {
            foreach (var item in this.AllNodes)
            {
                item.LeafNodesCount = getLeafCount(item);
            }
        }

        private int getLeafCount(HATNode<T> node)
        {
            if (node == null)
                return 0;

            if (node.Left == null && node.Right == null)
                return 1;
            else
            {
                return getLeafCount(node.Left) +
                 getLeafCount(node.Right);
            }
        }
    }
}
