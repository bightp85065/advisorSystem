using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace advisorSystem.lib
{
    public class SQLHelper
    {
        public static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["RaymondConnection"].ToString();

        public SqlConnection cn;

        public SQLHelper()
        {
            cn = new SqlConnection(connString);
        }

        public void select()
        {
            string sql = @"INSERT INTO [ntust].[table](Column1,Column2,Column3)";
            int DeviceStatusID = 0;
            string DeviceStatusName = "";

            cn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        DeviceStatusID = Convert.ToInt32(dr["DeviceStatusID"]);
                        DeviceStatusName = dr["DeviceStatusName"].ToString();
                    }
                    dr.Close();
                }

            }
            cn.Close();
        }

        public int insert(String table, JObject dataArr)
        {
            // 將 JSON 字串變成物件
            //JObject obj = JObject.Parse(@"{""Name"": ""Eric""}");
            //obj["Name"] == "Eric"

            // 將物件變成 JSON 字串
            //Person p = new Person();
            //p.Name = "Eric";
            // JsonConvert.SerializeObject(p); ==> { "Name": "Eric" }

            string queryStr = "INSERT INTO " + table + "(";
            string columnStr = "", valueStr = "";
            foreach (var x in dataArr) {
                columnStr += x.Key + ",";
                valueStr += "@" + x.Key + ",";
                //queryStr += "@" + x.Key + ",";
            }
            columnStr = columnStr.Substring(0, columnStr.Length-1);
            valueStr = valueStr.Substring(0, valueStr.Length - 1);

            queryStr += columnStr+") values ("+ valueStr+")";

            System.Diagnostics.Debug.Print(queryStr);
            cn.Open();
            SqlCommand sqlCmd = new SqlCommand(queryStr, cn);

            foreach (var x in dataArr)
            {
                sqlCmd.Parameters.AddWithValue("@"+ x.Key, x.Value.ToString());
            }

            
            try{
                int modified = (int)sqlCmd.ExecuteNonQuery();

                if (cn.State == System.Data.ConnectionState.Open)
                    cn.Close();

                //cn.Close();

                return modified;
            }
            catch (Exception e){
                System.Diagnostics.Debug.Print(e.ToString());
                return 0;
            }
            


        }
        /*public static string strConn = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        
        //建立連接
        public static SqlConnection myConn = new SqlConnection(strConn);

        //打開連接
        myConn.Open();


        String strSQL = @"select * from tableName";


        //建立SQL命令對象
        SqlCommand myCommand = new SqlCommand(strSQL, myConn);


        //得到Data結果集
        SqlDataReader myDataReader = myCommand.ExecuteReader();



        //讀取結果
        while (myDataReader.Read())
        {
            if (myDataReader["id"].ToString() != "")
            {
                TextBox1.Text += myDataReader["id"].ToString();
                TextBox1.Text += " : ";
                TextBox1.Text += myDataReader["phoneTel"].ToString();
                TextBox1.Text += Environment.NewLine;                 //跳行
            }
        }*/


        //public static string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        //public static SqlConnection cn;



    }
}