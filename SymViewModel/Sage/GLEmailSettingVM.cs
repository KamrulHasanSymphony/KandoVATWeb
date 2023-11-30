using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SymViewModel.Sage
{
    public class GLEmailSettingVM
    {
        public string MailToAddress { get; set; }
        public string MailFromAddress = "Khalid.Sarower@symphonysoftt.com"; 
        public bool USsel = true;
        public string Password = "K123456_";
        public string UserName = "Khalid.Sarower@symphonysoftt.com";
        public string ServerName = "smtp.gmail.com";
        //public string ServerName = "smtp-mail.outlook.com";
        //public string ServerName = "smtp.mail.yahoo.com";
        public string MailBody { get; set; }
        public string MailHeader { get; set; }
        public string Fiscalyear { get; set; }
        public int Port = 587;
        public HttpPostedFileBase fileUploader { get; set; }
        public string FileName { get; set; }
    }
}
