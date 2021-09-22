using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace TimeStamp
{
    /// <summary>
    /// Summary description for FileHandler
    /// </summary>
    public class FileHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            //get data from Index.aspx AJAX post call
            string jsonString = String.Empty;
            HttpContext.Current.Request.InputStream.Position = 0;
            using (System.IO.StreamReader inputStream =
            new System.IO.StreamReader(HttpContext.Current.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
                System.Web.Script.Serialization.JavaScriptSerializer jSerialize =
                    new System.Web.Script.Serialization.JavaScriptSerializer();

                var file = jSerialize.Deserialize<FileData>(jsonString);

                if (file != null)
                {
                    //set values
                    string Name = file.Name;
                    string MD5HashFile = file.MD5HashFile;
                    DateTime ModifiedDate = file.ModifiedDate;

                    //convert date without milliseconds
                    var date = ModifiedDate;
                    date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
                    int id = 0;

                    try
                    {
                        //insert into database
                        string conStr = ConfigurationManager.ConnectionStrings["RegistrationConnectionString"].ConnectionString;
                        using (SqlConnection con = new SqlConnection(conStr))
                        {
                            SqlCommand cmd = new SqlCommand("spUploadFile", con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar));
                            cmd.Parameters["@Name"].Value = (Name);
                            cmd.Parameters.Add(new SqlParameter("@MD5HashFile", SqlDbType.NVarChar));
                            cmd.Parameters["@MD5HashFile"].Value = (MD5HashFile);
                            cmd.Parameters.Add(new SqlParameter("@LastModifiedDate", SqlDbType.DateTime));
                            cmd.Parameters["@LastModifiedDate"].Value = (date);

                            con.Open();
                            var exec = cmd.ExecuteScalar();
                            id = int.Parse(exec.ToString());
                            con.Close();
                        }

                    }
                    catch (Exception e)
                    {
                        context.Response.Write("Error: "+e.ToString());
                    }
                    //response message
                    context.Response.Write("Certificate ID: " + id + ". \n");
                    context.Response.Write("File name: "+ Name + " \n");
                    context.Response.Write("MD5HashFile: " + MD5HashFile + " \n");
                    context.Response.Write("Last modified date: " + ModifiedDate);
                }
            }
            }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}