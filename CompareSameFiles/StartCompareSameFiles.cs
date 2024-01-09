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

        ConcurrentBag<SameFileInfo> files;
        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            listView2.Items.Clear();
            textBox3.Clear();
            this.Invalidate();
            var filter = textBox1.Text;
            long size = 1024;
            long.TryParse(textBox2.Text, out size);
            textBox2.Text = size.ToString();
            files = PickSameFiles.Comparer(fileSysList, filter, size);
            RefreshList2();
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

                listView2.Items.Add(new ListViewItem(new string[] { fileName,file.FileList.Count.ToString(), FileSizeToString(file.Size) }, imgIndex));
            }
            listView2.EndUpdate();
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
            if(listView1.View==View.LargeIcon)
            {
                listView1.View = View.Details;
            }
            else
            {
                listView1.View = View.LargeIcon;
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView2.SelectedIndices.Count > 0)
            {
                if (listView1.SelectedIndices.Count > 0)
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
    }
}
