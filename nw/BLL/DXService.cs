using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class DXService
    {
        DXDAL psDal = new DXDAL();
        public string Insert(string jsrid,string tim, string msg, string type)
        {
            string str = Guid.NewGuid().ToString();
            string sql = string.Format("insert into FDSGLXT_DXJLB(ID,JSRID,FSSJ,FSNR,FSLX) values('{0}','{1}',to_date('{2}','yyyy/mm/dd hh24:mi:ss'),'{3}','{4}')",
                str,
                jsrid,
                tim,
                msg,
                type);
            psDal.Insert(sql);
            return str;
        }
    }
}
