using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDS.FST
{
    public abstract class FSTs
    {
        protected const int INVALIDNODE = 0;
        protected const int ROOTNODE = 1;
        protected const int NORMALNODE = 2;
        protected bool IsEndNode = false;

        public void SetEnd()
        {
            IsEndNode = true;
        }
        protected void SetNotEnd()
        {
            IsEndNode = false;
        }

        char keyChar;

        protected short state;
        public short State { get => state; }
        public char KeyChar { get => keyChar; }

        public FSTs(char key)
        {
            keyChar = key;
        }
        private FSTs parentNode;

        protected Dictionary<char, FSTs> childNodes = new Dictionary<char, FSTs>();

        public FSTs AddChild(FSTs child)
        {
            if (!childNodes.ContainsKey(child.KeyChar))
            {
                childNodes.Add(child.KeyChar, child);
                child.parentNode = this;
            }
            return childNodes[child.KeyChar];
        }

        public static ICollection<FSTs> GetAllEnds(FSTs lyfst)
        {
            ICollection<FSTs> allEnds = new Collection<FSTs>();
            YieldEnd(lyfst, ref allEnds);
            return allEnds;
        }

        static void YieldEnd(FSTs lyFST, ref ICollection<FSTs> collection)
        {
            if (lyFST.IsEndNode)
            {
                collection.Add(lyFST);
            }
            if (lyFST.childNodes.Count > 0)
            {
                foreach (FSTs ly in lyFST.childNodes.Values)
                {
                    YieldEnd(ly, ref collection);
                }
            }
        }

        static string YieldString(FSTs fst)
        {
            if (fst.parentNode == null)
            {
                return fst.KeyChar.ToString();
            }
            else
            {
                return YieldString(fst.parentNode) + fst.KeyChar;

            }
        }

        public string RetriveFull()
        {
            return YieldString(this);
        }

    }
}
