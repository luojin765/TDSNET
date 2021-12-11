using CNChar;
using DoActions;
using Microsoft.Win32;
using PInvoke;
using QueryEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using SystemMenu;
using UsnJournal;



namespace tdsCshapu
{
    public partial class Form1 : Form
    {
        string USER_PROGRAM_PATH = "";//获取环境目录变量USER
        string ALLUSER_PROGRAM_PATH = "";   //获取环境目录变量ALLUSER
        public static Color Mdeep = Color.FromArgb(58, 64, 80);
        public static Color Hdeep = Color.FromArgb(48, 50, 63);
        public static Color Ldeep = Color.FromArgb(87, 97, 121);
        public static Color SHALLOW = Color.FromArgb(211, 82, 48);
        public static Color HIGHLIGHT = Color.FromArgb(233, 233, 233);

        const char Yii = '1';
        const char Ling = '0';
        const int Zongshu = 45; //筛选字符的个数
        public static List<Dictionary<UInt64, FrnFilePath>> fileList = new List<Dictionary<UInt64, FrnFilePath>>();   //file结果,list数组，元素是字典型数据库
        List<FrnFilePath> vlist = new List<FrnFilePath>() { };      //listview 绑定
        List<FrnFilePath> Record = new List<FrnFilePath>() { }; //记录相关


        public static Keys HOTK_SHOW = Keys.OemPeriod;
        private ListViewItem[] CurrentCacheItemsSource;
        private int firstitem = 0;
        public static int findmax = 100;  //最大显示数量
        readonly int mywidth = Screen.PrimaryScreen.Bounds.Width * 1 / 2;

        //listview 绑定
        static string keyword = string.Empty;  //全局关键词

        IEnumerable<DriveInfo> volumes;                //驱动器句柄
        string key1, key2;  //重复词组输入判断

        static Thread GoSearch;                    //全局化线程方便控制
        static Thread usnJournalThread; //全局化线程方便控制

        AutoResetEvent gOs = new AutoResetEvent(false);  //信号
        bool Threadrunning = false;  //线程运行标识
        bool Threadrest = false;   //线程重启标识


        bool IsActivated;
        bool LocatingWin;
        bool Txtselected = false;
        bool initialFinished = false;
        bool CUSTOMHIDE = true;



        #region Properties

        public List<NtfsUsnJournal> _usnJournal = new List<NtfsUsnJournal>() { };


        #endregion

        #region private member variables

        private List<Win32Api.USN_JOURNAL_DATA> _usnCurrentJournalState = new List<Win32Api.USN_JOURNAL_DATA>();



        #endregion

        #region delegates代理

        private delegate void StatusInfo(String rtnCode);
        private delegate void dosize(int rtnCode);
        private delegate void EnableTxt();

        #endregion







        public Form1()
        {

            InitializeComponent();
            IFileHelper.ListViewSysImages(istView1);
            SetDoubleBuffering(istView1, true);


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

        public static void SetDoubleBuffering(System.Windows.Forms.Control control, bool value)
        {
            System.Reflection.PropertyInfo controlProperty = typeof(System.Windows.Forms.Control)
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            controlProperty.SetValue(control, value, null);
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            this.istView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;//不显示表头
            Initial();



        }


        //#region 获取所有用户文件夹
        [DllImport("shfolder.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);
        private const int MAX_PATH = 260;
        private const int CSIDL_COMMON_PROGRAMS = 0x0017;
        public static string GetAllUsersDesktopFolderPath()
        {
            StringBuilder sbPath = new StringBuilder(MAX_PATH);
            SHGetFolderPath(IntPtr.Zero, CSIDL_COMMON_PROGRAMS, IntPtr.Zero, 0, sbPath);
            return sbPath.ToString();
        }

        //#endregion

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
            vlist.Clear(); fileList.Clear(); volumes = null; _usnJournal.Clear(); _usnCurrentJournalState.Clear();


            usnJournalThread = new Thread(ListFilesThreadStart)
            {
                IsBackground = true
            };
            usnJournalThread.Start();

        }

        public IEnumerable<DriveInfo> GetAllFixedNtfsDrives()
        {
            return DriveInfo.GetDrives()
                .Where(d => (d.IsReady == true && (d.DriveType == DriveType.Fixed || d.DriveType == DriveType.Removable) && d.DriveFormat.ToUpper() == "NTFS"));//固定磁盘
        }

        private void ListFilesThreadStart()
        {




            String msg = string.Empty;


            int dri_nums = -1;  //盘数


            volumes = GetAllFixedNtfsDrives();

            //获取环境变量及程序文件夹
            USER_PROGRAM_PATH = System.Environment.GetFolderPath(Environment.SpecialFolder.Programs,Environment.SpecialFolderOption.None);
            ALLUSER_PROGRAM_PATH = GetAllUsersDesktopFolderPath();

            dri_nums = volumes.Count();
            if (dri_nums > 0)
            {


                //DateTime startTime= DateTime.Now;

                for (int i = 0; i < dri_nums; i++)
                {

                    msg = ("建立索引..(" + volumes.ElementAt(i).Name.TrimEnd('\\').TrimEnd(':') + "盘)");
                    label1.BeginInvoke(new StatusInfo(ShowStatuesInfo), msg);

                    _usnJournal.Add(new NtfsUsnJournal(volumes.ElementAt(i)));

                    _usnCurrentJournalState.Add(new Win32Api.USN_JOURNAL_DATA());
                    if (!SaveJournalState(i))
                    {
                        _usnJournal.Last().CreateUsnJournal(1000 * 1024, 16 * 1024);  //尝试重建USN
                        if (!SaveJournalState(i))
                        {
                            MessageBox.Show("文件读取重建失败");
                        }

                    }


                    fileList.Add(_usnJournal.Last().GetNtfsVolumeAllentries(i, volumes.ElementAt(i).Name.TrimEnd('\\'), out NtfsUsnJournal.UsnJournalReturnCode rtnCode));

                    //重整parent索引
                    foreach (FrnFilePath f in fileList[i].Values)
                    {
                        if (f.parentFileReferenceNumber.HasValue && fileList[i].ContainsKey(f.parentFileReferenceNumber.Value))
                        {
                            if (f.parentFrn == null)
                            {
                                f.parentFrn = fileList[i][f.parentFileReferenceNumber.Value];
                                f.parentFileReferenceNumber = null;
                            }
                        }
                    }


                    foreach (FrnFilePath f in fileList[i].Values)
                    {
                        string path = GetPath(f);

                        try
                        {

                            UpdateTime(f, path);
                        }
                        catch
                        {
                        }

                        if (f != null)
                        {
                            string[] ext = f.fileName.Split('.');

                            if (ext.Last() == "LNK")
                            {
                                if (path.IndexOf(USER_PROGRAM_PATH, StringComparison.OrdinalIgnoreCase) > -1 || path.IndexOf(ALLUSER_PROGRAM_PATH, StringComparison.OrdinalIgnoreCase) > -1)
                                {
                                    { f.timestamp = long.MaxValue; }
                                }
                            }
                        }



                    }

                    fileList[i] = fileList[i].OrderByDescending(o => o.Value.timestamp).ToDictionary(p => p.Key, o => o.Value);

                }

            }
            readsets();  //记录相关* //



            StringBuilder drinfo = new StringBuilder();
            foreach (DriveInfo dr in volumes)
            {
                drinfo.Append(dr.Name + ",");
            }

            initialFinished = true;

            msg = "输入关键词:(" + drinfo.ToString().TrimEnd(',') + ")";
            label1.BeginInvoke(new StatusInfo(ShowStatuesInfo), msg);
            this.BeginInvoke(new EnableTxt(EnableCon));
            Gogosearch();
            //  System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            CompressMem(10);//内存缓存

        }


        private string GetTime(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                   return new FileInfo(path).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch
                {
                   
                }
            }
            
            return "";
            
        }

        private void UpdateTime(FrnFilePath f,string path="")
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = GetPath(f);
            }
            try
            {
            f.timestamp = new FileInfo(path).LastWriteTime.Ticks;
            }
            catch
            {
                f.timestamp = long.MinValue;
            }

            //if (File.Exists(path))
            //{
            //    f.timestamp = File.GetLastWriteTime(GetPath(f)).Ticks;
            //}
            //else if (Directory.Exists(path))
            //{
            //    f.timestamp = Directory.GetCreationTime(GetPath(f)).Ticks;
            //}
        }
        private void EnableCon()
        {
            Keywords.Enabled = true;
            Keywords.Focus();

            ShowRecord();  //启动完毕展示记录
        }


        public void Gogosearch()
        {
            if (initialFinished == true)
            {



                try
                {
                    GoSearch = new Thread(SearchFilesThreadStart)
                    {
                        IsBackground = true,
                        Priority = System.Threading.ThreadPriority.Highest
                    }; //启动搜索线程
                    GoSearch.Start();
                }
                catch
                {
                }
            }
        }


        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

        public void CompressMem(int mbsize)
        {
            try
            {
                Process Mem;
                Mem = Process.GetCurrentProcess();
                SetProcessWorkingSetSize(Mem.Handle, mbsize * 1024 * 1024, mbsize * 1024 * 1024);
            }
            catch
            {

            }
        }



        private void ShowStatuesInfo(string msg)
        {
            label1.Text = msg;

        }


        bool isAll = false;
        private void ListView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {

            if (CurrentCacheItemsSource != null && Threadrest == false && e.ItemIndex >= firstitem && e.ItemIndex < firstitem + CurrentCacheItemsSource.Length && e.ItemIndex - firstitem < CurrentCacheItemsSource.Length)
            {
                //A cache hit, so get the ListViewItem from the cache instead of making a new one.

                if (CurrentCacheItemsSource[e.ItemIndex - firstitem] != null) e.Item = CurrentCacheItemsSource[e.ItemIndex - firstitem];

            }
            if (e.Item == null)
            {
                if ((vlist != null && vlist.Count > 0 && e.ItemIndex < vlist.Count() && e.ItemIndex >= 0))
                {
                    FrnFilePath f = vlist[e.ItemIndex];
                    string name = getfile(f.fileName);
                    string path2 = GetPath(f);                    

                    if (f.IcoIndex.HasValue)
                    {
                        e.Item = GenerateListViewItem(f, name, path2);
                    }
                    else
                    {
                        f.IcoIndex = IFileHelper.FileIconIndex(@path2);
                        e.Item = GenerateListViewItem(f,name, path2);
                    }
                }


            }
            if (e.Item == null)
            {
                e.Item = new ListViewItem(new string[] { "", "" });
            }
        }

        private ListViewItem GenerateListViewItem(FrnFilePath f,string name,string path)
        {
            return new ListViewItem(new string[] { name, path, GetTime(path)}, f.IcoIndex.Value);
        }

        private void Keywords_TextChanged(object sender, EventArgs e)
        {

            Txtselected = false;
            key1 = Keywords.Text.Trim();
            if (!(key1 == key2))
            {
                dosearch(key1);
                key2 = key1;
            }

        }

        bool IFShowR = true;
        private void dosearch(string words)
        {
            Threadrest = true;
            if (string.IsNullOrWhiteSpace(words))
            {



                IFShowR = true;
                gOs.Set();
            }
            else
            {
                if (fileList.Count > 0)
                {
                    IFShowR = false;
                    keyword = words;
                    gOs.Set();


                }
            }
        }
        int txtH = 42;
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
        private void SearchFilesThreadStart()
        {
            Threadrunning = true;

            while (Threadrunning == true)
            {

            Restart:
                List<FrnFilePath> vvlist = new List<FrnFilePath>();      //listview 绑定的缓存
                string[] dwords = null;
                string[] words;
                int dlen = 0;
                int len;
                UInt64 unidwords = 0;
                UInt64 uniwords;
                bool DoDirectory = false;
                gOs.WaitOne();
                Threadrest = false;  //重启标签


                if (IFShowR == true) { this.BeginInvoke(new EnableTxt(ShowRecord)); continue; }


                int findnum = 0;


               

                string[] driverNames=null;
                if (keyword.Contains(":"))
                {
                    driverNames = (keyword.Split(':'))[0].Split(',');
                    keyword = (keyword.Split(':'))[1];
                }

                keyword = keyword.ToUpper().Replace("  ", " ").Replace("  ", " ");
                isAll = false;



                if (keyword.Contains(" /A")) { keyword = keyword.Replace(" /A", ""); isAll = true; }

                if (keyword.Contains("\\"))
                {

                    string[] tmp = keyword.Split('\\');
                    string tmpdword = tmp[0].Replace(" ", " ");
                    string tmpword = tmp[1].Replace(" ", " ");

                    dlen = tmpdword.Length;
                    len = tmpword.Length;
                    unidwords = TBS(SpellCN.GetSpellCode(tmpdword));
               
                    uniwords = TBS(SpellCN.GetSpellCode(tmpword));


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
                    words = keyword.Split(' ');
                    string tmpword = keyword.Replace(" ", "");
                    len = tmpword.Length;
                    uniwords = TBS(SpellCN.GetSpellCode(tmpword));

                }

                bool Finded;

                try
                {
                    if (DoUSNupdate == true)
                    {

                        for (int i = 0; i <= _usnJournal.Count - 1; i++)
                        {
                            try
                            {
                                DoWhileFileChanges(i);
                            }

                            catch
                            {
                                continue;
                            }
                        }
                        DoUSNupdate = false;

                    }

                    if (Threadrest == true) { goto Restart; }


                    for(int d= 0;d < fileList.Count;d++)
                    {

                        Dictionary<UInt64, FrnFilePath> l = fileList[d];
                        //并行过慢,PARALLEL.foreach,改单线程运行`


                        if (!(l.Values.Count > 0 && Directory.Exists(volumes.ToList()[d].Name))) continue;

                        if (driverNames != null) 
                        {
                            bool driverFound = false;
                            foreach (string driverName in driverNames)
                            {
                                if (driverName.ToUpper() == volumes.ToList()[d].Name.TrimEnd('\\').Trim(':').ToUpper())
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

                        foreach (FrnFilePath f in l.Values)
                        {
                            if (Threadrest == true) { goto Restart; }


                            Finded = true;


                            if (DoDirectory == true)
                            {

                                if (f.parentFileReferenceNumber.HasValue && l.TryGetValue(f.parentFileReferenceNumber.Value, out FrnFilePath dictmp))
                                {

                                    foreach (string key in dwords)
                                    {

                                        if (((unidwords | dictmp.keyindex) != dictmp.keyindex) || (dictmp.fileName.IndexOf(key, StringComparison.OrdinalIgnoreCase) < 0))
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

                            if (!Finded) { continue; }

                            foreach (string key in words)
                            {


                                if (((uniwords | f.keyindex) != f.keyindex) || (f.fileName.IndexOf(key, StringComparison.OrdinalIgnoreCase) < 0))
                                {
                                    Finded = false;
                                    break;
                                }

                            }

                            if (Finded)
                            {                               

                                findnum++;                               

                                vvlist.Add(f);

                                if (findmax != 0 && findnum > findmax && isAll == false) break;

                                if (findnum == 100)
                                {

                                    if (vvlist.Count < vlist.Count)
                                    {
                                        try
                                        {
                                            istView1.BeginInvoke(new System.EventHandler(listupdate_Cache), vvlist.Count);  //异步BeginInvoke
                                            vlist = vvlist.OrderByDescending(o=>o.timestamp).ToList();
                                        }
                                        catch { }

                                    }
                                    else
                                    {
                                        try
                                        {
                                            vlist = vvlist.OrderByDescending(o => o.timestamp).ToList();

                                            istView1.BeginInvoke(new System.EventHandler(listupdate_Cache), vvlist.Count);  //异步BeginInvoke
                                        }
                                        catch { }
                                    }

                                    refcache = true;
                                }
                            }
                        }
                    }//foreach

                }catch(Exception ex)
                {
                    Log_write(ex.Message);
                }


                if (vvlist.Count > 0)
                {

                    if (vvlist.Count < vlist.Count)
                    {

                        istView1.BeginInvoke(new System.EventHandler(listupdate), vvlist.Count);  //必须异步BeginInvoke，不然不同步
                        vlist = vvlist.OrderByDescending(o => o.timestamp).ToList();



                    }
                    else
                    {

                        vlist = vvlist.OrderByDescending(o => o.timestamp).ToList();
                        istView1.BeginInvoke(new System.EventHandler(listupdate), vvlist.Count);  //必须异步BeginInvoke，不然不同步
                    }

                }
                else
                {
                    istView1.BeginInvoke(new System.EventHandler(listupdate), 0);  //异步BeginInvoke
                }

                refcache = true;
                vvlist = null;
            }
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
            try
            {

                if (istView1.VirtualListSize > 0) istView1.Invalidate();  //刷新cache内核不，然不显示

            }
            catch
            {

            }

            if ((findmax != 0 && size > findmax) && isAll == false) { ShowStatuesInfo("搜索到" + findmax.ToString() + "+个对象"); }
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
        /// <summary>
        /// 查询并跟踪USN状态，更新后保存当前状态再继续跟踪
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool SaveJournalState(int index)        //保存USN状态
        {

            Win32Api.USN_JOURNAL_DATA journalState = new Win32Api.USN_JOURNAL_DATA();
            NtfsUsnJournal.UsnJournalReturnCode rtn = _usnJournal[index].GetUsnJournalState(ref journalState);
            if (rtn == NtfsUsnJournal.UsnJournalReturnCode.USN_JOURNAL_SUCCESS)
            {
                _usnCurrentJournalState[index] = journalState;
                return true;
            }
            return false;
        }

        private void DoWhileFileChanges(int index)  //筛选USN状态改变
        {
            if (_usnCurrentJournalState[index].UsnJournalID != 0)
            {

                uint reasonMask = Win32Api.USN_REASON_FILE_CREATE | Win32Api.USN_REASON_FILE_DELETE | Win32Api.USN_REASON_RENAME_NEW_NAME|Win32Api.USN_REASON_DATA_OVERWRITE|Win32Api.USN_REASON_DATA_EXTEND|Win32Api.USN_REASON_DATA_TRUNCATION;
                _ = _usnJournal[index].GetUsnJournalEntries(_usnCurrentJournalState[index], reasonMask, out List<Win32Api.UsnEntry> usnEntries, out Win32Api.USN_JOURNAL_DATA newUsnState);


                foreach (Win32Api.UsnEntry f in usnEntries)
                {
                    uint value;
                    value = f.Reason & Win32Api.USN_REASON_RENAME_NEW_NAME;

                    if (0 != value && fileList.Count > index)
                    {
                        if (fileList[index].ContainsKey(f.FileReferenceNumber) && fileList[index].ContainsKey(f.ParentFileReferenceNumber))
                        {
                            string nacn = SpellCN.GetSpellCode(f.Name.ToUpper());
                            fileList[index][f.FileReferenceNumber].keyindex = TBS(nacn);
                            fileList[index][f.FileReferenceNumber].fileName = f.Name + "|" + nacn;
                            fileList[index][f.FileReferenceNumber].parentFrn = fileList[index][f.ParentFileReferenceNumber];
                            UpdateTime(fileList[index][f.FileReferenceNumber]);
                        }
                    }

                    value = f.Reason & (Win32Api.USN_REASON_DATA_OVERWRITE | Win32Api.USN_REASON_DATA_EXTEND| Win32Api.USN_REASON_DATA_TRUNCATION);
                    if (0 != value && fileList.Count > index)
                    {
                        if (fileList[index].ContainsKey(f.FileReferenceNumber) && fileList[index].ContainsKey(f.ParentFileReferenceNumber))
                        {                            
                            UpdateTime(fileList[index][f.FileReferenceNumber]);
                        }
                    }

                    value = f.Reason & Win32Api.USN_REASON_FILE_CREATE;
                    if (0 != value && fileList.Count > index)
                    {
                        if (!fileList[index].ContainsKey(f.FileReferenceNumber) && !string.IsNullOrWhiteSpace(f.Name) && fileList[index].ContainsKey(f.ParentFileReferenceNumber))
                        {                            
                            string nacn = SpellCN.GetSpellCode(f.Name.ToUpper());
                            fileList[index].Add(f.FileReferenceNumber, new FrnFilePath(f.FileReferenceNumber,null, f.Name + "|" + nacn, null));
                            fileList[index][f.FileReferenceNumber].SetVolandVolName(index, volumes.ElementAt(index).Name.TrimEnd('\\'));
                            fileList[index][f.FileReferenceNumber].keyindex = TBS(nacn);
                            fileList[index][f.FileReferenceNumber].parentFrn = fileList[index][f.ParentFileReferenceNumber];
                            UpdateTime(fileList[index][f.FileReferenceNumber]);
                        }
                    }

                    value = f.Reason & Win32Api.USN_REASON_FILE_DELETE;
                    if (0 != value && fileList.Count > index)
                    {
                        if (fileList[index].ContainsKey(f.FileReferenceNumber))
                        {
                            fileList[index].Remove(f.FileReferenceNumber);
                        }
                    }
                    _usnCurrentJournalState[index] = newUsnState;   //更新状态
                }
            }
        }
        /// <summary>
        ///  二进制转换逻辑搜索使用
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>

        static char[] alphbet = { '@', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '-', '_', '[', ']', '(', ')', '/' };
        public static string getfile(string filename)
        {
            if (filename.Length > 0) { string[] fn = filename.Split('|'); if (fn.GetUpperBound(0) > 0) { return fn[0]; } }

            return string.Empty;
        }


        public static UInt64 TBS(string txt)
        {
            char[] alph = new char[Zongshu];

            for (int i = 0; i < Zongshu; i++)
            {
                if (txt.Contains(alphbet[i])) { alph[i] = Yii; } else { alph[i] = Ling; }
            }
            return Convert.ToUInt64(new string(alph), 2);
        }



        private void ListView1_MouseClick(object sender, MouseEventArgs e)
        {



            if (e.Button == MouseButtons.Right)
            {

                if (Control.ModifierKeys == Keys.Shift && (istView1.VirtualListSize > 0) && (istView1.SelectedIndices.Count == 1))
                {

                    //好像自动识别了多文件
                    FrnFilePath f = vlist[istView1.SelectedIndices[0]];
                    if (!(f == null))
                    {
                        string path = GetPath(f);
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


        private void Golistview(int index)
        {

            //建立复制文件列表



            if (istView1.SelectedIndices.Count > 0)
            {
                ListView.SelectedIndexCollection collects = istView1.SelectedIndices;
                System.Collections.Specialized.StringCollection Copies = new System.Collections.Specialized.StringCollection();
                StringBuilder pathcopies = new StringBuilder();


                foreach (int x in collects)
                {
                    try
                    {

                        FrnFilePath f;
                        switch (index)
                        {
                            case 0:  //打开文件
                                f = vlist[x];
                                if (!(f == null))
                                {

                                    string path = string.Empty;
                                    //识别
                                    try
                                    {
                                        path = GetPath(f);

                                    }
                                    catch
                                    {
                                        ifhide = false;
                                        MessageBox.Show("打开失败");
                                        break;
                                    }

                                    if (Path.GetExtension(getfile(f.fileName)).Length == 0)
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
                                                    Process.Start("explorer.exe",path);

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
                                                    Process.Start("explorer.exe", path);

                                                }
                                                catch(Exception ex)
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
                                f = vlist[x];
                                if (f != null)
                                {
                                    string path = string.Empty;
                                    try
                                    {
                                        path = Path.GetDirectoryName(GetPath(f));
                                        
                                        Process.Start("explorer.exe", path);
                                        UpdateRecord(f);//记录相关* // 

                                    }
                                    catch(Exception ex)
                                    {
                                        ifhide = false;

                                        MessageBox.Show(ex.Message);
                                    }
                                }
                                break;

                            case 2:

                                f = vlist[x];
                                if (f != null)
                                {
                                    string path = GetPath(f);
                                    UpdateRecord(f);//记录相关* // 
                                    Copies.Add(path);

                                }
                                break;
                            case 3:
                                f = vlist[x];
                                if (f != null)
                                {

                                    string path =GetPath(f);
                                    UpdateRecord(f); //记录相关* //
                                    pathcopies.Append(path).Append("\r\n");
                                }
                                break;

                            case 4:

                                f = vlist[x];
                                if (!(f == null))
                                {
                                    string path = GetPath(f);
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
                                f = vlist[x];
                                if (!(f == null))
                                {


                                    UpdateRecord(f);
                                    pathcopies.Append(getfile(f.fileName)).Append("\r\n");
                                }
                                break;
                            case 6:
                                f = vlist[x];
                                if (!(f == null))
                                {

                                    string path = Path.GetDirectoryName(GetPath(f));
                                    UpdateRecord(f); //记录相关* // 
                                    pathcopies.Append(path).Append("\r\n");
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


        /// <summary>
        /// 热键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 


        [DllImportAttribute("user32.dll", EntryPoint = "UnregisterHotKey")]
        public static extern bool UnregisterHotKey
            (
                IntPtr hWnd,        //注册热键的窗口句柄        
                int id              //热键编号上面注册热键的编号        
            );

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint keyValue, Keys vk);
        // [DllImport("user32.dll")]   //0512


        //private static extern bool SetForegroundWindow(IntPtr hWnd);

        //[DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        //static extern int ShowWindow(IntPtr hWnd, uint nCmdShow);

        const int HTLEFT = 10;
        const int HTRIGHT = 11;
        const int HTTOP = 12;
        const int HTTOPLEFT = 13;
        const int HTTOPRIGHT = 14;
        const int HTBOTTOM = 15;
        const int HTBOTTOMLEFT = 0x10;
        const int HTBOTTOMRIGHT = 17;
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


            }
            else
            {
                ifhide = true;


                this.Visible = true;

                //int q;   //激活内存开销
                //foreach (Dictionary<UInt64, FrnFilePath> l in fileList)
                //{

                //    foreach (FrnFilePath f in l.Values)
                //    {
                //        q=f.fileName.Length;

                //    } 
                //}

                if (this.Width < 100) this.Width = 300;
                if (this.Height < 100) this.Height = 200;
                this.Activate();

                Keywords.Focus();
                Keywords.SelectAll();



                // CompressMem(50);

                //    ShowWindow(this.Handle, 1);
                //SetForegroundWindow(this.Handle);
                //ListView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                //  RegisterHotKey(Handle, 8617, 2, Keys.OemPeriod);

            }
        }
        public bool DoUSNupdate = false;
        private void Form1_Activated(object sender, EventArgs e)
        {
            //*****搜索完毕后更新USN进行更新

            if ((initialFinished == true) && (IsActivated == false))
            {

                IsActivated = true;
                ifhide = true;


                DoUSNupdate = true;

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
        public bool ifhide = true;
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
                case Keys.Back:
                    if (istView1.SelectedIndices.Count > 0)
                    {
                        //RightMenu.Show(Cursor.Position);
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

                if (Record.Count > 1) { Record.Sort(new CComparer()); }

                while (Record.Count > 50)
                {
                    Record.RemoveAt(Record.Count - 1);
                }


                if (Record.Count < vlist.Count)
                {
                    //this.BeginInvoke(new dosize(Sizecalc), Record.Count);
                    vlist = Record;
                    this.BeginInvoke(new System.EventHandler(Recordupdate), Record.Count);  //异步invoke

                }
                else
                {
                    vlist = Record;
                    this.BeginInvoke(new System.EventHandler(Recordupdate), Record.Count);
                    //this.BeginInvoke(new dosize(Sizecalc), Record.Count);

                }

            }
            else
            {
                this.BeginInvoke(new System.EventHandler(Recordupdate), 0);
                // this.BeginInvoke(new dosize(Sizecalc), Record.Count);
            }
            refcache = true;


        }

        public class CComparer : IComparer<FrnFilePath>
        {
            public int Compare(FrnFilePath left, FrnFilePath right)
            {
                if (left.weight > right.weight)
                    return -1;
                else if (left.weight == right.weight)
                    return 0;
                else
                    return 1;
            }
        }

        //记录相关*
        private void UpdateRecord(FrnFilePath targ)
        {
            bool existed = false;
            if (Record.Count > 0)
            {
                for (int i = 0; i < Record.Count; i++)
                {
                    if ((Record[i].fileReferenceNumber == targ.fileReferenceNumber) || (Record[i].parentFileReferenceNumber == targ.parentFileReferenceNumber && Record[i].fileName == targ.fileName))
                    {
                        Record[i].fileReferenceNumber = targ.fileReferenceNumber;
                        Record[i].fileName = targ.fileName;

                        existed = true;

                        if (Record[i].weight < 600)
                        {
                            Record[i].weight = (short)(Record[i].weight + 50);
                        }
                    }
                    else
                    {
                        Record[i].weight = (short)(Record[i].weight - 5);

                    }
                    if (Record[i].weight < 10)
                    {
                        Record[i].weight = 10;
                    }


                }
            }
            if (existed == false)
            {
                targ.weight = 50;

                if (Path.GetExtension(getfile(targ.fileName)).Equals(".xls", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(getfile(targ.fileName)).Equals(".xlsx", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(getfile(targ.fileName)).Equals(".doc", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(getfile(targ.fileName)).Equals(".docx", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(getfile(targ.fileName)).Equals(".ppt", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(getfile(targ.fileName)).Equals(".pptx", StringComparison.OrdinalIgnoreCase))
                {
                    FrnFilePath tmp = new FrnFilePath(targ.fileReferenceNumber, targ.parentFileReferenceNumber, targ.fileName)
                    {
                        Volume = targ.Volume,
                        VolumeName = targ.VolumeName,
                        weight = targ.weight
                    };
                    Record.Add(tmp);
                }
                else
                {
                    Record.Add(targ);

                }

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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (StreamWriter fs = new StreamWriter(path, false, System.Text.Encoding.GetEncoding("gb2312")))
            {
                foreach (FrnFilePath f in Record)
                {
                    string fp = GetPath(f);
                    if (File.Exists(fp) || Directory.Exists(fp))
                    {
                        fs.WriteLine(f.fileReferenceNumber.ToString() + "@" + f.parentFileReferenceNumber.Value.ToString() + "@" + f.fileName + "@" + f.Volume + "@" + f.VolumeName + "@" + f.weight);
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

                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    using (StreamReader fs = new StreamReader(path, System.Text.Encoding.GetEncoding("gb2312")))
                    {
                        Record.Clear();
                        while (!fs.EndOfStream)
                        {
                            // try
                            {
                                string[] KeyValue = fs.ReadLine().Split('@');

                                if (KeyValue != null && KeyValue.GetUpperBound(0) == 5)
                                {
                                    if (Path.GetExtension(getfile(KeyValue[2])).Equals(".xls", StringComparison.OrdinalIgnoreCase) ||
                     Path.GetExtension(getfile(KeyValue[2])).Equals(".xlsx", StringComparison.OrdinalIgnoreCase) ||
                     Path.GetExtension(getfile(KeyValue[2])).Equals(".doc", StringComparison.OrdinalIgnoreCase) ||
                     Path.GetExtension(getfile(KeyValue[2])).Equals(".docx", StringComparison.OrdinalIgnoreCase) ||
                     Path.GetExtension(getfile(KeyValue[2])).Equals(".ppt", StringComparison.OrdinalIgnoreCase) ||
                     Path.GetExtension(getfile(KeyValue[2])).Equals(".pptx", StringComparison.OrdinalIgnoreCase))
                                    {   //特殊文件
                                        FrnFilePath f = new FrnFilePath(UInt64.Parse(KeyValue[0]), UInt64.Parse(KeyValue[1]), KeyValue[2])
                                        {

                                            Volume = short.Parse(KeyValue[3]),
                                            VolumeName = KeyValue[4].ToCharArray()[0],
                                            weight = short.Parse(KeyValue[5])
                                        };
                                        for (int i = 0; i < 10; i++)
                                        {
                                            GetPath(f);


                                        }
                                        Record.Add(f);

                                    }
                                    else  //其他文件
                                    {
                                        if ((fileList[int.Parse(KeyValue[3])].TryGetValue(UInt64.Parse(KeyValue[0]), out FrnFilePath f)))
                                        {
                                            f.weight = Int16.Parse(KeyValue[5]);
                                            Record.Add(f);
                                        }
                                    }
                                }
                            }
                            // catch
                            {

                            }
                        }
                        fs.Close();
                    }
                }
                //  catch { }
            }
        }

        bool Clickdown = false;
        int mouseX = 0, mouseY = 0;

        private void ListView1_MouseDown(object sender, MouseEventArgs e)
        {
            Clickdown = true;
            mouseX = e.X;
            mouseY = e.Y;
        }

        static string GetPath(FrnFilePath f)
        {          
             string path= MyFunctions.GetPathStr(f);
            if (path.EndsWith(':'))
            {
                return path+"\\";
            }
            else
            {
                return path;
            }
        }

        static string _GetPath2(FrnFilePath f)
        {

            if (f.parentFileReferenceNumber.HasValue)
            {
                string fn = "";
                if (fileList[f.Volume].ContainsKey(f.parentFileReferenceNumber.Value))
                {
                    fn = getfile(fileList[f.Volume][f.parentFileReferenceNumber.Value].fileName);
                }
                if (fn.Length == 0) { return string.Empty; }
                return MyFunctions.GetPathLink(f.Volume, f.parentFileReferenceNumber.Value) + "\\" + fn;
            }
            else
            { return string.Empty; }
        }


        private ListViewItem prItem;
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
                    FrnFilePath f;
                    foreach (int x in collects)
                    {
                        f = vlist[x];
                        if (!(f == null))
                        {
                            string path =GetPath(f);
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
                    catch { }

            }
        }

        private void ListView1_MouseUp(object sender, MouseEventArgs e)
        {
            Clickdown = false;
            mouseX = 0;
            mouseY = 0;
        }

             



        private void 开机启动SToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void 关于LeahyGoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About();
        }
        private void About()
        {
            string ver = "5.0924.10381";
            ifhide = false;
            MessageBox.Show("版本号:" + ver + "\r\nLJ@Nanjing@20211210");
        }
        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Golistview(0);
            }
            catch(Exception ex)
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
        System.Drawing.Point XY = new System.Drawing.Point();

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
                打开OToolStripMenuItem.Text = "批量打开选定项目(&O)";
                打开文件夹DToolStripMenuItem.Text = "批量打开选定项目所在文件夹(&D)";
                复制文件FToolStripMenuItem.Text = "批量复制选定项目(&F)";
                复制文件路径CToolStripMenuItem.Text = "批量复制路径(&C)";
                删除文件MToolStripMenuItem.Text = "批量删除该" + fnum.ToString() + "个选定项目(&M)";
                复制文件名NToolStripMenuItem.Text = "批量复制文件名(&N)";
                复制目录路径EToolStripMenuItem.Text = "批量复制目录路径(&E)";
            }
            else if (fnum == 1)
            {
                调用系统菜单ToolStripMenuItem.Visible = true;
                toolStripSeparator2.Visible = true;
                FrnFilePath f = vlist[istView1.SelectedIndices[0]];
                if (!(f == null))
                {
                    toolStripTextBox1.Text = getfile(f.fileName);
                    toolStripTextBox1.Enabled = true; ;
                    打开OToolStripMenuItem.Text = "打开(&O)";
                    打开文件夹DToolStripMenuItem.Text = "打开项目所在文件夹(&D)";
                    复制文件FToolStripMenuItem.Text = "复制该选定项目(&F)";
                    复制文件路径CToolStripMenuItem.Text = "复制路径(&C)";
                    删除文件MToolStripMenuItem.Text = "删除选定项目(&M)";
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
                FrnFilePath f = vlist[istView1.SelectedIndices[0]];
                if (!(f == null))
                {
                    string path = GetPath(f);
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

                FrnFilePath f = vlist[istView1.SelectedIndices[0]];
                if (toolStripTextBox1.Text != getfile(f.fileName))
                {
                    ifhide = false;
                    if (MessageBox.Show("是否重命名?", "重命名", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {

                        if (!(f == null))
                        {
                            string path = GetPath(f);
                            string pathnew =Path.GetDirectoryName(GetPath(f))+"\\"+toolStripTextBox1.Text;
                            if (Path.GetExtension(getfile(f.fileName)).Length == 0)
                            {
                                if (Directory.Exists(path))
                                {


                                    try
                                    {
                                        Directory.Move(path, pathnew);



                                        string nacn = SpellCN.GetSpellCode(System.IO.Path.GetFileName(pathnew).ToUpper());
                                        f.keyindex = TBS(nacn);
                                        f.fileName = string.Format("{0}|{1}", System.IO.Path.GetFileName(pathnew), nacn);

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

                                        string nacn = SpellCN.GetSpellCode(System.IO.Path.GetFileName(pathnew).ToUpper());
                                        f.keyindex = TBS(nacn);
                                        f.fileName = string.Format("{0}|{1}", System.IO.Path.GetFileName(pathnew), nacn);
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


        //窗口尺寸
        //改变窗体大小


        //改变窗体大小



        #region //移动窗体
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        #endregion
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

        public bool refcache = false;
        private void IstView1_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
          
            if ((CurrentCacheItemsSource != null && e.StartIndex >= firstitem && e.EndIndex <= firstitem + CurrentCacheItemsSource.Length))
            {
                if (refcache == false) { return; }
            }

            firstitem = e.StartIndex;
            int length = e.EndIndex - e.StartIndex + 1;
            CurrentCacheItemsSource = null;
            CurrentCacheItemsSource = new ListViewItem[length];


            for (int i = 0; i < length; i++)
            {
                if (i + firstitem < vlist.Count)
                {

                    FrnFilePath f = vlist[i + firstitem];
                    string name = getfile(f.fileName);
                    string path2 =GetPath(f);
                  

                    if (f.IcoIndex.HasValue)
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
                                int ext = 0;
                                try
                                {
                                    ext = IFileHelper.FileIconIndex(@path2);
                                    f.IcoIndex = ext;
                                }
                                catch
                                { }
                                CurrentCacheItemsSource[i] = GenerateListViewItem(f, name, path2);
                                continue;

                            }
                            else
                            {
                                int ext = 0;
                                try
                                {
                                    ext = IFileHelper.FileIconIndex(exten);//exten
                                    f.IcoIndex = ext;
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

        private void IstView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void IstView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {

        }

        private void Label1_DoubleClick(object sender, EventArgs e)
        {

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
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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


    }
}