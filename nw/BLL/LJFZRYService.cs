using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    class LJFZRYService
    {
        private LJFZRYDAL ljfzryDal = new LJFZRYDAL();
        public List<LJFZRY> GetSJHSFromJBM(string jbm)
        {
            if (jbm == null || jbm == "") throw new Exception("局编码有问题");
            string sql = string.Format("select SJH,XM,ID from FDSGLXT_LJFZRYGLB where JBM='{0}'", jbm);
            List<LJFZRY> ljfzrys = ljfzryDal.Select(sql);
            if (ljfzrys.Count == 0) throw new Exception("管理平台未添加人员ID or 找不到对应人员");
            return ljfzrys;
        }
    }
}
