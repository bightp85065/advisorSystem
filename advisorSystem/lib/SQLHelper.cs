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

        public JObject queryResult = new JObject();

        public SQLHelper()
        {
            cn = new SqlConnection(connString);
            System.Diagnostics.Debug.Print(connString);
        }

        public JObject select(string table, JObject dataArr, string condi="", string select="*")
        {
            JArray data = new JArray();
            String whereCondi = "WHERE ";
            foreach (var x in dataArr)
            {
                whereCondi += x.Key + "='"+x.Value+"' AND";
            }
            whereCondi = whereCondi.Substring(0, whereCondi.Length - 3);
            

            string qs = "SELECT "+select+" FROM "+ table + " "+ condi + whereCondi + ";";

            System.Diagnostics.Debug.Print(qs);

            using (cn)
            {

                //2.開啟資料庫
                cn.Open();
                //3.引用SqlCommand物件
                SqlCommand command = new SqlCommand(qs, cn);

                using (SqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        data.Add(new JObject());
                        for (int i = 0; i < dr.FieldCount; ++i){
                            data[data.Count-1][dr.GetName(i)]= dr[i].ToString();
                        }
                    }
                    dr.Close();
                }
                cn.Close();
            }
            queryResult["status"] = true;
            queryResult["data"] = data;
            return queryResult;

        }

        public JObject insert(String table, JObject dataArr)
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
            }
            columnStr = columnStr.Substring(0, columnStr.Length-1);
            valueStr = valueStr.Substring(0, valueStr.Length - 1);

            queryStr += columnStr+") values ("+ valueStr+")";

            cn.Open();
            SqlCommand sqlCmd = new SqlCommand(queryStr, cn);
            foreach (var x in dataArr)
            {
                sqlCmd.Parameters.AddWithValue("@"+ x.Key, x.Value.ToString());
            }

            
            try{
                int modified = (int)sqlCmd.ExecuteNonQuery();

                //int modified = (int)sqlCmd.ExecuteScalar();

                if (cn.State == System.Data.ConnectionState.Open)
                    cn.Close();

                queryResult["status"] = true;
                return queryResult;
            }
            catch (SqlException odbcEx)
            {
                // Handle more specific SqlException exception here.
                System.Diagnostics.Debug.Print(odbcEx.ToString());

                JArray errorMessages = new JArray();

                queryResult["status"] = false;

                for (int i = 0; i < odbcEx.Errors.Count; i++)
                {
                    errorMessages.Add(odbcEx.Errors[i].Message);
                    //errorMessages[i] = odbcEx.Errors[i].Message;
                }

                queryResult["msg"] = errorMessages;
                queryResult["code"] = odbcEx.Number;
                return queryResult;
            }
            catch (Exception ex)
            {
                // Handle generic ones here.
                System.Diagnostics.Debug.Print(ex.ToString());

                queryResult["status"] = false;
                queryResult["msg"] = ex.ToString();
                return queryResult;
            }

        }



        public JObject delete(String table, JObject dataArr)
        {

            string queryStr = "DELETE FROM " + table + "";
            string columnCondi = " WHERE";
            foreach (var x in dataArr)
            {
                columnCondi += " " + x.Key + " = @" + x.Key + " AND";
            }
            columnCondi = columnCondi.Substring(0, columnCondi.Length - 3);
            queryStr += columnCondi;

            System.Diagnostics.Debug.Print("------------");
            System.Diagnostics.Debug.Print(queryStr);

            cn.Open();
            SqlCommand sqlCmd = new SqlCommand(queryStr, cn);

            foreach (var x in dataArr)
            {
                sqlCmd.Parameters.AddWithValue("@" + x.Key, x.Value.ToString());
            }


            try
            {
                int modified = (int)sqlCmd.ExecuteNonQuery();

                //int modified = (int)sqlCmd.ExecuteScalar();

                if (cn.State == System.Data.ConnectionState.Open)
                    cn.Close();

                //cn.Close();
                queryResult["status"] = true;
                return queryResult;
            }
            catch (SqlException odbcEx)
            {
                // Handle more specific SqlException exception here.
                System.Diagnostics.Debug.Print(odbcEx.ToString());

                JArray errorMessages = new JArray();

                queryResult["status"] = false;

                for (int i = 0; i < odbcEx.Errors.Count; i++)
                {
                    errorMessages.Add(odbcEx.Errors[i].Message);
                    //errorMessages[i] = odbcEx.Errors[i].Message;
                }

                queryResult["msg"] = errorMessages;
                queryResult["code"] = odbcEx.Number;
                return queryResult;
            }
            catch (Exception ex)
            {
                // Handle generic ones here.
                System.Diagnostics.Debug.Print(ex.ToString());

                queryResult["status"] = false;
                queryResult["msg"] = ex.ToString();
                return queryResult;
            }

        }

    }
}
 