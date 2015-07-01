using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL1
{
    public class PSDAL
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
