using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Tjs;

//
// app.ico == Any closet is a walk-in closet if you try hard enough..ico
// Based on icons by Paul Davey aka Mattahan. All rights reserved.
// 

namespace Wizard
{
    public partial class Wizard : Form
    {
        const string NAME_CUSTOM_RESOLUTION = "(�Զ���)";

        // ���ڲ���������
        WizardConfig _curConfig = new WizardConfig();

        // ��¼Ŀǰ�Ĳ���
        int _curStep = -1;

        // �����������
        GroupBox[] _stepGroups = null;

        // ���账����ָ��
        delegate bool StepHandler();
        StepHandler[] _stepHandlers;

        // ��ȡ/���õ�ǰ����
        int Step
        {
            get { return _curStep; }
            set
            {
                // ����δ����
                if(_curStep == value)
                {
                    return;
                }

                // ������һ��ҳ��
                if (_curStep >= 0 && _curStep < _stepGroups.Length)
                {
                    _stepGroups[_curStep].Enabled = false;
                }

                // ���²���
                _curStep = value;
                if (_curStep < 0) 
                {
                    _curStep = 0; 
                }
                else if (_curStep >= _stepGroups.Length) 
                {
                    _curStep = _stepGroups.Length - 1; 
                }

                // ���յ�ǰ������ʽ���ض�Ӧ���
                _stepGroups[_curStep].BringToFront();
                _stepGroups[_curStep].Enabled = true;

                btnOK.Hide();
                btnExit.Hide();

                bool canNext = false;
                // ���յ�ǰ������ö�Ӧ�Ĵ�����
                if (_curStep < _stepHandlers.Length)
                {
                    StepHandler handler = _stepHandlers[_curStep];
                    canNext = handler();
                }

                // ���ư�ť��ʾ
                btnNext.Enabled = !canNext ? false : _curStep < _stepGroups.Length - 1;
                btnPrev.Enabled = _curStep > 0;
                if (!btnPrev.Enabled) btnNext.Focus();
                if (btnNext.Enabled) btnNext.BringToFront();
            }
        }

        public Wizard()
        {
            InitializeComponent();

            this.SuspendLayout();

            // �趨����ʱ�Ĺ���·��Ϊ�����Ŀ¼
            _curConfig.BaseFolder = Directory.GetCurrentDirectory();

            // ��ʼ���ֱ�������
            cbResolution.Items.Clear();
            cbResolution.Items.Add(NAME_CUSTOM_RESOLUTION);
            foreach(Resolution res in Resolution.List)
            {
                cbResolution.Items.Add(res);
            }
            cbResolution.SelectedIndex = cbResolution.Items.Count - 1;

            // ��ʼ���򵼸���������λ�ã����浽�������Ա�����
            _stepGroups = new GroupBox[] { gbStep1, gbStep2, gbStep3, gbStep4 };
            for (int i = 1; i < _stepGroups.Length; i++)
            {
                // �Ѱ���λ�ö�ͬ������һ����λ��
                _stepGroups[i].Location = _stepGroups[0].Location;
                _stepGroups[i].Enabled = false;
            }

            // �󶨵�ǰ����
            _stepHandlers = new StepHandler[] { 
                new StepHandler(this.OnStep1),
                new StepHandler(this.OnStep2), 
                new StepHandler(this.OnStep3),
                new StepHandler(this.OnStep4),
            };

            this.Step = 0;

            this.ResumeLayout();
        }

        private void test()
        {
            return;

            string strTitle = ";System.title =\"ģ�幤��\";";
            string strW = ";scWidth =1024;";
            string strH = ";scHeight =768;";

            Regex regTitle = new Regex(@"\s*;\s*System.title\s*=");
            Regex regW = new Regex(@"\s*;\s*scWidth\s*=");
            Regex regH = new Regex(@"\s*;\s*scHeight\s*=");

            bool ret = false;
            ret = regTitle.IsMatch(strTitle);
            ret = regW.IsMatch(strW);
            ret = regH.IsMatch(strH);

            string[] layouts = Directory.GetFiles(_curConfig.ThemeDataFolder, WizardConfig.UI_LAYOUT);

            // ����tjsֵ��ȡ
            foreach (string layout in layouts)
            {
                using (StreamReader r = new StreamReader(layout))
                {
                    TjsParser parser = new TjsParser();
                    TjsValue val = null;
                    do 
                    {
                        val = parser.Parse(r);
                    } while (val != null);
                }
            }

            // ����tjs���Ŷ�ȡ
            using (StreamReader r = new StreamReader(layouts[0]))
            {
                TjsParser parser = new TjsParser();
                TjsParser.Token token = null;
                do
                {
                    token = parser.GetNext(r);
                } while (token != null && token.t != TjsParser.TokenType.Unknow);
            }

            // ��Դת��������Ĳ�������
            ResConfig config = new ResConfig();
            config.files.Add(new ResFile(@"a.png"));
            config.files.Add(new ResFile(@"b.png"));
            config.name = "TestTest";
            config.path = @"c:\";

            config.Save(@"c:\test.xml");
            ResConfig newConfig = ResConfig.Load(@"c:\test.xml");

            ResConverter cov = new ResConverter();
            cov.Start(config, @"d:\", 1024, 768, 1920, 1080);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Step = Step + 1;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            Step = Step - 1;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // ��ʼ����
            if(MessageBox.Show("��ʼ" + CurOP + "��Ŀ��", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                OnBuild();
            }
        }

        bool OnStep1()
        {
            if(_curConfig.IsModifyProject)
            {
                // ˢ����ĿĿ¼�б�
                lstProject.BeginUpdate();
                lstProject.Items.Clear();
                
                // ��ȡ������ĿĿ¼
                string projectRoot = _curConfig.BaseFolder + WizardConfig.PROJECT_FOLDER;
                int lastSelected = ShowNameList(projectRoot, lstProject, _curConfig.ProjectName);

                // �ָ��ϴ�ѡ�еĽ��
                if (lastSelected < 0 && lstProject.Items.Count > 0) lastSelected = 0;
                lstProject.SelectedIndex = lastSelected;

                lstProject.EndUpdate();

                return lstProject.Items.Count > 0;
            }
            else
            {
                // ˢ������Ŀ¼�б�
                lstTemplate.BeginUpdate();
                lstTemplate.Items.Clear();
                
                // ��Ĭ��������ӵ���һ��
                lstTemplate.Items.Add(WizardConfig.NAME_DEFAULT_THEME);

                // ��ȡ��������Ŀ¼
                string themeRoot = _curConfig.BaseFolder + WizardConfig.THEME_FOLDER;
                int lastSelected = ShowNameList(themeRoot, lstTemplate, _curConfig.ThemeName);

                // �ָ��ϴ�ѡ�еĽ����û��ѡ��ǿ��ѡ��Ĭ������
                if (lastSelected < 0) lastSelected = 0;
                lstTemplate.SelectedIndex = lastSelected;

                lstTemplate.EndUpdate();

                return lstTemplate.Items.Count > 0;
            }
        }

        bool OnStep2()
        {
            if (_curConfig.IsModifyProject)
            {
                ProjectProperty info = _curConfig.ProjectInfo;

                txtResolution.Text = "��Ŀԭʼ�ֱ��ʣ�";

                // �ڶ�����˵�����ڣ�ĿǰҲֻ����ôһ�����Կ�����ʾ
                txtResolution.Text += string.Format("{0}{0}��{3}�� {1}x{2}",
                                                   Environment.NewLine, info.width, info.height, _curConfig.ProjectName);

                // ��ʾ��Ŀ�����ļ�
                ShowFiles(_curConfig.ProjectFolder);
            }
            else
            {
                ProjectProperty info = _curConfig.ThemeInfo;

                txtResolution.Text = "����ԭʼ�ֱ��ʣ�";

                // �ڶ�����˵�����ڣ�ĿǰҲֻ����ôһ�����Կ�����ʾ
                string name = _curConfig.IsDefaultTheme ? WizardConfig.NAME_DEFAULT_THEME : _curConfig.ThemeName;
                txtResolution.Text += string.Format("{0}{0}��{3}�� {1}x{2}",
                                                   Environment.NewLine, info.width, info.height, name);

                // �Ƿ�ѡ����Ĭ�����⣬ûѡ�򸽼�Ĭ����������
                if (!_curConfig.IsDefaultTheme)
                {
                    ProjectProperty baseInfo = _curConfig.ReadBaseTemplateInfo();
                    txtResolution.Text += string.Format("{0}{0}��{3}�� {1}x{2}",
                        Environment.NewLine, baseInfo.width, baseInfo.height, WizardConfig.NAME_DEFAULT_THEME);

                    txtResolution.Text += string.Format("{0}{0}ע�⣺��{2}�������ǡ�{1}���е�ͬ���ļ���",
                                                   Environment.NewLine, WizardConfig.NAME_DEFAULT_THEME, name);
                }

                // ���ﱾ��Ӧ�ø������Ų�����������ʾÿ���ļ��������
                // �ȼ���һ���ļ���Ŀ¼�ɡ���
                ShowFiles(_curConfig.ThemeFolder);
            }

            // �����²����õĺ���
            test();

            return true;
        }

        bool OnStep3()
        {
            // ������һ���Ľ��
            _curConfig._width = (int)numWidth.Value;
            _curConfig._height = (int)numHeight.Value;
            
            txtProjectName.SelectAll();
            txtProjectName.Focus();

            // �޸ķֱ��ʵĻ��������޸���Ŀ����
            txtProjectName.Enabled = !_curConfig.IsModifyProject;
            txtFolderName.Enabled = !_curConfig.IsModifyProject;
            checkFolder.Enabled = !_curConfig.IsModifyProject;

            return true;
        }

        bool OnStep4()
        {
            // ������һ���Ľ��
            _curConfig.ProjectName = txtProjectName.Text;
            if (checkFolder.Checked)
            {
                _curConfig.ProjectFolder = txtFolderName.Text;
            }

            // ���ݵ�ǰ�������ɱ���
            StringWriter otuput = new StringWriter();
            
            btnOK.Enabled = _curConfig.IsReady(otuput);
            txtReport.Text = otuput.ToString();

            btnOK.BringToFront();
            btnOK.Show();
            btnOK.Focus();
            btnExit.Hide();

            return btnOK.Enabled;
        }

        void OnBuild()
        {
            // ����Logging
            LoggingBegin();

            // ��ʼ������Ŀ
            try
            {
                // ��ֹ��ť
                btnPrev.Enabled = false;
                btnCancel.Enabled = false;
                btnOK.Enabled = false;
                btnExit.Enabled = false;

                // ������Ŀת������������Ŀ���ã�����UI��ʾ�¼�
                WizardConverter conv = new WizardConverter(_curConfig);
                conv.NotifyProcessEvent += new ResConverter.NotifyProcessHandler(conv_NotifyProcessEvent);
                conv.LoggingEvent += new WizardConverter.MessageHandler(conv_LoggingEvent);
                conv.ErrorEvent += new WizardConverter.MessageHandler(conv_ErrorEvent);

                // ����һ���߳��������ļ�����ֹUI����
                Thread t = new Thread(new ThreadStart(conv.Start));
                t.Start();
                while(!t.Join(100))
                {
                    Application.DoEvents();
                }
                
                // ������ɣ���ʾ�˳���ť
                btnOK.Hide();
                btnExit.BringToFront();
                btnExit.Show();
                btnExit.Enabled = true;

                ReportAppend("��Ŀ" + CurOP + "��ϣ�");
            }
            catch (System.Exception e)
            {
                // ��ʾ����ԭ��
                ReportAppend(e.Message);

                // �ָ���ť
                btnCancel.Enabled = true;
                btnPrev.Enabled = true;
            }

            // ����Logging
            LoggingEnd();
        }

        // �򵥵�Logging������ֱ�Ӵ�ӡ�ڱ�����
        string _titleSaved = null;
        void LoggingBegin()
        {
            // ���洰�ڱ���
            if (_titleSaved == null) { _titleSaved = this.Text; }
        }
        void LoggingEnd()
        {
            // �ָ����ڱ���
            this.Invoke(new ThreadStart(delegate()
            {
                if (_titleSaved != null) { this.Text = _titleSaved; _titleSaved = null; }
            }));
        }
        void Logging(string msg)
        {
            if (_titleSaved == null)
            {
                Debug.Assert(false, "call LoggingBegin() first");
                return;
            }

            this.BeginInvoke(new ThreadStart(delegate()
            {
                this.Text = string.Format("{0}: {1}", _titleSaved, msg);
            }));
        }

        // ֱ�����ı��ؼ�����ʾһЩ��Ϣ
        void ReportAppend(string report)
        {
            this.BeginInvoke(new ThreadStart(delegate()
            {
                txtReport.Text += report + Environment.NewLine;
            }));
        }

        // ��ȡ��Ŀ���������б�������һ��ѡ�е�����ֵ
        private int ShowNameList(string root, ListBox lst, string lastSelect)
        {
            // ˢ��Ŀ¼�б�
            int selected = -1;
            lastSelect = lastSelect.ToLower();
            
            try
            {
                string[] themes = Directory.GetDirectories(root);
                foreach (string theme in themes)
                {
                    // ֻ��Ŀ¼��
                    string name = Path.GetFileName(theme);
                    
                    // ����Ĭ��ģ����������
                    if(name.StartsWith(".") || name.ToLower() == WizardConfig.PROJECT_DEFAULT_THEME)
                    {
                        continue;
                    }

                    lst.Items.Add(name);

                    // ƥ���һ��Ŀ¼����ͬ��������Ϊѡ������ص�ʱ�򱣳�ѡ����ȷ
                    if (selected < 0 && lastSelect == name)
                    {
                        selected = lst.Items.Count - 1;
                    }
                }
            }
            catch (System.Exception e)
            {
                // �����˾Ͳ�����
            }

            return selected;
        }

        // ��ʾĳ��Ŀ¼�������ļ����б�ؼ�
        private void ShowFiles(string root)
        {
            try
            {
                lstScale.BeginUpdate();
                lstScale.Items.Clear();

                // ��ȡ����Ŀ¼�µ��ļ��б�
                string[] subDirs = Directory.GetDirectories(root);
                foreach (string dir in subDirs)
                {
                    // ����.svnĿ¼
                    if (dir.StartsWith(".")) continue;
                    lstScale.Items.Add(string.Format("<dir> {0}", Path.GetFileName(dir)));
                }
                string[] files = Directory.GetFiles(root);
                foreach (string file in files)
                {
                    lstScale.Items.Add(Path.GetFileName(file));
                }
                lstScale.EndUpdate();
            }
            catch (System.Exception e) { }
        }

        // �ڽ�������ʾ��ȡ����������
        private void ShowProperty(ProjectProperty info)
        {
            // ��ȡ��Ŀ˵��
            txtReadme.Text = info.readme;
            txtProjectName.Text = info.title;

            // ѡ���ֱ���
            int w = info.width, h = info.height;
            for (int i = 0; i < cbResolution.Items.Count; i++)
            {
                Resolution r = cbResolution.Items[i] as Resolution;
                if (r != null && r._w == w && r._h == h)
                {
                    cbResolution.SelectedIndex = i;
                    break;
                }
            }
        }

        // ��ǰ�Ĳ����ؼ���
        private string CurOP
        {
            get { return _curConfig.IsModifyProject ? "�޸�" : "����"; }
        }

        // ����Ƿ��ڲ��������б���ֹ������ѡ��ؼ��໥����
        bool _isSelectingRes = false;
        private void cbResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            Resolution res = cbResolution.SelectedItem as Resolution;
            if(res != null)
            {
                _isSelectingRes = true;
                numWidth.Value = res._w;
                numHeight.Value = res._h;
                _isSelectingRes = false;
            }
        }

        private void numResolution_ValueChanged(object sender, EventArgs e)
        {
            if(!_isSelectingRes && cbResolution.Items.Count > 0)
            {
                cbResolution.SelectedIndex = 0;
            }
        }

        private void checkFolder_CheckedChanged(object sender, EventArgs e)
        {
            txtFolderName.ReadOnly = !checkFolder.Checked;
            if(!checkFolder.Checked)
            {
                txtFolderName.Text = txtProjectName.Text;
            }
        }

        private void txtProjectName_TextChanged(object sender, EventArgs e)
        {
            if (!checkFolder.Checked)
            {
                txtFolderName.Text = txtProjectName.Text;
            }
        }

        private void txtProjectName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnNext_Click(sender, null);
            }
        }

        private void lstTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ��¼ѡȡ������Ŀ¼
            string theme = string.Empty;
            if (lstTemplate.SelectedIndex > 0)
            {
                string lastSelect = lstTemplate.SelectedItem as string;
                theme = lastSelect.Trim();
            }

            if (theme != _curConfig.ThemeName || theme == string.Empty)
            {
                _curConfig.ThemeName = theme;
                ShowProperty(_curConfig.ThemeInfo);
            }
        }

        private void lstProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ��¼ѡȡ����ĿĿ¼
            string project = string.Empty;
            if (lstProject.SelectedIndex >= 0)
            {
                string lastSelect = lstProject.SelectedItem as string;
                project = lastSelect.Trim();
            }

            if (project != _curConfig.ProjectName)
            {
                _curConfig.ProjectName = project;
                _curConfig.ProjectFolder = project;
                ShowProperty(_curConfig.ProjectInfo);
                txtFolderName.Text = project;
            }
        }

        private void Wizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!btnExit.Enabled)
            {
                MessageBox.Show("����" + CurOP + "��Ŀ�����Ժ򡭡�", (_titleSaved != null) ? _titleSaved : this.Text);
                e.Cancel = true;
            }
        }

        void conv_NotifyProcessEvent(ResConverter sender, ResConverter.NotifyProcessEventArgs e)
        {
            Logging(string.Format("({0}/{1}){2} ת���С���", e.index, e.count, e.file));
        }

        void conv_ErrorEvent(WizardConverter sender, WizardConverter.MessageEventArgs e)
        {
            ReportAppend(e.msg);
        }

        void conv_LoggingEvent(WizardConverter sender, WizardConverter.MessageEventArgs e)
        {
            Logging(e.msg);
        }

        void tabSelect_Selected(object sender, TabControlEventArgs e)
        {
            if(e.TabPage == tabTemplate)
            {
                _curConfig.IsModifyProject = false;
                _curConfig.ProjectFolder = string.Empty;
            }
            else if(e.TabPage == tabProject)
            {
                _curConfig.IsModifyProject = true;
            }

            // ǿ�ƽ���ˢ��
            txtReadme.Text = "";
            _curStep = -1;
            this.Step += 1;
        }
    }
}