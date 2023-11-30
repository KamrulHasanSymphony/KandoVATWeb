using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymServices.Sage
{
    public class GLEmailDAL
    {
        public void EmailPassword(GLUserVM vm)
        {
            GLEmailSettingVM emailSettingVM = new GLEmailSettingVM();
            //SettingRepo _setDAL = new SettingRepo();
            emailSettingVM.MailHeader = "Test Header"; //_setDAL.settingValue("Mail", "MailSubject");
            //emailSettingVM.MailHeader = emailSettingVM.MailHeader.Replace("vmonth", FiscalPeriod);
            string mailbody = "Test Body"; //_setDAL.settingValue("Mail", "MailBody");

            try
            {
                emailSettingVM.MailToAddress = "Khalid.Sarower@symphonysoftt.com";
                if (!string.IsNullOrWhiteSpace(emailSettingVM.MailToAddress))
                {
                    //emailSettingVM.MailBody = mailbody.Replace("vmonth", FiscalPeriod);
                    //emailSettingVM.MailBody = mailbody.Replace("vname", item["EmpName"].ToString());
                    emailSettingVM.MailBody = mailbody + "\r\nNew Password: " + vm.Password;

                    //emailSettingVM.FileName = item["EmpName"].ToString() + " (" + FiscalPeriod + ")";
                    using (var smpt = new SmtpClient())
                    {
                        smpt.EnableSsl = emailSettingVM.USsel;
                        smpt.Host = emailSettingVM.ServerName;
                        smpt.Port = emailSettingVM.Port;
                        smpt.UseDefaultCredentials = false;
                        smpt.EnableSsl = true;
                        smpt.Credentials = new NetworkCredential(emailSettingVM.UserName, emailSettingVM.Password);
                        MailMessage mailmessage = new MailMessage(
                            emailSettingVM.MailFromAddress,
                            emailSettingVM.MailToAddress,
                            emailSettingVM.MailHeader,
                            emailSettingVM.MailBody);
                        //mailmessage.Attachments.Add(new Attachment(rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat), emailSettingVM.FileName + ".pdf"));

                        smpt.Send(mailmessage);
                        mailmessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    }
                }
            }
            catch (SmtpFailedRecipientException ex)
            {

                // throw ex;
            }
        }


        public void SendEmail(GLEmailSettingVM emailSettingVM, Thread thread)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(emailSettingVM.MailToAddress))
                {
                    //emailSettingVM.FileName = item["EmpName"].ToString() + " (" + FiscalPeriod + ")";
                    using (var smpt = new SmtpClient())
                    {
                        smpt.EnableSsl = emailSettingVM.USsel;
                        smpt.Host = emailSettingVM.ServerName;
                        smpt.Port = emailSettingVM.Port;
                        smpt.UseDefaultCredentials = false;
                        smpt.EnableSsl = true;
                        smpt.Credentials = new NetworkCredential(emailSettingVM.UserName, emailSettingVM.Password);
                        MailMessage mailmessage = new MailMessage(
                            emailSettingVM.MailFromAddress,
                            emailSettingVM.MailToAddress,
                            emailSettingVM.MailHeader,
                            

                            emailSettingVM.MailBody);
                        //mailmessage.Attachments.Add(new Attachment(rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat), emailSettingVM.FileName + ".pdf"));
                        //mailmessage.CC.Add("");
                        mailmessage.IsBodyHtml = true;
                        smpt.Send(mailmessage);
                        mailmessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    }
                    Thread.Sleep(500);
                }
            }
            catch (SmtpFailedRecipientException ex)
            {

                // throw ex;
            }
            thread.Abort();
        }
    }
}
