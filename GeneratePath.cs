using System;
using System.Linq;

namespace QueryEngine
{
    //内存测试用
    //public class FilePath
    //{
    //    public UInt64 fileReferenceNumber;
    //    public UInt64? parentFileReferenceNumber;
    //    public UInt64 keyindex;
    //    public string fileName;
    //   public FilePath(UInt64 fileReferenceNumber, UInt64? parentFileReferenceNumber)
    //  {
    //      this.fileReferenceNumber = fileReferenceNumber;
    // this.parentFileReferenceNumber = parentFileReferenceNumber;
    //  this.fileName = fileName;
    //  }
    //}
    public class FrnFilePath
    {
        public char VolumeName; //根目录名称
        public short Volume;  //根目录判断
        public short weight;
        public UInt64 fileReferenceNumber;
        public FrnFilePath parentFrn=null;
        public UInt64? parentFileReferenceNumber;
        public UInt64 keyindex;
        public string fileName = "";
        public string path;  //根目录路径
        public int? IcoIndex;
        //  public string exten;  //扩展名
        public Int64 timestamp=long.MinValue;        

        public void SetVolandVolName(int vol, string volname)
        {
            this.Volume = (short)vol;
            this.VolumeName = volname.ToArray()[0];
        }


        public FrnFilePath(UInt64 fileReferenceNumber, UInt64? parentFileReferenceNumber, string fileName, string path = null)
        {
            this.fileReferenceNumber = fileReferenceNumber;
            this.parentFileReferenceNumber = parentFileReferenceNumber;
            this.fileName = fileName;
            this.path = path;
        }
    }
}
