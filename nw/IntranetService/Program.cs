using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace IntranetService
{
    class Program
    {
        static void Main(string[] args)
        {
            //BLL.SPService spService = new BLL.SPService("com5", true);
            BLL.BackgroundService backgroundService = new BLL.BackgroundService(ConfigurationManager.AppSettings["com"].ToString());

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("内网服务开启");
                Console.ReadKey();
            }
        }
    }
}
