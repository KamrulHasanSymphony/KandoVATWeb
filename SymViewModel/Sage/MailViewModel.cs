using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SymViewModel.Sage
{
    public class MailViewModel
    {
        public HttpPostedFileBase file { get; set; }
        public HttpPostedFileBase attch1 { get; set; }
        public HttpPostedFileBase attch2 { get; set; }
        public HttpPostedFileBase attch3 { get; set; }
        public HttpPostedFileBase attch4 { get; set; }
        public List<MailDetailViewModel> Vms{ get; set; }
        public string FromMail { get; set; }
        public string MailPassord { get; set; }
        public string MailSubject { get; set; }
        public string SendDate { get; set; }
        public string SendTime { get; set; }
    }
}
