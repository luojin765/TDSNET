using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using TDSNET.Engine.Actions.USN;
using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Threading;

namespace TDSNET.Engine.Actions
{

    public class IFileHelper
    {
        public static uint SHGFI_ICON = 0x100;
        public static uint SHGFI_DISPLAYNAME = 0x200;
        public static uint SHGFI_TYPENAME = 0x400;
        public static uint SHGFI_ATTRIBUTES = 0x800;
        public static uint SHGFI_ICONLOCATION = 0x1000;
        public static uint SHGFI_EXETYPE = 0x2000;
        public static uint SHGFI_SYSICONINDEX = 0x4000;
        public static uint SHGFI_LINKOVERLAY = 0x8000;
        public static uint SHGFI_SELECTED = 0x10000;
        public static uint SHGFI_LARGEICON = 0x0;
        public static uint SHGFI_SMALLICON = 0x1;
        public static uint SHGFI_OPENICON = 0x2;
        public static uint SHGFI_SHELLICONSIZE = 0x4;
        public static uint SHGFI_PIDL = 0x8;
        public static uint SHGFI_USEFILEATTRIBUTES = 0x10;
        public static uint FILE_ATTRIBUTE_NORMAL = 0x80;
        public static uint LVM_FIRST = 0x1000;
        public static uint LVM_SETIMAGELIST = LVM_FIRST + 3;
        public static uint LVSIL_NORMAL = 0;
        public static uint LVSIL_SMALL = 1;
        [DllImport("Shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath,
    uint dwFileAttributes, ref SHFILEINFO psfi,
    int cbfileInfo, uint uFlags);
        [DllImport("User32.DLL")]
        public static extern int SendMessage(IntPtr hWnd,
        uint Msg, IntPtr wParam, IntPtr lParam);

        static public void ListViewSysImages(ListView AListView)
        {
            SHFILEINFO vFileInfo = new SHFILEINFO();
            //IntPtr 
            //vImageList = SHGetFileInfo("", 0, ref vFileInfo,
            //Marshal.SizeOf(vFileInfo), SHGFI_SHELLICONSIZE |
            //SHGFI_SYSICONINDEX | SHGFI_LARGEICON);
            //SendMessage(AListView.Handle, LVM_SETIMAGELIST, (IntPtr)LVSIL_NORMAL,
            //vImageList);

            // 不要尺寸 sysiconsize
            IntPtr vImageList = SHGetFileInfo("", 0, ref vFileInfo,
        Marshal.SizeOf(vFileInfo), SHGFI_SYSICONINDEX | SHGFI_SMALLICON);  //SMALLICON改large
            SendMessage(AListView.Handle, LVM_SETIMAGELIST, (IntPtr)LVSIL_SMALL,
            vImageList);

        }
        internal IFileHelper(Action callback)
        {
            try
            {
                this.callback = callback;
                //多并发，需要线程池控制
                ThreadPool.QueueUserWorkItem(IconWork);
            }
            catch (Exception ex)
            {
                throw new Exception("日志初始化异常:" + ex.Message);
            }
        }

        Action? callback=null;

        static private readonly Channel<(string,FrnFileOrigin)> buffer = Channel.CreateUnbounded<(string, FrnFileOrigin)>(new UnboundedChannelOptions() { SingleReader = true });


        static ConcurrentDictionary<string, int> iconCache = new ConcurrentDictionary<string, int>();


        public void FileIconIndexAsync(string AFileName,FrnFileOrigin frnFileOrigin)
        {
            buffer.Writer.WriteAsync((AFileName, frnFileOrigin));
        }

        async void IconWork(object? _)
        {
            while (await buffer.Reader.WaitToReadAsync())
            {
                while (buffer.Reader.TryRead(out (string, FrnFileOrigin) logContent))
                {
                    FileIconIndexWork(logContent.Item1,logContent.Item2);
                }

                callback?.Invoke();
            }
        }

        static private void FileIconIndexWork(string AFileName,FrnFileOrigin frnFileOrigin)
        {
            string exten = Path.GetExtension(AFileName);

            if (string.IsNullOrWhiteSpace(exten) || string.IsNullOrWhiteSpace(AFileName))
            {
                frnFileOrigin.IcoIndex= 0;
            }

            if (iconCache.TryGetValue(exten, out int index))
            {
                frnFileOrigin.IcoIndex = index;
            }
            else
            {
            
                frnFileOrigin.IcoIndex = FileIconIndex(AFileName, exten);
            }
        }        

        private static int FileIconIndex(string AFileName,string exten)
        {
            try
            {
                SHFILEINFO vFileInfo = new SHFILEINFO();
                IntPtr pSH = SHGetFileInfo(AFileName, 0, ref vFileInfo,
                Marshal.SizeOf(vFileInfo), SHGFI_SYSICONINDEX | SHGFI_USEFILEATTRIBUTES);
                if (pSH != IntPtr.Zero)
                {
                    if (!string.Equals(exten,".exe",StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(exten, ".lnk", StringComparison.OrdinalIgnoreCase))
                    {
                        iconCache.TryAdd(exten, vFileInfo.iIcon);
                    }
                    return vFileInfo.iIcon;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public int dwAttributes;
            public string szDisplayName;
            public string szTypeName;
        }
    }
}
