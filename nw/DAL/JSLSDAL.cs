using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class JSLSDAL
    {
        public void Insert(string sql, string context)
        {
            try
            {
                DbHelper.ExecuteSql(sql, context);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
