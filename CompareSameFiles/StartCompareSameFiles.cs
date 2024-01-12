using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Presentation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TDSNET.Engine.Actions;
using TDSNET.Engine.Actions.USN;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static TDSNET.CompareSameFiles.PickSameFiles;
using static TDSNET.CompareSameFiles.PickSameFiles.SameFileInfo;

namespace TDSNET.CompareSameFiles
{
    public partial class StartCompareSameFiles : Form
    {
        readonly IList<FileSys> fileSysList;
        public StartCompareSameFiles(IList<FileSys> fileSysList)
        {
            InitializeComponent();
            this.fileSysList = fileSysList;
            IFileHelper.ListViewSysImages(listView1);
            IFileHelper.ListViewSysImages(listView2);

        }

        private void StartCompareSameFiles_Load(object sender, EventArgs e)
        {

        }


        string title = "就绪";

        public static string FileSizeToString(long bytes)
        {
            var order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizePostfix.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return string.Format("{0:0.#} {1}", len, sizePostfix[order]);
        }

        private static string[] sizePostfix = { "bytes", "KB", "MB", "GB", "TB" };

        IList<SameFileInfo> files;


        private  void UpdateUSN(IList<FileSys> fileSysList)
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
        private void button1_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            
            UpdateUSN(fileSysList);

            listView1.Items.Clear();
            listView2.Items.Clear();
            textBox3.Clear();
            this.Invalidate();
            var filter = textBox1.Text;
            long size = 1024;
            long.TryParse(textBox2.Text, out size);
            textBox2.Text = size.ToString();

            Task.Run(() =>
            {
                files = PickSameFiles.Comparer(fileSysList, filter, size, (s) => { this.Invoke(() => { this.Text = s; }); }).ToArray();
                this.Invoke(RefreshList2);
                this.Invoke(() =>
                {
                    MessageBox.Show("Done");

                    this.Enabled = true;
                });

            });

        }

        private void RefreshList2()
        {
            listView1.Items.Clear();
            listView2.Items.Clear();
            textBox3.Clear();

            listView2.BeginUpdate();
            for (int i = 0; i < files.Count; i++)
            {
                var file = files.ElementAt(i);
                var fileName = Path.GetFileName(file.FileList.FirstOrDefault().Path);
                var imgIndex = IFileHelper.FileIconIndex(fileName, Path.GetExtension(fileName));

                listView2.Items.Add(new ListViewItem(new string[] { i.ToString(), fileName, FileSizeToString(file.Size), file.FileList.Count.ToString() }, imgIndex));
            }
            listView2.EndUpdate();
            title = $"共{files.Count}重复项";
        }

        private void RefreshList1()
        {
            listView1.Items.Clear();
            textBox3.Clear();


            if (listView2.SelectedIndices.Count > 0)
            {
                listView1.BeginUpdate();

                var file = files.ElementAt(listView2.SelectedIndices[0]);

                for (int i = 0; i < file.FileList.Count; i++)
                {
                    var origin = file.FileList[i];
                    var fileName = Path.GetFileName(origin.Path);
                    var imgIndex = IFileHelper.FileIconIndex(fileName, Path.GetExtension(fileName));
                    listView1.Items.Add(new ListViewItem(new string[] { fileName, origin.Path }, imgIndex));
                }

                listView1.EndUpdate();
            }
        }

        private void RefreshTextbox3()
        {
            if (listView2.SelectedIndices.Count > 0)
            {
                if (listView1.SelectedIndices.Count > 0)
                {
                    var file = files.ElementAt(listView2.SelectedIndices[0]).FileList[listView1.SelectedIndices[0]];
                    textBox3.Text = $"FilePath:{file.Path}\r\nFileSize:{FileSizeToString(files.ElementAt(listView2.SelectedIndices[0]).Size)}";
                }
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.SelectedIndices.Count > 0)
            {
                RefreshList1();
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                RefreshTextbox3();
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.View == View.LargeIcon)
            {
                listView1.View = View.Details;
            }
            else
            {
                listView1.View = View.SmallIcon;
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenOneFile();
        }

        private void OpenOneFile()
        {
            if (listView2.SelectedIndices.Count == 1)
            {
                if (listView1.SelectedIndices.Count == 1)
                {
                    var file = files.ElementAt(listView2.SelectedIndices[0]).FileList[listView1.SelectedIndices[0]];
                    if (File.Exists(file.Path))
                    {
                        if (string.Equals(Path.GetExtension(file.Path), ".exe", StringComparison.OrdinalIgnoreCase))
                        {
                            Process.Start(file.Path);
                        }
                        else
                        {
                            Process.Start(@"explorer.exe", file.Path);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"文件不存在\r\n{file.Path}");
                    }
                }
            }
        }

        private void DeleteFiles()
        {
            if (listView2.SelectedIndices.Count == 1)
            {
                if (listView1.SelectedIndices.Count > 0)
                {
                    List<FrnFileWithPath> filesPath = new List<FrnFileWithPath>();

                    var f = files.ElementAt(listView2.SelectedIndices[0]).FileList;
                    for (int i = 0; i < listView1.SelectedIndices.Count; i++)
                    {
                        filesPath.Add(f[listView1.SelectedIndices[i]]);
                    }

                    StringBuilder stringBuilder = new StringBuilder();
                    foreach(var frn in filesPath)
                    {
                        stringBuilder.AppendLine(frn.Path);
                    }

                    if (MessageBox.Show("是否删除以下文件?\r\n"+stringBuilder.ToString(), "确认", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        foreach (var frn in filesPath)
                        {
                            try
                            {
                                File.Delete(frn.Path);
                                f.Remove(frn);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("删除错误: " + ex.Message + "\r\n" + frn);
                            }
                        }
                    }
                    RefreshList1();
                }
            }
        }

        private void OpenOneFileDirectory()
        {
            if (listView2.SelectedIndices.Count == 1)
            {
                if (listView1.SelectedIndices.Count == 1)
                {
                    var file = files.ElementAt(listView2.SelectedIndices[0]).FileList[listView1.SelectedIndices[0]];
                    if (File.Exists(file.Path))
                    {
                        Process.Start(@"explorer.exe", "/select,\"" + file.Path + "\"");
                    }
                    else
                    {
                        MessageBox.Show($"文件不存在\r\n{file.Path}");
                    }
                }
            }
        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (files == null || files.Count == 0)
            {
                return;
            }

            System.Windows.Forms.ListView lv = sender as System.Windows.Forms.ListView;
            // 检查点击的列是不是现在的排序列.
            switch (e.Column)
            {
                case 1:
                    {
                        files = files.OrderBy(o => Path.GetFileName(o.FileList.FirstOrDefault()?.Path ?? "")).ToArray();
                        RefreshList2();
                        break;
                    }
                case 2:
                    {
                        files = files.OrderBy(o => o.Size).ToArray();
                        RefreshList2();
                        break;
                    }
                case 3:
                    {
                        files = files.OrderBy(o => o.FileList.Count).ToArray();
                        RefreshList2();
                        break;
                    }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenOneFile();
        }

        private void 打开目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenOneFileDirectory();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (listView2.SelectedIndices.Count != 1)
            {
                打开ToolStripMenuItem.Visible = false;
                打开目录ToolStripMenuItem.Visible = false;
            }
            else
            {
                打开ToolStripMenuItem.Visible = true;
                打开目录ToolStripMenuItem.Visible = true;
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteFiles();
        }
    }
}
