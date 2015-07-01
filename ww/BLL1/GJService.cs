#define hv
#define zy
//张岩代码关闭
//#define zy
using DAL1;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL1
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
        public List <GJ> GetGJS(string sbbh, string jssj)
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
            GetTable(sbbh, jssj);

            return gjs;
        }

#if zy
        //insertPathTime函数互斥锁
        static object lockerInsertPathTime = new object();
#endif
        public void GetTable(string sbbh, string jssj)
        {
            string sql = string.Format("select * from FDS_GJB_BACKUP t where t.sbbh='{0}' and t.dwsj >= to_date('{1}','yyyy/mm/dd hh24:mi:ss') order by t.dwsj", sbbh, jssj);
            ThreadPool.QueueUserWorkItem((m) =>
                {
                    try
                    {
#if zy
                        lock (lockerInsertPathTime)  //insertPathTime函数互斥锁
                        {
                            string sq = (string)m;
                            insertPathTime(gjDal.SelectDT(sq));
                        }
#endif
                    }
                    catch
                    {
                        LogService.Mess("张岩代码有异常", @"d:\wwlog\fwException");
                    }
                }, sql);
        }
        #region 张岩代码_陈思捷调用
        /// <summary>
        /// 根据列车轨迹，向径路旅行时间表中插入数据（在删除进路之前调用）
        /// </summary>
        /// <param name="dtPathTable">完整的1条轨迹</param>
        public static void insertPathTime(DataTable dtPathTableOrg)
        {
            DataTable dtInsertToPathTime = new DataTable();   //需要插入到径路时间表的table
            dtInsertToPathTime.Columns.Add("ID");   //id
            dtInsertToPathTime.Columns.Add("HOURS");   //小时
            dtInsertToPathTime.Columns.Add("O_STATION_ID");  //起点站号
            dtInsertToPathTime.Columns.Add("O_STATION_NAME");  //起点站名
            dtInsertToPathTime.Columns.Add("D_STATION_ID");  //终点站号
            dtInsertToPathTime.Columns.Add("D_STATION_NAME");  //终点站名
            dtInsertToPathTime.Columns.Add("TRAVAL_TIME");  //旅行用时（单位分钟）

            DataTable dtUpdateToPathTime = new DataTable();   //需要更新到径路时间表的table
            dtUpdateToPathTime.Columns.Add("ID");   //id
            dtUpdateToPathTime.Columns.Add("HOURS");   //小时
            dtUpdateToPathTime.Columns.Add("O_STATION_ID");  //起点站号
            dtUpdateToPathTime.Columns.Add("O_STATION_NAME");  //起点站名
            dtUpdateToPathTime.Columns.Add("D_STATION_ID");  //终点站号
            dtUpdateToPathTime.Columns.Add("D_STATION_NAME");  //终点站名
            dtUpdateToPathTime.Columns.Add("TRAVAL_TIME");  //旅行用时（单位分钟）

            //去除dtPathTable中重复的车站
            DataTable dtPathTable = DeleteSameRow(dtPathTableOrg, "dwddid"); //去除重复的车站ID

            //遍历dtPathTable
            for (int i = 0; i < dtPathTable.Rows.Count; i++)
            {
                if (dtPathTable.Rows[i]["dwddid"].ToString() != null
                    && dtPathTable.Rows[i]["dwddid"].ToString() != "0")  //如果起始车站id合法
                {
                    DateTime OTime = DateTime.Parse(dtPathTable.Rows[i]["DWSJ"].ToString());

                    //根据小时和O站点查询已有数据库，得到集合
                    string sql0 = "select * from FDS_PATH_TIME t where t.hours =" + OTime.Hour.ToString() + "and t.o_station_id = '"
                                    + dtPathTable.Rows[i]["DWDDID"] + "'";
                    DataSet dtPath = DbHelper.Query(sql0);

                    //遍历dtPathTable
                    for (int j = 0; j < dtPathTable.Rows.Count; j++)
                    {
                        DateTime DTime = DateTime.Parse(dtPathTable.Rows[j]["DWSJ"].ToString());
                        if (DTime > OTime
                            && dtPathTable.Rows[i]["dwddid"].ToString() != null
                            && dtPathTable.Rows[j]["dwddid"].ToString() != null
                            && dtPathTable.Rows[i]["dwddid"].ToString() != dtPathTable.Rows[j]["dwddid"].ToString()
                            && dtPathTable.Rows[i]["dwddid"].ToString() != "0"
                            && dtPathTable.Rows[j]["dwddid"].ToString() != "0")  //一个完整的OD
                        {
                            //根据到达车站，查找是否已经存在ODPath
                            DataRow[] dtFindPath = dtPath.Tables[0].Select("D_STATION_ID = '" + dtPathTable.Rows[j]["DWDDID"] + "'");

                            //    findPathFromO(, dtPath.Tables[0].Rows);

                            if (0 == dtFindPath.Length)// dtPath.Tables[0].Rows.Count)  //如果不存在时间进路
                            {
                                //判断dtInsertToPathTime中是否有相同的hours、o_station_id、D_STATION_ID
                                /*    DataRow[] dtFindSame = dtInsertToPathTime.Select("hours =" + OTime.Hour.ToString() + "and o_station_id = '" 
                                        + dtPathTable.Rows[i]["DWDDID"].ToString() +"' and D_STATION_ID = '" + dtPathTable.Rows[j]["DWDDID"] + "'" );

                                    //如果不重复则需要增加一行
                                    if (0 == dtFindSame.Length)
                                    {*/
                                //需要增加一行
                                DataRow drInsertToPathTime = dtInsertToPathTime.NewRow();
                                DataSet dsSeq = DbHelper.Query("select seq_fds_path_time.nextval as id from dual");
                                drInsertToPathTime["ID"] = Convert.ToInt32(dsSeq.Tables[0].Rows[0]["id"].ToString());  //id
                                drInsertToPathTime["HOURS"] = OTime.Hour;   //小时
                                drInsertToPathTime["O_STATION_ID"] = dtPathTable.Rows[i]["DWDDID"].ToString();  //起点站号
                                drInsertToPathTime["O_STATION_NAME"] = dtPathTable.Rows[i]["DWDD"].ToString();  //起点站名
                                drInsertToPathTime["D_STATION_ID"] = dtPathTable.Rows[j]["DWDDID"].ToString();  //终点站号
                                drInsertToPathTime["D_STATION_NAME"] = dtPathTable.Rows[j]["DWDD"].ToString();  //终点站名
                                drInsertToPathTime["TRAVAL_TIME"] = Convert.ToInt32((DTime - OTime).TotalMinutes);  //旅行用时（单位分钟）
                                dtInsertToPathTime.Rows.Add(drInsertToPathTime);  //向径路时间表中增加一行记录
                                // DataSet dt2 = OracleHelper.Query("insert into fds_path_time ( hours,o_station_id,o_station_name,d_station_id,d_station_name,traval_time ) values ( " + OTime.Hour.ToString() + ", '" + dtPathTable.Rows[i]["DWDDID"] + "', '" + dtPathTable.Rows[i]["DWDD"] + "', '" + dtPathTable.Rows[j]["DWDDID"] + "', '" + dtPathTable.Rows[j]["DWDD"] + "', " + (DTime - OTime).TotalMinutes.ToString() + " )");
                                /*   }
                                   else
                                   {
                                       int stp = 0;
                                       stp++;
                                   }*/
                            }
                            else  //如果存在时间进路
                            {
                                //判断dtInsertToPathTime中是否有相同的hours、o_station_id、D_STATION_ID
                                /*    DataRow[] dtFindSame = dtUpdateToPathTime.Select("hours =" + OTime.Hour.ToString() + "and o_station_id = '" 
                                        + dtPathTable.Rows[i]["DWDDID"].ToString() +"' and D_STATION_ID = '" + dtPathTable.Rows[j]["DWDDID"] + "'" );

                                    //如果不重复则需要增加一行
                                    if (0 == dtFindSame.Length)
                                    {*/
                                //需要更新
                                int hours = Convert.ToInt32(OTime.Hour.ToString());//小时
                                string o_station_id = dtPathTable.Rows[i]["DWDDID"].ToString();  //起点站号
                                string d_station_id = dtPathTable.Rows[j]["DWDDID"].ToString();  //终点站号
                                int travel_time = Convert.ToInt32(dtFindPath[0]["TRAVAL_TIME"].ToString());  //旅行用时（单位分钟）
                                int new_traval_time = Convert.ToInt32((DTime - OTime).TotalMinutes);  //旅行用时（单位分钟）
                                //int iTravelTime = (travel_time + new_traval_time) / 2;  //重新计算平均时间
                                int iTravelTime = Convert.ToInt32(0.8 * Convert.ToDouble(travel_time) + 0.2 * Convert.ToDouble(new_traval_time));  //重新计算移动平均时间

                                //需要更新一行
                                DataRow drUpdateToPathTime = dtUpdateToPathTime.NewRow();
                                drUpdateToPathTime["ID"] = Convert.ToInt32(dtFindPath[0]["id"].ToString());   //小时
                                drUpdateToPathTime["HOURS"] = dtFindPath[0]["HOURS"];   //小时
                                drUpdateToPathTime["O_STATION_ID"] = dtFindPath[0]["O_STATION_ID"];  //起点站号
                                drUpdateToPathTime["O_STATION_NAME"] = dtFindPath[0]["O_STATION_NAME"];   //起点站名
                                drUpdateToPathTime["D_STATION_ID"] = dtFindPath[0]["D_STATION_ID"];  //终点站号
                                drUpdateToPathTime["D_STATION_NAME"] = dtFindPath[0]["D_STATION_NAME"];  //终点站名
                                drUpdateToPathTime["TRAVAL_TIME"] = iTravelTime;  //旅行用时（单位分钟）
                                dtUpdateToPathTime.Rows.Add(drUpdateToPathTime);  //向径路时间表中增加一行记录
                                //DataSet dt6 = OracleHelper.Query("update FDS_PATH_TIME t set t.traval_time = " + iTravelTime.ToString() + " where t.hours =" + OTime.Hour.ToString() + "and t.o_station_id = '" + o_station_id + "' and t.d_station_id = '" + d_station_id + "'");
                                /*     }
                                     else
                                     {
                                         int stp = 0;
                                         stp++;
                                     }*/
                            }
                        }
                    }
                }
            }
            if (0 != dtInsertToPathTime.Rows.Count)  //如果有需要增加的记录
            {
                string columns = "ID, HOURS, O_STATION_ID, O_STATION_NAME, D_STATION_ID, D_STATION_NAME, TRAVAL_TIME";
                string tableName = "fds_path_time";
                DataSet ds = new DataSet();
                ds.Tables.Add(dtInsertToPathTime);
                MultiInsertData(ds, columns, tableName);
            }
            if (0 != dtUpdateToPathTime.Rows.Count)  //如果有需要更新的记录
            {
                string columns = "ID, HOURS, O_STATION_ID, O_STATION_NAME, D_STATION_ID, D_STATION_NAME, TRAVAL_TIME";
                string tableName = "fds_path_time";
                dtUpdateToPathTime.AcceptChanges();
                MultiUpdateData(dtUpdateToPathTime, columns, tableName);
            }
        }

        //批量插入
        public static bool MultiInsertData(DataSet ds, string Columns, string tableName)
        {
            //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ToString();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ToString();
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string SQLString = string.Format("select {0} from {1} where rownum=0", Columns, tableName);

                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        OracleDataAdapter myDataAdapter = new OracleDataAdapter();
                        myDataAdapter.SelectCommand = new OracleCommand(SQLString, connection);
                        myDataAdapter.UpdateBatchSize = 0;
                        OracleCommandBuilder custCB = new OracleCommandBuilder(myDataAdapter);
                        DataTable dt = ds.Tables[0].Copy();
                        DataTable dtTemp = dt.Clone();

                        int times = 0;
                        for (int count = 0; count < dt.Rows.Count; times++)
                        {
                            for (int i = 0; i < 400 && 400 * times + i < dt.Rows.Count; i++, count++)
                            {
                                dtTemp.Rows.Add(dt.Rows[count].ItemArray);
                            }
                            myDataAdapter.Update(dtTemp);
                            dtTemp.Rows.Clear();
                        }
                        dt.Dispose();
                        dtTemp.Dispose();
                        myDataAdapter.Dispose();
                        return true;
                    }
                    catch (System.Data.OracleClient.OracleException E)
                    {
                        connection.Close();
                        return false;
                    }
                }
            }
        }

        //批量更新
        public static bool MultiUpdateData(DataTable data, string Columns, string tableName)
        {
            //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ToString();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ToString();
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string SQLString = string.Format("select {0} from {1} where rownum=0", Columns, tableName);
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        OracleDataAdapter myDataAdapter = new OracleDataAdapter();
                        myDataAdapter.SelectCommand = new OracleCommand(SQLString, connection);
                        OracleCommandBuilder custCB = new OracleCommandBuilder(myDataAdapter);
                        custCB.ConflictOption = ConflictOption.OverwriteChanges;
                        custCB.SetAllValues = true;
                        foreach (DataRow dr in data.Rows)
                        {
                            if (dr.RowState == DataRowState.Unchanged)
                            {
                                dr.SetModified();
                            }
                        }
                        myDataAdapter.Update(data);
                        data.AcceptChanges();
                        myDataAdapter.Dispose();
                        return true;
                    }
                    catch (System.Data.OracleClient.OracleException E)
                    {
                        connection.Close();
                        return false;
                    }
                }
            }
        }

        #region 删除DataTable重复列，类似distinct
        /// <summary>   
        /// 删除DataTable重复列，类似distinct   
        /// </summary>   
        /// <param name="dt">DataTable</param>   
        /// <param name="Field">字段名</param>   
        /// <returns></returns>   
        public static DataTable DeleteSameRow(DataTable dt, string Field)
        {
            ArrayList indexList = new ArrayList();
            // 找出待删除的行索引   
            for (int i = 0; i < dt.Rows.Count - 1; i++)
            {
                if (!IsContain(indexList, i))
                {
                    for (int j = i + 1; j < dt.Rows.Count; j++)
                    {
                        if (dt.Rows[i][Field].ToString() == dt.Rows[j][Field].ToString())
                        {
                            indexList.Add(j);
                        }
                    }
                }
            }
            indexList.Sort();
            // 排序
            for (int i = indexList.Count - 1; i >= 0; i--)// 根据待删除索引列表删除行  
            {
                int index = Convert.ToInt32(indexList[i]);
                dt.Rows.RemoveAt(index);
            }
            return dt;
        }

        /// <summary>   
        /// 判断数组中是否存在   
        /// </summary>   
        /// <param name="indexList">数组</param>   
        /// <param name="index">索引</param>   
        /// <returns></returns>   
        public static bool IsContain(ArrayList indexList, int index)
        {
            for (int i = 0; i < indexList.Count; i++)
            {
                int tempIndex = Convert.ToInt32(indexList[i]);
                if (tempIndex == index)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #endregion


        //#region 张岩代码
        ///// <summary>
        ///// 根据列车轨迹，向径路旅行时间表中插入数据（在删除进路之前调用）
        ///// </summary>
        ///// <param name="dtPathTable">完整c</param>
        //public static void insertPathTime(DataTable dtPathTable)
        //{
        //    DataTable dtInsertToPathTime = new DataTable();   //需要插入到径路时间表的table
        //    dtInsertToPathTime.Columns.Add("ID");   //id
        //    dtInsertToPathTime.Columns.Add("HOURS");   //小时
        //    dtInsertToPathTime.Columns.Add("O_STATION_ID");  //起点站号
        //    dtInsertToPathTime.Columns.Add("O_STATION_NAME");  //起点站名
        //    dtInsertToPathTime.Columns.Add("D_STATION_ID");  //终点站号
        //    dtInsertToPathTime.Columns.Add("D_STATION_NAME");  //终点站名
        //    dtInsertToPathTime.Columns.Add("TRAVAL_TIME");  //旅行用时（单位分钟）

        //    DataTable dtUpdateToPathTime = new DataTable();   //需要更新到径路时间表的table
        //    dtUpdateToPathTime.Columns.Add("ID");   //id
        //    dtUpdateToPathTime.Columns.Add("HOURS");   //小时
        //    dtUpdateToPathTime.Columns.Add("O_STATION_ID");  //起点站号
        //    dtUpdateToPathTime.Columns.Add("O_STATION_NAME");  //起点站名
        //    dtUpdateToPathTime.Columns.Add("D_STATION_ID");  //终点站号
        //    dtUpdateToPathTime.Columns.Add("D_STATION_NAME");  //终点站名
        //    dtUpdateToPathTime.Columns.Add("TRAVAL_TIME");  //旅行用时（单位分钟）

        //    //遍历dtPathTable
        //    for (int i = 0; i < dtPathTable.Rows.Count; i++)
        //    {
        //        if (dtPathTable.Rows[i]["dwddid"].ToString() != null
        //            && dtPathTable.Rows[i]["dwddid"].ToString() != "0")  //如果起始车站id合法
        //        {
        //            DateTime OTime = DateTime.Parse(dtPathTable.Rows[i]["DWSJ"].ToString());

        //            //根据小时和O站点查询已有数据库，得到集合
        //            string sql0 = "select * from FDS_PATH_TIME t where t.hours =" + OTime.Hour.ToString() + "and t.o_station_id = '"
        //                            + dtPathTable.Rows[i]["DWDDID"] + "'";
        //            DataSet dtPath = DbHelper.Query(sql0);

        //            //遍历dtPathTable
        //            for (int j = 0; j < dtPathTable.Rows.Count; j++)
        //            {
        //                DateTime DTime = DateTime.Parse(dtPathTable.Rows[j]["DWSJ"].ToString());
        //                if (DTime > OTime
        //                    && dtPathTable.Rows[i]["dwddid"].ToString() != null
        //                    && dtPathTable.Rows[j]["dwddid"].ToString() != null
        //                    && dtPathTable.Rows[i]["dwddid"].ToString() != dtPathTable.Rows[j]["dwddid"].ToString()
        //                    && dtPathTable.Rows[i]["dwddid"].ToString() != "0"
        //                    && dtPathTable.Rows[j]["dwddid"].ToString() != "0")  //一个完整的OD
        //                {
        //                    //根据到达车站，查找是否已经存在ODPath
        //                    DataRow[] dtFindPath = dtPath.Tables[0].Select("D_STATION_ID = '" + dtPathTable.Rows[j]["DWDDID"] + "'");

        //                    //    findPathFromO(, dtPath.Tables[0].Rows);

        //                    if (0 == dtFindPath.Length)// dtPath.Tables[0].Rows.Count)  //如果不存在时间进路
        //                    {
        //                        //判断dtInsertToPathTime中是否有相同的hours、o_station_id、D_STATION_ID
        //                        DataRow[] dtFindSame = dtInsertToPathTime.Select("hours =" + OTime.Hour.ToString() + "and o_station_id = '"
        //                            + dtPathTable.Rows[i]["DWDDID"].ToString() + "' and D_STATION_ID = '" + dtPathTable.Rows[j]["DWDDID"] + "'");

        //                        //如果不重复则需要增加一行
        //                        if (0 == dtFindSame.Length)
        //                        {
        //                            //需要增加一行
        //                            DataRow drInsertToPathTime = dtInsertToPathTime.NewRow();
        //                            DataSet dsSeq = DbHelper.Query("select seq_fds_path_time.nextval as id from dual");
        //                            drInsertToPathTime["ID"] = Convert.ToInt32(dsSeq.Tables[0].Rows[0]["id"].ToString());  //id
        //                            drInsertToPathTime["HOURS"] = OTime.Hour;   //小时
        //                            drInsertToPathTime["O_STATION_ID"] = dtPathTable.Rows[i]["DWDDID"].ToString();  //起点站号
        //                            drInsertToPathTime["O_STATION_NAME"] = dtPathTable.Rows[i]["DWDD"].ToString();  //起点站名
        //                            drInsertToPathTime["D_STATION_ID"] = dtPathTable.Rows[j]["DWDDID"].ToString();  //终点站号
        //                            drInsertToPathTime["D_STATION_NAME"] = dtPathTable.Rows[j]["DWDD"].ToString();  //终点站名
        //                            drInsertToPathTime["TRAVAL_TIME"] = Convert.ToInt32((DTime - OTime).TotalMinutes);  //旅行用时（单位分钟）
        //                            dtInsertToPathTime.Rows.Add(drInsertToPathTime);  //向径路时间表中增加一行记录
        //                            // DataSet dt2 = OracleHelper.Query("insert into fds_path_time ( hours,o_station_id,o_station_name,d_station_id,d_station_name,traval_time ) values ( " + OTime.Hour.ToString() + ", '" + dtPathTable.Rows[i]["DWDDID"] + "', '" + dtPathTable.Rows[i]["DWDD"] + "', '" + dtPathTable.Rows[j]["DWDDID"] + "', '" + dtPathTable.Rows[j]["DWDD"] + "', " + (DTime - OTime).TotalMinutes.ToString() + " )");
        //                        }

        //                    }
        //                    else  //如果存在时间进路
        //                    {
        //                        //判断dtInsertToPathTime中是否有相同的hours、o_station_id、D_STATION_ID
        //                        DataRow[] dtFindSame = dtUpdateToPathTime.Select("hours =" + OTime.Hour.ToString() + "and o_station_id = '"
        //                            + dtPathTable.Rows[i]["DWDDID"].ToString() + "' and D_STATION_ID = '" + dtPathTable.Rows[j]["DWDDID"] + "'");

        //                        //如果不重复则需要增加一行
        //                        if (0 == dtFindSame.Length)
        //                        {
        //                            //需要更新
        //                            int hours = Convert.ToInt32(OTime.Hour.ToString());//小时
        //                            string o_station_id = dtPathTable.Rows[i]["DWDDID"].ToString();  //起点站号
        //                            string d_station_id = dtPathTable.Rows[j]["DWDDID"].ToString();  //终点站号
        //                            int travel_time = Convert.ToInt32(dtFindPath[0]["TRAVAL_TIME"].ToString());  //旅行用时（单位分钟）
        //                            int new_traval_time = Convert.ToInt32((DTime - OTime).TotalMinutes);  //旅行用时（单位分钟）
        //                            //int iTravelTime = (travel_time + new_traval_time) / 2;  //重新计算平均时间
        //                            int iTravelTime = Convert.ToInt32(0.8 * Convert.ToDouble(travel_time) + 0.2 * Convert.ToDouble(new_traval_time));  //重新计算移动平均时间

        //                            //需要更新一行
        //                            DataRow drUpdateToPathTime = dtUpdateToPathTime.NewRow();
        //                            drUpdateToPathTime["ID"] = Convert.ToInt32(dtFindPath[0]["id"].ToString());   //小时
        //                            drUpdateToPathTime["HOURS"] = dtFindPath[0]["HOURS"];   //小时
        //                            drUpdateToPathTime["O_STATION_ID"] = dtFindPath[0]["O_STATION_ID"];  //起点站号
        //                            drUpdateToPathTime["O_STATION_NAME"] = dtFindPath[0]["O_STATION_NAME"];   //起点站名
        //                            drUpdateToPathTime["D_STATION_ID"] = dtFindPath[0]["D_STATION_ID"];  //终点站号
        //                            drUpdateToPathTime["D_STATION_NAME"] = dtFindPath[0]["D_STATION_NAME"];  //终点站名
        //                            drUpdateToPathTime["TRAVAL_TIME"] = iTravelTime;  //旅行用时（单位分钟）
        //                            dtUpdateToPathTime.Rows.Add(drUpdateToPathTime);  //向径路时间表中增加一行记录
        //                            //DataSet dt6 = OracleHelper.Query("update FDS_PATH_TIME t set t.traval_time = " + iTravelTime.ToString() + " where t.hours =" + OTime.Hour.ToString() + "and t.o_station_id = '" + o_station_id + "' and t.d_station_id = '" + d_station_id + "'");
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (0 != dtInsertToPathTime.Rows.Count)  //如果有需要增加的记录
        //    {
        //        string columns = "ID, HOURS, O_STATION_ID, O_STATION_NAME, D_STATION_ID, D_STATION_NAME, TRAVAL_TIME";
        //        string tableName = "fds_path_time";
        //        DataSet ds = new DataSet();
        //        ds.Tables.Add(dtInsertToPathTime);
        //        MultiInsertData(ds, columns, tableName);
        //    }
        //    if (0 != dtUpdateToPathTime.Rows.Count)  //如果有需要更新的记录
        //    {
        //        string columns = "ID, HOURS, O_STATION_ID, O_STATION_NAME, D_STATION_ID, D_STATION_NAME, TRAVAL_TIME";
        //        string tableName = "fds_path_time";
        //        dtUpdateToPathTime.AcceptChanges();
        //        MultiUpdateData(dtUpdateToPathTime, columns, tableName);
        //    }
        //}


        ////批量插入
        //public static bool MultiInsertData(DataSet ds, string Columns, string tableName)
        //{
        //    //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ToString();
        //    string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ToString();
        //    using (OracleConnection connection = new OracleConnection(connectionString))
        //    {
        //        string SQLString = string.Format("select {0} from {1} where rownum=0", Columns, tableName);

        //        using (OracleCommand cmd = new OracleCommand(SQLString, connection))
        //        {
        //            try
        //            {
        //                connection.Open();
        //                OracleDataAdapter myDataAdapter = new OracleDataAdapter();
        //                myDataAdapter.SelectCommand = new OracleCommand(SQLString, connection);
        //                myDataAdapter.UpdateBatchSize = 0;
        //                OracleCommandBuilder custCB = new OracleCommandBuilder(myDataAdapter);
        //                DataTable dt = ds.Tables[0].Copy();
        //                DataTable dtTemp = dt.Clone();

        //                int times = 0;
        //                for (int count = 0; count < dt.Rows.Count; times++)
        //                {
        //                    for (int i = 0; i < 400 && 400 * times + i < dt.Rows.Count; i++, count++)
        //                    {
        //                        dtTemp.Rows.Add(dt.Rows[count].ItemArray);
        //                    }
        //                    myDataAdapter.Update(dtTemp);
        //                    dtTemp.Rows.Clear();
        //                }
        //                dt.Dispose();
        //                dtTemp.Dispose();
        //                myDataAdapter.Dispose();
        //                return true;
        //            }
        //            catch (System.Data.OracleClient.OracleException E)
        //            {
        //                connection.Close();
        //                return false;
        //            }
        //        }
        //    }
        //}

        ////批量更新
        //public static bool MultiUpdateData(DataTable data, string Columns, string tableName)
        //{
        //    //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ToString();
        //    string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ToString();
        //    using (OracleConnection connection = new OracleConnection(connectionString))
        //    {
        //        string SQLString = string.Format("select {0} from {1} where rownum=0", Columns, tableName);
        //        using (OracleCommand cmd = new OracleCommand(SQLString, connection))
        //        {
        //            try
        //            {
        //                connection.Open();
        //                OracleDataAdapter myDataAdapter = new OracleDataAdapter();
        //                myDataAdapter.SelectCommand = new OracleCommand(SQLString, connection);
        //                OracleCommandBuilder custCB = new OracleCommandBuilder(myDataAdapter);
        //                custCB.ConflictOption = ConflictOption.OverwriteChanges;
        //                custCB.SetAllValues = true;
        //                foreach (DataRow dr in data.Rows)
        //                {
        //                    if (dr.RowState == DataRowState.Unchanged)
        //                    {
        //                        dr.SetModified();
        //                    }
        //                }
        //                myDataAdapter.Update(data);
        //                data.AcceptChanges();
        //                myDataAdapter.Dispose();
        //                return true;
        //            }
        //            catch (System.Data.OracleClient.OracleException E)
        //            {
        //                connection.Close();
        //                return false;
        //            }
        //        }
        //    }
        //}
        //#endregion
    }
}
