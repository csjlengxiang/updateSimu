using DAL1;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL1
{
    class PositionService
    {
        public PositionService()
        {
            FullStations();
        }
        private class Station
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public double J { get; set; }
            public double W { get; set; }
            public string JW { get; set; }
            public Station(string id, string name, string jw)
            {
                ID = id;
                Name = name;
                J = Convert.ToDouble(jw.Split(',')[0]);
                W = Convert.ToDouble(jw.Split(',')[1]);
                JW = jw;
            }
            public override string ToString()
            {
                return Name + ":" + JW;
            }
        }

        private List<Station> stations = new List<Station>();
        private double rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        private double getDis(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
            Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * 100000;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }

        /// <summary>
        /// 此处改成读取数据库的鬼东西
        /// </summary>
        private void FullStations()
        {
            //stations.Add(new Station("北京南", "116.385153,39.872198"));
            //stations.Add(new Station("廊坊", "116.714292,39.515313"));
            //stations.Add(new Station("天津西", "117.215906,39.141797"));
            //stations.Add(new Station("沧州", "116.882295,38.315467"));
            //stations.Add(new Station("德州", "116.295593,37.456663"));
            //stations.Add(new Station("济南", "116.997278,36.676998"));

            //string p = Directory.GetCurrentDirectory() + @"\ok.xls";
            //MessageBox.Show(p);
            //Console.WriteLine(p);

            //DataSet ds = ExcelHelper.GetDataSet(p, "select * from [Sheet1$]");
            DataSet ds = DbHelper.Query("select * from FDSGLXT_CZB");

            DataTable dt = ds.Tables[0];

            //List<mo1> s1 = new List<mo1>();
            int cnt = 0;
            foreach (DataRow dr in dt.Rows)
            {
                //ZM：站名
                //JD：经
                //WD：纬 
                if (dr["ZM"].ToString() == "" || dr["JD"].ToString() == "" || dr["WD"].ToString() == "" || dr["DBM"].ToString() == "未发现" || dr["TIMIS"].ToString() == "未发现" || dr["ZMLM"].ToString() == "未发现")
                    continue;
                stations.Add(new Station(dr["CZID"].ToString() ,dr["ZM"].ToString(), dr["JD"].ToString() + "," + dr["WD"].ToString()));
                cnt++;
            }
            //MessageBox.Show(cnt.ToString());

        }
        public string GetNear(double j, double w, ref string stationName)
        {
            double dis = Double.MaxValue;
            string near = "";
            for (int i = 0; i < stations.Count; i++)
            {
                double d = getDis(j, w, stations[i].J, stations[i].W);
                if (d < dis)
                {
                    dis = d;
                    near = stations[i].ID;
                    stationName = stations[i].Name;
                    //return near;
                }
            }
            return near;
        }
    }
}
