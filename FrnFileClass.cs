using System;
using System.IO;
using System.Linq;

namespace QueryEngine
{
    
    public class FrnFileOrigin//  : IComparable<FrnFileOrigin>
    {
        private string volumeName; //根目录名称
        public UInt64 fileReferenceNumber;
        public FrnFileOrigin parentFrn = null;

        public UInt64 keyindex;
        private string fileName = "";
        public int? IcoIndex;
        public bool orderFirst=false;
        
        public string FileName 
        { get => fileName; 
            
            set
            {              
                fileName = string.Intern(value);                
            }
        }
        public string VolumeName { 
            get {
                return volumeName;
            }
            set
            {
                volumeName = string.Intern(value.Trim('\\').Trim(':'));
            }
        }

        public FrnFileOrigin(UInt64 fileReferenceNumber, string fileName)
        {
            this.fileReferenceNumber = fileReferenceNumber;
            this.FileName = fileName;
        }               

   }

    public sealed class FrnFileFull: FrnFileOrigin
    {
        internal  UInt64? parentFileReferenceNumber = null;
              
        public FrnFileFull(UInt64 fileReferenceNumber, UInt64? parentFileReferenceNumber, string fileName)
                : base(fileReferenceNumber, fileName)
        {
            this.parentFileReferenceNumber = parentFileReferenceNumber;
        }
    }
}
