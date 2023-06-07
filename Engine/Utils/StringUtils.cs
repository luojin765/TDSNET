using System;
using System.Text;
using TDSNET.Engine.Actions.USN;

namespace TDSNET.Engine.Utils
{
    class StringUtils
    {
        protected const ulong ROOT_FILE_REFERENCE_NUMBER = 0x5000000000005L;



        public static string GetPathStr(FrnFileOrigin f)
        {
            string sb = "";
            if (f.parentFrn != null)
            {
                sb = string.Concat(GetPathStr(f.parentFrn), "\\", tdsCshapu.Form1.getfile(f.fileName));
            }
            else
            {
                sb = f.VolumeName + ":";
            }

            return sb;

        }



    }
}