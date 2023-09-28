using System;
using System.Text;
using TDSNET.Engine.Actions.USN;

namespace TDSNET.Engine.Utils
{
    class StringUtils
    {
        protected const ulong ROOT_FILE_REFERENCE_NUMBER = 0x5000000000005L;



        public static ReadOnlySpan<char> GetPathStr(FrnFileOrigin f, ReadOnlySpan<char> tailStr)
        {
            if (f.parentFrn != null)
            {
                //尾递归

                tailStr = string.Concat("\\", tdsCshapu.Form1.getfile(f.fileName), tailStr).AsSpan();
                return GetPathStr(f.parentFrn, tailStr);
            }
            else
            {
                var path = new char[1+1+tailStr.Length];

                path[0] = f.VolumeName;
                path[1] = ':';
                Array.Copy(tailStr.ToArray(),0, path, 2, tailStr.Length);

                return path.AsSpan();
            }
        }

        public static ReadOnlySpan<char> GetExtension(ReadOnlySpan<char> fullPath)
        {
            var index = fullPath.IndexOf('.');
            if (index != -1 && index < fullPath.Length - 2)
            {
                var ext = fullPath.Slice(index, fullPath.Length - index);
                return ext;
            }
            else
            {
                return ReadOnlySpan<char>.Empty;
            }
        }


    }
}