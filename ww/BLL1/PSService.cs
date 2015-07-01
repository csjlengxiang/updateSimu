using DAL1;
using DAL1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL1
{
    class PSService
    {
        PSDAL psDal = new PSDAL();
        public void Insert(string psTim, string dwdd)
        {
            string sql = string.Format("insert into FDSGLXT_PSJLB(ID,PSSJ,PSDDID) values('{0}',to_date('{1}','yyyy/mm/dd hh24:mi:ss'),'{2}')",
                Guid.NewGuid().ToString(),
                psTim,
                dwdd);
            psDal.Insert(sql);
        }
    }
}
