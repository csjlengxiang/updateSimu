using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL1
{
    public class LogService
    {
        static object o = new object();
        public static void Mess(string data, string path = @"d:\wwlog\GpsMessges")
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                lock (o)
                {
                    StreamWriter sw = File.AppendText(path + @"\" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                    sw.WriteLine("【" + DateTime.Now.ToString() + "】" + data);
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
