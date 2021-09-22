using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TimeStamp
{
    /// <summary>
    /// Summary description for CheckFileHandler
    /// </summary>
    public class CheckFileHandler : IHttpHandler
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

                    try
                    {
                        //check if file exists in database
                        string conStr = ConfigurationManager.ConnectionStrings["RegistrationConnectionString"].ConnectionString;
                        using (SqlConnection con = new SqlConnection(conStr))
                        {
                            SqlCommand checkFile = new SqlCommand("SELECT COUNT(*) FROM tblFiles WHERE Name = @Name AND MD5HashFile = @MD5HashFile  AND  LastModifiedDate = @LastModifiedDate", con);
                            checkFile.Parameters.AddWithValue("@Name", Name);
                            checkFile.Parameters.AddWithValue("@MD5HashFile", MD5HashFile);
                            checkFile.Parameters.AddWithValue("@LastModifiedDate", date);
                            con.Open();
                            int FileExist = (int)checkFile.ExecuteScalar();
                            con.Close();

                            if (FileExist > 0)
                            {
                                //File exist
                                context.Response.Write("Fingerprint is found in Database: \n\nFile Name: " + Name + " \nMD5HashFile: " + MD5HashFile + " \nLast modified date: " + date + "");
                            }
                            else
                            {
                                //File doesn't exist.
                                context.Response.Write("Unrecognized MD5 Fingerprint!");
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        context.Response.Write("Error: " + e.ToString());
                    }
               

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