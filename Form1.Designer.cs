
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
            components = new System.ComponentModel.Container();
            System.Windows.Forms.ColumnHeader columnHeader1;
            System.Windows.Forms.ColumnHeader columnHeader2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            istView1 = new System.Windows.Forms.ListView();
            RightMenu = new System.Windows.Forms.ContextMenuStrip(components);
            toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            打开OToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            打开文件夹DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            复制文件FToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            复制文件名NToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            复制文件路径CToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            复制目录路径EToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            刷新RToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            删除文件MToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            调用系统菜单ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            Keywords = new System.Windows.Forms.TextBox();
            notifyIcon1 = new System.Windows.Forms.NotifyIcon(components);
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            显示主界面SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            文件内容查询FToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            开机启动SToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            缓存刷新CToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            关于LToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            退出QToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            label1 = new System.Windows.Forms.Label();
            文件同步TToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            RightMenu.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
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
            istView1.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            istView1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            istView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            istView1.CausesValidation = false;
            istView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2 });
            istView1.ContextMenuStrip = RightMenu;
            istView1.Font = new System.Drawing.Font("微软雅黑", 12F);
            istView1.ForeColor = System.Drawing.Color.DimGray;
            istView1.FullRowSelect = true;
            istView1.LabelWrap = false;
            istView1.Location = new System.Drawing.Point(5, 41);
            istView1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            istView1.Name = "istView1";
            istView1.Size = new System.Drawing.Size(845, 456);
            istView1.TabIndex = 0;
            istView1.UseCompatibleStateImageBehavior = false;
            istView1.View = System.Windows.Forms.View.Details;
            istView1.VirtualMode = true;
            istView1.CacheVirtualItems += IstView1_CacheVirtualItems;
            istView1.RetrieveVirtualItem += ListView1_RetrieveVirtualItem;
            istView1.SelectedIndexChanged += istView1_SelectedIndexChanged_1;
            istView1.KeyDown += ListView1_KeyDown;
            istView1.MouseClick += ListView1_MouseClick;
            istView1.MouseDoubleClick += ListView1_MouseDoubleClick;
            istView1.MouseDown += ListView1_MouseDown;
            istView1.MouseMove += ListView1_MouseMove;
            istView1.MouseUp += ListView1_MouseUp;
            // 
            // RightMenu
            // 
            RightMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            RightMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripTextBox1, toolStripSeparator4, 打开OToolStripMenuItem, 打开文件夹DToolStripMenuItem, 复制文件FToolStripMenuItem, 复制文件名NToolStripMenuItem, 复制文件路径CToolStripMenuItem, 复制目录路径EToolStripMenuItem, toolStripSeparator1, 刷新RToolStripMenuItem, toolStripSeparator2, 删除文件MToolStripMenuItem, toolStripSeparator3, 调用系统菜单ToolStripMenuItem });
            RightMenu.Name = "RightMenu";
            RightMenu.Size = new System.Drawing.Size(207, 273);
            RightMenu.Opening += RightMenu_Opening;
            // 
            // toolStripTextBox1
            // 
            toolStripTextBox1.Name = "toolStripTextBox1";
            toolStripTextBox1.Size = new System.Drawing.Size(100, 27);
            toolStripTextBox1.Enter += ToolStripTextBox1_Enter;
            toolStripTextBox1.KeyDown += toolStripTextBox1_KeyDown;
            toolStripTextBox1.Click += ToolStripTextBox1_Click_1;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(203, 6);
            // 
            // 打开OToolStripMenuItem
            // 
            打开OToolStripMenuItem.Name = "打开OToolStripMenuItem";
            打开OToolStripMenuItem.Size = new System.Drawing.Size(206, 24);
            打开OToolStripMenuItem.Text = "打开文件(%O)";
            打开OToolStripMenuItem.Click += 打开OToolStripMenuItem_Click;
            // 
            // 打开文件夹DToolStripMenuItem
            // 
            打开文件夹DToolStripMenuItem.Name = "打开文件夹DToolStripMenuItem";
            打开文件夹DToolStripMenuItem.Size = new System.Drawing.Size(206, 24);
            打开文件夹DToolStripMenuItem.Text = "打开目录(&D)";
            打开文件夹DToolStripMenuItem.Click += 打开文件夹DToolStripMenuItem_Click;
            // 
            // 复制文件FToolStripMenuItem
            // 
            复制文件FToolStripMenuItem.Name = "复制文件FToolStripMenuItem";
            复制文件FToolStripMenuItem.Size = new System.Drawing.Size(206, 24);
            复制文件FToolStripMenuItem.Text = "复制文件(&C)";
            复制文件FToolStripMenuItem.Click += 复制文件FToolStripMenuItem_Click;
            // 
            // 复制文件名NToolStripMenuItem
            // 
            复制文件名NToolStripMenuItem.Name = "复制文件名NToolStripMenuItem";
            复制文件名NToolStripMenuItem.Size = new System.Drawing.Size(206, 24);
            复制文件名NToolStripMenuItem.Text = "复制文件名(&N)";
            复制文件名NToolStripMenuItem.Click += 复制文件名NToolStripMenuItem_Click;
            // 
            // 复制文件路径CToolStripMenuItem
            // 
            复制文件路径CToolStripMenuItem.Name = "复制文件路径CToolStripMenuItem";
            复制文件路径CToolStripMenuItem.Size = new System.Drawing.Size(206, 24);
            复制文件路径CToolStripMenuItem.Text = "复制文件路径(F)";
            复制文件路径CToolStripMenuItem.Click += 复制文件路径CToolStripMenuItem_Click;
            // 
            // 复制目录路径EToolStripMenuItem
            // 
            复制目录路径EToolStripMenuItem.Name = "复制目录路径EToolStripMenuItem";
            复制目录路径EToolStripMenuItem.Size = new System.Drawing.Size(206, 24);
            复制目录路径EToolStripMenuItem.Text = "复制目录路径（&E）";
            复制目录路径EToolStripMenuItem.Click += 复制目录路径EToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(203, 6);
            // 
            // 刷新RToolStripMenuItem
            // 
            刷新RToolStripMenuItem.Name = "刷新RToolStripMenuItem";
            刷新RToolStripMenuItem.Size = new System.Drawing.Size(206, 24);
            刷新RToolStripMenuItem.Text = "刷新(&R)";
            刷新RToolStripMenuItem.Click += 刷新RToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(203, 6);
            // 
            // 删除文件MToolStripMenuItem
            // 
            删除文件MToolStripMenuItem.Name = "删除文件MToolStripMenuItem";
            删除文件MToolStripMenuItem.Size = new System.Drawing.Size(206, 24);
            删除文件MToolStripMenuItem.Text = "删除文件(&M)";
            删除文件MToolStripMenuItem.Click += 删除文件MToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(203, 6);
            // 
            // 调用系统菜单ToolStripMenuItem
            // 
            调用系统菜单ToolStripMenuItem.Name = "调用系统菜单ToolStripMenuItem";
            调用系统菜单ToolStripMenuItem.Size = new System.Drawing.Size(206, 24);
            调用系统菜单ToolStripMenuItem.Text = "系统菜单(&T)";
            调用系统菜单ToolStripMenuItem.Click += 调用系统菜单ToolStripMenuItem_Click;
            // 
            // Keywords
            // 
            Keywords.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            Keywords.BorderStyle = System.Windows.Forms.BorderStyle.None;
            Keywords.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold);
            Keywords.Location = new System.Drawing.Point(5, 5);
            Keywords.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            Keywords.Name = "Keywords";
            Keywords.Size = new System.Drawing.Size(845, 32);
            Keywords.TabIndex = 1;
            Keywords.Click += Keywords_Click;
            Keywords.TextChanged += Keywords_TextChanged;
            Keywords.KeyDown += Keywords_KeyDown;
            Keywords.KeyPress += Keywords_KeyPress;
            Keywords.MouseDown += Label1_MouseDown;
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Icon = (System.Drawing.Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "双击或快捷键显示主界面";
            notifyIcon1.Visible = true;
            notifyIcon1.DoubleClick += 显示主界面SToolStripMenuItem_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { 显示主界面SToolStripMenuItem, 文件内容查询FToolStripMenuItem, 文件同步TToolStripMenuItem, 开机启动SToolStripMenuItem1, 缓存刷新CToolStripMenuItem, 关于LToolStripMenuItem, 退出QToolStripMenuItem1 });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(211, 200);
            // 
            // 显示主界面SToolStripMenuItem
            // 
            显示主界面SToolStripMenuItem.Name = "显示主界面SToolStripMenuItem";
            显示主界面SToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            显示主界面SToolStripMenuItem.Text = "显示主界面(&M)";
            显示主界面SToolStripMenuItem.Click += 显示主界面SToolStripMenuItem_Click;
            // 
            // 文件内容查询FToolStripMenuItem
            // 
            文件内容查询FToolStripMenuItem.Name = "文件内容查询FToolStripMenuItem";
            文件内容查询FToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            文件内容查询FToolStripMenuItem.Text = "文件内容查询(&F)";
            文件内容查询FToolStripMenuItem.Click += 文件内容查询FToolStripMenuItem_Click;
            // 
            // 开机启动SToolStripMenuItem1
            // 
            开机启动SToolStripMenuItem1.Name = "开机启动SToolStripMenuItem1";
            开机启动SToolStripMenuItem1.Size = new System.Drawing.Size(210, 24);
            开机启动SToolStripMenuItem1.Text = "开机启动(&S)";
            开机启动SToolStripMenuItem1.Click += 开机启动SToolStripMenuItem1_Click;
            // 
            // 缓存刷新CToolStripMenuItem
            // 
            缓存刷新CToolStripMenuItem.Name = "缓存刷新CToolStripMenuItem";
            缓存刷新CToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            缓存刷新CToolStripMenuItem.Text = "缓存刷新(&R)";
            缓存刷新CToolStripMenuItem.Click += 缓存刷新CToolStripMenuItem_Click;
            // 
            // 关于LToolStripMenuItem
            // 
            关于LToolStripMenuItem.Name = "关于LToolStripMenuItem";
            关于LToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            关于LToolStripMenuItem.Text = "关于(&A)";
            关于LToolStripMenuItem.Click += 关于LeahyGoToolStripMenuItem_Click;
            // 
            // 退出QToolStripMenuItem1
            // 
            退出QToolStripMenuItem1.Name = "退出QToolStripMenuItem1";
            退出QToolStripMenuItem1.Size = new System.Drawing.Size(210, 24);
            退出QToolStripMenuItem1.Text = "退出(&Q)";
            退出QToolStripMenuItem1.Click += 退出QToolStripMenuItem1_Click;
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            label1.ForeColor = System.Drawing.Color.DarkGray;
            label1.Location = new System.Drawing.Point(597, 6);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(248, 28);
            label1.TabIndex = 2;
            label1.Text = "初始化...";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            label1.MouseDown += Label1_MouseDown;
            // 
            // 文件同步TToolStripMenuItem
            // 
            文件同步TToolStripMenuItem.Name = "文件同步TToolStripMenuItem";
            文件同步TToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            文件同步TToolStripMenuItem.Text = "文件同步(&T)";
            文件同步TToolStripMenuItem.Click += 文件同步TToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(856, 504);
            ControlBox = false;
            Controls.Add(istView1);
            Controls.Add(label1);
            Controls.Add(Keywords);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Name = "Form1";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Form1";
            Activated += Form1_Activated;
            Deactivate += Form1_Deactivate;
            FormClosing += Form1_FormClosing;
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            Shown += Form1_Shown;
            KeyDown += Form1_KeyDown;
            RightMenu.ResumeLayout(false);
            RightMenu.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListView istView1;
        private System.Windows.Forms.TextBox Keywords;
        private System.Windows.Forms.ColumnHeader columnHeader1;
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
        private System.Windows.Forms.ToolStripMenuItem 文件内容查询FToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 文件同步TToolStripMenuItem;
    }
}

