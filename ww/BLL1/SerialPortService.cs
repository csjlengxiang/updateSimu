using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL1
{
    public delegate void csjSerialPortDataReceivedBuf(byte[] readBuffer);
    public delegate void csjSerialPortDataReceivedStr(string readStr);
    public class csjSerialPort : SerialPort
    {
        byte st = 0xfe;
        byte en = 0xff;
        event csjSerialPortDataReceivedBuf DataReceivedBuf;
        event csjSerialPortDataReceivedStr DataReceivedStr;
        Thread thread;
        volatile bool _keepReading;
        byte[] readBuffer;
        int bufferLen;
        int dataStart;
        int dataEnd;
        public csjSerialPort(csjSerialPortDataReceivedBuf _DataReceived, string _com = "com5", int _maxLen = 500000, int _rate = 9600, Parity _parity = Parity.None, int _bit = 8, StopBits _sb = StopBits.One)
            : base(_com, _rate, _parity, _bit, _sb)
        {
            if (_DataReceived != null)
            {
                DataReceivedBuf += _DataReceived;
            }
            ReadTimeout = 500;
            WriteTimeout = -1;
            thread = null;
            _keepReading = false;
            readBuffer = new byte[_maxLen];
            //不知道起点在哪里
            dataStart = -1;
            //数据结尾
            dataEnd = 0;
            bufferLen = _maxLen;
        }
        public csjSerialPort(csjSerialPortDataReceivedStr _DataReceived, string _com = "com5", int _maxLen = 500000, int _rate = 9600, Parity _parity = Parity.None, int _bit = 8, StopBits _sb = StopBits.One)
            : base(_com, _rate, _parity, _bit, _sb)
        {
            if (_DataReceived != null)
            {
                DataReceivedStr += _DataReceived;
            }
            ReadTimeout = 500;
            WriteTimeout = -1;
            thread = null;
            _keepReading = false;
            readBuffer = new byte[_maxLen];
            //不知道起点在哪里
            dataStart = -1;
            //数据结尾
            dataEnd = 0;
            bufferLen = _maxLen;
        }

        /// <summary>
        /// 线程相关...
        /// </summary>
        private void StartReading()
        {
            if (!_keepReading)
            {
                _keepReading = true;
                thread = new Thread(new ThreadStart(ReadPort));
                thread.IsBackground = true;
                thread.Start();
            }
        }

        private void StopReading()
        {
            if (_keepReading)
            {
                _keepReading = false;
                thread.Join();
                thread.Abort();
                thread = null;
            }
        }

        public new bool Open()
        {
            Close();
            base.Open();
            if (IsOpen)
            {
                StartReading();
                return true;
            }
            else return false;
        }

        public new void Close()
        {
            StopReading();
            base.Close();
        }
        public void Send(string str)
        {
            Send(Encoding.UTF8.GetBytes(str));
        }
        object o = new object();
        public void Send(byte[] data)
        {
            byte[] s = new byte[data.Length + 2];
            s[0] = 0xfe;
            s[s.Length - 1] = 0xff;
            for (int i = 0; i < data.Length; i++)
                s[i + 1] = data[i];
            lock (o)
            {
                base.Write(s, 0, s.Length);
            }
        }


        /// <summary>
        /// 直接处理好一行数据返回好了
        /// </summary>
        private void ReadPort()
        {
            while (_keepReading)
            {
                if (IsOpen)
                {
                    int cnt = BytesToRead;
                    if (cnt > 0)
                    {
                        try
                        {
                            //Application.DoEvents();
                            if (dataEnd + cnt > bufferLen)
                            {
                                int cnt1 = bufferLen - dataEnd;
                                int cnt2 = cnt - cnt1;
                                Read(readBuffer, dataEnd, cnt1);
                                Read(readBuffer, 0, cnt2);
                            }
                            else
                                Read(readBuffer, dataEnd, cnt);

                            //获取想要数据...
                            ProcBuf(dataEnd, cnt);

                            dataEnd = (dataEnd + cnt) % bufferLen;
                            //Thread.Sleep(1);
                        }
                        catch (TimeoutException)
                        {
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
        /// <summary>
        /// 循环队列获得数据...
        /// </summary>
        /// <param name="start"></param>
        /// <param name="cnt"></param>
        private void ProcBuf(int start, int cnt)
        {
            int pos;
            for (int i = 0; i < cnt; i++)
            {
                pos = (start + i) % bufferLen;
                if (dataStart >= 0)
                {
                    if (readBuffer[pos] == en)
                    {
                        //csjSerialPortDataReceived()
                        //[ (dataStart+1)%bufferLen , (en - 1+bufferLen)%bufferLen ]的数据

                        #region 拷贝[dataStart,pos]之间的数据，响应托管
                        int len;
                        if (dataStart <= pos)
                        {
                            len = pos - dataStart + 1;
                        }
                        else
                        {
                            len = bufferLen - dataStart;
                            len += pos + 1;
                        }
                        len -= 2;
                        byte[] data = new byte[len];

                        for (int j = 0; j < len; j++)
                        {
                            int id = (dataStart + j + 1) % bufferLen;
                            data[j] = readBuffer[id];
                        }
                        if (DataReceivedBuf != null)
                            DataReceivedBuf(data);
                        //采用新托管
                        if (DataReceivedStr != null)
                            DataReceivedStr(Encoding.UTF8.GetString(data));

                        #endregion

                        dataStart = -1;
                    }
                    //如果等于头，说明丢头了？？
                    if (readBuffer[pos] == st)
                    {
                        dataStart = pos;
                    }
                }
                else
                {
                    if (readBuffer[pos] == st)
                    {
                        dataStart = pos;
                    }
                }
            }
        }
    }
}
