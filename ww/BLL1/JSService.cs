using DAL1;
using DAL1;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL1
{ 
    public class JSService
    {
        private JSDAL jsDal = new JSDAL();

        /// <summary>
        /// 根据锁号挑选出未销号的加锁信息，有且只有一条...
        /// </summary>
        /// <param name="sh"></param>
        /// <returns></returns>
        private JS SelectBySBBH(string sbbh)
        {
            try
            {
                string sql = string.Format("select * from FDSGLXT_JSJLB where sbbh='{0}'", sbbh);
                List<JS> list = jsDal.Select(sql);
                if (list.Count == 0)
                    throw new Exception("加锁表里找不到对应设备编号");

                return jsDal.Select(sql)[0];
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        /// <summary>
        /// 拿最新轨迹点去更新加锁表(注意预加锁状态不更新状态...)，根据锁号来查询的
        /// </summary>
        /// <param name="gj"></param>
        /// <returns></returns>
        //public void UpdateByGJAndGetJS(GJ gj, ref string preZTBJ, ref JS js)
        //{
        //    /*
        //    zxjd jd
        //    zxwd wd
        //    zxsj dwsj
        //    zxdy dy
        //    zxdd dwdd
        //    ztbj dwzt
        //     */
        //    try
        //    {
        //        js = SelectBySBBH(gj.SBBH);

        //        preZTBJ = js.ZTBJ;

        //        string sql = string.Format("update FDSGLXT_JSJLB set zxjd='{0}',zxwd='{1}',zxsj=to_date('{2}','yyyy/mm/dd hh24:mi:ss'),zxdd='{3}',ztbj='{4}',zxdy='{5}' where sbbh='{6}'",
        //            gj.JD, gj.WD, gj.DWSJ, gj.DWDD, gj.DWZT, gj.DY, gj.SBBH);

        //        jsDal.Update(sql);

        //        //更新js状态
        //        js.ZTBJ = gj.DWZT;
        //        js.ZXJD = gj.JD;
        //        js.ZXWD = gj.WD;
        //        js.ZXSJ = gj.DWSJ;
        //        js.ZXDD = gj.DWDD;
        //        js.ZXDY = gj.DY;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        private string GJStateToJSState(string gjState)
        {
            if (gjState == GJ.js)
                return JS.js;
            if (gjState == GJ.dw)
                return JS.js;
            if (gjState == GJ.ps)
                return JS.ps;
            return null;
        }
        /// <summary>
        /// 拿最新轨迹点去更新加锁表(注意预加锁状态不更新状态...)，根据锁号来查询的
        /// </summary>
        /// <param name="gj"></param>
        /// <returns></returns>
        public void UpdateByGJAndGetJS2(GJ gj, ref string preZTBJ, ref JS js)
        {
            /*
            zxjd jd
            zxwd wd
            zxsj dwsj
            zxdy dy
            zxdd dwdd
            ztbj dwzt
             */
            try
            {
                js = SelectBySBBH(gj.SBBH);

                preZTBJ = js.ZTBJ;

                #region 如果处于预加锁状态，不更新状态
                string jsState;
                if(preZTBJ == JS.yjs)
                {
                    jsState = preZTBJ;
                }
                else
                {
                    jsState = GJStateToJSState(gj.DWZT);
                }
                #endregion

                string sql = string.Format("update FDSGLXT_JSJLB set zxjd='{0}',zxwd='{1}',zxsj=to_date('{2}','yyyy/mm/dd hh24:mi:ss'),zxdd='{3}',zxddid='{4}',ztbj='{5}',zxdy='{6}' where sbbh='{7}'",
                    gj.JD, gj.WD, gj.DWSJ, gj.DWDD, gj.DWDDID, jsState, gj.DY, gj.SBBH);

                jsDal.Update(sql);

                //更新js状态
                js.ZTBJ = jsState;
                js.ZXJD = gj.JD;
                js.ZXWD = gj.WD;
                js.ZXSJ = gj.DWSJ;
                js.ZXDD = gj.DWDD;
                js.ZXDDID = gj.DWDDID;
                js.ZXDY = gj.DY;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 根据锁号销号
        /// </summary>
        /// <param name="sh"></param>
        /// <returns></returns>
        public void XiaoHao(string sbbh)
        {
            try
            {
                string sql = string.Format("delete FDSGLXT_JSJLB where sbbh='{0}'", sbbh);
                jsDal.Update(sql);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #region 预加锁
        public void YJS(string msg)
        {
            try
            {
                string[] dates = msg.Split(';');
                List<JS> jss = new List<JS>();
                foreach (string date in dates)
                {
                    JS js = YJSLoadJSEntity(date);
                    jss.Add(js);
                }

                foreach (JS js in jss)
                {
                    //insert into FDSGLXT_JSJLB()
                    //insert into FDSGLXT_JSJLB t(JLH,QSCZID,ZDCZID,JIARYYHM,SH,SJH,CH,JSSJ,ZTBJ,YJSPCH,HQHYYID,SBBH) 
//values('900','123456789012345678901234567890123456','123456789012345678901234567890123457','BJP','shh','sjhh','chh',to_date('2015/1/15 8:59:36','yyyy/mm/dd hh24:mi:ss'),'1','yjspchh','1','sbbbb');
                    string sql = string.Format("insert into FDSGLXT_JSJLB t(JLH,QSCZID,ZDCZID,JIARYYHM,SH,SJH,CH,JSSJ,ZTBJ,YJSPCH,HQHYYID,SBBH,CZID,HYZRID) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}',to_date('{7}','yyyy/mm/dd hh24:mi:ss'),'{8}','{9}','{10}','{11}','{12}','{13}')",
                        js.JLH,
                        js.QSCZID,
                        js.ZDCZID,
                        js.JIARYYHM,
                        js.SH,
                        js.SJH,
                        js.CH,
                        js.JSSJ,
                        js.ZTBJ,
                        js.YJSPCH,
                        js.HQHYYID,
                        js.SBBH,
                        js.CZID,
                        js.HYZRID
                        );
                    jsDal.Insert(sql);
                }
            }
            catch
            {
                throw new Exception("载入预加锁数据失败");
            }
        }

        private JS YJSLoadJSEntity(string date)
        {
            JS js = new JS();
            string[] strs = date.Split('|');
            js.JLH = strs[0];
            js.QSCZID = strs[1];
            js.ZDCZID = strs[2];
            js.JIARYYHM = strs[3];
            js.SH = strs[4];
            js.SJH = strs[5];
            js.CH = strs[6];
            js.JSSJ = strs[7];
            js.ZTBJ = strs[8];
            js.YJSPCH = strs[9];
            js.HQHYYID = strs[10];
            js.SBBH = strs[11];
            js.CZID = strs[12];
            js.HYZRID = strs[13];
            return js;
        }
        #endregion 

        #region 确认

        public void YJSQR(string msg)
        {
            try
            {
                string[] dates = msg.Split(';');
                List<JS> jss = new List<JS>();
                foreach (string date in dates)
                {
                    JS js = YJSQRLoadJSEntity(date);
                    jss.Add(js);
                }

                foreach (JS js in jss)
                {
                    
                    //update FDSGLXT_JSJLB set ZTBJ='2',JIARYYHM='BJP',JSSJ=to_date('2015/10/15 8:59:36','yyyy/mm/dd hh24:mi:ss') where JLH='100'

                    string sql = string.Format("update FDSGLXT_JSJLB set ZTBJ='{0}',JIARYYHM='{1}',JSSJ=to_date('{2}','yyyy/mm/dd hh24:mi:ss') where JLH='{3}'",
                        js.ZTBJ,
                        js.JIARYYHM,
                        js.JSSJ,
                        js.JLH);

                    jsDal.Update(sql);
                }
            }
            catch
            {
                throw new Exception("载入预加锁确认数据失败");
            }
        }
        private JS YJSQRLoadJSEntity(string date)
        {
            JS js = new JS();
            string[] strs = date.Split('|');
            js.JLH = strs[0];
            js.ZTBJ = strs[1];
            js.JIARYYHM = strs[2];
            js.JSSJ = strs[3];
            return js;
        }
        #endregion

        #region 取消
        public void YJSQX(string msg)
        {
            try
            {
                string[] dates = msg.Split(';');

                foreach (string str in dates)
                {
                    //delete from FDSGLXT_JSJLB where jlh='100'
                    string sql = string.Format("delete from FDSGLXT_JSJLB where jlh='{0}'", str);

                    jsDal.Update(sql);
                }
            }
            catch
            {
                throw new Exception("载入预加锁确认数据失败");
            }
        }
        #endregion 

        #region 补封
        public void BF(string msg)
        {
            try
            {
                string[] dates = msg.Split(';');
                List<JS> jss = new List<JS>();
                foreach (string date in dates)
                {
                    JS js = BFLoadJSEntity(date);
                    jss.Add(js);
                }

                foreach (JS js in jss)
                {

                    //update FDSGLXT_JSJLB set ZTBJ='2',JIARYYHM='BJP',JSSJ=to_date('2015/10/15 8:59:36','yyyy/mm/dd hh24:mi:ss') where JLH='100'
                    //补封：加锁表记录id，锁号，补封人id(本车站登陆账号)，补封时间，加锁状态|||||||设备编号、SIM卡号、后勤货运员ID（实际短信）
                    string sql = string.Format("update FDSGLXT_JSJLB set SH='{0}',JIARYYHM='{1}',JSSJ=to_date('{2}','yyyy/mm/dd hh24:mi:ss'),ZTBJ='{3}',SBBH='{4}',SJH='{5}',HQHYYID='{6}' where JLH='{7}'",
                        js.SH,
                        js.JIARYYHM,
                        js.JSSJ,
                        js.ZTBJ,
                        js.SBBH,
                        js.SJH,
                        js.HQHYYID,
                        js.JLH);

                    jsDal.Update(sql);
                }
            }
            catch
            {
                throw new Exception("载入预加锁确认数据失败");
            }
        }
        private JS BFLoadJSEntity(string date)
        {
            JS js = new JS();
            string[] strs = date.Split('|');
            js.JLH = strs[0];
            js.SH = strs[1];
            js.JIARYYHM = strs[2];
            js.JSSJ = strs[3];
            js.ZTBJ = strs[4];
            js.SBBH = strs[5];
            js.SJH = strs[6];
            js.HQHYYID = strs[7];
            
            return js;
        }
        #endregion

        #region 拆锁
        public void CS(string msg)
        {
            try
            {
                string[] dates = msg.Split(';');
                List<JS> jss = new List<JS>();
                foreach (string date in dates)
                {
                    JS js = CSLoadJSEntity(date);
                    jss.Add(js);
                }

                foreach (JS js in jss)
                {
                    string sql = string.Format("update FDSGLXT_JSJLB set ZTBJ='{0}',JIERYYHM='{1}',CSSJ=to_date('{2}','yyyy/mm/dd hh24:mi:ss') where JLH='{3}'",
                        js.ZTBJ,
                        js.JIERYYHM,
                        js.CSSJ,
                        js.JLH);

                    jsDal.Update(sql);
                }
            }
            catch
            {
                throw new Exception("载入拆锁数据失败");
            }
        }
        private JS CSLoadJSEntity(string date)
        {
            JS js = new JS();
            string[] strs = date.Split('|');
            js.JLH = strs[0];
            js.ZTBJ = strs[1];
            js.JIERYYHM = strs[2];
            js.CSSJ = strs[3];
            return js;
        }
        #endregion 
    }
}
