using DocumentFormat.OpenXml.Office2021.Excel.NamedSheetViews;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TDSNET;
using TDSNET.UI;

namespace tdsCshapu
{
    static class Program
    {

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {     
            bool flag = false;
            System.Threading.Mutex hMutex = new System.Threading.Mutex(true, Application.ProductName, out flag);
            
            //waitOne可以删除，因为无需考虑资源等待，仅判定是否新建即可
            bool b = hMutex.WaitOne(0, false);

            if(!flag)
            {
                //获取欲启动进程名
                string strProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                ////获取版本号 
                //CommonData.VersionNumber = Application.ProductVersion; 
                //检查进程是否已经启动，已经启动则显示报错信息退出程序。 
                var p = System.Diagnostics.Process.GetProcessesByName(strProcessName);
                if (p?.Length > 1)
                {
                    PostThreadMessage(p[0].Threads[0].Id, 0x010, IntPtr.Zero, IntPtr.Zero);
                    Application.Exit();
                    Environment.Exit(0);
                    return;
                }
            }

    

            //处理未捕获的异常   
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常   
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            //处理非UI线程异常   
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);


            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new Form1();
            Application.AddMessageFilter(new MsgRecev(() => form.autoshoworhide()));

            Application.Run(form);
        }
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string str = "";
            Exception error = e.Exception as Exception;
            if (error != null)
            {
                str = string.Format("UI出现应用程序未处理的异常\n异常类型：{0}\n异常消息：{1}\n异常位置：{2}\n",
                     error.GetType().Name, error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("应用程序线程错误:{0}", e);
            }
            try { Log_write(str); } catch { };

          //重启
          //  System.Diagnostics.Process.Start(Application.ExecutablePath);
            Application.ExitThread();
            Application.Exit();
            System.Environment.Exit(0);
        }

                
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostThreadMessage(int threadId, uint msg, IntPtr wParam, IntPtr lParam);

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = "";
            Exception error = e.ExceptionObject as Exception;
            if (error != null)
            {
                str = string.Format(",非UI Application UnhandledException:{0};\n堆栈信息:{1}", error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("Application UnhandledError:{0}", e);
            }
            try { Log_write(str); } catch { };
            
            
            //重启;
            //System.Diagnostics.Process.Start(Application.ExecutablePath);

            Application.ExitThread();
            Application.Exit();
            System.Environment.Exit(0);
        }
         static void Log_write(string log)
        {
            string path = Application.StartupPath + "\\" + "Sys.log";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (System.IO.StreamWriter fs = new System.IO.StreamWriter(path, true, System.Text.Encoding.GetEncoding("gb2312")))
            {

                fs.WriteLine(DateTime.Now.ToString() + log);

                fs.Close();
            }
        }
    }
}
