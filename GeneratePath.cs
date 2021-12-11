using System;
using System.IO;
using System.Linq;

namespace QueryEngine
{
    
    public class FrnFileOrigin
    {
        public char VolumeName; //根目录名称
        public UInt64 fileReferenceNumber;
        public FrnFileOrigin parentFrn = null;

        public UInt64 keyindex;
        public string fileName = "";
        public int? IcoIndex;
        public Int64 timestamp = long.MinValue;


        public void SetVolandVolName(string volname)
        {
            //this.Volume = (short)vol;
            this.VolumeName = volname.ToArray()[0];
        }

        public short weight;
        public FrnFileOrigin(UInt64 fileReferenceNumber, string fileName)
        {
            this.fileReferenceNumber = fileReferenceNumber;
            this.fileName = fileName;
        }               

   }

    public class FrnFileFull: FrnFileOrigin
    {
        internal  UInt64? parentFileReferenceNumber = null;
        
        internal FrnFileOrigin CompressToOrigin()
        {
            FrnFileOrigin fi= new FrnFileOrigin(fileReferenceNumber, fileName);
            
        fi.VolumeName=VolumeName; //根目录名称
        fi.parentFrn = parentFrn;
        fi.keyindex=keyindex;
        fi.IcoIndex = IcoIndex;
        fi.timestamp = timestamp;
        return fi;
    }

        public FrnFileFull(UInt64 fileReferenceNumber, UInt64? parentFileReferenceNumber, string fileName)
                : base(fileReferenceNumber, fileName)
        {
            this.parentFileReferenceNumber = parentFileReferenceNumber;
        }
    }
}
