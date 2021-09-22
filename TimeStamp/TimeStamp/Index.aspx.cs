using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TimeStamp
{
    public partial class Index : System.Web.UI.Page
    {
       
        protected void Page_Load(object sender, EventArgs e)
        {
         
        }

        protected void CheckCertificate_btn_Click(object sender, EventArgs e)
        {
            string id = "";
            string fileName = "";
            string MD5HashFile = "";
            string modifiedDate = "";


            try { 
                string inputContent;
                using (StreamReader inputStreamReader = new StreamReader(CheckCertificate.PostedFile.InputStream))
                {
                    inputContent = inputStreamReader.ReadToEnd();
                }
                int idStart = inputContent.IndexOf("Certificate ID: ")+16;
                int idEnd = inputContent.IndexOf(".");
                id = inputContent.Substring(idStart, (idEnd - idStart));

                int nameStart = inputContent.IndexOf("File name: ") + 11;
                int nameEnd = inputContent.IndexOf("MD5HashFile:")-2;
                fileName = inputContent.Substring(nameStart, (nameEnd - nameStart));

                int MD5HashFileStart = inputContent.IndexOf("MD5HashFile: ") + 13;
                int MD5HashFileEnd = inputContent.IndexOf("Last modified date") - 2;
                MD5HashFile = inputContent.Substring(MD5HashFileStart, (MD5HashFileEnd - MD5HashFileStart));

                int modifiedDateStart = inputContent.IndexOf("Last modified date: ") + 20;
                modifiedDate = inputContent.Substring(modifiedDateStart, (inputContent.Length)-modifiedDateStart);
            }
            catch(Exception)
            {
                Response.Write("<script>alert('Incorrect certificate format!');</script>");
            }
            
            try
            {
                string conStr = ConfigurationManager.ConnectionStrings["RegistrationConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    SqlCommand checkCertificate = new SqlCommand("SELECT COUNT(*) FROM tblFiles WHERE ID = @ID AND Name = @Name AND MD5HashFile = @MD5HashFile  AND  LastModifiedDate = @LastModifiedDate", con);
                    checkCertificate.Parameters.AddWithValue("@ID", int.Parse(id));
                    checkCertificate.Parameters.AddWithValue("@Name", fileName);
                    checkCertificate.Parameters.AddWithValue("@MD5HashFile", MD5HashFile);
                    checkCertificate.Parameters.AddWithValue("@LastModifiedDate", DateTime.Parse(modifiedDate));
                    con.Open();
                    int CertificateExist = (int)checkCertificate.ExecuteScalar();
                    con.Close();

                    if (CertificateExist > 0)
                    {
                        //Certificate exist
                        Response.Write("<script>alert('Certificate Digital Signature is Valid: \\n\\nCertificate ID: " + id+"\\nFile Name: " + fileName + " \\nMD5HashFile: " + MD5HashFile + " \\nLast modified date: "+ modifiedDate + "');</script>");
                    }
                    else
                    {
                        //Certificate doesn't exist.
                        Response.Write("<script>alert('Bad Certificate Signature!');</script>");
                    }
                }
            }
            catch(Exception ex)
            {
                Response.Write("<script>alert('Database error!'"+ ex.ToString()+");</script>");
            }



        }
    }
}


