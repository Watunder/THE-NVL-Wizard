using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Drawing.Imaging;

namespace Wizard
{
    public class ResFile
    {
        // �ļ����Կ�ѡֵ
        public static readonly string SCALER_DEFAULT = "auto";
        public static readonly string QUALITY_DEFAULT = "high";
        public static readonly string QUALITY_NORMAL = "low";
        public static readonly string QUALITY_LOW = "normal";

        // �ļ�·��
        [XmlAttribute]
        public string path = string.Empty;

        // ���Ų���
        [XmlAttribute]
        public string scaler = SCALER_DEFAULT;

        // ��������
        [XmlAttribute]
        public string quality = QUALITY_DEFAULT;

        public override string ToString()
        {
            return path;
        }

        public ResFile(string path)
        {
            this.path = path;
        }
    }

    [XmlRootAttribute("Config", IsNullable = false)]
    public class ResConfig
    {
        // ֻ�Ǹ����ֶ��ѣ��������д
        [XmlAttribute]
        public string name = "Ĭ����Դ�ļ��б�";

        [XmlElement("File")]
        public List<ResFile> files = new List<ResFile>();

        // ��¼��Դ�ļ��ĸ�Ŀ¼��һ�������Դ�ļ����ڵ�Ŀ¼
        [XmlIgnoreAttribute]
        public string path = string.Empty;

        // ���ļ�������ResConfig����
        static public ResConfig Load(string filename)
        {
            try
            {
                using (StreamReader r = new StreamReader(string.Format(filename)))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ResConfig));
                    ResConfig obj = (ResConfig)serializer.Deserialize(r);
                    r.Close();

                    obj.path = Path.GetDirectoryName(filename);
                    return obj;
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        // �ѵ�ǰResConfig���󴢴浽�ļ���
        public bool Save(string filename)
        {
            try
            {
                using (StreamWriter w = new StreamWriter(string.Format(filename)))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ResConfig));
                    serializer.Serialize(w, this);
                    w.Close();
                    return true;
                }
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }

    class ResConverter
    {
        #region �¼���ת������֪ͨ
        public class NotifyProcessEventArgs : EventArgs
        {
            public readonly string file;
            public readonly int index;
            public readonly int count;

            public NotifyProcessEventArgs(string file, int index, int count)
            {
                this.file = file;
                this.index = index;
                this.count = count;
            }
        }
        // ת������֪ͨ��Ӧ����
        public delegate void NotifyProcessHandler(ResConverter sender, NotifyProcessEventArgs e);
        // ת������֪ͨ�¼�
        public event NotifyProcessHandler NotifyProcessEvent;
        // ת������֪ͨ������
        protected void OnNotifyProcess(string file, int index, int count)
        {
            if(NotifyProcessEvent != null)
            {
                NotifyProcessEventArgs e = new NotifyProcessEventArgs(file, index, count);
                NotifyProcessEvent(this, e);
            }
        }
        #endregion

        enum Quality
        {
            LOW,
            NORMAL,
            HIGH,
        }

        // ��������ת��ָ������Դ
        public void Start( // ��Դ�ļ��б�
                           ResConfig config,
                           // Ŀ��·��
                           string destFolder,
                           // Դ��Ļ��С
                           int srcWidth, int srcHeight,
                           // Ŀ����Ļ��С
                           int destWidth, int destHeight )
        {
            // ������Ч����
            if (srcWidth <= 0 || srcHeight <= 0 || destWidth <= 0 || destHeight <= 0)
                return;

            if (srcWidth == destWidth && srcHeight == destHeight)
                return;

            int cur = 0;

            // ��rootΪ��Ŀ¼��������ͼƬ������
            string baseDir = config.path;
            foreach (ResFile file in config.files)
            {
                OnNotifyProcess(file.path, ++cur, config.files.Count);

                string inputFile = Path.GetFullPath(Path.Combine(baseDir, file.path));
                string destFile = Path.GetFullPath(Path.Combine(destFolder, file.path));

                // ѡ����������
                Quality q = Quality.HIGH;
                if (file.quality.ToLower() == ResFile.QUALITY_LOW)
                    q = Quality.LOW;
                else if (file.quality.ToLower() == ResFile.QUALITY_NORMAL)
                    q = Quality.NORMAL;

                try
                {
                    Bitmap dest = null;
                    ImageFormat format = null;

                    // ��ȡԴͼƬ
                    using (Bitmap source = new Bitmap(inputFile))
                    {
                        // ���ݲ��Լ�������ӳ�䣨��δʵ�֣�
                        Dictionary<Rectangle, Rectangle> rects =
                            CalcRects(source, srcWidth, srcHeight, destWidth, destHeight);

                        // ʵʩת��
                        dest = Scale(source, srcWidth, srcHeight, destWidth, destHeight, q, rects);

                        format = source.RawFormat;
                    }

                    if(dest != null)
                    {
                        dest.Save(destFile, format);
                    }

                    // ת�����

                }
                catch (System.Exception e)
                {
                    // ת�����ִ���
                    Console.WriteLine(e.Message);
                }
            }
        }

        // ���ݲ��Լ�������ӳ��
        Dictionary<Rectangle, Rectangle> CalcRects( Bitmap source,
                                                    // Դ��Ļ��С
                                                    int srcWidth, int srcHeight,
                                                    // Ŀ����Ļ��С
                                                    int destWidth, int destHeight )
        {
            // ��δʵ��
            return null;
        }

        // ���ݸ���������ӳ���������������ԴͼƬ���ŵ�Ŀ���С
        Bitmap Scale( Image source,
                      // Դ��Ļ��С
                      int srcWidth, int srcHeight,
                      // Ŀ����Ļ��С
                      int destWidth, int destHeight,
                      // ת������
                      Quality q,
                      // ת������ӳ��
                      Dictionary<Rectangle, Rectangle> rects)
        {
            // ������Ч����
            if (srcWidth <= 0 || srcHeight <= 0 || destWidth <= 0 || destHeight <= 0)
                return null;

            int realWidth =  (source.Width * destWidth) / srcWidth;
            int realHeight = (source.Height * destHeight) / srcHeight;

            Bitmap dest = new Bitmap(realWidth, realHeight);
            using (Graphics g = Graphics.FromImage(dest))
            {
                // ���������������������㷨
                switch (q)
                {
                    case Quality.LOW:
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                        break;
                    case Quality.NORMAL:
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        break;
                    case Quality.HIGH:
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        break;
                }

                if (rects == null || rects.Count == 0)
                {
                    // ֱ�����ŵ�ָ���Ĵ�С
                    g.DrawImage(source, 0, 0, realWidth, realHeight);
                }
                else
                {
                    // ���չ滮����������
                    foreach (KeyValuePair<Rectangle, Rectangle> kp in rects)
                    {
                        if (kp.Value.Left >= realWidth || kp.Value.Top >= realHeight)
                        {
                            // ������Ч����
                            continue;
                        }

                        g.DrawImage(source, kp.Value, kp.Key, GraphicsUnit.Pixel);
                    }
                }
            }

            return dest;
        }
    }
}
