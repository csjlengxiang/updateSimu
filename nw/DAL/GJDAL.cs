using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class GJDAL
    {
        public void Insert(string sql)
        {
            try
            {
                DbHelper.ExecuteSql(sql);
            }
            catch (Exception e) 
            {
                throw e;
            }
        }
        public List<GJ> Select(string sql)
        {
            try
            {
                List<GJ> gjs = new List<GJ>();
                DataSet ds = DbHelper.Query(sql);
                foreach (DataRow dr in ds.Tables[0].Rows)
                    gjs.Add(LoadEntity(dr));
                return gjs;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private GJ LoadEntity(DataRow dr)
        {
            GJ gj = new GJ();

            gj.ID = dr["ID"].ToString();
            gj.DWSJ = dr["DWSJ"].ToString();
            gj.JD = dr["JD"].ToString();
            gj.WD = dr["WD"].ToString();

            gj.DWZT = dr["DWZT"].ToString(); 
            gj.DWDDID = dr["DWDDID"].ToString();
            gj.DWDD = dr["DWDD"].ToString();
            gj.DY = dr["DY"].ToString();
            gj.SBBH = dr["SBBH"].ToString();

            return gj;
        }
        public DataTable SelectDT(string sql)
        {
            try
            {
                List<GJ> gjs = new List<GJ>();
                DataSet ds = DbHelper.Query(sql);
                return ds.Tables[0];
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
