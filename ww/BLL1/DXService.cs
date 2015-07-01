using DAL1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL1
{
    public class DXService
    {
        DXDAL psDal = new DXDAL();
        public void Insert(string jsrid,string tim, string msg, string type)
        {
            string sql = string.Format("insert into FDSGLXT_DXJLB(ID,JSRID,FSSJ,FSNR,FSLX) values('{0}','{1}',to_date('{2}','yyyy/mm/dd hh24:mi:ss'),'{3}','{4}')",
                Guid.NewGuid().ToString(),
                jsrid,
                tim,
                msg,
                type);
            psDal.Insert(sql);
        }
    }
}
