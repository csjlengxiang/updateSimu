using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data;

namespace DAL
{
    public class LJFZRYDAL
    {
        public List<LJFZRY> Select(string sql)
        {
            try
            {
                List<LJFZRY> ljfzrys = new List<LJFZRY>();
                DataSet ds = DbHelper.Query(sql);
                foreach (DataRow dr in ds.Tables[0].Rows)
                    ljfzrys.Add(LoadEntity(dr));
                return ljfzrys;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private LJFZRY LoadEntity(DataRow dr)
        {
            LJFZRY ljfzry = new LJFZRY();

            ljfzry.SJH = dr["SJH"].ToString();
            ljfzry.ID = dr["ID"].ToString();
            ljfzry.XM = dr["XM"].ToString();
            return ljfzry;
        }
    }
}
