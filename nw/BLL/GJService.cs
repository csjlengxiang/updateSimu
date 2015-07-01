//#define zy

using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL
{
    class GJService
    {
        //private GJ gj = new GJ();
        private GJDAL gjDal = new GJDAL();

        public GJ LoadGJ(string str)
        {
            try
            {
                string[] strs = str.Split(' ');
                GJ gj = new GJ();
                gj.ID = Guid.NewGuid().ToString();
                gj.SBBH = strs[0];
                gj.DWZT = strs[1];
                gj.DWSJ = strs[2] + " " + strs[3];
                gj.JD = strs[4];
                gj.WD = strs[5];
                gj.DY = "5";
                //gj.DWDD = positionService.GetNear(Convert.ToDouble(gj.JD), Convert.ToDouble(gj.WD));
                return gj;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + " 载入轨迹点失败!");
            }
        }
        /// <summary>
        /// 插入已load的gj点,若空则是已load
        /// </summary>
        /// <returns></returns>
        public void Insert(GJ gj)
        {
            try
            {
                string sql = string.Format("insert into FDS_GJB(ID,DWSJ,DWDD,DWDDID,JD,WD,DWZT,DY,SBBH) values('{0}',to_date('{1}','yyyy/mm/dd hh24:mi:ss'),'{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                        gj.ID, gj.DWSJ, gj.DWDD, gj.DWDDID, gj.JD, gj.WD, gj.DWZT, gj.DY, gj.SBBH);
                gjDal.Insert(sql);

                sql = string.Format("insert into FDS_GJB_BACKUP(ID,DWSJ,DWDD,DWDDID,JD,WD,DWZT,DY,SBBH) values('{0}',to_date('{1}','yyyy/mm/dd hh24:mi:ss'),'{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                        gj.ID, gj.DWSJ, gj.DWDD, gj.DWDDID, gj.JD, gj.WD, gj.DWZT, gj.DY, gj.SBBH);
                gjDal.Insert(sql);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 通过设备编号获得轨迹，也就是说有几个加锁记录，查询就有几条记录
        /// </summary>
        /// <param name="sbbh"></param>
        /// <param name="jssj"></param>
        /// <returns></returns>
        public string GetGJStr(string sbbh, string jssj)
        {
            try
            {
                List<GJ> gjs = GetGJS(sbbh, jssj);
                int num = gjs.Count;
                string ret = "";
                for (int i = 0; i < num; i++)
                {
                    GJ gj = gjs[i];
                    ret += gj.DWSJ + ',' + gj.JD + ',' + gj.WD;
                    if (i != num - 1) ret += ';';
                }
                return ret;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<GJ> GetGJS(string sbbh, string jssj)
        {
            //List<GJ> gjs = new List<GJ>();
            string sql;
#if hv
            //sql = string.Format("select * from FDS_GJB_Rectify t where t.sbbh='{0}' and t.dwsj >= to_date('{1}','yyyy/mm/dd hh24:mi:ss') order by t.dwsj", sbbh, jssj);
            //gjs = gjDal.SelectRectify(sql);
#endif
            sql = string.Format("select * from FDS_GJB_BACKUP t where t.sbbh='{0}' and t.dwsj >= to_date('{1}','yyyy/mm/dd hh24:mi:ss') order by t.dwsj", sbbh, jssj);
            //gjs.AddRange(gjDal.Select(sql));
            List<GJ> gjs = gjDal.Select(sql);
            //GetTable(sbbh, jssj);

            return gjs;
        }
        public void GetTable(string sbbh, string jssj)
        {
            string sql = string.Format("select * from FDS_GJB_BACKUP t where t.sbbh='{0}' and t.dwsj >= to_date('{1}','yyyy/mm/dd hh24:mi:ss') order by t.dwsj", sbbh, jssj);
            ThreadPool.QueueUserWorkItem((m) =>
            {
                try
                {
#if zy
                        string sq = (string)m;
                        insertPathTime(gjDal.SelectDT(sq));
#endif
                }
                catch
                {
                    LogService.Mess("张岩代码有异常", @"c:\fwException");
                }
            }, sql);
        }

        //#region 张岩代码
        ///// <summary>
        ///// 根据列车轨迹，向径路旅行时间表中插入数据（在删除进路之前调用）
        ///// </summary>
        ///// <param name="dtPathTable">完整的1条轨迹</param>
        //public void insertPathTime(DataTable dtPathTable)
        //{
        //    DataTable dtInsertToPathTime = new DataTable();   //需要插入到径路时间表的table
        //    dtInsertToPathTime.Columns.Add("HOURS");   //小时
        //    dtInsertToPathTime.Columns.Add("O_STATION_ID");  //起点站号
        //    dtInsertToPathTime.Columns.Add("O_STATION_NAME");  //起点站名
        //    dtInsertToPathTime.Columns.Add("D_STATION_ID");  //终点站号
        //    dtInsertToPathTime.Columns.Add("D_STATION_NAME");  //终点站名
        //    dtInsertToPathTime.Columns.Add("TRAVAL_TIME");  //旅行用时（单位分钟）

        //    DataTable dtUpdateToPathTime = new DataTable();   //需要更新到径路时间表的table
        //    dtUpdateToPathTime.Columns.Add("HOURS");   //小时
        //    dtUpdateToPathTime.Columns.Add("O_STATION_ID");  //起点站号
        //    dtUpdateToPathTime.Columns.Add("O_STATION_NAME");  //起点站名
        //    dtUpdateToPathTime.Columns.Add("D_STATION_ID");  //终点站号
        //    dtUpdateToPathTime.Columns.Add("D_STATION_NAME");  //终点站名
        //    dtUpdateToPathTime.Columns.Add("TRAVAL_TIME");  //旅行用时（单位分钟）

        //    //遍历dtPathTable
        //    for (int i = 0; i < dtPathTable.Rows.Count; i++)
        //    {
        //        DateTime OTime = DateTime.Parse(dtPathTable.Rows[i]["DWSJ"].ToString());
        //        //遍历dtPathTable
        //        for (int j = 0; j < dtPathTable.Rows.Count; j++)
        //        {
        //            DateTime DTime = DateTime.Parse(dtPathTable.Rows[j]["DWSJ"].ToString());
        //            if (DTime > OTime)  //一个完整的OD
        //            {
        //                //根据小时和OD站点查询已有数据库
        //                string sql0 = "select * from FDS_PATH_TIME t where t.hours =" + OTime.Hour.ToString() + "and t.o_station_id = '"
        //                                + dtPathTable.Rows[i]["DWDDID"] + "' and t.d_station_id = '" + dtPathTable.Rows[j]["DWDDID"] + "'";
        //                DataSet dtPath = DbHelper.Query(sql0);

        //                if (0 == dtPath.Tables[0].Rows.Count)  //如果不存在时间进路
        //                {
        //                    //需要增加一行
        //                    DataRow drInsertToPathTime = dtInsertToPathTime.NewRow();
        //                    drInsertToPathTime["HOURS"] = OTime.Hour;   //小时
        //                    drInsertToPathTime["O_STATION_ID"] = dtPathTable.Rows[i]["DWDDID"].ToString();  //起点站号
        //                    drInsertToPathTime["O_STATION_NAME"] = dtPathTable.Rows[i]["DWDD"].ToString();  //起点站名
        //                    drInsertToPathTime["D_STATION_ID"] = dtPathTable.Rows[j]["DWDDID"].ToString();  //终点站号
        //                    drInsertToPathTime["D_STATION_NAME"] = dtPathTable.Rows[j]["DWDD"].ToString();  //终点站名
        //                    drInsertToPathTime["TRAVAL_TIME"] = (DTime - OTime).TotalMinutes;  //旅行用时（单位分钟）
        //                    dtInsertToPathTime.Rows.Add(drInsertToPathTime);  //向径路时间表中增加一行记录
        //                    DataSet dt2 = DbHelper.Query("insert into fds_path_time ( hours,o_station_id,o_station_name,d_station_id,d_station_name,traval_time ) values ( " + OTime.Hour.ToString() + ", '" + dtPathTable.Rows[i]["DWDDID"] + "', '" + dtPathTable.Rows[i]["DWDD"] + "', '" + dtPathTable.Rows[j]["DWDDID"] + "', '" + dtPathTable.Rows[j]["DWDD"] + "', " + (DTime - OTime).TotalMinutes.ToString() + " )");
        //                }
        //                else  //如果存在时间进路
        //                {
        //                    //需要更新
        //                    int hours = Convert.ToInt32(OTime.Hour.ToString());//小时
        //                    string o_station_id = dtPathTable.Rows[i]["DWDDID"].ToString();  //起点站号
        //                    string d_station_id = dtPathTable.Rows[j]["DWDDID"].ToString();  //终点站号
        //                    int travel_time = Convert.ToInt32(dtPath.Tables[0].Rows[0]["TRAVAL_TIME"].ToString());  //旅行用时（单位分钟）
        //                    int new_traval_time = Convert.ToInt32((DTime - OTime).TotalMinutes);  //旅行用时（单位分钟）
        //                    //int iTravelTime = (travel_time + new_traval_time) / 2;  //重新计算平均时间
        //                    int iTravelTime = Convert.ToInt32(0.8 * Convert.ToDouble(travel_time) + 0.6 * Convert.ToDouble(new_traval_time));  //重新计算移动平均时间


        //                    DataSet dt6 = DbHelper.Query("update FDS_PATH_TIME t set t.traval_time = " + iTravelTime.ToString() + " where t.hours =" + OTime.Hour.ToString() + "and t.o_station_id = '" + o_station_id + "' and t.d_station_id = '" + d_station_id + "'");
        //                }
        //            }
        //        }
        //    }
        //    if (0 != dtInsertToPathTime.Rows.Count)  //如果有需要增加的记录
        //    {
        //        // string sql1 = "select t.hours, t.o_station_id,t.o_station_name, t.d_station_id, t.d_station_name, t.traval_time from FDS_PATH_TIME t";
        //        // bool res = OracleHelper.InsertToPathTime(sql1, dtInsertToPathTime);

        //    }

        //    if (null != dtUpdateToPathTime.Rows.Count)  //如果有需要更新的记录
        //    {

        //    }
        //}
        //#endregion

    }
}
