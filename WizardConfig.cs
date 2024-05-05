using System;
using System.Collections.Generic;
using System.Text;
using Tjs;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Drawing;

namespace Wizard
{
    // �ֱ������ö���
    class Resolution
    {
        public int _w;
        public int _h;

        public Resolution(int w, int h) { _w = w; _h = h; }
        public override string ToString()
        {
            const float delta = 0.001f;

            string ratioStr = string.Empty;

            float ratio = (float)_w / _h;
            if (Math.Abs(ratio - 4.0 / 3.0) < delta)
            {
                ratioStr = "(4:3)";
            }
            else if (Math.Abs(ratio - 16.0 / 10.0) < delta)
            {
                ratioStr = "(16:10)";
            }
            else if (Math.Abs(ratio - 16.0 / 9.0) < delta)
            {
                ratioStr = "(16:9)";
            }
            else if (Math.Abs(ratio - 5.0 / 4.0) < delta)
            {
                ratioStr = "(5:4)";
            }

            return string.Format("{0}x{1} {2}", _w, _h, ratioStr);
        }

        public static Resolution[] List
        {
            get
            {
                return new Resolution[] {
                //new Resolution(640, 480),
                new Resolution(800, 600),
                new Resolution(1024, 768),
                new Resolution(1152, 864),
                new Resolution(1280, 720),
                new Resolution(1280, 800),
                //new Resolution(1280, 960),
                //new Resolution(1280, 1024),
                //new Resolution(1366, 768),
                //new Resolution(1400, 1050),
                //new Resolution(1440, 900),
                //new Resolution(1680, 1050),
                //new Resolution(1920, 1080),
                };
            }
        }
    }

    // ģ��Ļ�������
    class ProjectProperty
    {
        public string readme = string.Empty;

        public string title
        {
            get
            {
                // ��ȡ����
                string ret = null;
                if (setting != null)
                {
                    ret = setting.GetString("title");
                }
                return ret == null ? string.Empty : ret;
            }
        }
        
        public int width
        {
            get
            {
                // ��ȡԤ����
                double ret = double.NaN;
                if (setting != null)
                {
                    TjsValue val = setting.GetValue("width");
                    if (val != null) ret = val.ToDouble();
                }
                return double.IsNaN(ret) ? 0 : (int)ret;
            }
        }

        public int height
        {
            get
            {
                // ��ȡԤ��߶�
                double ret = double.NaN;
                if (setting != null)
                {
                    TjsValue val = setting.GetValue("height");
                    if (val != null) ret = val.ToDouble();
                }
                return double.IsNaN(ret) ? 0 : (int)ret;
            }
        }

        public int thumbnailwidth
        {
            get
            {
                // ��ȡ����ͼ��С
                double ret = double.NaN;
                if (setting != null)
                {
                    TjsValue val = setting.GetValue("savedata/thumbnailwidth");
                    if(val != null) ret = val.ToDouble();
                }
                return double.IsNaN(ret) ? 0 : (int)ret;
            }
        }

        public TjsDict setting = null;

        public void LoadSetting(string file)
        {
            setting = null;

            if (File.Exists(file))
            {
                setting = TjsValue.Load(file) as TjsDict;
            }
        }
    }

    // ��װһЩ�����ķ�������ת������
    class TjsHelper
    {
        public static double ScaleInteger(TjsDict dict, string name, double scale)
        {
            TjsValue val = dict.GetValue(name);
            double num = (val != null) ? val.ToDouble() : double.NaN;
            if (!double.IsNaN(num))
            {
                num = num * scale;
                dict.SetNumber(name, Math.Floor(num));
                return num;
            }

            string str = dict.GetString(name);
            if(double.TryParse(str, out num))
            {
                num = num * scale;
                dict.SetString(name, Math.Floor(num).ToString());
                return num;
            }

            return double.NaN;
        }

        public static TjsArray ScalePosArray(TjsDict dict, string name, double scaleX, double scaleY)
        {
            TjsValue v = null;
            if (dict.val.TryGetValue(name, out v))
            {
                // ����ǲ�������
                TjsArray arr = v as TjsArray;
                if (arr != null)
                {
                    // ���ж�ȡ����Ԫ�ص���������
                    List<TjsValue> arraynew = new List<TjsValue>();
                    foreach (TjsValue pos in arr.val)
                    {
                        Point p = Point.Empty;
                        if (TryGetPos(pos, out p))
                        {
                            // ����������
                            TjsArray posnew = CreatePos((int)(p.X * scaleX), (int)(p.Y * scaleY));
                            arraynew.Add(posnew);
                        }
                        else
                        {
                            Debug.Assert(false, "invalid struct in pos array");
                        }
                    }

                    dict.val[name] = new TjsArray(arraynew);
                    return arr;
                }
            }

            return null;
        }

        public static TjsArray ScaleButton(TjsDict dict, string name, double scaleX, double scaleY)
        {
            TjsValue v = null;
            if (dict.val.TryGetValue(name, out v))
            {
                // ��ť�϶��¼��һ���Ƿ���ʾ: x, y, shown
                TjsArray xys = v as TjsArray;
                if (xys != null && xys.val.Count == 3)
                {
                    TjsNumber x = xys.val[0] as TjsNumber;
                    TjsNumber y = xys.val[1] as TjsNumber;
                    TjsNumber s = xys.val[2] as TjsNumber;

                    if (x != null && y != null && s != null)
                    {
                        TjsArray xysnew = CreatePos((int)(x.val * scaleX), (int)(y.val * scaleY));
                        xysnew.val.Add(new TjsNumber(s.val));
                        dict.val[name] = xysnew;
                        return xysnew;
                    }
                    else
                    {
                        Debug.Assert(false, "invalid element in button struct");
                    }
                }
                else
                {
                    Debug.Assert(false, "invalid button struct");
                }
            }

            return null;
        }

        public static TjsArray CreatePos(int x, int y)
        {
            List<TjsValue> inner = new List<TjsValue>();
            inner.Add(new TjsNumber(x));
            inner.Add(new TjsNumber(y));
            return new TjsArray(inner);
        }

        public static bool TryGetPos(TjsValue pos, out Point p)
        {
            TjsArray xy = pos as TjsArray;
            if (xy != null && xy.val.Count == 2)
            {
                TjsNumber x = xy.val[0] as TjsNumber;
                TjsNumber y = xy.val[1] as TjsNumber;
                if (x != null && y != null)
                {
                    p = new Point((int)x.val, (int)y.val);
                    return true;
                }
                else
                {
                    Debug.Assert(false, "invalid element in pos struct");
                }
            }
            else
            {
                Debug.Assert(false, "invalid pos struct");
            }

            p = Point.Empty;
            return false;
        }
    }

    // ��Ŀ�����ö���
    class WizardConfig
    {
        // һЩ����
        public const string THEME_FOLDER = "\\skin";
        public const string TEMPLATE_FOLDER = "\\project\\template";
        public const string DATA_FOLDER = "\\data";
        public const string PROJECT_FOLDER = "\\project";

        public const string UI_LAYOUT = "macro\\ui*.tjs";
        public const string UI_SETTING = "macro\\setting.tjs";
        public const string UI_CONFIG = "Config.tjs";
        public const string UI_README = "Readme.txt";

        public const int DEFAULT_WIDTH = 1024;
        public const int DEFAULT_HEIGHT = 768;

        public const string NAME_DEFAULT_THEME = "Ĭ������";
        public const string PROJECT_DEFAULT_THEME = "template";

        // ����ָ����ͼƬ�ļ�
        const string PIC_IGNORE1 = @"data\system";
        const string PIC_IGNORE2 = @"system";
        public static bool IgnorePicture(string relFile)
        {
            string file = relFile.ToLower();
            return file.StartsWith(PIC_IGNORE1) || file.StartsWith(PIC_IGNORE2);
        }

        #region ���ݳ�Ա
        private string _baseFolder = string.Empty; // nvlmaker��Ŀ¼
        private string _themeName = string.Empty; // ����Ŀ¼��

        public int _height; // �ֱ���-�߶�
        public int _width;  // �ֱ���-���

        private string _projectName = string.Empty;     // ��Ŀ����
        private string _projectFolder = string.Empty;   // ��ĿĿ¼������ȡ������ΪĿ¼

        // Ŀǰ���žͰ�Ĭ����
        private string _scaler = ResFile.SCALER_DEFAULT; // ���Ų��ԣ�Ŀǰֻ������:(
        private string _quality = ResFile.QUALITY_DEFAULT;   // ����������Ĭ���Ǹ�

        // �Ƿ�����Ŀ�༭ģʽ
        private bool _modifyproject = false;

        // �����ϴζ�ȡ�����ԣ������ζ�ȡ������Bug���������ġ�
        private ProjectProperty _info = null;
        #endregion

        // nvlmaker��·��
        public string BaseFolder
        {
            get
            {
                // �����Ŀ¼����·������������β�� ��\��
                return _baseFolder;
            }
            set
            {
                // �����£���֤��Ϊ��ָ���հ��ִ�
                _baseFolder = (value == null ? string.Empty : value.Trim());
            }
        }

        // ����ģ��·��
        public string BaseTemplateFolder
        {
            get
            {
                return this.BaseFolder + TEMPLATE_FOLDER;
            }
        }

        // �Ƿ�ѡ����Ĭ������
        public bool IsDefaultTheme
        {
            get
            {
                return string.IsNullOrEmpty(this.ThemeName);
            }
        }

        // �Ƿ��ڱ༭��Ŀģʽ
        public bool IsModifyProject
        {
            get { return _modifyproject; }
            set
            {
                if (_modifyproject != value)
                {
                    this._modifyproject = value;
                    this._info = null;

                    this.ThemeName = string.Empty;
                    this.ProjectName = string.Empty;
                }
            }
        }

        // ��������
        public string ThemeName
        {
            get
            {
                return _themeName;
            }
            set
            {
                // �����£���֤��Ϊ��ָ���հ��ִ�
                string themeName = (value == null ? string.Empty : value.Trim());

                // ���������������Ԥ��������
                if(themeName != _themeName)
                {
                    this._themeName = themeName;
                    this._info = null;
                }
            }
        }

        // ����·��
        public string ThemeFolder
        {
            get
            {
                if (this.IsDefaultTheme)
                {
                    return this.BaseTemplateFolder;
                }
                else
                {
                    // ��������Ŀ¼�͸�Ŀ¼
                    return this.BaseFolder + THEME_FOLDER + "\\" + this.ThemeName;
                }
            }
        }

        // ���������ļ�
        public string ThemeSetting
        {
            get
            {
                return Path.Combine(this.ThemeDataFolder, UI_SETTING);
            }
        }

        // ���������Ŀ¼
        public string ThemeDataFolder
        {
            get
            {
                if (this.IsDefaultTheme)
                {
                    return this.ThemeFolder + DATA_FOLDER; 
                }
                else
                {
                    return this.ThemeFolder;
                }
            }
        }

        // ��ѡ���������
        public ProjectProperty ThemeInfo
        {
            get { return ReadInfo(this.ThemeDataFolder, this.ThemeSetting); }
        }

        // Ŀ����Ŀ·��
        public string ProjectFolder
        {
            get
            {
                if (_projectFolder.Length == 0)
                {
                    return this.BaseFolder + PROJECT_FOLDER + "\\" + _projectName;
                }
                else
                {
                    return this.BaseFolder + PROJECT_FOLDER + "\\" + _projectFolder;
                }
            }
            set
            {
                // 0�����ִ���ʾû�е���������ĿĿ¼
                _projectFolder = (value == null ? string.Empty : value.Trim());
            }
        }

        // Ŀ����Ŀ����·��
        public string ProjectDataFolder
        {
            get
            {
                return this.ProjectFolder + DATA_FOLDER;
            }
        }

        // Ŀ����Ŀ����
        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                // �����£���֤��Ϊ��ָ���հ��ִ�
                string projectName = (value == null ? string.Empty : value.Trim());

                if(_projectName != projectName)
                {
                    _projectName = projectName;
                    
                    // ��Ҫ����info��Ϣ
                    if(this.IsModifyProject) _info = null;
                }
            }
        }

        // ��Ŀ�����ļ�
        public string ProjectSetting
        {
            get
            {
                return Path.Combine(this.ProjectDataFolder, UI_SETTING);
            }
        }

        // ��ѡ��Ŀ������
        public ProjectProperty ProjectInfo
        {
            get { return ReadInfo(this.ProjectDataFolder, this.ProjectSetting); }
        }

        // �����������Ƿ��Ѿ��걸���ѳ�����Ϣд��output
        public bool IsReady(TextWriter output)
        {
            try
            {
                string path = this.BaseFolder;
                if (string.IsNullOrEmpty(_baseFolder) || !Directory.Exists(path))
                {
                    if (output != null) output.WriteLine("���������Ŀ¼�����ڡ�");
                    return false;
                }

                if (_height <= 0 || _width <= 0)
                {
                    if (output != null) output.WriteLine("������Ч�ķֱ������á�");
                    return false;
                }

                path = this.ProjectFolder;
                if (string.IsNullOrEmpty(_projectName))
                {
                    if (output != null) output.WriteLine("������Ч����Ŀ���ơ�");
                    return false;
                }
                else if (!this.IsModifyProject && Directory.Exists(path))
                {
                    if (output != null) output.WriteLine("������Ŀ�ļ����Ѵ��ڣ��������Ŀ������������·����");
                    return false;
                }

                if (this.IsModifyProject)
                {
                    ProjectProperty info = ProjectInfo;
                    if (info == null || (info.width == this._width && info.height == this._height))
                    {
                        if (output != null) output.WriteLine("������Ŀ�ֱ���δ�Ķ�������Ҫ����ת��");
                        return false;
                    }
                }
                else
                {
                    path = this.ThemeFolder;
                    if (!string.IsNullOrEmpty(path))
                    {
                        if (!Directory.Exists(path))
                        {
                            if (output != null) output.WriteLine("��������Ŀ¼�����ڡ�");
                            return false;
                        }

                        path = this.ThemeSetting;
                        if (string.IsNullOrEmpty(path) || !File.Exists(path))
                        {
                            if (output != null) output.WriteLine("���棺����ȱ�������ļ�");
                        }
                    }

                    ProjectProperty info = ThemeInfo;
                    if (info == null || info.height <= 0 || info.width <= 0)
                    {
                        if (output != null) output.WriteLine("���棺����ֱ��ʴ���");
                    }

                    ProjectProperty baseInfo = ReadBaseTemplateInfo();
                    if (baseInfo != info && (baseInfo == null || baseInfo.height <= 0 || baseInfo.width <= 0))
                    {
                        if (output != null) output.WriteLine("���棺Ĭ������ֱ��ʴ���");
                    }
                }

                // �������ñ���
                if(output != null)
                {
                    output.WriteLine(this.ToString());
                }
            }
            catch (System.Exception e)
            {
                if (output != null) output.WriteLine("��Ч����Ŀ���ã�" + e.Message);
                return false;
            }

            return true;
        }

        // �������õ��������ɱ���
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (this.IsModifyProject)
            {
                sb.AppendFormat("���޸���Ŀ�����嵥��"); sb.Append(Environment.NewLine);

                sb.Append(Environment.NewLine);
                ProjectProperty info = this.ProjectInfo;
                sb.AppendFormat("ԭʼ�ֱ��ʣ�{0}x{1}", info.width, info.height);sb.Append(Environment.NewLine);
                sb.AppendFormat("Ŀ��ֱ��ʣ�{0}x{1}", this._width, this._height);
                if (info.width != _width || info.height != this._height)
                {
                    sb.AppendFormat(" (�ֱ������޸�)");
                }
                sb.Append(Environment.NewLine);
            }
            else
            {
                sb.AppendFormat("��������Ŀ�����嵥��"); sb.Append(Environment.NewLine);

                sb.Append(Environment.NewLine);
                string theme = (this.IsDefaultTheme) ? NAME_DEFAULT_THEME : this.ThemeName;
                sb.AppendFormat("��ѡ���⣺{0}", theme); sb.Append(Environment.NewLine);
                sb.AppendFormat("�ֱ����趨��{0}x{1}", this._width, this._height);
                ProjectProperty info = this.ThemeInfo;
                if (info.width != _width || info.height != this._height)
                {
                    sb.AppendFormat(" (��ԭʼ�ֱ���)");
                }
                sb.Append(Environment.NewLine);
            }

            sb.Append(Environment.NewLine);
            sb.AppendFormat("��Ŀ���ƣ�{0}", this._projectName);sb.Append(Environment.NewLine);
            sb.AppendFormat("��Ŀλ�ã�{0}", this.ProjectFolder); sb.Append(Environment.NewLine);
            
            sb.Append(Environment.NewLine);
            sb.AppendFormat("���Ų��ԣ�{0}", this._scaler); sb.Append(Environment.NewLine);
            sb.AppendFormat("����������{0}", this._quality); sb.Append(Environment.NewLine);
            sb.AppendFormat("NVLMakerĿ¼��{0}", this.BaseFolder);sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        // ��DataĿ¼�¶�ȡ����
        private ProjectProperty ReadInfo(string dataFolder, string setting)
        {
            // ֱ�ӷ�����һ�ζ�ȡ��ֵ
            if (this._info != null)
            {
                return this._info;
            }

            ProjectProperty info = new ProjectProperty();
            this._info = info;

            // ��ȡreadme�ļ���Ϊ��ʾ����
            try
            {
                string readmefile = Path.Combine(dataFolder, UI_README);
                if (File.Exists(readmefile))
                {
                    using (StreamReader r = new StreamReader(readmefile))
                    {
                        info.readme = r.ReadToEnd();
                    }
                }
                else
                {
                    string configfile = Path.Combine(dataFolder, UI_CONFIG);
                    if (File.Exists(configfile))
                    {
                        using (StreamReader r = new StreamReader(configfile))
                        {
                            info.readme = r.ReadToEnd();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                // ����Ĳ�����
                this._info = null;
                info.readme = e.Message;
            }

            // ��ȡ�����ļ�
            try
            {
                info.LoadSetting(setting);
            }
            catch (System.Exception e)
            {
                // ����Ĳ�����
                this._info = null;
                info.readme = e.Message;
            }
            
            return info;
        }

        // ��ȡ����ģ�������
        public ProjectProperty ReadBaseTemplateInfo()
        {
            // ���ѡ����Ĭ�ϵ����⣬�򷵻���������
            if(this.IsDefaultTheme)
            {
                return this.ThemeInfo;
            }

            // ����Ͳ���readme�ˣ�Ҳ�������棬ÿ�ε��ö����ļ���һ��
            string file = Path.Combine(this.BaseTemplateFolder + DATA_FOLDER, UI_SETTING);
            ProjectProperty info = new ProjectProperty();
            try
            {
                info.LoadSetting(file);
            }
            catch (System.Exception e)
            {
                info.readme = e.Message;
            }
            return info;
        }
    }

    // ������Ŀ����ת����Ŀ�ļ�
    class WizardConverter : ResConverter
    {
        #region �¼���Logging��Report��Ϣ֪ͨ
        public class MessageEventArgs : EventArgs
        {
            public readonly string msg;
            public MessageEventArgs(string msg)
            {
                this.msg = msg;
            }
        }
        // ��Ϣ֪ͨ��Ӧ����
        public delegate void MessageHandler(WizardConverter sender, MessageEventArgs e);
        
        // Logging�¼�
        public event MessageHandler LoggingEvent;
        protected void OnLogging(string msg)
        {
            if (LoggingEvent != null)
            {
                LoggingEvent(this, new MessageEventArgs(msg));
            }
        }

        // ������Ϣ֪ͨ�¼�
        public event MessageHandler ErrorEvent;
        protected void OnError(string msg)
        {
            if (ErrorEvent != null)
            {
                ErrorEvent(this, new MessageEventArgs(msg));
            }
        }
        #endregion

        WizardConfig _config;

        public WizardConverter(WizardConfig config)
        {
            _config = config;
        }

        // �������ô���Ŀ����Ŀ
        public void Start()
        {
            if (_config.IsModifyProject)
            {
                // �������ж�ȡĿ���С
                int dw = _config._width, dh = _config._height;

                // ����Ŀ������ļ���ת��
                string project = _config.ProjectFolder;

                // ��ȡ��Ŀԭʼ����
                ProjectProperty projInfo = _config.ProjectInfo;
                int sw = projInfo.width, sh = projInfo.height;

                ConvertFiles(project, sw, sh, project, dw, dh);

                // ������������
                AdjustSettings(sw, sh);
            }
            else
            {
                // �������ж�ȡĿ���С
                int dw = _config._width, dh = _config._height;

                // �ȴӻ���ģ��Ŀ¼�����ļ�����ĿĿ¼
                string template = _config.BaseTemplateFolder;
                string project = _config.ProjectFolder;

                // ��ȡ����ģ�������
                ProjectProperty baseInfo = _config.ReadBaseTemplateInfo();

                int sw = baseInfo.width;
                if (sw <= 0) sw = WizardConfig.DEFAULT_WIDTH;
                int sh = baseInfo.height;
                if (sh <= 0) sh = WizardConfig.DEFAULT_HEIGHT;

                ConvertFiles(template, sw, sh, project, dw, dh);

                // �����������꣬д����Ŀ����
                AdjustSettings(sw, sh);

                // ���ѡ���˷�Ĭ�����⣬�ٴ�����Ŀ¼�����ļ�����Ŀ�����ļ���
                if (_config.ThemeFolder != template)
                {
                    // ��ȡ��ѡ��������
                    ProjectProperty themeInfo = _config.ThemeInfo;

                    sw = themeInfo.width;
                    if (sw <= 0) sw = WizardConfig.DEFAULT_WIDTH;
                    sh = themeInfo.height;
                    if (sh <= 0) sh = WizardConfig.DEFAULT_HEIGHT;

                    // ������ļ�ֱ�ӿ�������Ŀ¼
                    ConvertFiles(_config.ThemeFolder, sw, sh, _config.ProjectDataFolder, dw, dh);

                    // �����������꣬д����Ŀ����
                    AdjustSettings(sw, sh);
                }
            }
        }

        // �����������ļ�
        void ConvertFiles(string srcPath, int sw, int sh, string destPath, int dw, int dh)
        {
            // Դ�ļ��б�
            List<string> srcFiles = new List<string>();
            try
            {
                bool ignoreCopy = (srcPath.ToLower() == destPath.ToLower());

                // ����Ŀ¼����ȡ�ļ��б�
                CreateDir(srcPath, destPath, srcFiles);

                // ����ͼƬת�����ã����ڼ�¼��Ҫת����ͼƬ�ļ��������ļ���ֱ�ӿ���
                ResConfig resource = new ResConfig();
                resource.path = srcPath;
                resource.name = WizardConfig.NAME_DEFAULT_THEME;

                // ���������ļ�
                int cutLen = srcPath.Length;
                foreach (string srcfile in srcFiles)
                {
                    // �ص�ģ��Ŀ¼�Ծ���ȡ���·��
                    string relFile = srcfile.Substring(cutLen + 1);

                    // ȡ����չ��
                    string ext = Path.GetExtension(relFile).ToLower();

                    if ( // ��������Դ�ļ���ͬ�ǾͲ���ת����
                         (sw != dw || sh != dh) &&
                        // ����ĳЩͼƬ
                         !WizardConfig.IgnorePicture(relFile) &&
                        // ֻת����Щ��չ����Ӧ���ļ�
                         (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp"))
                    {
                        // ��ͼƬ����ӵ�ת������
                        resource.files.Add(new ResFile(relFile));
                    }
                    else if (!ignoreCopy)
                    {
                        // ֱ�ӿ���
                        OnLogging(string.Format("����{0}", relFile));
                        File.Copy(srcfile, Path.Combine(destPath, relFile), true);
                    }
                }

                OnLogging("ͼƬת���С���");

                if (resource.files.Count > 0)
                {
                    // ������Դת��������������ʼת��
                    Start(resource, destPath, sw, sh, dw, dh);
                }

            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // ����Ŀ����Ŀ�ļ����е�����
        void AdjustSettings(int sw, int sh)
        {
            string dataPath = _config.ProjectDataFolder;
            int dh = _config._height;
            int dw = _config._width;
            string title = _config.ProjectName;

            try
            {
                ModifySetting(dataPath, title, sw, sh, dh, dw);
            }
            catch (System.Exception e)
            {
                OnError("�޸�setting.tjsʧ��:" + e.Message);
            }

            try
            {
                ModifyConfig(dataPath, title, sw, sh, dh, dw);
            }
            catch (System.Exception e)
            {
                OnError("�޸�Config.tjsʧ��:" + e.Message);
            }

            // ����Ƿ���Ҫת��
            if (sw != dw || sh != dh)
            {
                try
                {
                    ModifyLayout(dataPath, sw, sh, dh, dw);
                }
                catch (System.Exception e)
                {
                    OnError("�޸Ľ��沼���ļ�ʧ��:" + e.Message);
                }
            }
        }

        // �޸��ֵ��ļ�
        static void ModifyDict(TjsDict dict, int sw, int sh, int dw, int dh)
        {
            double scaleX = (double)dw / sw;
            double scaleY = (double)dh / sh;

            TjsHelper.ScaleInteger(dict, "left", scaleX);
            TjsHelper.ScaleInteger(dict, "x", scaleX);
            TjsHelper.ScaleInteger(dict, "marginr", scaleX);
            TjsHelper.ScaleInteger(dict, "marginl", scaleX);

            TjsHelper.ScaleInteger(dict, "top", scaleY);
            TjsHelper.ScaleInteger(dict, "y", scaleY);
            TjsHelper.ScaleInteger(dict, "margint", scaleY);
            TjsHelper.ScaleInteger(dict, "marginb", scaleY);

            // �޸�locate����
            TjsHelper.ScalePosArray(dict, "locate", scaleX, scaleY);

            foreach (KeyValuePair<string, TjsValue> kv in dict.val)
            {
                TjsDict inner = kv.Value as TjsDict;
                if(inner != null)
                {
                    ModifyDict(inner, sw, sh, dw, dh);
                }
            }
        }

        // �޸�UI�����ļ�
        static void ModifyLayout(string dataPath, int sw, int sh, int dh, int dw)
        {
            // ����layout
            string[] layouts = Directory.GetFiles(dataPath, WizardConfig.UI_LAYOUT);
            foreach (string layout in layouts)
            {
                TjsDict setting = TjsValue.Load(layout) as TjsDict;

                if (setting != null)
                {
                    ModifyDict(setting, sw, sh, dw, dh);

                    // ������ļ���İ�ť�����⴦��
                    if(layout.ToLower().EndsWith("uislpos.tjs"))
                    {
                        double scaleX = (double)dw / sw;
                        double scaleY = (double)dh / sh;
                        TjsHelper.ScaleButton(setting, "back", scaleX, scaleY);
                        TjsHelper.ScaleButton(setting, "up", scaleX, scaleY);
                        TjsHelper.ScaleButton(setting, "down", scaleX, scaleY);
                    }
                }

                setting.Save(layout, Encoding.Unicode);
            }
        }

        // �޸�config.tjs
        static void ModifyConfig(string dataPath, string title, int sw, int sh, int dh, int dw)
        {
            // ����config
            string configFile = Path.Combine(dataPath, WizardConfig.UI_CONFIG);
            if (File.Exists(configFile))
            {
                Regex regTitle = new Regex(@"\s*;\s*System.title\s*=");
                Regex regW = new Regex(@"\s*;\s*scWidth\s*=");
                Regex regH = new Regex(@"\s*;\s*scHeight\s*=");
                Regex regThumb = new Regex(@"\s*;\s*thumbnailWidth\s*=");

                StringBuilder buf = new StringBuilder();
                using (StreamReader r = new StreamReader(configFile))
                {
                    while (!r.EndOfStream)
                    {
                        string line = r.ReadLine();
                        if (regTitle.IsMatch(line))
                        {
                            buf.AppendLine(string.Format(";System.title = \"{0}\";", title));
                        }
                        else if (regW.IsMatch(line))
                        {
                            buf.AppendLine(string.Format(";scWidth = {0};", dw));
                        }
                        else if (regH.IsMatch(line))
                        {
                            buf.AppendLine(string.Format(";scHeight = {0};", dh));
                        }
                        else if (regThumb.IsMatch(line))
                        {
                            // ��ʱ����һ�����Զ������ڶ�ȡsetting
                            string settingFile = Path.Combine(dataPath, WizardConfig.UI_SETTING);
                            ProjectProperty info = new ProjectProperty();
                            info.LoadSetting(settingFile);

                            // ��������ͼ
                            int tw = info.thumbnailwidth;
                            if (tw > 0)
                            {
                                buf.AppendLine(string.Format(";thumbnailWidth = {0};", tw));
                            }
                        }
                        else
                        {
                            buf.AppendLine(line);
                        }
                    }
                }

                using (StreamWriter w = new StreamWriter(configFile, false, Encoding.Unicode))
                {
                    w.Write(buf.ToString());
                }
            }
        }

        // �޸�setting.tjs
        static void ModifySetting(string dataPath, string title, int sw, int sh, int dh, int dw)
        {
            // ����setting
            string settingFile = Path.Combine(dataPath, WizardConfig.UI_SETTING);
            
            // ��ʱ����һ�����Զ������ڶ�ȡsetting
            ProjectProperty info = new ProjectProperty();
            info.LoadSetting(settingFile);
            TjsDict setting = info.setting;

            if (setting != null)
            {
                setting.SetString("title", title);
                setting.SetNumber("width", dw);
                setting.SetNumber("height", dh);

                // ��������ͼ���
                int tw = info.thumbnailwidth;
                if (tw > 0)
                {
                    tw = tw * dw / sw;
                    setting.SetValue("savedata/thumbnailwidth", new TjsString(tw.ToString()));
                }

                setting.Save(settingFile, Encoding.Unicode);
            }
        }

        // ���ߺ����������ļ��У�����¼���е��ļ�
        static void CreateDir(string source, string dest, List<string> files)
        {
            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }

            if (files != null)
            {
                string[] curFiles = Directory.GetFiles(source);
                files.AddRange(curFiles);
            }

            string[] subDirs = Directory.GetDirectories(source);
            if (subDirs.Length == 0)
            {
                // ľ���ҵ��κ���Ŀ¼
                return;
            }

            foreach (string dir in subDirs)
            {
                string name = Path.GetFileName(dir);
                CreateDir(dir, Path.Combine(dest, name), files);
            }
        }
    }
}
