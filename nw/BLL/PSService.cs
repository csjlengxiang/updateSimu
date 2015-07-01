
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    class PSService
    {
        PSDAL psDal = new PSDAL();
        public void Insert(string psTim, string dwdd, string sbbh, string jd,string wd)
        {
            string sql = string.Format("insert into FDSGLXT_PSJLB(ID,PSSJ,PSDDID,SBBH,JD,WD) values('{0}',to_date('{1}','yyyy/mm/dd hh24:mi:ss'),'{2}','{3}','{4}','{5}')",
                Guid.NewGuid().ToString(),
                psTim,
                dwdd,
                sbbh,
                jd,
                wd);
            psDal.Insert(sql);
        }
    }
}
