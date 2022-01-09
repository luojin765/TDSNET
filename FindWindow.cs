using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;


namespace tdsCshapu
{
    class FindWindowS
    {

        const uint BM_CLICK = 0xF5;

        const uint WM_SETTEXT = 0xC;

        const uint WM_GETTEXT = 0xD;

        const uint BM_GETCHECK = 0xF0;

        const uint BM_SETCHECK = 0xF1;

        const uint BM_GETSTATE = 0xF2;

        const uint BM_SETSTATE = 0xF3;

        const uint BM_SETSTYLE = 0xF4;

        const uint WM_KEYDOWN = 0xF0100;

        const uint WM_KEYUP = 0xF0101;

        const uint EN_CHANGE = 0x300;

        const uint EN_UPDATE = 0x400;

        const uint BFFM_SETSTATUSTEXT = 0x468;

        const uint BFFM_SETSELECTION = 0x467;

        const uint BFFM_SELCHANGED = 0x2;

        const uint WM_SYSKEYDOWN = 0x104;

        const uint WM_CHAR = 0x0102;

        static List<IntPtr> EditHandle = new List<IntPtr>();

        static string[] keys = { "另存为", "浏览", "保存", "载入族", "导入/链接 RVT", "链接 CAD 格式", "导入 CAD 格式", "另存为...", "导入", "导出", "打开", "保存副本", "Open", "制作副本", "保存副本", "保存为", "存储为", "存储", "置入", "载入", "插入图片", "选择文件/文件夹", "请选择文件/文件夹", "导入文本文件", "Save", "Save as", "打开项目" };

        static string[] buttons = { "另存为(&S)", "保存(&S)", "打开(&O)", "存储为(&)", "置入(&P)", "插入(&S)", "载入(&L)", "&Open", "&Save", "发送(&S)", "存入百度网盘" };

        private delegate bool EnumChildProc(IntPtr hWnd, int lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, uint hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, uint wMsg, int wParam, string lParam);

        [DllImport("user32.dll", EntryPoint = "GetClassName", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hwnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "EnumChildWindows", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool EnumChildWindows(IntPtr hwnd, EnumChildProc wMsg, int lParam);
        static public bool EnumChildProcC(IntPtr hwnd, int lParam)
        {
            StringBuilder dwWindowClass = new StringBuilder(100);
            GetClassName(hwnd, dwWindowClass, 100);

            //   MessageBox.Show(dwWindowClass.ToString());
            if (dwWindowClass.ToString().Contains("EDIT") || dwWindowClass.ToString().Contains("Edit"))
            {


                EditHandle.Add(hwnd);
            }
            return true;
        }







        static public bool testWindow()
        {
            foreach (string k in keys)
            {

                EditHandle.Clear();

                IntPtr msgHandle = FindWindow(null, k);

                if (msgHandle != IntPtr.Zero)
                {
                    
                        EnumChildWindows(msgHandle, EnumChildProcC, 0);
                   
                   
                    //   MessageBox.Show(EditHandle.Count.ToString());
                    if (EditHandle.Count > 0)
                    {
                        
                            foreach (string b in buttons)
                            {
                                IntPtr btnHandle = FindWindowEx(msgHandle, 0, null, b);
                                if (btnHandle != IntPtr.Zero)
                                {
                                    return true;

                                }
                            }
                        
                       
                    }
                }
            }
            return false;
        }

        static public void FindWindow(string path)
        {
            foreach (string k in keys)
            {

                EditHandle.Clear();

                IntPtr msgHandle = FindWindow(null, k);

                if (msgHandle != IntPtr.Zero)
                {


                   
                        EnumChildWindows(msgHandle, EnumChildProcC, 0);
                  




                    //   MessageBox.Show(EditHandle.Count.ToString());
                   
                  
                    if (EditHandle.Count > 0)
                    {

                        foreach (IntPtr h in EditHandle)
                        {
                           
                                SendMessage(h, WM_CHAR, 32, null);
                                //     SendMessage(h, EN_CHANGE, 0, null);
                                SendMessage(h, WM_SETTEXT, 0, path);
                            

                        }

                        foreach (string b in buttons)
                        {
                            IntPtr btnHandle = FindWindowEx(msgHandle, 0, null, b);


                            if (btnHandle != IntPtr.Zero)
                            {
                               
                                    SendMessage(btnHandle, BM_CLICK, 0, null);

                                    foreach (IntPtr h in EditHandle)
                                    {

                                        SendMessage(h, WM_SETTEXT, 0, null);
                                    }
                                
                                return;


                            }
                        }
                    }
                   
                }
            }

        }

    }
}