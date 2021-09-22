using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;

namespace CodeProject
{
    /// <summary>
    /// Description résumée de MailHandler
    /// </summary>
    public class MailHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string jsonString = String.Empty;
            HttpContext.Current.Request.InputStream.Position = 0;
            using (System.IO.StreamReader inputStream = new System.IO.StreamReader(HttpContext.Current.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
                System.Web.Script.Serialization.JavaScriptSerializer jSerialize = new System.Web.Script.Serialization.JavaScriptSerializer();
                var email = jSerialize.Deserialize<Mail>(jsonString);

                if (email != null)
                {
                    string from = email.From;
                    string to = email.To;
                    string body = email.Body;
                    //You can write here the code to send Email, see ,the Class System.Net.Mail.MailMessage on MSDN
                    //Once the Mail is sent succefully, you can send back a response to the Client informing him that everything is okay !
                    context.Response.Write(jSerialize.Serialize(
                         new
                         {
                             Response = "Message Has been sent succesfully"
                         }));
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