using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL1
{
    public class InternetService
    {
        csjSerialPort mySerialPort;
        BackgroundService backgroundService;

        SendService sendService = new SendService();
        public InternetService(string com)
        {
            mySerialPort = new csjSerialPort(Oper, com);
            mySerialPort.Open();
            backgroundService = new BackgroundService();
        }
        public void Send(string str)
        {
            backgroundService.Oper(str);
            mySerialPort.Send(str);
        }
        private void Oper(string str)
        {
            //短信，调用短信接口
            //同步
            try
            {
                string[] strs = str.Split('$');



                string type = strs[0];
                string ctx = strs[1];
                switch (type)
                {
                    case "1":
                        backgroundService.jsService.YJS(ctx);
                        break;
                    case "2":
                        backgroundService.jsService.YJSQR(ctx);
                        break;
                    case "3":
                        backgroundService.jsService.YJSQX(ctx);
                        break;
                    case "4":
                        backgroundService.jsService.BF(ctx);
                        break;
                    case "5":
                        backgroundService.jsService.CS(ctx);
                        break;
                    case "6":
                        /////////////短信////////////
                        string[] datas = ctx.Split(' ');
                        string sjh = datas[0];
                        string sh = datas[1];
                        string ch = datas[2];
                        string ty = datas[3];

                        string ct = "";
                        if (ty == "j")
                        {
                            ct = "锁[" + sh + "]在[" + ch + "]加锁成功";

                        }
                        else
                        {
                            ct = "锁[" + sh + "]在[" + ch + "]异常破锁";

                        }
                        //ct = System.DateTime.Now.ToShortDateString() + ":" + ct;
                        sendService.sendOnce(sjh, ct);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                LogService.Mess(e.Message, @"d:\wwlog\dxservice");
            }

        }
    }
}
