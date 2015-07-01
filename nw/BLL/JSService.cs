using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    class JSService
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
                if (preZTBJ == JS.yjs)
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
    }
}
