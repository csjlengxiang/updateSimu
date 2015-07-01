using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL1
{
    public class CZRYDAL
    {
        public List<CZRY> Select(string sql)
        {
            try
            {
                List<CZRY> czrys = new List<CZRY>();
                DataSet ds = DbHelper.Query(sql);
                foreach (DataRow dr in ds.Tables[0].Rows)
                    czrys.Add(LoadEntity(dr));
                return czrys;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private CZRY LoadEntity(DataRow dr)
        {
            CZRY czry = new CZRY();

            czry.SJH = dr["SJH"].ToString();

            return czry;
        }
    }
}
