using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Tjs
{
    #region Tjs��������
    public enum TjsType
    {
        Void,
        Number,
        String,
        Array,
        Dictionary,
    }

    // ����
    public class TjsValue
    {
        public TjsType t;

        #region ����
        // ����ȫ��������Ԫ
        public static string Indent
        {
            get { return _indent; }
            set { _indent = value; }
        }

        // ������Ԫ
        protected static string _indent = " ";
        // ������ջ
        protected static string _indentStack = string.Empty;
        #endregion

        public static TjsValue Load(string file)
        {
            using (StreamReader r = new StreamReader(file))
            {
                TjsParser parser = new TjsParser();
                return parser.Parse(r);
            }
        }

        public void Save(string file, Encoding encode)
        {
            using (StreamWriter w = new StreamWriter(file, false, encode))
            {
                w.Write(this.ToString());
            }
        }

        public virtual double ToDouble()
        {
            return double.NaN;
        }
    }

    // ��ֵ
    public class TjsVoid : TjsValue
    {
        public TjsVoid()
        {
            this.t = TjsType.Void;
        }

        public override string ToString()
        {
            return "void";
        }
    }

    // �ַ���
    public class TjsString : TjsValue
    {
        public readonly string val;

        public TjsString(string val)
        {
            this.val = val;
            this.t = TjsType.String;
        }

        public override string ToString()
        {
            return string.Format("\"{0}\"", this.val);
        }

        public override double ToDouble()
        {
            double ret = double.NaN;
            if (this.val != null) double.TryParse(this.val, out ret);
            return ret;
        }
    }

    // ����
    public class TjsNumber : TjsValue
    {
        public readonly double val;

        public TjsNumber(double val)
        {
            this.val = val;
            this.t = TjsType.Number;
        }

        public override string ToString()
        {
            return this.val.ToString();
        }

        public override double ToDouble()
        {
            return this.val;
        }
    }

    // ����
    public class TjsArray : TjsValue
    {
        public readonly List<TjsValue> val;

        public TjsArray(List<TjsValue> val)
        {
            this.val = val;
            this.t = TjsType.Array;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();

            // ���浱ǰ��������������
            string savedIndent = _indentStack;
            if (_indent != null)
            {
                _indentStack += _indent;
            }

            // ��ǰĬ������
            string currentIndent = _indentStack;

            //buf.AppendLine("(const) [");
			buf.AppendLine("["); // ����2.28
            int count = 0;
            foreach (TjsValue v in this.val)
            {
                buf.Append(currentIndent); buf.Append(v.ToString());

                // ĩβ׷�Ӷ��ŷָ���
                if (++count < this.val.Count) buf.AppendLine(",");
            }
            buf.AppendLine(""); buf.Append(savedIndent); buf.Append("]");

            // �ָ�����
            _indentStack = savedIndent;
            return buf.ToString();
        }
    }

    // �ֵ�
    public class TjsDict : TjsValue
    {
        public readonly Dictionary<string, TjsValue> val;

        public TjsDict(Dictionary<string, TjsValue> val)
        {
            this.val = val;
            this.t = TjsType.Dictionary;
        }

        public string GetString(string name)
        {
            TjsValue v = null;
            this.val.TryGetValue(name, out v);

            TjsString ret = v as TjsString;
            return (ret != null) ? ret.val : null;
        }

        public TjsString SetString(string name, string value)
        {
            TjsString v = new TjsString(value);
            this.val[name] = v;
            return v;
        }

        public double GetNumber(string name)
        {
            TjsValue v = null;
            this.val.TryGetValue(name, out v);

            TjsNumber ret = v as TjsNumber;
            return (ret != null) ? ret.val : double.NaN;
        }

        public TjsNumber SetNumber(string name, double value)
        {
            TjsNumber v = new TjsNumber(value);
            this.val[name] = v;
            return v;
        }

        public TjsValue GetValue(string namepath)
        {
            int split = namepath.IndexOf("/");
            if (split < 0)
            {
                // ��ȡ����ֵ
                TjsValue v = null;
                this.val.TryGetValue(namepath, out v);
                return v;
            }
            else
            {
                string name = namepath.Substring(0, split);

                TjsValue v = null;
                if (this.val.TryGetValue(name, out v))
                {
                    TjsDict next = v as TjsDict;
                    if (next != null)
                    {
                        return next.GetValue(namepath.Substring(split + 1));
                    }
                }
            }

            return null;
        }

        public TjsDict SetValue(string namepath, TjsValue value)
        {
            int split = namepath.IndexOf("/");
            if(split < 0)
            {
                // ������ײ��dict
                this.val[namepath] = value;
                return this;
            }
            else
            {
                string name = namepath.Substring(0, split);

                TjsValue v = null;
                if (this.val.TryGetValue(name, out v))
                {
                    TjsDict next = v as TjsDict;
                    if (next != null)
                    {
                        return next.SetValue(namepath.Substring(split+1), value);
                    }
                }
            }

            return null;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();

            // ���浱ǰ��������������
            string savedIndent = _indentStack;
            if (_indent != null)
            {
                _indentStack += _indent;
            }

            // ��ǰĬ������
            string currentIndent = _indentStack;

            //buf.AppendLine("(const) %[");
			buf.AppendLine("%["); // ����2.28
            int count = 0;
            foreach (KeyValuePair<string, TjsValue> kv in this.val)
            {
                buf.Append(currentIndent); buf.Append("\""); buf.Append(kv.Key);
                buf.Append("\" => "); buf.Append(kv.Value.ToString());

                // ĩβ׷�Ӷ��ŷָ���
                if (++count < this.val.Count) buf.AppendLine(",");
            }
            buf.AppendLine(""); buf.Append(savedIndent); buf.Append("]");

            // �ָ�����
            _indentStack = savedIndent;
            return buf.ToString();
        }
    }
    #endregion

    // Tjs���ݽ�����
    class TjsParser
    {
        // ��Ĭ��buffer��С��ʼ��
        public TjsParser()
        {
            Reset(DEFAULT_BUFFER_SIZE);
        }

        // ��ָ��buffer��С��ʼ��
        public TjsParser(int bufferSize)
        {
            Reset(bufferSize);
        }

        #region Tjs���ŵ�Ԫ
        public enum TokenType
        {
            Unknow,
            String,
            Number,
            Symbol,
        }

        public class Token
        {
            public string val = string.Empty;
            public TokenType t = TokenType.Unknow;

            public TjsString ToTjsString()
            {
                if(this.val.Length >= 2)
                {
                    string inner = this.val.Substring(1, this.val.Length - 2);
                    TjsString val = new TjsString(inner);
                    return val;
                }
                return null;
            }

            public TjsNumber ToTjsNumber()
            {
                double inner = 0;
                if(double.TryParse(this.val, out inner))
                {
                    TjsNumber val = new TjsNumber(inner);
                    return val;
                }
                return null;
            }
        }
        #endregion

        #region ���������н���Tjs����
        Regex _regNumber = new Regex(@"[0-9\.]");
        Regex _regNonChar = new Regex(@"\s");
        Regex _regSeprater = new Regex(@"[,]");

        // Ĭ��ʹ�õ�buffer��С
        const int DEFAULT_BUFFER_SIZE = 8192;
        
        // �����ȡ��������
        char[] _buffer;

        // ָ��buffer�н�Ҫ��ȡ���ַ�
        int _pos;
        
        // �ѽ������ַ���
        int _parsed;

        // buffer�е�ʵ����Ч����
        int _len;

        // �������б���
        void Reset(int size)
        {
            // �����ȡ��������
            if (_buffer == null || _buffer.Length != size)
            {
                _buffer = new char[size];
            }

            // ָ��buffer�н�Ҫ��ȡ���ַ�����ʼ״̬����Buffer����
            _pos = size;

            // �Ѵ�����ַ��������_pos�ĳ�ֵ
            _parsed = -size;

            // buffer�е�ʵ����Ч����
            _len = 0;

            // ���ô�����Ϣ
            _error = false;
        }

        // �ѽ������ַ���
        public int Parsed
        {
            get { return _parsed + _pos; }
        }

        // ��ȡ�����������bufferδ���Ĳ���
        void UpdateBuffer(TextReader r)
        {
            for (int i = _pos; i < _len; i++)
            {
                _buffer[i - _pos] = _buffer[i];
            }

            // �����µ���ʼ��
            int start = _len > _pos ? _len - _pos : 0;

            // ͳ�ƽ������ַ���
            _parsed += _pos;

            // ���õ�ǰλ��
            _pos = 0;
            _len = start;

            // ��������������ȡ��buffer��
            if (r.Peek() >= 0)
            {
                _len += r.ReadBlock(_buffer, start, _buffer.Length - start);
            }
        }

        public Token GetNext(TextReader r)
        {
            // ���buffer��������Ҫ����
            if (_pos >= _buffer.Length)
            {
                UpdateBuffer(r);
            }

            TokenType t = TokenType.Unknow;
            int head = _pos; // ָ���һ����Ч�ַ�
            int tail = -1; // ָ�����һ����Ч�ַ�

            StringBuilder stored = new StringBuilder();

            while (_pos < _len)
            {
                // ��ȡһ���ַ���ת���ִ���Ϊ��ƥ��������ʽ
                string cur = _buffer[_pos++].ToString();

                //
                // ʹ��if-else����ΪҪ��break���˳�whileѭ��
                //
                if(t == TokenType.Unknow)
                {
                    // ���������Ч�ַ�
                    if (_regNonChar.IsMatch(cur)) { head++; }
                    // �����ַ���
                    else if (cur[0] == '"') { t = TokenType.String; }
                    // ��������
                    else if (_regNumber.IsMatch(cur)) { t = TokenType.Number; }
                    // �������Ϊ����
                    else { t = TokenType.Symbol; }
                }
                else if(t == TokenType.String)
                {
                    // �Դ���Ϊ��β
                    if (cur[0] == '"') { tail = _pos - 1; break; }
                }
                else if (t == TokenType.Number)
                {
                    // ���������Ч�ַ�
                    if (_regNonChar.IsMatch(cur)) { tail = _pos - 2; break; }
                    // ��������ַ�
                    else if (!_regNumber.IsMatch(cur)) { _pos--; tail = _pos - 1; break; }
                }
                else if(t == TokenType.Symbol)
                {
                    // ���������Ч�ַ�
                    if (_regNonChar.IsMatch(cur)) { tail = _pos - 2; break; }
                    // ��������ַ�
                    else if (_regSeprater.IsMatch(cur)) { _pos--; tail = _pos - 1; break; }
                }

                // ����Ƿ�buffer����
                if (_pos >= _buffer.Length)
                {
                    Debug.Assert(_pos == _buffer.Length, "_pos should not larger than buffer size");

                    if (_pos > head)
                    {
                        // ��buffer��δ���token���д���
                        stored.Append(_buffer, head, _pos - head);
                    }

                    UpdateBuffer(r);
                    head = _pos;
                }
                // ��ȡ����
                else if (_pos >= _len)
                {
                    Debug.Assert(_pos == _len, "_pos should not larger than actual size");

                    tail = _pos - 1;
                }
            }

            // ׷�ӵ�ǰ���
            if (tail >= head)
            {
                stored.Append(_buffer, head, tail - head + 1);
            }

            // ��������ֵ
            Token token = new Token();
            token.t = t;
            if (stored.Length > 0)
            {
                token.val = stored.ToString();
            }
            return token;
        }
        #endregion

        #region ��ʾ������Ϣ
        bool _error;
        public bool IsError
        {
            get { return _error; }
        }

        void ShowError(string msg)
        {
            _error = true;
            Console.Write("Error: ");
            Console.WriteLine(msg);
        }

        void ShowError(Token token)
        {
            _error = true;
            Console.Write("Error occurred at offset ");
            Console.Write(this.Parsed);
            Console.WriteLine(", parse terminated.");
            if(token == null)
            {
                Console.WriteLine("<< blank token >>");
            }
            else
            {
                Console.Write("Type=");
                Console.Write(token.t.ToString());
                Console.Write(", Value=");
                Console.WriteLine(token.val);
            }
        }
        #endregion

        #region ����һ���ֵ�
        enum DictState
        {
            Key,
            Value,
        };
        TjsDict ParseDict(TextReader r)
        {
            TjsParser.Token token = null;

            Dictionary<string, TjsValue> inner = new Dictionary<string, TjsValue>();

            // ��ʼ״̬Ϊ��ȡkey״̬
            DictState s = DictState.Key;
            string key = null;

            do
            {
                token = GetNext(r);

                if(s == DictState.Key)
                {
                    // ��ȡ��ֵ
                    if(token.t == TokenType.String && key == null)
                    {
                        TjsString tmp = token.ToTjsString();
                        if(tmp == null)
                        {
                            // ��Ч�ļ�ֵ
                            ShowError("Invalid Key");
                            break;
                        }

                        // ȥ��˫����
                        key = tmp.val;
                        
                        if(inner.ContainsKey(key))
                        {
                            // �ظ��ļ�ֵ
                            ShowError("Duplicated Key");
                            break;
                        }

                        // �л�����ȡValue״̬
                        s = DictState.Value;
                    }
                    else
                    {
                        // ����ļ�ֵ
                        ShowError("Expect a Key");
                        break;
                    }
                }
                else if(s == DictState.Value)
                {
                    if (token.t == TokenType.Symbol)
                    {
                        if(key != null && token.val == "=>")
                        {
                            // ��ȡһ��ֵ
                            TjsValue val = Parse(r);

                            // ֱ�ӷ��ش���
                            if (val == null) return null;

                            inner.Add(key, val);
                            key = null;
                        }
                        else if(key == null && token.val == ",")
                        {
                            // �л�Ϊ��ȡkey״̬
                            s = DictState.Key;
                        }
                        else if(key == null && token.val == "]")
                        {
                            // ��ȡ���
                            TjsDict ret = new TjsDict(inner);
                            return ret;
                        }
                        else
                        {
                            // ��Ч�ķ���
                            ShowError("Invalid Symbol");
                            break;
                        }
                    }
                    else
                    {
                        // ����ķ���
                        ShowError("Expect a Symbol");
                        break;
                    }
                }

            } while (token.t != TokenType.Unknow);

            ShowError("Dictionary Parsing Failed");
            ShowError(token);
            return null;
        }
        #endregion

        #region ����һ������
        TjsArray ParseArray(TextReader r)
        {
            TjsParser.Token token = null;

            List<TjsValue> inner = new List<TjsValue>();

            do
            {
                // ��ȡһ��ֵ
                TjsValue val = Parse(r);

                // ֱ�ӷ��ش���
                if (val == null) return null;

                inner.Add(val);

                // ��ȡ���ķ���
                token = GetNext(r);
                if(token.t == TokenType.Symbol)
                {
                    if(token.val == "]")
                    {
                        // ��ȡ���
                        TjsArray ret = new TjsArray(inner);
                        return ret;
                    }
                    else if(token.val != ",")
                    {
                        // ����ķ���
                        ShowError("Expect a Comma");
                        break;
                    }
                }
                else
                {
                    // ����ķ���
                    ShowError("Expect a Symbol");
                    break;
                }

            } while (token.t != TokenType.Unknow);

            ShowError("Array Parsing Failed");
            ShowError(token);
            return null;
        }
        #endregion

        #region ����һ��TjsValue
        public TjsValue Parse(TextReader r)
        {
            TjsParser.Token token = null;
            do
            {
                token = GetNext(r);
                if(token.t == TokenType.Number)
                {
                    // ����������
                    TjsNumber ret = token.ToTjsNumber();
                    if (ret == null)
                    {
                        // ���ָ�ʽ����
                        ShowError("Invalid Number");
                        break;
                    }
                    return ret;
                }
                else if(token.t == TokenType.String)
                {
                    // �������ַ���
                    TjsString ret = token.ToTjsString();
                    if (ret == null)
                    {
                        // �ַ�����ʽ����
                        ShowError("Invalid String");
                        break;
                    }
                    return ret;
                }
                else if(token.t == TokenType.Symbol)
                {
				    // ����2.28
                    if(token.val == "(const)" || token.val == "int" || token.val == "string")
                    {
                        // ɶҲ����
                    }
                    else if(token.val == "void")
                    {
                        // ��������ֵ
                        return new TjsVoid();
                    }
                    else if(token.val == "%[")
                    {
                        // �����ֵ�
                        TjsDict ret = ParseDict(r);
                        return ret;
                    }
                    else if(token.val == "[")
                    {
                        // ��������
                        TjsArray ret = ParseArray(r);
                        return ret;
                    }
                    else
                    {
                        // ��Ч�ķ���
                        ShowError("Invalid Symbol");
                        break;
                    }
                }
            } while (token.t != TokenType.Unknow);

            if(token.t != TokenType.Unknow)
            {
                ShowError("Value Parsing Failed");
                ShowError(token);
            }

            return null;
        }
        #endregion

    }
}
