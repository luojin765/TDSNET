using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TDSNET.Engine.Actions;
using TDSNET.Engine.Actions.USN;
using TDSNET.Engine.Utils;

namespace TDSNET.Utils
{
    static internal class PathHelper
    {
        static IFileHelper iFileHelper = new IFileHelper(null);

        public static IFileHelper IFileHelper { get => iFileHelper; }

        /// <summary>
        ///  二进制转换逻辑搜索使用
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        internal static ReadOnlySpan<char> getfilePath(ReadOnlySpan<char> filename)
        {
            var index1 = filename.IndexOf('|');
            if (index1 < 0) return ReadOnlySpan<char>.Empty;
            var filename2 = filename.Slice(index1 + 1, filename.Length - index1 - 1);

            var index2 = filename2.IndexOf('|');
            if (index1 < 0)
            {
                return filename2;
            }
            else
            {
                return filename2.Slice(0, index2).ToString();
            }

            //if (filename.Length > 0) { string[] fn = filename.ToString().Split('|'); if (fn.GetUpperBound(0) > 0) { return fn[1]; } }


        }


        internal static ReadOnlySpan<char> GetPath(FrnFileOrigin f)
        {
            var path = StringUtils.GetPathStr(f, ReadOnlySpan<char>.Empty);
            if (path.EndsWith(":".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                var pathChar = new char[path.Length + 1];
                Array.Copy(path.ToArray(), pathChar, path.Length);
                pathChar[pathChar.Length - 1] = '\\';
                return pathChar.AsSpan();
            }
            else
            {
                return path;
            }
        }
    }
}
