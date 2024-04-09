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
    static internal class FrnExtension
    {
        internal static void UpdateIconIndex(this FrnFileOrigin frnFileOrigin)
        {

        }

        internal static void UpdateIconIndexAsync(this FrnFileOrigin frnFileOrigin)
        {

        }

        internal static void GetIconTargetPathOrExtent(FrnFileOrigin f, out string pathOrExten, out int index)
        {
            index = -1;
            var name = PathHelper.getfilePath(f.fileName).ToString();
            pathOrExten = string.Empty;

            if (f.IcoIndex != -1)
            {
                var path2 = PathHelper.GetPath(f).ToString();                
                return;
            }

            try
            {
                pathOrExten = StringUtils.GetExtension(name).ToString();
            }
            catch
            { 
            }

            if (pathOrExten.Length == 0)
            {
                pathOrExten = "";
                index = 3;
            }
            else if (pathOrExten.Equals(".exe", StringComparison.OrdinalIgnoreCase) || pathOrExten.Equals(".lnk", StringComparison.OrdinalIgnoreCase))
            {
                index = 0;
                pathOrExten = PathHelper.GetPath(f).ToString();
            }
            else
            {
                index= 0;                
            }
        }
    }
}
