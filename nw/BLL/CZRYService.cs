using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    class CZRYService
    {
        private CZRYDAL czryDal = new CZRYDAL();
        public string GetSJHFromID(string id)
        {
            if (id == null || id == "") throw new Exception("管理平台未添加人员ID or 找不到对应人员");
            string sql = string.Format("select SJH from FDSGLXT_CZRYGLB where ID='{0}'",id);
            List<CZRY> czrys = czryDal.Select(sql);
            if (czrys.Count == 0) throw new Exception("管理平台未添加人员ID or 找不到对应人员");
            return czrys[0].SJH;
        }
    }
}
