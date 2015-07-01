using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Net;
using System.IO;
namespace BLL1
{
    class ExcelHelper
    {
//string p = Directory.GetCurrentDirectory() + @"\坐标.xls";
//ds = ExcelHelper.GetDataSet(p, "select * from [Sheet1$]");
        public static DataSet GetDataSet(string path, string strCom)
        {
            //MessageBox.Show(path);
            try
            {
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + "; Extended Properties=Excel 8.0;";
                // MessageBox.Show(path);
                OleDbConnection myConn = new OleDbConnection(strCon);
                // MessageBox.Show(strCon);
                myConn.Open();

                OleDbDataAdapter myCommand = new OleDbDataAdapter(strCom, myConn);

                DataSet ds = new DataSet();

                myCommand.Fill(ds, "[Sheet1$]");

                myConn.Close();

                return ds;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
