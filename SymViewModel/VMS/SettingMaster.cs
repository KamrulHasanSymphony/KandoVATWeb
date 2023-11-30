using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymViewModel.VMS
{
    public class SettingVM
    {
        public string SettingId { get; set; }
        public string SettingGroup { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        public string SettingType { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }

    }
}
