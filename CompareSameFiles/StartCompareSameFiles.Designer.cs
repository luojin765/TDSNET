namespace TDSNET.CompareSameFiles
{
    partial class StartCompareSameFiles
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new System.Windows.Forms.Panel();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            textBox2 = new System.Windows.Forms.TextBox();
            textBox1 = new System.Windows.Forms.TextBox();
            button1 = new System.Windows.Forms.Button();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            listView2 = new System.Windows.Forms.ListView();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            textBox3 = new System.Windows.Forms.TextBox();
            listView1 = new System.Windows.Forms.ListView();
            columnHeader4 = new System.Windows.Forms.ColumnHeader();
            columnHeader5 = new System.Windows.Forms.ColumnHeader();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(textBox2);
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(button1);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(992, 53);
            panel1.TabIndex = 0;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 20);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(44, 17);
            label2.TabIndex = 2;
            label2.Text = "扩展名";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(272, 18);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(88, 17);
            label1.TabIndex = 2;
            label1.Text = "最小大小(字节)";
            // 
            // textBox2
            // 
            textBox2.Location = new System.Drawing.Point(373, 15);
            textBox2.Name = "textBox2";
            textBox2.Size = new System.Drawing.Size(142, 23);
            textBox2.TabIndex = 1;
            textBox2.Text = "1024";
            // 
            // textBox1
            // 
            textBox1.Location = new System.Drawing.Point(62, 15);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(142, 23);
            textBox1.TabIndex = 1;
            textBox1.Text = "doc|docx";
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(847, 12);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(124, 33);
            button1.TabIndex = 0;
            button1.Text = "Go";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 53);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(listView2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(textBox3);
            splitContainer1.Panel2.Controls.Add(listView1);
            splitContainer1.Size = new System.Drawing.Size(992, 624);
            splitContainer1.SplitterDistance = 330;
            splitContainer1.TabIndex = 1;
            // 
            // listView2
            // 
            listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader2, columnHeader1, columnHeader3 });
            listView2.Dock = System.Windows.Forms.DockStyle.Fill;
            listView2.FullRowSelect = true;
            listView2.Location = new System.Drawing.Point(0, 0);
            listView2.MultiSelect = false;
            listView2.Name = "listView2";
            listView2.Size = new System.Drawing.Size(330, 624);
            listView2.TabIndex = 0;
            listView2.UseCompatibleStateImageBehavior = false;
            listView2.View = System.Windows.Forms.View.Details;
            listView2.SelectedIndexChanged += listView2_SelectedIndexChanged;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "文件名";
            // 
            // columnHeader1
            // 
            columnHeader1.DisplayIndex = 2;
            columnHeader1.Text = "大小";
            // 
            // columnHeader3
            // 
            columnHeader3.DisplayIndex = 1;
            columnHeader3.Text = "数量";
            // 
            // textBox3
            // 
            textBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            textBox3.Location = new System.Drawing.Point(0, 539);
            textBox3.Multiline = true;
            textBox3.Name = "textBox3";
            textBox3.ReadOnly = true;
            textBox3.Size = new System.Drawing.Size(658, 85);
            textBox3.TabIndex = 3;
            textBox3.TextChanged += textBox3_TextChanged;
            textBox3.MouseDoubleClick += textBox3_MouseDoubleClick;
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader4, columnHeader5 });
            listView1.Dock = System.Windows.Forms.DockStyle.Top;
            listView1.Location = new System.Drawing.Point(0, 0);
            listView1.MultiSelect = false;
            listView1.Name = "listView1";
            listView1.Size = new System.Drawing.Size(658, 539);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = System.Windows.Forms.View.SmallIcon;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            listView1.MouseDoubleClick += listView1_MouseDoubleClick;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "文件名";
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "路径";
            // 
            // StartCompareSameFiles
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(992, 677);
            Controls.Add(splitContainer1);
            Controls.Add(panel1);
            Name = "StartCompareSameFiles";
            Text = "StartCompareSameFiles";
            Load += StartCompareSameFiles_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
    }
}