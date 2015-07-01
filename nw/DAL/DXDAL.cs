using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
namespace DAL
{
    public class DXDAL
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
    }
}
