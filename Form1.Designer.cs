
namespace tdsCshapu
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ColumnHeader columnHeader1;
            System.Windows.Forms.ColumnHeader columnHeader2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.istView1 = new System.Windows.Forms.ListView();
            this.RightMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.打开OToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开文件夹DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.复制文件FToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.复制文件名NToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.复制文件路径CToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.复制目录路径EToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.刷新RToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.删除文件MToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.调用系统菜单ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Keywords = new System.Windows.Forms.TextBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.显示主界面SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.开机启动SToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.缓存刷新CToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于LToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出QToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.RightMenu.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnHeader1
            // 
            columnHeader1.Name = "columnHeader1";
            columnHeader1.Text = "文件名";
            // 
            // columnHeader2
            // 
            columnHeader2.Name = "columnHeader2";
            columnHeader2.Text = "目录";
            // 
            // istView1
            // 
            this.istView1.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.istView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.istView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.istView1.CausesValidation = false;
            this.istView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader1,
            columnHeader2,
            this.columnHeader3});
            this.istView1.ContextMenuStrip = this.RightMenu;
            this.istView1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.istView1.ForeColor = System.Drawing.Color.DimGray;
            this.istView1.FullRowSelect = true;
            this.istView1.HideSelection = false;
            this.istView1.LabelWrap = false;
            this.istView1.Location = new System.Drawing.Point(4, 35);
            this.istView1.Name = "istView1";
            this.istView1.Size = new System.Drawing.Size(657, 388);
            this.istView1.TabIndex = 0;
            this.istView1.UseCompatibleStateImageBehavior = false;
            this.istView1.View = System.Windows.Forms.View.Details;
            this.istView1.VirtualMode = true;
            this.istView1.CacheVirtualItems += new System.Windows.Forms.CacheVirtualItemsEventHandler(this.IstView1_CacheVirtualItems);
            this.istView1.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.ListView1_RetrieveVirtualItem);
            this.istView1.SelectedIndexChanged += new System.EventHandler(this.istView1_SelectedIndexChanged_1);
            this.istView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListView1_KeyDown);
            this.istView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ListView1_MouseClick);
            this.istView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListView1_MouseDoubleClick);
            this.istView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListView1_MouseDown);
            this.istView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListView1_MouseMove);
            this.istView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ListView1_MouseUp);
            // 
            // RightMenu
            // 
            this.RightMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox1,
            this.toolStripSeparator4,
            this.打开OToolStripMenuItem,
            this.打开文件夹DToolStripMenuItem,
            this.复制文件FToolStripMenuItem,
            this.复制文件名NToolStripMenuItem,
            this.复制文件路径CToolStripMenuItem,
            this.复制目录路径EToolStripMenuItem,
            this.toolStripSeparator1,
            this.刷新RToolStripMenuItem,
            this.toolStripSeparator2,
            this.删除文件MToolStripMenuItem,
            this.toolStripSeparator3,
            this.调用系统菜单ToolStripMenuItem});
            this.RightMenu.Name = "RightMenu";
            this.RightMenu.Size = new System.Drawing.Size(180, 251);
            this.RightMenu.Opening += new System.ComponentModel.CancelEventHandler(this.RightMenu_Opening);
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 23);
            this.toolStripTextBox1.Enter += new System.EventHandler(this.ToolStripTextBox1_Enter);
            this.toolStripTextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBox1_KeyDown);
            this.toolStripTextBox1.Click += new System.EventHandler(this.ToolStripTextBox1_Click_1);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(176, 6);
            // 
            // 打开OToolStripMenuItem
            // 
            this.打开OToolStripMenuItem.Name = "打开OToolStripMenuItem";
            this.打开OToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.打开OToolStripMenuItem.Text = "打开文件(%O)";
            this.打开OToolStripMenuItem.Click += new System.EventHandler(this.打开OToolStripMenuItem_Click);
            // 
            // 打开文件夹DToolStripMenuItem
            // 
            this.打开文件夹DToolStripMenuItem.Name = "打开文件夹DToolStripMenuItem";
            this.打开文件夹DToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.打开文件夹DToolStripMenuItem.Text = "打开目录(&D)";
            this.打开文件夹DToolStripMenuItem.Click += new System.EventHandler(this.打开文件夹DToolStripMenuItem_Click);
            // 
            // 复制文件FToolStripMenuItem
            // 
            this.复制文件FToolStripMenuItem.Name = "复制文件FToolStripMenuItem";
            this.复制文件FToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.复制文件FToolStripMenuItem.Text = "复制文件(&C)";
            this.复制文件FToolStripMenuItem.Click += new System.EventHandler(this.复制文件FToolStripMenuItem_Click);
            // 
            // 复制文件名NToolStripMenuItem
            // 
            this.复制文件名NToolStripMenuItem.Name = "复制文件名NToolStripMenuItem";
            this.复制文件名NToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.复制文件名NToolStripMenuItem.Text = "复制文件名(&N)";
            this.复制文件名NToolStripMenuItem.Click += new System.EventHandler(this.复制文件名NToolStripMenuItem_Click);
            // 
            // 复制文件路径CToolStripMenuItem
            // 
            this.复制文件路径CToolStripMenuItem.Name = "复制文件路径CToolStripMenuItem";
            this.复制文件路径CToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.复制文件路径CToolStripMenuItem.Text = "复制文件路径(F)";
            this.复制文件路径CToolStripMenuItem.Click += new System.EventHandler(this.复制文件路径CToolStripMenuItem_Click);
            // 
            // 复制目录路径EToolStripMenuItem
            // 
            this.复制目录路径EToolStripMenuItem.Name = "复制目录路径EToolStripMenuItem";
            this.复制目录路径EToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.复制目录路径EToolStripMenuItem.Text = "复制目录路径（&E）";
            this.复制目录路径EToolStripMenuItem.Click += new System.EventHandler(this.复制目录路径EToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(176, 6);
            // 
            // 刷新RToolStripMenuItem
            // 
            this.刷新RToolStripMenuItem.Name = "刷新RToolStripMenuItem";
            this.刷新RToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.刷新RToolStripMenuItem.Text = "刷新(&R)";
            this.刷新RToolStripMenuItem.Click += new System.EventHandler(this.刷新RToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(176, 6);
            // 
            // 删除文件MToolStripMenuItem
            // 
            this.删除文件MToolStripMenuItem.Name = "删除文件MToolStripMenuItem";
            this.删除文件MToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.删除文件MToolStripMenuItem.Text = "删除文件(&M)";
            this.删除文件MToolStripMenuItem.Click += new System.EventHandler(this.删除文件MToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(176, 6);
            // 
            // 调用系统菜单ToolStripMenuItem
            // 
            this.调用系统菜单ToolStripMenuItem.Name = "调用系统菜单ToolStripMenuItem";
            this.调用系统菜单ToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.调用系统菜单ToolStripMenuItem.Text = "系统菜单(&T)";
            this.调用系统菜单ToolStripMenuItem.Click += new System.EventHandler(this.调用系统菜单ToolStripMenuItem_Click);
            // 
            // Keywords
            // 
            this.Keywords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Keywords.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Keywords.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Keywords.Location = new System.Drawing.Point(4, 4);
            this.Keywords.Margin = new System.Windows.Forms.Padding(6);
            this.Keywords.Name = "Keywords";
            this.Keywords.Size = new System.Drawing.Size(657, 26);
            this.Keywords.TabIndex = 1;
            this.Keywords.Click += new System.EventHandler(this.Keywords_Click);
            this.Keywords.TextChanged += new System.EventHandler(this.Keywords_TextChanged);
            this.Keywords.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Keywords_KeyDown);
            this.Keywords.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Keywords_KeyPress);
            this.Keywords.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Label1_MouseDown);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "双击或快捷键显示主界面";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.显示主界面SToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.显示主界面SToolStripMenuItem,
            this.开机启动SToolStripMenuItem1,
            this.缓存刷新CToolStripMenuItem,
            this.关于LToolStripMenuItem,
            this.退出QToolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(152, 114);
            // 
            // 显示主界面SToolStripMenuItem
            // 
            this.显示主界面SToolStripMenuItem.Name = "显示主界面SToolStripMenuItem";
            this.显示主界面SToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.显示主界面SToolStripMenuItem.Text = "显示主界面(&S)";
            this.显示主界面SToolStripMenuItem.Click += new System.EventHandler(this.显示主界面SToolStripMenuItem_Click);
            // 
            // 开机启动SToolStripMenuItem1
            // 
            this.开机启动SToolStripMenuItem1.Name = "开机启动SToolStripMenuItem1";
            this.开机启动SToolStripMenuItem1.Size = new System.Drawing.Size(151, 22);
            this.开机启动SToolStripMenuItem1.Text = "开机启动(&S)";
            this.开机启动SToolStripMenuItem1.Click += new System.EventHandler(this.开机启动SToolStripMenuItem1_Click);
            // 
            // 缓存刷新CToolStripMenuItem
            // 
            this.缓存刷新CToolStripMenuItem.Name = "缓存刷新CToolStripMenuItem";
            this.缓存刷新CToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.缓存刷新CToolStripMenuItem.Text = "缓存刷新(&R)";
            this.缓存刷新CToolStripMenuItem.Click += new System.EventHandler(this.缓存刷新CToolStripMenuItem_Click);
            // 
            // 关于LToolStripMenuItem
            // 
            this.关于LToolStripMenuItem.Name = "关于LToolStripMenuItem";
            this.关于LToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.关于LToolStripMenuItem.Text = "关于(&A)";
            this.关于LToolStripMenuItem.Click += new System.EventHandler(this.关于LeahyGoToolStripMenuItem_Click);
            // 
            // 退出QToolStripMenuItem1
            // 
            this.退出QToolStripMenuItem1.Name = "退出QToolStripMenuItem1";
            this.退出QToolStripMenuItem1.Size = new System.Drawing.Size(151, 22);
            this.退出QToolStripMenuItem1.Text = "退出(&Q)";
            this.退出QToolStripMenuItem1.Click += new System.EventHandler(this.退出QToolStripMenuItem1_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.DarkGray;
            this.label1.Location = new System.Drawing.Point(464, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(193, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "初始化...";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Label1_MouseDown);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Time";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 428);
            this.ControlBox = false;
            this.Controls.Add(this.istView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Keywords);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.RightMenu.ResumeLayout(false);
            this.RightMenu.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView istView1;
        private System.Windows.Forms.TextBox Keywords;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ContextMenuStrip RightMenu;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 显示主界面SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开OToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开文件夹DToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 复制文件FToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 复制文件名NToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 复制文件路径CToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 复制目录路径EToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem 刷新RToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem 删除文件MToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem 调用系统菜单ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 开机启动SToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 缓存刷新CToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关于LToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出QToolStripMenuItem1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}

