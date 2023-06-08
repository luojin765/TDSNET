using System;
using System.Text;
using TDSNET.Engine.Actions.USN;

namespace TDSNET.Engine.Utils
{
    class StringUtils
    {
        protected const ulong ROOT_FILE_REFERENCE_NUMBER = 0x5000000000005L;



        public static string GetPathStr(FrnFileOrigin f,string tailStr="")
        {
            string sb = "";
            if (f.parentFrn != null)
            {
                //尾递归
                tailStr = string.Concat("\\", tdsCshapu.Form1.getfile(f.fileName), tailStr);
                return GetPathStr(f.parentFrn, tailStr);
            }
            else
            {
                return string.Concat(f.VolumeName, ":",tailStr);
            }

            

        }



    }
}