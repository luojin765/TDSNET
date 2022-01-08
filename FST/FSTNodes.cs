using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDS.FST
{
    public class FSTNodes : FSTs
    {
        public FSTNodes(char key) : base(key)
        {
        }

        public void StrMatch(StringBuilder keyString, ref ICollection<FSTNodes> collection, int charNum = 0)
        {
            if (charNum < keyString.Length && keyString[charNum] != KeyChar)
            {
                return;
            }
            else
            {
                if (IsEndNode)
                {
                    collection.Add(this);
                }

                if (childNodes.Count > 0)
                {
                    foreach (FSTNodes fst in childNodes.Values)
                    {
                        fst.StrMatch(keyString, ref collection, ++charNum);
                    }
                }
            }
        }
    }
}
