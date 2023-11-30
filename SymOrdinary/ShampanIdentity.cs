using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace SymOrdinary
{
    public class ShampanIdentity : IIdentity
    {
        public ShampanIdentity(string basicTicket)
        {
            string[] ticketData = basicTicket.Split(new string[] { "__#" }, StringSplitOptions.None);
            this.Name = ticketData[0];
            this.FullName = ticketData[1];
            this.CompanyId = ticketData[2];
            this.CompanyName = ticketData[3];
            this.BranchId = ticketData[4];
            this.BranchCode = ticketData[5];
            this.WorkStationIP = ticketData[6];
            this.WorkStationName = ticketData[7];
            this.SessionDate = ticketData[8];
            this.UserId = ticketData[9];
            this.IsAdmin = Convert.ToBoolean(ticketData[10]);
            this.InitialCatalog = ticketData[11];
            this.Address1 = ticketData[12];
            this.Address2 = ticketData[13];
            this.Address3 = ticketData[14];
            this.TelephoneNo = ticketData[15];
            this.FaxNo = ticketData[16];


            this.IsAuthenticated = true;
        }

        public string Name { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public string AuthenticationType { get { return "ShampanAuthentication"; } }

        public string UserId { get; private set; }
        public string FullName { get; private set; }
        public string CompanyId { get; private set; }
        public string CompanyName { get; private set; }
        public string BranchId { get; private set; }
        public string BranchCode { get; private set; }
        public string WorkStationIP { get; private set; }
        public string WorkStationName { get; private set; }
        public string SessionDate { get; private set; }
        public string InitialCatalog { get; private set; }
        public bool IsAdmin { get; private set; }
        public string Address1     { get; private set; }
        public string Address2     { get; private set; }
        public string Address3     { get; private set; }
        public string TelephoneNo { get; private set; }
        public string FaxNo       { get; private set; }

        public string[] PermittedRoles { get; private set; }


        public static string CreateBasicTicket(
                                            string name,
                                            string fullName,
                                            string companyId,
                                            string companyName,
                                            string branchId,
                                            string branchCode,
                                            string workStationIP,
                                            string workStationName,
                                            string sessionDate,
                                            string userId,
                                            bool isAdmin,
                                            string initialCatalog,
                                            string  Address1   ,
                                            string  Address2   ,
                                            string  Address3   ,
                                            string  TelephoneNo,
                                            string  FaxNo      


            )
        {
            return name + "__#"
                + fullName + "__#"
                + companyId + "__#"
                + companyName + "__#"
                + branchId + "__#"
                + branchCode + "__#"
                + workStationIP + "__#"
                + workStationName + "__#"
                + sessionDate + "__#"
                + userId + "__#"
                + isAdmin + "__#"
                + initialCatalog + "__#"
                + Address1 + "__#"
                + Address2 + "__#"
                + Address3 + "__#"
                + TelephoneNo + "__#"
                + FaxNo  
                ;
        }

        public static string CreateRoleTicket(string[] roles)
        {
            string rolesString = "";
            for (int i = 0; i < roles.Length; i++)
            {
                rolesString += roles[i] + ",";
            }
            rolesString.TrimEnd(new char[] { ',' });

            return rolesString + "__#";
        }

        public void SetRoles(string roleTicket)
        {
            this.PermittedRoles = roleTicket == "" ? new string[0] : roleTicket.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        }

        public bool IsPermitted(string formId)
        {
            try
            {
                DataTable menu = JsonConvert.DeserializeObject<DataTable>(HttpContext.Current.Session["AllRoles"].ToString());
                DataTable userMenu = JsonConvert.DeserializeObject<DataTable>(HttpContext.Current.Session["UserRoles"].ToString());

                if (menu == null)
                    throw new Exception("Roles Not Found");

                if (userMenu == null)
                    throw new Exception("User Roles Not Found");

                bool flag = true;

                if (formId.Length == 9)
                {
                    DataRow[] rows = menu.Select("FormID = '" + formId + "'");

                    if (rows.Length == 0)
                        return false;

                    DataRow[] userRows = userMenu.Select("FormID = '" + formId + "'");

                    if (userRows.Length == 0)
                        return false;


                    if (rows[0]["Access"].ToString() != "1" || (userRows[0]["Access"].ToString() != "1"))
                    {
                        flag = false;
                    }
                }
                else
                {
                    DataRow[] rows = menu.Select("FormID = '" + formId + "'");

                    if (rows.Length == 0)
                        return false;


                    if (rows[0]["Access"].ToString() != "1")
                    {
                        flag = false;
                    }
                }

                return flag;
            }
            catch (Exception e)
            {
                return false;
            }

        }


    }
}
