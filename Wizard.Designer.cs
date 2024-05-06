namespace Wizard
{
    partial class Wizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Wizard));
            this.gbStep2 = new System.Windows.Forms.GroupBox();
            this.lstScale = new System.Windows.Forms.ListBox();
            this.txtResolution = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbResolution = new System.Windows.Forms.ComboBox();
            this.numWidth = new System.Windows.Forms.NumericUpDown();
            this.numHeight = new System.Windows.Forms.NumericUpDown();
            this.gbStep3 = new System.Windows.Forms.GroupBox();
            this.checkFolder = new System.Windows.Forms.CheckBox();
            this.txtFolderName = new System.Windows.Forms.TextBox();
            this.txtProjectName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.gbStep4 = new System.Windows.Forms.GroupBox();
            this.txtReport = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.txtReadme = new System.Windows.Forms.TextBox();
            this.tabSelect = new System.Windows.Forms.TabControl();
            this.tabTemplate = new System.Windows.Forms.TabPage();
            this.lstTemplate = new System.Windows.Forms.ListBox();
            this.tabProject = new System.Windows.Forms.TabPage();
            this.lstProject = new System.Windows.Forms.ListBox();
            this.gbStep1 = new System.Windows.Forms.GroupBox();
            this.gbStep2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            this.gbStep3.SuspendLayout();
            this.gbStep4.SuspendLayout();
            this.tabSelect.SuspendLayout();
            this.tabTemplate.SuspendLayout();
            this.tabProject.SuspendLayout();
            this.gbStep1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbStep2
            // 
            this.gbStep2.Controls.Add(this.lstScale);
            this.gbStep2.Controls.Add(this.txtResolution);
            this.gbStep2.Controls.Add(this.label2);
            this.gbStep2.Controls.Add(this.label1);
            this.gbStep2.Controls.Add(this.cbResolution);
            this.gbStep2.Controls.Add(this.numWidth);
            this.gbStep2.Controls.Add(this.numHeight);
            this.gbStep2.Location = new System.Drawing.Point(539, 15);
            this.gbStep2.Margin = new System.Windows.Forms.Padding(4);
            this.gbStep2.Name = "gbStep2";
            this.gbStep2.Padding = new System.Windows.Forms.Padding(4);
            this.gbStep2.Size = new System.Drawing.Size(493, 270);
            this.gbStep2.TabIndex = 8;
            this.gbStep2.TabStop = false;
            this.gbStep2.Text = "2.设定分辨率";
            // 
            // lstScale
            // 
            this.lstScale.FormattingEnabled = true;
            this.lstScale.ItemHeight = 15;
            this.lstScale.Location = new System.Drawing.Point(297, 24);
            this.lstScale.Margin = new System.Windows.Forms.Padding(4);
            this.lstScale.Name = "lstScale";
            this.lstScale.Size = new System.Drawing.Size(175, 229);
            this.lstScale.TabIndex = 14;
            // 
            // txtResolution
            // 
            this.txtResolution.Location = new System.Drawing.Point(23, 91);
            this.txtResolution.Margin = new System.Windows.Forms.Padding(4);
            this.txtResolution.Multiline = true;
            this.txtResolution.Name = "txtResolution";
            this.txtResolution.ReadOnly = true;
            this.txtResolution.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResolution.Size = new System.Drawing.Size(252, 162);
            this.txtResolution.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(151, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "高";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 62);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "宽";
            // 
            // cbResolution
            // 
            this.cbResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbResolution.FormattingEnabled = true;
            this.cbResolution.Location = new System.Drawing.Point(23, 25);
            this.cbResolution.Margin = new System.Windows.Forms.Padding(4);
            this.cbResolution.Name = "cbResolution";
            this.cbResolution.Size = new System.Drawing.Size(252, 23);
            this.cbResolution.TabIndex = 8;
            this.cbResolution.SelectedIndexChanged += new System.EventHandler(this.cbResolution_SelectedIndexChanged);
            // 
            // numWidth
            // 
            this.numWidth.Location = new System.Drawing.Point(49, 58);
            this.numWidth.Margin = new System.Windows.Forms.Padding(4);
            this.numWidth.Maximum = new decimal(new int[] {
            3840,
            0,
            0,
            0});
            this.numWidth.Name = "numWidth";
            this.numWidth.Size = new System.Drawing.Size(95, 25);
            this.numWidth.TabIndex = 6;
            this.numWidth.ValueChanged += new System.EventHandler(this.numResolution_ValueChanged);
            // 
            // numHeight
            // 
            this.numHeight.Location = new System.Drawing.Point(181, 58);
            this.numHeight.Margin = new System.Windows.Forms.Padding(4);
            this.numHeight.Maximum = new decimal(new int[] {
            2160,
            0,
            0,
            0});
            this.numHeight.Name = "numHeight";
            this.numHeight.Size = new System.Drawing.Size(95, 25);
            this.numHeight.TabIndex = 7;
            this.numHeight.ValueChanged += new System.EventHandler(this.numResolution_ValueChanged);
            // 
            // gbStep3
            // 
            this.gbStep3.Controls.Add(this.checkFolder);
            this.gbStep3.Controls.Add(this.txtFolderName);
            this.gbStep3.Controls.Add(this.txtProjectName);
            this.gbStep3.Controls.Add(this.label5);
            this.gbStep3.Location = new System.Drawing.Point(16, 352);
            this.gbStep3.Margin = new System.Windows.Forms.Padding(4);
            this.gbStep3.Name = "gbStep3";
            this.gbStep3.Padding = new System.Windows.Forms.Padding(4);
            this.gbStep3.Size = new System.Drawing.Size(493, 270);
            this.gbStep3.TabIndex = 9;
            this.gbStep3.TabStop = false;
            this.gbStep3.Text = "3.设定名称";
            // 
            // checkFolder
            // 
            this.checkFolder.AutoSize = true;
            this.checkFolder.Location = new System.Drawing.Point(121, 141);
            this.checkFolder.Margin = new System.Windows.Forms.Padding(4);
            this.checkFolder.Name = "checkFolder";
            this.checkFolder.Size = new System.Drawing.Size(134, 19);
            this.checkFolder.TabIndex = 15;
            this.checkFolder.Text = "单独设定目录名";
            this.checkFolder.UseVisualStyleBackColor = true;
            this.checkFolder.CheckedChanged += new System.EventHandler(this.checkFolder_CheckedChanged);
            // 
            // txtFolderName
            // 
            this.txtFolderName.Location = new System.Drawing.Point(123, 169);
            this.txtFolderName.Margin = new System.Windows.Forms.Padding(4);
            this.txtFolderName.MaxLength = 64;
            this.txtFolderName.Name = "txtFolderName";
            this.txtFolderName.ReadOnly = true;
            this.txtFolderName.Size = new System.Drawing.Size(252, 25);
            this.txtFolderName.TabIndex = 14;
            // 
            // txtProjectName
            // 
            this.txtProjectName.Location = new System.Drawing.Point(121, 92);
            this.txtProjectName.Margin = new System.Windows.Forms.Padding(4);
            this.txtProjectName.MaxLength = 64;
            this.txtProjectName.Name = "txtProjectName";
            this.txtProjectName.Size = new System.Drawing.Size(252, 25);
            this.txtProjectName.TabIndex = 13;
            this.txtProjectName.TextChanged += new System.EventHandler(this.txtProjectName_TextChanged);
            this.txtProjectName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtProjectName_KeyUp);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(119, 74);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "项目名称";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(403, 292);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(107, 40);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "完成";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(403, 292);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(107, 40);
            this.btnExit.TabIndex = 10;
            this.btnExit.Text = "结束";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbStep4
            // 
            this.gbStep4.Controls.Add(this.txtReport);
            this.gbStep4.Location = new System.Drawing.Point(539, 352);
            this.gbStep4.Margin = new System.Windows.Forms.Padding(4);
            this.gbStep4.Name = "gbStep4";
            this.gbStep4.Padding = new System.Windows.Forms.Padding(4);
            this.gbStep4.Size = new System.Drawing.Size(493, 270);
            this.gbStep4.TabIndex = 11;
            this.gbStep4.TabStop = false;
            this.gbStep4.Text = "4.文件转换";
            // 
            // txtReport
            // 
            this.txtReport.Location = new System.Drawing.Point(23, 24);
            this.txtReport.Margin = new System.Windows.Forms.Padding(4);
            this.txtReport.Multiline = true;
            this.txtReport.Name = "txtReport";
            this.txtReport.ReadOnly = true;
            this.txtReport.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtReport.Size = new System.Drawing.Size(449, 230);
            this.txtReport.TabIndex = 15;
            this.txtReport.WordWrap = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(16, 292);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(107, 40);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(288, 292);
            this.btnPrev.Margin = new System.Windows.Forms.Padding(4);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(107, 40);
            this.btnPrev.TabIndex = 15;
            this.btnPrev.Text = "上一步";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(403, 292);
            this.btnNext.Margin = new System.Windows.Forms.Padding(4);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(107, 40);
            this.btnNext.TabIndex = 17;
            this.btnNext.Text = "下一步";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // txtReadme
            // 
            this.txtReadme.Location = new System.Drawing.Point(156, 24);
            this.txtReadme.Margin = new System.Windows.Forms.Padding(4);
            this.txtReadme.Multiline = true;
            this.txtReadme.Name = "txtReadme";
            this.txtReadme.ReadOnly = true;
            this.txtReadme.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtReadme.Size = new System.Drawing.Size(316, 230);
            this.txtReadme.TabIndex = 15;
            this.txtReadme.WordWrap = false;
            // 
            // tabSelect
            // 
            this.tabSelect.Controls.Add(this.tabTemplate);
            this.tabSelect.Controls.Add(this.tabProject);
            this.tabSelect.Location = new System.Drawing.Point(7, 21);
            this.tabSelect.Margin = new System.Windows.Forms.Padding(4);
            this.tabSelect.Name = "tabSelect";
            this.tabSelect.SelectedIndex = 0;
            this.tabSelect.Size = new System.Drawing.Size(147, 234);
            this.tabSelect.TabIndex = 16;
            this.tabSelect.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabSelect_Selected);
            // 
            // tabTemplate
            // 
            this.tabTemplate.Controls.Add(this.lstTemplate);
            this.tabTemplate.Location = new System.Drawing.Point(4, 25);
            this.tabTemplate.Margin = new System.Windows.Forms.Padding(4);
            this.tabTemplate.Name = "tabTemplate";
            this.tabTemplate.Padding = new System.Windows.Forms.Padding(4);
            this.tabTemplate.Size = new System.Drawing.Size(139, 205);
            this.tabTemplate.TabIndex = 0;
            this.tabTemplate.Text = "主题";
            this.tabTemplate.UseVisualStyleBackColor = true;
            // 
            // lstTemplate
            // 
            this.lstTemplate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTemplate.FormattingEnabled = true;
            this.lstTemplate.ItemHeight = 15;
            this.lstTemplate.Location = new System.Drawing.Point(4, 4);
            this.lstTemplate.Margin = new System.Windows.Forms.Padding(4);
            this.lstTemplate.Name = "lstTemplate";
            this.lstTemplate.Size = new System.Drawing.Size(131, 197);
            this.lstTemplate.TabIndex = 4;
            this.lstTemplate.SelectedIndexChanged += new System.EventHandler(this.lstTemplate_SelectedIndexChanged);
            // 
            // tabProject
            // 
            this.tabProject.Controls.Add(this.lstProject);
            this.tabProject.Location = new System.Drawing.Point(4, 25);
            this.tabProject.Margin = new System.Windows.Forms.Padding(4);
            this.tabProject.Name = "tabProject";
            this.tabProject.Padding = new System.Windows.Forms.Padding(4);
            this.tabProject.Size = new System.Drawing.Size(139, 205);
            this.tabProject.TabIndex = 1;
            this.tabProject.Text = "项目";
            this.tabProject.UseVisualStyleBackColor = true;
            // 
            // lstProject
            // 
            this.lstProject.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstProject.FormattingEnabled = true;
            this.lstProject.ItemHeight = 15;
            this.lstProject.Location = new System.Drawing.Point(4, 4);
            this.lstProject.Margin = new System.Windows.Forms.Padding(4);
            this.lstProject.Name = "lstProject";
            this.lstProject.Size = new System.Drawing.Size(131, 197);
            this.lstProject.TabIndex = 5;
            this.lstProject.SelectedIndexChanged += new System.EventHandler(this.lstProject_SelectedIndexChanged);
            // 
            // gbStep1
            // 
            this.gbStep1.Controls.Add(this.tabSelect);
            this.gbStep1.Controls.Add(this.txtReadme);
            this.gbStep1.Location = new System.Drawing.Point(16, 15);
            this.gbStep1.Margin = new System.Windows.Forms.Padding(4);
            this.gbStep1.Name = "gbStep1";
            this.gbStep1.Padding = new System.Windows.Forms.Padding(4);
            this.gbStep1.Size = new System.Drawing.Size(493, 270);
            this.gbStep1.TabIndex = 6;
            this.gbStep1.TabStop = false;
            this.gbStep1.Text = "1.选择主题或项目";
            // 
            // Wizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 346);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.gbStep3);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.gbStep4);
            this.Controls.Add(this.gbStep2);
            this.Controls.Add(this.gbStep1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(543, 393);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(543, 393);
            this.Name = "Wizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "项目管理向导";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Wizard_FormClosing);
            this.gbStep2.ResumeLayout(false);
            this.gbStep2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            this.gbStep3.ResumeLayout(false);
            this.gbStep3.PerformLayout();
            this.gbStep4.ResumeLayout(false);
            this.gbStep4.PerformLayout();
            this.tabSelect.ResumeLayout(false);
            this.tabTemplate.ResumeLayout(false);
            this.tabProject.ResumeLayout(false);
            this.gbStep1.ResumeLayout(false);
            this.gbStep1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbStep2;
        private System.Windows.Forms.TextBox txtResolution;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbResolution;
        private System.Windows.Forms.NumericUpDown numWidth;
        private System.Windows.Forms.NumericUpDown numHeight;
        private System.Windows.Forms.GroupBox gbStep3;
        private System.Windows.Forms.TextBox txtProjectName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkFolder;
        private System.Windows.Forms.TextBox txtFolderName;
        private System.Windows.Forms.ListBox lstScale;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox gbStep4;
        private System.Windows.Forms.TextBox txtReport;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.TextBox txtReadme;
        private System.Windows.Forms.TabControl tabSelect;
        private System.Windows.Forms.TabPage tabTemplate;
        private System.Windows.Forms.ListBox lstTemplate;
        private System.Windows.Forms.TabPage tabProject;
        private System.Windows.Forms.ListBox lstProject;
        private System.Windows.Forms.GroupBox gbStep1;
    }
}

