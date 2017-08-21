using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp.HAT
{
    public class HATNode<T> : Node<T>
    {
        public int FileBlockIndex { get; set; }

        public int Index { get; set; }

        public int LeafNodesCount { get; set; }

        public int Version { get; set; }

        public string Tag { get; set; }

        public string Hash { get; set; }

        public bool IsBlockNode {get; set;}

        public HATNode() : base() { }
        public HATNode(T data) : base(data, null) { }
        public HATNode(T data, HATNode<T> left, HATNode<T> right)
        {
            base.Value = data;
            NodeList<T> children = new NodeList<T>(2);
            children[0] = left;
            children[1] = right;

            base.Neighbors = children;
        }

        public HATNode<T> Left
        {
            get
            {
                if (base.Neighbors == null)
                    return null;
                else
                    return (HATNode<T>)base.Neighbors[0];
            }
            set
            {
                if (base.Neighbors == null)
                    base.Neighbors = new NodeList<T>(2);

                base.Neighbors[0] = value;
            }
        }

        public HATNode<T> Right
        {
            get
            {
                if (base.Neighbors == null)
                    return null;
                else
                    return (HATNode<T>)base.Neighbors[1];
            }
            set
            {
                if (base.Neighbors == null)
                    base.Neighbors = new NodeList<T>(2);

                base.Neighbors[1] = value;
            }
        }
    }
}
