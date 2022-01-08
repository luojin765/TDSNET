using TDS.FST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDS.Controller
{
    public class FSTController
    {
        Dictionary<char, FSTNodes> fstRoots = new Dictionary<char, FSTNodes>();
        Dictionary<char, FSTNodes> fstEnds = new Dictionary<char, FSTNodes>();
        Collection<FSTs> fstCollection = new Collection<FSTs>();


        public IEnumerable Search(string key)
        {
            string[] keys = key.Split(' ');
            IEnumerable<FSTNodes> fsts = null;
            foreach (string k in keys)
            {
                if (fsts == null)
                {
                    fsts = Screen(k);
                }
                else if (fsts.Count() == 0)
                {
                    return fsts;
                }
                else
                {
                    fsts = fsts.Intersect(Screen(k));
                }
            }
            return fsts;

        }


        private ICollection<FSTNodes> Screen(string keyStr)
        {
            ICollection<FSTNodes> fsts = new Collection<FSTNodes>();
            foreach (FSTNodes fst in Screen(keyStr[0]))
            {
                fst.StrMatch(new StringBuilder(keyStr), ref fsts);
            }
            return fsts;

        }

        private IEnumerable Screen(char key)
        {
            foreach (FSTNodes fst in fstCollection)
            {
                if (fst.KeyChar == key)
                {
                    yield return fst;
                }
            }
        }


        public IReadOnlyDictionary<char, FSTNodes> FSTRoots { get => fstRoots; }
        public IReadOnlyDictionary<char, FSTNodes> FstEnds { get => fstEnds; }

        public void AddStrFST(string str)
        {
            char[] chars = str.ToCharArray();

            FSTs parent = null;

            if (chars.Length > 0)
            {


                for (int i = 0; i < chars.Length; i++)
                {
                    if (i == 0)
                    {
                        if (!fstRoots.ContainsKey(chars[0]))
                        {
                            fstRoots.Add(chars[0], new FSTNodes(chars[0]));
                        }
                        parent = fstRoots[chars[0]];

                    }
                    else
                    {
                        parent = parent.AddChild(new FSTNodes(chars[i]));
                    }

                    if (!fstCollection.Contains(parent))
                    {
                        fstCollection.Add(parent);
                    }
                }

                parent.SetEnd();
                if (!fstEnds.ContainsKey(chars[0]))
                {
                    fstEnds.Add(chars[0], new FSTNodes(chars[0]));
                }

            }
        }
    }
}
