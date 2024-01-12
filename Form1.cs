using FileContentSearch;
using FileSync;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TDSNET;
using TDSNET.CompareSameFiles;
using TDSNET.Engine.Actions;
using TDSNET.Engine.Actions.USN;
using TDSNET.Engine.Utils;
using TDSNET.UI;



namespace tdsCshapu
{
    public partial class Form1 : Form
    {
        static readonly ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 4 };
        static readonly ParallelOptions parallelOptionsTwo = new ParallelOptions() { MaxDegreeOfParallelism = 2 };

        private const int MAX_PATH = 260;

        private const int CSIDL_COMMON_PROGRAMS = 0x0017;

        const int HTLEFT = 10;

        //[DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        //static extern int ShowWindow(IntPtr hWnd, uint nCmdShow);
        const int HTRIGHT = 11;

        //private static extern bool SetForegroundWindow(IntPtr hWnd);
        const int HTTOP = 12;

        // [DllImport("user32.dll")]   //0512
        const int HTTOPLEFT = 13;

        const int HTTOPRIGHT = 14;

        const int HTBOTTOM = 15;

        const int HTBOTTOMLEFT = 0x10;

        const int HTBOTTOMRIGHT = 17;

        //listview 绑定
        static string keyword = string.Empty;

        static Thread GoSearch;

        AutoResetEvent gOs = new AutoResetEvent(false);

        //全局化线程方便控制
        static Thread usnJournalThread;


        readonly int mywidth = Screen.PrimaryScreen.Bounds.Width * 1 / 2;

        string USER_PROGRAM_PATH = "";//获取环境目录变量USER
        string ALLUSER_PROGRAM_PATH = "";   //获取环境目录变量ALLUSER
        List<FrnFileOrigin> vlist = new List<FrnFileOrigin>(500);

        //listview 绑定
        List<FrnFileOrigin> Record = new List<FrnFileOrigin>() { };

        private ListViewItem[] CurrentCacheItemsSource;

        private int firstitem = 0;

        string key1, key2;

        //信号
        bool Threadrunning = false;

        //线程运行标识
        bool Threadrest = false;

        bool IsActivated;

        //线程重启标识
        bool LocatingWin;

        //全局化线程方便控制
        bool Txtselected = false;

        //重复词组输入判断
        bool initialFinished = false;

        //全局关键词
        bool CUSTOMHIDE = true;

        List<FileSys> fileSysList = new List<FileSys>();

        bool isAll = false;

        bool IFShowR = true;

        int txtH = 42;

        bool Clickdown = false;

        int mouseX = 0, mouseY = 0;

        private ListViewItem prItem;

        System.Drawing.Point XY = new System.Drawing.Point();

        public static Color Mdeep = Color.FromArgb(58, 64, 80);
        public static Color Hdeep = Color.FromArgb(48, 50, 63);
        public static Color Ldeep = Color.FromArgb(87, 97, 121);
        public static Color SHALLOW = Color.FromArgb(211, 82, 48);
        public static Color HIGHLIGHT = Color.FromArgb(233, 233, 233);
        //筛选字符的个数

        //记录相关


        public static Keys HOTK_SHOW = Keys.OemPeriod;
        public static int findmax = 100;  //最大显示数量
        public bool DoUSNupdate = false;
        public bool ForbidUSNupdate = false;

        public bool ifhide = true;

        public bool refcache = false;


        class tmp
        {

            string a;
            string b;
        }
        public Form1()
        {

            InitializeComponent();
            istView1.VirtualListSize = 0;
            IFileHelper.ListViewSysImages(istView1);
            SetDoubleBuffering(istView1, true);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //挂载消息
            this.IFileHelper = new IFileHelper(null);
        }

        void refreshCallback()
        {
            if (tempCacheEventArgs != null)
            {
                Thread.Sleep(500);
                this.Invoke(() =>
                {

                    IstView1_CacheVirtualItems(istView1, tempCacheEventArgs);
                    ListView1_RetrieveVirtualItem(istView1, tempRetrieveVirtualItemEventArgs);


                    istView1.Invalidate();
                });
            }

        }


        IFileHelper IFileHelper = null;
        //#region 获取所有用户文件夹
        [DllImport("shfolder.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);

        static void Log_write(string log)
        {
            string path = Application.StartupPath + "\\" + "Sys.log";
            // System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (System.IO.StreamWriter fs = new System.IO.StreamWriter(path, true, System.Text.Encoding.GetEncoding("gb2312")))
            {

                fs.WriteLine(DateTime.Now.ToString() + log);

                fs.Close();
            }
        }

        [DllImport("shell32.dll", ExactSpelling = true)]
        private static extern void ILFree(IntPtr pidlList);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern IntPtr ILCreateFromPathW(string pszPath);

        [DllImport("shell32.dll", ExactSpelling = true)]
        private static extern int SHOpenFolderAndSelectItems(IntPtr pidlList, uint cild, IntPtr children, uint dwFlags);

        static ReadOnlySpan<char> GetPath(FrnFileOrigin f)
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

        private void ESCPress()
        {
            Keywords.Focus();
            if (Keywords.Text.Length > 0)
            {
                if ((Txtselected == false))
                {
                    Keywords.SelectAll();
                    Txtselected = true;

                }
                else
                {
                    Txtselected = false;

                    istView1.SelectedIndices.Clear();
                    Keywords.Clear();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.istView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;//不显示表头
            Initial();



        }

        private void ListFilesThreadStart()
        {




            String msg = string.Empty;


            int dri_nums = -1;  //盘数


            fileSysList.Clear();
            foreach (DriveInfo driInfo in GetAllFixedNtfsDrives())
            {
                fileSysList.Add(new FileSys(driInfo));
            }

            //获取环境变量及程序文件夹
            USER_PROGRAM_PATH = System.Environment.GetFolderPath(Environment.SpecialFolder.Programs, Environment.SpecialFolderOption.None);
            ALLUSER_PROGRAM_PATH = GetAllUsersDesktopFolderPath();

            dri_nums = fileSysList.Count();

            if (dri_nums > 0)
            {
                int totalcount = 0;

                //DateTime startTime= DateTime.Now;


                ConcurrentDictionary<char, char> SpellDict = new ConcurrentDictionary<char, char>();


                List<Task> tasks = new List<Task>();

                foreach (FileSys fs in fileSysList)
                {
                    tasks.Add(new Task(() =>
                    {

                        msg = ("建立索引..(" + fs.driveInfo.Name.TrimEnd('\\').TrimEnd(':') + "盘)");
                        label1.BeginInvoke(new StatusInfo(ShowStatuesInfo), msg);

                        fs.ntfsUsnJournal = new NtfsUsnJournal(fs.driveInfo);

                        fs.usnStates = new Win32Api.USN_JOURNAL_DATA();
                        if (!fs.SaveJournalState())
                        {

                            fs.ntfsUsnJournal.CreateUsnJournal(1000 * 1024, 16 * 1024);  //尝试重建USN
                            if (!fs.SaveJournalState())
                            {
                                MessageBox.Show("文件读取重建失败");
                            }
                        }
                        fs.CreateFiles();

                        //重整parent索引
                        foreach (FrnFileOrigin ffull in fs.files.Values)
                        {
                            FrnFileOrigin f = ffull as FrnFileOrigin;
                            if (f.additionInfo.parentFileReferenceNumber.HasValue && fs.files.ContainsKey(f.additionInfo.parentFileReferenceNumber.Value))
                            {
                                if (f.parentFrn == null)
                                {
                                    f.parentFrn = fs.files[f.additionInfo.parentFileReferenceNumber.Value];
                                }
                            }
                        }


                        Parallel.ForEach(fs.files.Values, parallelOptionsTwo, f =>
                        {
                            string nacn = SpellCN.GetSpellCode(f.fileName, SpellDict);
                            f.keyindex = FileSys.TBS(nacn);
                            if (!string.Equals(nacn, f.fileName, StringComparison.OrdinalIgnoreCase))
                            {
                                f.fileName = string.Intern(string.Concat("|", f.fileName, "|", nacn, "|"));
                            }
                            else
                            {
                                f.fileName = string.Intern(string.Concat("|", f.fileName, "|"));
                            }

                        });

                        Parallel.ForEach(fs.files.Values, parallelOptions, f =>
                        {
                            string ext = StringUtils.GetExtension(getfile(f.fileName)).ToString();

                            if (string.Equals(ext, ".LNK", StringComparison.OrdinalIgnoreCase))
                            {
                                var path = GetPath(f);
                                if (path.IndexOf(USER_PROGRAM_PATH, StringComparison.OrdinalIgnoreCase) != -1 || path.IndexOf(ALLUSER_PROGRAM_PATH, StringComparison.OrdinalIgnoreCase) != -1)
                                {
                                    f.additionInfo.orderFirst = true;
                                }
                            }
                        });

                        fs.files = fs.files.OrderByDescending(o => o.Value.additionInfo.orderFirst).ToDictionary(p => p.Key, o => o.Value);
                        fs.Compress();
                        totalcount += fs.files.Count;
                    }));

                    tasks.Last().Start();
                }

                Task.WaitAll(tasks.ToArray());
                SpellDict = null;

                vlist = new List<FrnFileOrigin>(new FrnFileOrigin[totalcount]);

            }
            readsets();  //记录相关* //



            StringBuilder drinfo = new StringBuilder();
            foreach (FileSys fs in fileSysList)
            {
                drinfo.Append(fs.driveInfo.Name + ",");
            }

            initialFinished = true;

            msg = "输入关键词:(" + drinfo.ToString().TrimEnd(',') + ")";
            label1.BeginInvoke(new StatusInfo(ShowStatuesInfo), msg);
            this.BeginInvoke(new EnableTxt(EnableCon));
            Execute_Search_Thread();

            CompressMem(20);//内存缓存

        }

        private void EnableCon()
        {
            Keywords.Enabled = true;
            Keywords.Focus();

            ShowRecord();  //启动完毕展示记录
        }

        private void ShowStatuesInfo(string msg)
        {
            label1.Text = msg;

        }

        RetrieveVirtualItemEventArgs tempRetrieveVirtualItemEventArgs;

        private void ListView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {

            tempRetrieveVirtualItemEventArgs = e;

            if (CurrentCacheItemsSource != null && Threadrest == false && e.ItemIndex >= firstitem && e.ItemIndex < firstitem + CurrentCacheItemsSource.Length && e.ItemIndex - firstitem < CurrentCacheItemsSource.Length)
            {
                //A cache hit, so get the ListViewItem from the cache instead of making a new one.

                if (CurrentCacheItemsSource[e.ItemIndex - firstitem] != null) e.Item = CurrentCacheItemsSource[e.ItemIndex - firstitem];

            }

            if (e.Item == null)
            {
                if ((vlist != null && vresultNum > 0 && e.ItemIndex < vresultNum && e.ItemIndex >= 0))
                {

                    FrnFileOrigin f = vlist[e.ItemIndex];
                    var name = getfile(f.fileName).ToString();
                    var path2 = GetPath(f).ToString();
                    string exten = string.Empty;
                    try
                    {
                        exten = StringUtils.GetExtension(name).ToString();
                    }
                    catch
                    { }
                    if (exten != null)
                    {


                        if (f.IcoIndex != -1)
                        {
                            e.Item = GenerateListViewItem(f, name, path2);
                        }
                        else if (exten.Length == 0)
                        {
                            int ext = 3;

                            f.IcoIndex = ext;

                            e.Item = GenerateListViewItem(f, name, path2);
                        }
                        else if (exten.Equals(".exe", StringComparison.OrdinalIgnoreCase) || exten.Equals(".lnk", StringComparison.OrdinalIgnoreCase))
                        {
                            f.IcoIndex = 0;

                            try
                            {
                                IFileHelper.FileIconIndexAsync(path2, f);
                            }
                            catch
                            { }
                            e.Item = GenerateListViewItem(f, name, path2);

                        }
                        else
                        {
                            f.IcoIndex = 0;
                            try
                            {
                                IFileHelper.FileIconIndexAsync(exten, f);//exten
                            }
                            catch { }
                            e.Item = GenerateListViewItem(f, name, path2);
                        }
                    }

                }
                //}
                if (e.Item == null)
                {
                    e.Item = new ListViewItem(new string[] { "", "", "" });
                }
            }
        }

        private ListViewItem GenerateListViewItem(FrnFileOrigin f, string name, string path)
        {
            return new ListViewItem(new string[] { name, path }, f.IcoIndex);
        }

        readonly object inputLock = new();

        private void Keywords_TextChanged(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                lock (inputLock)
                {
                    Txtselected = false;
                    key1 = Keywords.Text.Trim();
                    if (!(key1 == key2))
                    {
                        dosearch(key1);
                        key2 = key1;
                    }
                }
            });
        }

        private void dosearch(string words)
        {
            if (string.IsNullOrWhiteSpace(words))
            {
                IFShowR = true;
                Threadrest = true;
                gOs.Set();
            }
            else
            {
                IFShowR = false;
                keyword = words;
                Threadrest = true;
                gOs.Set();
            }
        }
        int vresultNum = 0;



        private void SearchFilesThreadStart()
        {
            Threadrunning = true;

            while (Threadrunning == true)
            {
                string[] dwords = null;
                string[] words;
                int dlen = 0;
                int len;
                UInt64 unidwords = 0;
                UInt64 uniwords;
                bool DoDirectory = false;
                int resultNum = 0;

                gOs.WaitOne();
                Threadrest = false;  //重启标签

                if (IFShowR == true) { this.BeginInvoke(new EnableTxt(ShowRecord)); goto Restart; }

                string threadKeyword = keyword;

                string[] driverNames = null;

                if (threadKeyword.Contains(":"))
                {
                    driverNames = (threadKeyword.Split(':'))[0].Split(',');
                    threadKeyword = (threadKeyword.Split(':'))[1];
                }

                threadKeyword = threadKeyword.ToUpperInvariant().Replace("  ", " ").Replace("  ", " ");
                isAll = false;


                if (threadKeyword.Contains(" /A")) { threadKeyword = threadKeyword.Replace(" /A", ""); isAll = true; }

                if (threadKeyword.Contains("\\"))
                {

                    string[] tmp = threadKeyword.Split('\\');
                    string tmpdword = tmp[0].Replace(" ", " ");
                    string tmpword = tmp[1].Replace(" ", " ");
                    dlen = tmpdword.Length;
                    len = tmpword.Length;
                    unidwords = FileSys.TBS(SpellCN.GetSpellCode(tmpdword));

                    uniwords = FileSys.TBS(SpellCN.GetSpellCode(tmpword));


                    if (tmp[0].Contains(" "))
                    {
                        dwords = tmp[0].Split(' ');

                    }
                    else
                    {
                        dwords = new string[] { tmp[0] };
                    }
                    if (tmp[1].Contains(" "))
                    {
                        words = tmp[1].Split(' ');

                    }
                    else
                    {
                        words = new string[] { tmp[1] };
                    }

                    DoDirectory = true;
                }
                else
                {
                    words = threadKeyword.Split(' ');
                    string tmpword = threadKeyword.Replace(" ", "");
                    len = tmpword.Length;
                    uniwords = FileSys.TBS(SpellCN.GetSpellCode(tmpword));
                }

                try
                {
                    if (DoUSNupdate && !ForbidUSNupdate)
                    {

                        for (int i = 0; i < fileSysList.Count; i++)
                        {
                            try
                            {
                                fileSysList[i].DoWhileFileChanges();
                            }

                            catch
                            {
                                continue;
                            }
                        }

                    }




                    for (int d = 0; d < fileSysList.Count; d++)
                    {
                        if (Threadrest) { goto Restart; } //终止标签

                        var fs = fileSysList[d];
                        var l = fs.files;

                        if (!(l.Values.Count > 0 && Directory.Exists(fs.driveInfo.Name))) continue;

                        if (driverNames != null)
                        {
                            bool driverFound = false;
                            foreach (string driverName in driverNames)
                            {
                                if (string.Equals(driverName, fs.driveInfo.Name[0].ToString(), StringComparison.OrdinalIgnoreCase))
                                {
                                    driverFound = true;
                                    break;
                                }
                            }

                            if (!driverFound)
                            {
                                continue;
                            }
                        }


                        var comparisondType = StringComparison.OrdinalIgnoreCase;
                        var comparisonType = StringComparison.OrdinalIgnoreCase;
                        if (unidwords == 0)
                        {
                            comparisondType = StringComparison.Ordinal;
                        }
                        if (uniwords == 0)
                        {
                            comparisonType = StringComparison.Ordinal;
                        }

                        foreach (var f in fs.files.Values)
                        {

                            if (Threadrest) { break; } //终止标签

                            bool Finded = true;

                            if (DoDirectory)
                            {

                                if (f.parentFrn != null && l.TryGetValue(f.parentFrn.fileReferenceNumber, out FrnFileOrigin dictmp))
                                {
                                    foreach (string key in dwords)
                                    {
                                        if (((unidwords | dictmp.keyindex) != dictmp.keyindex) || (dictmp.fileName.IndexOf(key, comparisondType) == -1))
                                        {
                                            Finded = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    Finded = false;
                                }
                            }

                            if (Finded)
                            {
                                foreach (string key in words)
                                {
                                    if (((uniwords | f.keyindex) != f.keyindex) || (f.fileName.IndexOf(key, comparisonType) == -1))
                                    {
                                        Finded = false;
                                        break;
                                    }
                                }
                            }

                            if (Finded)
                            {
                                resultNum++;
                                vlist[resultNum - 1] = f;


                                if (findmax != 0 && resultNum > findmax && isAll == false)
                                {
                                    break;
                                }

                                if (resultNum == 200)//提前显示
                                {
                                    vresultNum = resultNum;
                                    refcache = true;
                                    istView1.BeginInvoke(new System.EventHandler(listupdate), vresultNum);  //必须异步BeginInvoke，不然不同步                                  
                                }

                            }
                        }


                    }


                }
                catch (Exception ex)
                {
                    Log_write("L683" + ex.Message + ";" + ex.StackTrace);
                }



                if (!Threadrest)
                {

                    if (resultNum > 0)
                    {
                        vresultNum = resultNum;

                        refcache = true;
                        istView1.BeginInvoke(new System.EventHandler(listupdate), vresultNum);  //必须异步BeginInvoke，不然不同步

                    }
                    else
                    {
                        istView1.BeginInvoke(new System.EventHandler(listupdate), 0);  //异步BeginInvoke
                    }
                }
Restart:;

            }
        }





        private void listupdate(object o, System.EventArgs e)
        {
            int size = (int)o;
            try
            {
                if (istView1.Items.Count > 0) { istView1.Items[0].EnsureVisible(); }//防止滚动时输入关键词而出错！
            }
            catch
            {

            }


            istView1.VirtualListSize = size;


            istView1.Invalidate();  //刷新cache内核不，然不显示







            if ((findmax != 0 && size > findmax) && isAll == false)
            {
                ShowStatuesInfo("搜索到" + findmax.ToString() + "+个对象");
            }
            else
            {
                //数量控制
                ShowStatuesInfo("搜索到" + size.ToString() + "个对象");
                istView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        private void listupdate_Cache(object o, System.EventArgs e)
        {
            int size = (int)o;

            istView1.VirtualListSize = size;


        }

        private void Recordupdate(object o, System.EventArgs e)
        {
            int size = (int)o;
            {
                try
                {
                    if (istView1.Items.Count > 0) { istView1.Items[0].EnsureVisible(); }//防止滚动时输入关键词而出错！
                }
                catch
                {

                }


                istView1.VirtualListSize = size;
                try
                {
                    if (istView1.VirtualListSize > 0) istView1.Invalidate();  //刷新cache内核不，然不显示
                }
                catch
                {

                }


                istView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

                ShowStatuesInfo("共" + size.ToString() + "条记录");



            }
        }

        private void ListView1_MouseClick(object sender, MouseEventArgs e)
        {



            if (e.Button == MouseButtons.Right)
            {

                if (Control.ModifierKeys == Keys.Shift && (istView1.VirtualListSize > 0) && (istView1.SelectedIndices.Count == 1))
                {

                    //好像自动识别了多文件
                    FrnFileOrigin f = (FrnFileOrigin)vlist[istView1.SelectedIndices[0]];
                    if (!(f == null))
                    {
                        string path = GetPath(f).ToString();
                        if (Path.GetExtension(getfile(f.fileName)).Length == 0)
                        {
                            if (Directory.Exists(path))
                            {
                                ShellContextMenu scm = new ShellContextMenu();
                                DirectoryInfo[] folders = new DirectoryInfo[1];
                                folders[0] = new DirectoryInfo(path);
                                scm.ShowContextMenu(folders, Cursor.Position);
                            }


                        }
                        else
                        {



                            if (File.Exists(path))
                            {
                                ShellContextMenu scm = new ShellContextMenu();
                                FileInfo[] files = new FileInfo[1];
                                files[0] = new FileInfo(path);
                                scm.ShowContextMenu(files, Cursor.Position);
                            }

                        }
                    }
                }
                else if (istView1.SelectedIndices.Count >= 1)
                {
                    RightMenu.Show(Cursor.Position);
                }


            }

        }

        private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (istView1.SelectedIndices.Count > 0)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Golistview(0);
                }
            }
        }

        //建立复制文件列表
        const int MOVE = 7;
        /// <summary>
        /// 访问list中文档
        /// </summary>
        /// <param name="index"></param>
        private void Golistview(int index)
        {

            string moveToDir = string.Empty;
            if(index== MOVE)
            {
                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();

                dialog.Description = "请选择要移动到的文件夹";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    moveToDir = dialog.SelectedPath.TrimEnd('\\');
                }
                else
                {
                    return;
                }
            }


            if (istView1.SelectedIndices.Count > 0)
            {
                ListView.SelectedIndexCollection collects = istView1.SelectedIndices;
                System.Collections.Specialized.StringCollection Copies = new System.Collections.Specialized.StringCollection();
                StringBuilder pathcopies = new StringBuilder();


                foreach (int x in collects)
                {
                    try
                    {

                        FrnFileOrigin f;
                        switch (index)
                        {
                            case 0:  //打开文件
                                f = (FrnFileOrigin)vlist[x];
                                if (!(f == null))
                                {

                                    string path = string.Empty;
                                    //识别
                                    try
                                    {
                                        path = GetPath(f).ToString();

                                    }
                                    catch
                                    {
                                        ifhide = false;
                                        MessageBox.Show("打开失败");
                                        break;
                                    }

                                    var ext = Path.GetExtension(getfile(f.fileName));
                                    if (ext.Length == 0)
                                    {
                                        if (Directory.Exists(path))
                                        {
                                            UpdateRecord(f);
                                            if (LocatingWin)
                                            {

                                                FindWindowS.FindWindow(path);
                                                if (CUSTOMHIDE == true) autoshoworhide();

                                                return;
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    Process.Start("explorer.exe", path);

                                                }
                                                catch
                                                {
                                                    ifhide = false;

                                                    MessageBox.Show("打开失败");

                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (File.Exists(path))
                                        {
                                            UpdateRecord(f); //记录相关* // 
                                            if (LocatingWin)
                                            {
                                                FindWindowS.FindWindow(Path.GetDirectoryName(path));
                                                if (CUSTOMHIDE == true) autoshoworhide();

                                                return;
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    if (ext.Equals(".exe", StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        Process p = new System.Diagnostics.Process();
                                                        p.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
                                                        p.StartInfo.FileName = path;
                                                        p.Start();
                                                    }
                                                    else
                                                    {
                                                        Process.Start("explorer.exe", path);
                                                    }

                                                }
                                                catch (Exception ex)
                                                {
                                                    ifhide = false;
                                                    MessageBox.Show(ex.Message);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ifhide = false;
                                            MessageBox.Show("文件不存在");
                                        }
                                    }
                                }
                                break;


                            case 1:
                                f = (FrnFileOrigin)vlist[x];
                                if (f != null)
                                {
                                    string path = string.Empty;
                                    try
                                    {
                                        path = Path.GetDirectoryName(GetPath(f).ToString());

                                        ExplorerFile(GetPath(f).ToString());

                                        //Process.Start("explorer.exe", path);
                                        UpdateRecord(f);//记录相关* // 

                                    }
                                    catch (Exception ex)
                                    {
                                        ifhide = false;

                                        MessageBox.Show(ex.Message);
                                    }
                                }
                                break;

                            case 2:

                                f = (FrnFileOrigin)vlist[x];
                                if (f != null)
                                {
                                    var path = GetPath(f);
                                    UpdateRecord(f);//记录相关* // 
                                    Copies.Add(path.ToString());

                                }
                                break;
                            case 3:
                                f = (FrnFileOrigin)vlist[x];
                                if (f != null)
                                {

                                    var path = GetPath(f);
                                    UpdateRecord(f); //记录相关* //
                                    pathcopies.Append(path).Append("\r\n");
                                }
                                break;

                            case 4:

                                f = (FrnFileOrigin)vlist[x];
                                if (!(f == null))
                                {
                                    var path = GetPath(f).ToString();
                                    try
                                    {
                                        DF(path);

                                    }
                                    catch
                                    {
                                        ifhide = false;
                                        MessageBox.Show("文件\r\n" + path + "\r\n删除失败,请检查权限或文件是否存在。");
                                    }
                                }
                                break;

                            case 5:
                                f = (FrnFileOrigin)vlist[x];
                                if (!(f == null))
                                {


                                    UpdateRecord(f);
                                    pathcopies.Append(getfile(f.fileName)).Append("\r\n");
                                }
                                break;
                            case 6:
                                f = (FrnFileOrigin)vlist[x];
                                if (!(f == null))
                                {

                                    string path = Path.GetDirectoryName(GetPath(f).ToString());
                                    UpdateRecord(f); //记录相关* // 
                                    pathcopies.Append(path).Append("\r\n");
                                }
                                break;
                            case MOVE: //移动
                                f = (FrnFileOrigin)vlist[x];
                                if (!(f == null))
                                {
                                    var path = GetPath(f).ToString();
                                    try
                                    {
                                        MF(path, moveToDir);
                                    }
                                    catch(Exception ex)
                                    {
                                        ifhide = false;
                                        MessageBox.Show("文件\r\n" + path + "\r\n移动失败: " + ex.Message);
                                    }
                                }
                                break;
                        }
                    }
                    catch
                    {
                    }
                }
                try
                {
                    switch (index)
                    {
                        case 2:
                            Clipboard.SetFileDropList(Copies);
                            break;
                        case 3:
                            Clipboard.SetText(pathcopies.ToString().TrimEnd((char[])"\r\n".ToCharArray()));
                            break;
                        case 4:
                            break;
                        case 5:
                            Clipboard.SetText(pathcopies.ToString().TrimEnd((char[])"\r\n".ToCharArray()));
                            break;
                        case 6:
                            Clipboard.SetText(pathcopies.ToString().TrimEnd((char[])"\r\n".ToCharArray()));
                            break;
                    }
                }
                catch
                {

                }

            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            //*****搜索完毕后更新USN进行更新

            if ((initialFinished == true) && (IsActivated == false))
            {

                IsActivated = true;
                ifhide = true;

                Refreshlist();


                //取消定位功能定位

                try
                {
                    if ((initialFinished == true) && (FindWindowS.testWindow() == true))
                    {
                        LocatingWin = true;

                        label1.ForeColor = SHALLOW;

                        ShowStatuesInfo("定位文件对话框");
                    }
                    else
                    {
                        LocatingWin = false;
                        label1.ForeColor = Color.Gainsboro;

                        ShowStatuesInfo("输入关键词");

                        ifhide = true;

                    }

                    label1.BackColor = Keywords.BackColor;

                }
                catch
                {
                    MessageBox.Show("定位fail");
                }



            }//******搜索完毕后更新USN进行更新



        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {

            if (CUSTOMHIDE == true && ifhide == true) { autoshoworhide(); }

            IsActivated = false;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                autoshoworhide();
                e.Cancel = true;

            }
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                ExitSub();
            }
        }

        private void ExitSub()
        {
            try
            {
                Threadrunning = false;
                if (GoSearch != null)
                {
                    GoSearch.Abort();
                }
            }
            catch { }
            try
            {
                usnJournalThread.Abort();

            }
            catch
            {

            }

            //记录相关* //
            try
            {

                buffercoolies();

            }
            catch
            {
            }

            UnregisterHotKey(Handle, 8617);
            //notifyIcon1.Dispose();
            Application.ExitThread();
            Application.Exit();
            System.Environment.Exit(0);

        }

        private void 退出QToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExitSub();
        }

        private void Keywords_Click(object sender, EventArgs e)
        {
            Txtselected = false;
        }

        private void ListView1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    Golistview(0);

                    break;

                case Keys.Space:
                    Golistview(1);
                    break;
                case Keys.P:
                    Golistview(3);
                    break;
                case Keys.D:
                    Golistview(6);
                    break;
                case Keys.Back:
                    if (istView1.SelectedIndices.Count > 0)
                    {
                        RightMenu.Show(Cursor.Position);
                    }
                    break;
                case Keys.Escape:
                    if (e.KeyCode == Keys.Escape)
                    {

                        Keywords.Focus();
                        Keywords.SelectAll();
                        Txtselected = true;

                    }
                    break;
            }
        }


        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            }
        }


        private void Keywords_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Down:
                    {
                        Txtselected = false;

                        if (istView1.VirtualListSize > 0) { istView1.Focus(); istView1.SelectedIndices.Clear(); istView1.SelectedIndices.Add(0); istView1.Items[0].EnsureVisible(); }
                        break;
                    }
                case Keys.Enter:
                    {
                        string tmp = Keywords.Text.Trim();

                        if (tmp.ToUpperInvariant() == "FF")
                        {
                            CSYDFile csyd = new CSYDFile();
                            csyd.ShowDialog();
                            csyd = null;
                            ClearMemory();
                            return;
                        }
                        else if (tmp.ToUpperInvariant() == "FS")
                        {
                            FileSync.FileSync fs = new FileSync.FileSync();
                            fs.ShowDialog();
                            fs.Dispose();
                            ClearMemory();
                            return;
                        }
                        else if (tmp.ToUpperInvariant() == "CP")
                        {

                            try
                            {
                                ForbidUSNupdate = true;
                                var win = new StartCompareSameFiles(fileSysList);
                                win.ShowDialog();
                                ClearMemory();
                                return;
                            }
                            finally
                            {
                                ForbidUSNupdate = false;

                            }

                        }


                        try
                        {

                            System.Diagnostics.Process.Start(tmp);
                            return;
                        }
                        catch
                        {

                        }

                        try
                        {
                            if (istView1.VirtualListSize > 0) { istView1.SelectedIndices.Clear(); istView1.SelectedIndices.Add(0); istView1.Items[0].EnsureVisible(); }
                            Golistview(0);
                            return;
                        }
                        catch
                        {
                        }



                        break;

                    }

            }

        }

        //记录相关
        private void ShowRecord()
        {


            if (Record.Count > 0)
            {
                while (Record.Count > 100)
                {
                    Record.RemoveAt(Record.Count - 1);
                }


                vresultNum = Record.Count;

                //this.BeginInvoke(new dosize(Sizecalc), Record.Count);
                for (int i = 0; i < Record.Count; i++)
                {
                    vlist[i] = Record[i];
                }
                this.BeginInvoke(new System.EventHandler(Recordupdate), vresultNum);  //异步invoke


            }
            else
            {
                this.BeginInvoke(new System.EventHandler(Recordupdate), 0);
                // this.BeginInvoke(new dosize(Sizecalc), Record.Count);
            }
            refcache = true;


        }

        //记录相关*
        private void UpdateRecord(FrnFileOrigin targ)
        {
            bool existed = false;
            if (Record.Count > 0)
            {
                for (int i = 0; i < Record.Count; i++)
                {
                    if ((Record[i].fileReferenceNumber == targ.fileReferenceNumber) || (Record[i].parentFrn == targ.parentFrn && Record[i].fileName == targ.fileName))
                    {
                        Record[i].fileReferenceNumber = targ.fileReferenceNumber;
                        Record[i].fileName = targ.fileName;

                        existed = true;

                    }

                }
            }
            if (existed == false)
            {
                Record.Add((FrnFileOrigin)targ);
            }


        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                autoshoworhide();
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            autoshoworhide();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        //记录相关* //
        private void buffercoolies()
        {
            string path = Application.StartupPath + "\\" + "Record.cah";
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (StreamWriter fs = new StreamWriter(path, false, System.Text.Encoding.GetEncoding("gb2312")))
            {
                foreach (FrnFileOrigin f in Record)
                {
                    string fp = GetPath(f).ToString();
                    if (File.Exists(fp) || Directory.Exists(fp))
                    {
                        fs.WriteLine(string.Concat(f.fileReferenceNumber, "@", f.parentFrn.fileReferenceNumber, "@", f.fileName, "@", 0, "@", f.VolumeName));
                    }
                }
                fs.Close();
            }
        }

        //记录相关* //
        private void readsets()
        {
            string path = Application.StartupPath + "\\" + "Record.cah";
            if (File.Exists(path))
            {
                //try
                {

                    // Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    using (StreamReader fs = new StreamReader(path, System.Text.Encoding.GetEncoding("gb2312")))
                    {
                        Record.Clear();
                        while (!fs.EndOfStream)
                        {
                            try
                            {
                                string[] KeyValue = fs.ReadLine().Split('@');

                                if (KeyValue != null && KeyValue.GetUpperBound(0) == 4)
                                {

                                    if ((fileSysList[int.Parse(KeyValue[3])].files.TryGetValue(UInt64.Parse(KeyValue[0]), out FrnFileOrigin f)))
                                    {
                                        Record.Add(f as FrnFileOrigin);
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                        fs.Close();
                    }
                }
                //  catch { }
            }
        }

        private void ListView1_MouseDown(object sender, MouseEventArgs e)
        {
            Clickdown = true;
            mouseX = e.X;
            mouseY = e.Y;
        }

        private void ListView1_MouseMove(object sender, MouseEventArgs e)
        {


            if (Clickdown == true)
            {
                Clickdown = false;
                int tmp = (mouseX - e.X) * (mouseY - e.Y);
                if ((istView1.SelectedIndices.Count > 0) && (tmp > 10 || tmp < -10))
                {
                    ListView.SelectedIndexCollection collects = istView1.SelectedIndices;

                    System.Collections.Specialized.StringCollection a = new System.Collections.Specialized.StringCollection();
                    FrnFileOrigin f;
                    foreach (int x in collects)
                    {
                        f = (FrnFileOrigin)vlist[x];
                        if (!(f == null))
                        {
                            string path = GetPath(f).ToString();
                            a.Add(path);
                        }
                    }
                    DataObject b = new DataObject();
                    b.SetFileDropList(a);
                    try
                    {
                        istView1.DoDragDrop(b, DragDropEffects.Copy);
                    }
                    catch
                    {
                        MessageBox.Show("无法拖拽");
                    }
                }
            }
            else
            {


                if (istView1.VirtualListSize == 0)
                    return;
                if (prItem != null)
                {
                    try
                    {

                        prItem.BackColor = istView1.BackColor;
                        prItem.ForeColor = istView1.ForeColor;
                    }
                    catch { }
                }
                prItem = istView1.GetItemAt(e.X, e.Y);
                if (prItem != null)
                    try
                    {
                        prItem.BackColor = HIGHLIGHT;
                        prItem.ForeColor = Color.DimGray;
                    }
                    catch
                    {
                    }

            }
        }

        private void ListView1_MouseUp(object sender, MouseEventArgs e)
        {
            Clickdown = false;
            mouseX = 0;
            mouseY = 0;
        }

        const string StartupPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        const string TDSNAME = "TDS-LeahyGo";
        private void 开机启动SToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(StartupPath, true))
                {
                    key?.SetValue(TDSNAME, string.Concat("\"", Application.ExecutablePath, "\""));
                }


                //added but not work
                //Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true).SetValue("TDS-LeahyGo", "\"" + Application.ExecutablePath + "\"");

                //Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true).SetValue("TDS-LeahyGo", "\"" + Application.ExecutablePath + "\"");

                //work as same as without wow6432node
                //Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", true).SetValue("TDS-LeahyGo", "\"" + Application.ExecutablePath + "\"");
                ifhide = false;
                MessageBox.Show("开机启动添加成功.");
            }
            catch
            {
                ifhide = false;
                MessageBox.Show("开机启动添加失败.");
            }

        }

        private void 关于LeahyGoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About();
        }

        private void About()
        {
            string ver = "8.0.20231207";
            ifhide = false;
            MessageBox.Show("版本号:" + ver + "\r\nluojin@BeiJing@2023");
        }

        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Golistview(0);
            }
            catch (Exception ex)
            {
                ifhide = false;
                MessageBox.Show(ex.Message);
            }
        }

        private void 打开文件夹DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Golistview(1);
            }
            catch
            {
                ifhide = false;
                MessageBox.Show("目标不存在或缺少权限。");
            }
        }

        private void 复制文件FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Golistview(2);
            }
            catch
            {
                ifhide = false;
                MessageBox.Show("目标不存在或缺少权限。");
            }
        }

        private void 复制文件路径CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Golistview(3);
            }
            catch
            {
                MessageBox.Show("目标不存在或缺少权限。");
            }
        }

        private void 删除文件MToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ifhide = false;

            if (MessageBox.Show("要彻底删除" + istView1.SelectedIndices.Count.ToString() + "个项目吗？", "文件将不进入回收站直接删除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                try
                {
                    Golistview(4);
                    Refreshlist();

                }
                catch
                {
                    MessageBox.Show("文件删除出错。");
                }
            }
        }

        private void RightMenu_Opening(object sender, CancelEventArgs e)
        {
            ifhide = false;

            XY = Cursor.Position;
            int fnum = istView1.SelectedIndices.Count;
            if (fnum > 1)
            {
                toolStripSeparator2.Visible = false;
                调用系统菜单ToolStripMenuItem.Visible = false;
                toolStripTextBox1.Text = "已选" + fnum.ToString() + "个项目";
                toolStripTextBox1.Enabled = false;
                打开OToolStripMenuItem.Text = "批量打开项目(&O)";
                打开文件夹DToolStripMenuItem.Text = "批量打开项目所在文件夹(&D)";
                复制文件FToolStripMenuItem.Text = "批量复制项目(&F)";
                复制文件路径CToolStripMenuItem.Text = "批量复制路径(&C)";
                删除文件MToolStripMenuItem.Text = "批量删除该" + fnum.ToString() + "个项目(&M)";
                复制文件名NToolStripMenuItem.Text = "批量复制文件名(&N)";
                复制目录路径EToolStripMenuItem.Text = "批量复制目录路径(&E)";
            }
            else if (fnum == 1)
            {
                调用系统菜单ToolStripMenuItem.Visible = true;
                toolStripSeparator2.Visible = true;
                FrnFileOrigin f = (FrnFileOrigin)vlist[istView1.SelectedIndices[0]];
                if (!(f == null))
                {
                    toolStripTextBox1.Text = getfile(f.fileName).ToString();
                    toolStripTextBox1.Enabled = true; ;
                    打开OToolStripMenuItem.Text = "打开(&O)";
                    打开文件夹DToolStripMenuItem.Text = "打开项目所在文件夹(&D)";
                    复制文件FToolStripMenuItem.Text = "复制该项目(&F)";
                    复制文件路径CToolStripMenuItem.Text = "复制路径(&C)";
                    删除文件MToolStripMenuItem.Text = "删除项目(&M)";
                    复制文件名NToolStripMenuItem.Text = "复制文件名(&N)";
                    复制目录路径EToolStripMenuItem.Text = "复制目录路径(&E)";
                }
            }
            else
            {
                RightMenu.Close();
            }
        }

        private void 调用系统菜单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ifhide = true;

            //好像自动识别了多文件
            try
            {
                FrnFileOrigin f = (FrnFileOrigin)vlist[istView1.SelectedIndices[0]];
                if (!(f == null))
                {
                    string path = GetPath(f).ToString();
                    if (Path.GetExtension(getfile(f.fileName)).Length == 0)
                    {
                        if (Directory.Exists(path))
                        {
                            ShellContextMenu scm = new ShellContextMenu();
                            DirectoryInfo[] folders = new DirectoryInfo[1];
                            folders[0] = new DirectoryInfo(path);
                            scm.ShowContextMenu(folders, XY);
                        }


                    }
                    else
                    {

                        if (File.Exists(path))
                        {
                            ShellContextMenu scm = new ShellContextMenu();
                            FileInfo[] files = new FileInfo[1];
                            files[0] = new FileInfo(path);
                            scm.ShowContextMenu(files, XY);
                        }

                    }
                }
            }

            catch
            {

                ifhide = false;
                MessageBox.Show("目标不存在或缺少权限。");
            }
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                FrnFileOrigin f = (FrnFileOrigin)vlist[istView1.SelectedIndices[0]];
                if (toolStripTextBox1.Text != getfile(f.fileName))
                {
                    ifhide = false;
                    if (MessageBox.Show("是否重命名?", "重命名", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {

                        if (!(f == null))
                        {
                            string path = GetPath(f).ToString();
                            string pathnew = Path.GetDirectoryName(GetPath(f).ToString()) + "\\" + toolStripTextBox1.Text;
                            if (Path.GetExtension(getfile(f.fileName)).Length == 0)
                            {
                                if (Directory.Exists(path))
                                {


                                    try
                                    {
                                        Directory.Move(path, pathnew);



                                        string nacn = SpellCN.GetSpellCode(System.IO.Path.GetFileName(pathnew));
                                        f.keyindex = FileSys.TBS(nacn);
                                        f.fileName = string.Intern(string.Format("|{0}|{1}|", System.IO.Path.GetFileName(pathnew), nacn));

                                        Refreshlist();

                                    }
                                    catch
                                    {
                                        MessageBox.Show("文件夹重命名出错，请检查文件夹是否存在或是否有权限。");
                                    }
                                }

                            }
                            else
                            {
                                if (File.Exists(path))
                                {

                                    try
                                    {
                                        File.Move(path, pathnew);

                                        string nacn = SpellCN.GetSpellCode(System.IO.Path.GetFileName(pathnew));
                                        f.keyindex = FileSys.TBS(nacn);
                                        f.fileName = string.Intern(string.Format("|{0}|{1}|", System.IO.Path.GetFileName(pathnew), nacn));
                                        Refreshlist();
                                    }
                                    catch
                                    {
                                        MessageBox.Show("文件重命名出错，请检查文件是否存在或是否有权限。");
                                    }
                                }

                            }
                        }
                    }
                }

            }
        }

        private void Refreshlist()
        {

            DoUSNupdate = true;


            dosearch(Keywords.Text);
        }

        private void 刷新RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Refreshlist();
            string strProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            ////获取版本号 
            //CommonData.VersionNumber = Application.ProductVersion; 
            //检查进程是否已经启动，已经启动则显示报错信息退出程序。 
            var p = System.Diagnostics.Process.GetProcessesByName(strProcessName);
            if (p?.Length > 0)
            {
                PostThreadMessage(p[0].Threads[0].Id, 0x010, IntPtr.Zero, IntPtr.Zero);
                Thread.Sleep(500);
            }

        }

        private void 快捷键HToolStripMenuItem_Click(object sender, EventArgs e)
        {
            help();
        }

        private void help()
        {
            string msghelp = "空格  打开项目所在目录；\r\n回车    打开文件或文件夹；\r\nESC    全选或清除关键字；\r\nshift+右键   调用系统文件菜单；\r\n退格键backspace   打开鼠标右键；\r\n";

            ifhide = false;
            MessageBox.Show(msghelp);
        }

        private void toolStripSeparator2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //  if (e.KeyCode == Keys.Escape)
            //{

            //      Keywords.Focus();
            //      Keywords.SelectAll();
            //      Txtselected = true;

            //  }

        }

        private void Keywords_MouseDown(object sender, MouseEventArgs e)
        {
            //移动窗体
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void Label1_MouseDown(object sender, MouseEventArgs e)
        {
            //移动窗体
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void 缓存刷新CToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //记录相关* //
            try
            {

                buffercoolies();//存储记录列表

            }
            catch
            {
            }
            Initial();
        }

        private void 开机启动SToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            try
            {
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true).SetValue("TDS-LeahyGo", "\"" + Application.ExecutablePath + "\"");
                ifhide = false;
                MessageBox.Show("开机启动添加成功.");
            }
            catch
            {
                ifhide = false;
                MessageBox.Show("开机启动添加失败.");

            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostThreadMessage(int threadId, uint msg, IntPtr wParam, IntPtr lParam);

        private void 快捷键说明SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            help();

        }

        private void 关于LToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About();

        }

        string GetDateFromPath(string path2)
        {
            if (File.Exists(path2))
            {
                return File.GetLastWriteTime(path2).ToString();
            }

            if (Directory.Exists(path2))
            {
                return Directory.GetCreationTime(path2).ToString();
            }

            return "null";

        }

        CacheVirtualItemsEventArgs tempCacheEventArgs;
        private void IstView1_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {

            tempCacheEventArgs = e;

            if (refcache == false && CurrentCacheItemsSource != null && e.StartIndex >= firstitem && e.EndIndex <= firstitem + CurrentCacheItemsSource.Length)
            {
                return;
            }


            firstitem = e.StartIndex;
            int length = e.EndIndex - e.StartIndex + 1;
            CurrentCacheItemsSource = null;
            CurrentCacheItemsSource = new ListViewItem[length];

            for (int i = 0; i < length; i++)
            {
                if (i + firstitem < vresultNum)
                {

                    FrnFileOrigin f = vlist[i + firstitem];
                    string name = getfile(f.fileName).ToString();
                    string path2 = GetPath(f).ToString();

                    if (i >= CurrentCacheItemsSource.Count())
                    {
                        break;
                    }
                    if (f.IcoIndex != -1)
                    {
                        CurrentCacheItemsSource[i] = GenerateListViewItem(f, name, path2);
                    }
                    else
                    {
                        string exten = string.Empty;
                        try
                        {
                            exten = Path.GetExtension(name);

                        }
                        catch
                        { }
                        if (exten != null)
                        {
                            if (exten.Length == 0)
                            {
                                int ext = 3;

                                f.IcoIndex = ext;

                                CurrentCacheItemsSource[i] = GenerateListViewItem(f, name, path2);
                                continue;
                            }
                            else
                            if (exten.Equals(".exe", StringComparison.OrdinalIgnoreCase) || exten.Equals(".lnk", StringComparison.OrdinalIgnoreCase))
                            {
                                f.IcoIndex = 0;
                                try
                                {
                                    IFileHelper.FileIconIndexAsync(@path2, f);
                                }
                                catch
                                { }
                                CurrentCacheItemsSource[i] = GenerateListViewItem(f, name, path2);
                                continue;

                            }
                            else
                            {
                                f.IcoIndex = 0;

                                try
                                {
                                    IFileHelper.FileIconIndexAsync(exten, f);//exten
                                }
                                catch { }
                                CurrentCacheItemsSource[i] = GenerateListViewItem(f, name, path2);
                                continue;
                            }
                        }
                        else
                        {
                            f.IcoIndex = 0;
                            CurrentCacheItemsSource[i] = GenerateListViewItem(f, name, path2);

                        }
                    }
                }
            }
            refcache = false;
            istView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }


        private void 显示主界面SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoshoworhide();

        }

        private void ToolStripTextBox1_Enter(object sender, EventArgs e)
        {
            ifhide = false;

        }

        private void ToolStripTextBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void Keywords_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == System.Convert.ToChar(13))
            {
                e.Handled = true;
            }
        }

        private void 复制文件名NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Golistview(5);
            }
            catch
            {
                MessageBox.Show("目标不存在或缺少权限。");
            }
        }

        private void 复制目录路径EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Golistview(6);
            }
            catch
            {
                MessageBox.Show("目标不存在或缺少权限。");
            }
        }

        protected override void WndProc(ref Message m)
        {

            switch (m.Msg)
            {
                case 0x0312:
                    // IntPtr id = m.WParam;
                    switch (m.WParam.ToString())
                    {
                        case "8617":
                            autoshoworhide();

                            break;

                    }
                    base.WndProc(ref m);

                    break;


                //窗口尺寸
                case 0x0084:

                    base.WndProc(ref m);
                    Point vPoint = new Point((int)m.LParam & 0xFFFF,
                        (int)m.LParam >> 16 & 0xFFFF);
                    vPoint = PointToClient(vPoint);
                    //只允许更改X方向尺寸
                    if (vPoint.X <= 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)HTTOPLEFT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)HTBOTTOMLEFT;
                        else
                            m.Result = (IntPtr)HTLEFT;
                    else if (vPoint.X >= ClientSize.Width - 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)HTTOPRIGHT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)HTBOTTOMRIGHT;
                        else
                            m.Result = (IntPtr)HTRIGHT;
                    else if (vPoint.Y <= 5)
                        m.Result = (IntPtr)HTTOP;
                    else if (vPoint.Y >= ClientSize.Height - 5)
                        m.Result = (IntPtr)HTBOTTOM;
                    break;
                case 0x0201://鼠标左键按下的消息 
                    m.Msg = 0x00A1;//更改消息为非客户区按下鼠标 
                    m.LParam = IntPtr.Zero;//默认值 
                    m.WParam = new IntPtr(2);//鼠标放在标题栏内 

                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
                    //窗口尺寸       
            }


        }

        /// <summary>
        /// /esc捕捉
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                ESCPress();

                return true;

            }
            if ((initialFinished == true) && (keyData == (Keys.Control | Keys.D1)))
            {

                int GoIndex = 0;
                if (istView1.VirtualListSize > GoIndex) { Txtselected = false; istView1.Focus(); istView1.SelectedIndices.Clear(); istView1.SelectedIndices.Add(GoIndex); Golistview(0); }

                return true;
            }
            if ((initialFinished == true) && (keyData == (Keys.Control | Keys.D2)))
            {

                int GoIndex = 1;
                if (istView1.VirtualListSize > GoIndex) { Txtselected = false; istView1.Focus(); istView1.SelectedIndices.Clear(); istView1.SelectedIndices.Add(GoIndex); Golistview(0); }

                return true;
            }
            if ((initialFinished == true) && (keyData == (Keys.Control | Keys.D3)))
            {

                int GoIndex = 2;
                if (istView1.VirtualListSize > GoIndex) { Txtselected = false; istView1.Focus(); istView1.SelectedIndices.Clear(); istView1.SelectedIndices.Add(GoIndex); Golistview(0); }

                return true;
            }
            if ((initialFinished == true) && (keyData == (Keys.Control | Keys.D4)))
            {

                int GoIndex = 3;
                if (istView1.VirtualListSize > GoIndex) { Txtselected = false; istView1.Focus(); istView1.SelectedIndices.Clear(); istView1.SelectedIndices.Add(GoIndex); Golistview(0); }

                return true;
            }
            if ((initialFinished == true) && (keyData == (Keys.Control | Keys.D5)))
            {

                int GoIndex = 4;
                if (istView1.VirtualListSize > GoIndex) { Txtselected = false; istView1.Focus(); istView1.SelectedIndices.Clear(); istView1.SelectedIndices.Add(GoIndex); Golistview(0); }

                return true;
            }
            if ((initialFinished == true) && (keyData == (Keys.Control | Keys.D6)))
            {

                int GoIndex = 5;
                if (istView1.VirtualListSize > GoIndex) { Txtselected = false; istView1.Focus(); istView1.SelectedIndices.Clear(); istView1.SelectedIndices.Add(GoIndex); Golistview(0); }

                return true;
            }
            if ((initialFinished == true) && (keyData == (Keys.Control | Keys.D7)))
            {

                int GoIndex = 6;
                if (istView1.VirtualListSize > GoIndex) { Txtselected = false; istView1.Focus(); istView1.SelectedIndices.Clear(); istView1.SelectedIndices.Add(GoIndex); Golistview(0); }

                return true;
            }
            if ((initialFinished == true) && (keyData == (Keys.Control | Keys.D8)))
            {

                int GoIndex = 7;
                if (istView1.VirtualListSize > GoIndex) { Txtselected = false; istView1.Focus(); istView1.SelectedIndices.Clear(); istView1.SelectedIndices.Add(GoIndex); Golistview(0); }

                return true;
            }
            if ((initialFinished == true) && (keyData == (Keys.Control | Keys.D9)))
            {

                int GoIndex = 8;
                if (istView1.VirtualListSize > GoIndex) { Txtselected = false; istView1.Focus(); istView1.SelectedIndices.Clear(); istView1.SelectedIndices.Add(GoIndex); Golistview(0); }

                return true;
            }
            if ((initialFinished == true) && (keyData == (Keys.Control | Keys.D0)))
            {

                int GoIndex = 9;
                if (istView1.VirtualListSize > GoIndex) { Txtselected = false; istView1.Focus(); istView1.SelectedIndices.Clear(); istView1.SelectedIndices.Add(GoIndex); Golistview(0); }
                return true;
            }


            return base.ProcessCmdKey(ref msg, keyData);
        }

        public static void SetDoubleBuffering(System.Windows.Forms.Control control, bool value)
        {
            System.Reflection.PropertyInfo controlProperty = typeof(System.Windows.Forms.Control)
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            controlProperty.SetValue(control, value, null);
        }

        public static string GetAllUsersDesktopFolderPath()
        {
            StringBuilder sbPath = new StringBuilder(MAX_PATH);
            SHGetFolderPath(IntPtr.Zero, CSIDL_COMMON_PROGRAMS, IntPtr.Zero, 0, sbPath);
            return sbPath.ToString();
        }

        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

        //[DllImport("kernel32.dll", EntryPoint = "VirtualLock")]
        //public static extern bool VirtualLock(IntPtr process, int maxSize);

        /// <summary>
        ///  二进制转换逻辑搜索使用
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static ReadOnlySpan<char> getfile(ReadOnlySpan<char> filename)
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



        public static void ExplorerFile(string filePath)
        {
            if (!File.Exists(filePath) && !Directory.Exists(filePath))
                return;

            if (Directory.Exists(filePath))
                Process.Start(@"explorer.exe", "/select,\"" + filePath + "\"");
            else
            {
                IntPtr pidlList = ILCreateFromPathW(filePath);
                if (pidlList != IntPtr.Zero)
                {
                    try
                    {
                        Marshal.ThrowExceptionForHR(SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0));
                    }
                    finally
                    {
                        ILFree(pidlList);
                    }
                }
            }
        }

        [DllImportAttribute("user32.dll", EntryPoint = "UnregisterHotKey")]
        public static extern bool UnregisterHotKey
            (
                IntPtr hWnd,        //注册热键的窗口句柄        
                int id              //热键编号上面注册热键的编号        
            );

        /// <summary>
        /// 热键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint keyValue, Keys vk);

        public void Initial()
        {
            try
            {
                CUSTOMREAD();
            }
            catch { };

            try
            {
                Threadrunning = false;
                Thread.Sleep(100);
                if (GoSearch != null) { GoSearch.Abort(); }
            }
            catch
            { }

            try
            {
                if (usnJournalThread != null) { usnJournalThread.Abort(); }
            }
            catch
            { }




            this.Visible = false;

            initialFinished = false;
            Keywords.Enabled = false;

            //  缓存刷新CToolStripMenuItem.Enabled = false;

            RegisterHotKey(Handle, 8617, 2, HOTK_SHOW);

            key1 = ""; key2 = "";

            usnJournalThread = new Thread(ListFilesThreadStart)
            {
                IsBackground = false,
            };
            usnJournalThread.Start();

        }

        //#endregion
        public IEnumerable<DriveInfo> GetAllFixedNtfsDrives()
        {
            return DriveInfo.GetDrives()
                .Where(d => (d.IsReady == true && (d.DriveType == DriveType.Fixed || d.DriveType == DriveType.Removable) && d.DriveFormat.ToUpperInvariant() == "NTFS"));//固定磁盘
        }

        public void Execute_Search_Thread()
        {
            if (initialFinished == true)
            {
                try
                {
                    GoSearch = new Thread(SearchFilesThreadStart)
                    {
                        IsBackground = false,
                        Priority = System.Threading.ThreadPriority.Highest
                    }; //启动搜索线程
                    GoSearch.Start();
                }
                catch
                {
                }
            }
        }

        public void CompressMem(int mbsize)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //try
            //{
            //    Process Mem;
            //    Mem = Process.GetCurrentProcess();

            //    SetProcessWorkingSetSize(Mem.Handle, mbsize * 1024 * 1024, mbsize * 1024 * 1024);

            //}
            //catch
            //{

            //}
        }

        public void Sizecalc(object o, System.EventArgs e)
        {
            int n = (int)o;

            try
            {
                int list_h_cal = 18 + Keywords.Size.Height + txtH * n;
                int setheight = (Screen.PrimaryScreen.Bounds.Height - this.Location.Y) * 3 / 4;
                if (list_h_cal > setheight)
                {
                    this.Size = new Size(this.Size.Width, setheight);
                }
                else if (n == 0)
                { this.Size = new Size(this.Size.Width, 12 + Keywords.Size.Height); }
                else
                {
                    this.Size = new Size(this.Size.Width, list_h_cal);
                }
            }
            catch {; }
        }

        public void Sizecalc(int n)
        {
            try
            {
                int list_h_cal = 18 + Keywords.Size.Height + txtH * n;
                int setheight = (Screen.PrimaryScreen.Bounds.Height - this.Location.Y) * 3 / 4;
                if (list_h_cal > setheight)
                {
                    this.Size = new Size(this.Size.Width, setheight);
                }
                else if (n == 0)
                { this.Size = new Size(this.Size.Width, 12 + Keywords.Height); }
                else
                {
                    this.Size = new Size(this.Size.Width, list_h_cal);

                }
            }
            catch {; }
        }

        public void autoshoworhide()
        {
            try
            {
                UnregisterHotKey(Handle, 8617);

                RegisterHotKey(Handle, 8617, 2, HOTK_SHOW);

            }
            catch
            { }


            if (this.Visible == true)
            {
                ////取消激活判断 
                //if (IsActivated == true)
                //{
                ifhide = false;

                this.Visible = false;

                try
                {
                    if (GoSearch != null && GoSearch.IsAlive)
                    {
                        Threadrest = true;
                        gOs.Set();
                        GoSearch = null;
                    }; //启动搜索线程                
                }
                catch
                {

                }
            }
            else
            {
                ifhide = true;


                this.Visible = true;


                try
                {
                    //重启线程
                    try
                    {
                        if (GoSearch != null && GoSearch.IsAlive)
                        {
                            Threadrest = true;
                            gOs.Set();
                            GoSearch = null;
                        }; //启动搜索线程                
                    }
                    catch
                    {

                    }



                }
                catch
                {

                }


                if (this.Width < 100) this.Width = 300;
                if (this.Height < 100) this.Height = 200;
                this.Activate();

                Keywords.Focus();
                Keywords.SelectAll();

            }
        }

        public void DF(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if (attr == FileAttributes.Directory)
            {
                Directory.Delete(path, true);
            }
            else
            {
                File.Delete(path);
            }
        }

        public void MF(string path,string newPath)
        {
            FileAttributes attr = File.GetAttributes(path);
            if (attr == FileAttributes.Directory)
            {                
                MessageBox.Show("暂不支持移动目录");
            }
            else
            {
                File.Move(path, newPath + '\\' + Path.GetFileName(path));
            }
        }




        #region Properties




        #endregion

        #region private member variables



        #endregion

        #region delegates代理

        private delegate void StatusInfo(String rtnCode);
        private delegate void dosize(int rtnCode);
        private delegate void EnableTxt();

        #endregion
        //窗口尺寸
        //改变窗体大小


        //改变窗体大小


        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED

                return cp;
            }
        }

        #region //移动窗体
        public const int WM_SYSCOMMAND = 0x0112;

        public const int SC_MOVE = 0xF010;

        public const int HTCAPTION = 0x0002;

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        #endregion
        //////////////////////
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    //Rectangle re = new Rectangle(this);
        //    base.OnPaint(e); // Visual styles are not enabled.
        //    if (!ScrollBarRenderer.IsSupported)
        //    {

        //        return;
        //    }


        //    ScrollBarRenderer.DrawRightHorizontalTrack(e.Graphics, ClientRectangle, ScrollBarState.Normal);    // Draw the thumb and thumb grip in the current state.
        //             ScrollBarRenderer.DrawHorizontalThumb(e.Graphics,
        //    ClientRectangle, ScrollBarState.Normal);
        //    ScrollBarRenderer.DrawHorizontalThumbGrip(e.Graphics,
        //        ClientRectangle, ScrollBarState.Normal);    // Draw the scroll arrows in the current state.
        //    ScrollBarRenderer.DrawArrowButton(e.Graphics,
        //            ClientRectangle, ScrollBarArrowButtonState.LeftNormal);

        //    ScrollBarRenderer.DrawArrowButton(e.Graphics,
        //            ClientRectangle, ScrollBarArrowButtonState.LeftNormal);
        //}

        #region   隐藏横向滚动
        //public class SubWindow : NativeWindow
        //{
        //    [DllImport("user32.dll")]
        //    public static extern int ShowScrollBar(IntPtr hWnd, int iBar, int bShow);

        //    const int SB_HORZ = 0;

        //    protected override void WndProc(ref System.Windows.Forms.Message m)
        //    {
        //        ShowScrollBar(m.HWnd, SB_HORZ, 0);
        //        base.WndProc(ref m);
        //    }
        //}
        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);

        //    SubWindow vSubWindow = new SubWindow();

        //    vSubWindow.AssignHandle(istView1.Handle);
        //}
        #endregion








        #region 自定义设置
        private void CUSTOMBUILT()
        {
            try
            {
                string path = Application.StartupPath + "\\" + "Settings.ini";
                //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using (StreamWriter fs = new StreamWriter(path, false, System.Text.Encoding.GetEncoding("gb2312")))
                {

                    fs.WriteLine("[参数设定]");
                    fs.WriteLine("HOTKEY1=" + ((Keys)Convert.ToInt32(Keys.Oem3)).ToString() + "       //为Ctrl+自定义键，默认为~键（Oem3），");
                    fs.WriteLine("LIST_MAX=100       //列表最大显条目，0为不设限，默认为100条");
                    fs.WriteLine("OPACITY=100       //透明度1-100，100为不透明，默认为100");
                    fs.WriteLine("THEME=0            //深暗配色1、明亮配色0，默认为0");
                    fs.WriteLine("HIDE=1            //失去焦点自动隐藏1、不自动隐藏0，默认为1");
                    fs.Close();
                }
            }
            catch { }

        }

        private void CUSTOMREAD()
        {
            bool A;  //四个参数
            int B;
            int C;
            Keys D;
            bool E;


            string path = Application.StartupPath + "\\" + "Settings.ini";
            if (File.Exists(path))
            {
                try
                {
                    // Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    using (StreamReader fs = new StreamReader(path, System.Text.Encoding.GetEncoding("gb2312")))
                    {

                        string[] Sets = fs.ReadToEnd().Split('\n');

                        //A
                        try
                        {

                            if (int.Parse(READPARA(Sets[4])) == 0) { A = false; } else { A = true; }


                        }
                        catch
                        {
                            A = true;
                        }

                        //B
                        try
                        {

                            B = int.Parse(READPARA(Sets[2]));

                        }
                        catch
                        {
                            B = 100;
                        }


                        //C

                        try
                        {

                            C = int.Parse(READPARA(Sets[3]));
                            if (C < 0) C = 0;
                            if (C > 100) C = 100;

                        }
                        catch
                        {
                            C = 100;
                        }

                        //D

                        try
                        {


                            D = (Keys)Enum.Parse(typeof(Keys), READPARA(Sets[1]), true);

                        }
                        catch
                        {
                            D = Keys.OemPeriod;
                        }
                        try
                        {

                            if (int.Parse(READPARA(Sets[5])) == 0) { E = false; } else { E = true; }


                        }
                        catch
                        {
                            E = true;
                        }

                        fs.Close();
                        CUSTOMDO(A, B, C, D, E);
                    }

                }
                catch
                {
                    CUSTOMRESET();
                }
            }
            else
            {
                CUSTOMBUILT();
                CUSTOMRESET();
            }
        }

        private string READPARA(string line)
        {
            string[] dyhm = line.Split('=');
            string[] para;
            if (dyhm.Length > 1)
            {
                para = dyhm[1].Split(' ');
                if (para.Length > 0) return para[0];
            }
            return string.Empty;
        }

        private void CUSTOMRESET()
        {
            CUSTOMDO(true, 100, 100, Keys.OemPeriod, true);
        }



        #endregion
        #region 具体动作

        private void HOTK(Keys key)
        {
            HOTK_SHOW = key;
            UnregisterHotKey(Handle, 8617);
            RegisterHotKey(Handle, 8617, 2, HOTK_SHOW);
        }

        private void istView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void CUSTOMDO(bool col, int max, int op, Keys key, bool hid)
        {
            COLORSET(col);
            Form1.findmax = max;
            this.Opacity = (double)op / 100;

            HOTK(key);
            CUSTOMHIDE = hid;
        }

        private void COLORSET(bool i)
        {
            //try
            //{
            if (i == true)
            {
                Form1.Mdeep = Color.FromArgb(58, 64, 80);
                Form1.Hdeep = Color.FromArgb(48, 50, 63);
                Form1.Ldeep = Color.FromArgb(87, 97, 121);
                Keywords.ForeColor = Color.WhiteSmoke;
                istView1.ForeColor = Color.Gainsboro;
            }
            else
            {
                Form1.Mdeep = Color.White;
                Form1.Hdeep = Control.DefaultBackColor;
                Form1.Ldeep = Color.White;

                Keywords.ForeColor = Color.Black;
                istView1.ForeColor = Color.DimGray;
            }
            Keywords.BackColor = Ldeep;
            istView1.BackColor = Mdeep;
            label1.BackColor = Keywords.BackColor;
            this.BackColor = Hdeep;
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }
        #endregion


        private void 文件内容查询FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CSYDFile csyd = new CSYDFile();
            csyd.ShowDialog();
            csyd.Dispose();
            csyd = null;
            ClearMemory();
            return;
        }

        private void 文件同步TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSync.FileSync fs = new FileSync.FileSync();
            fs.ShowDialog();
            fs.Dispose();
            ClearMemory();
            return;
        }

        private void 移动到ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Golistview(MOVE);
            }
            catch
            {
                ifhide = false;
                MessageBox.Show("目标不存在或缺少权限。");
            }
        }
    }
}