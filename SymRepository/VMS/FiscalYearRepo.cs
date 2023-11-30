using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using VATViewModel.DTOs;
using VATServer.Library;
using SymOrdinary;
using System.Web;

namespace SymRepository.VMS
{
    public class FiscalYearRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public FiscalYearRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public FiscalYearRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public List<FiscalYearVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new FiscalYearDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] FiscalYearUpdate(List<FiscalYearVM> Details, string modifiedBy)
        {
            return new FiscalYearDAL().FiscalYearUpdate(Details, modifiedBy,connVM);
        }

        public string[] FiscalYearInsert(List<FiscalYearVM> Details)
        {
            return new FiscalYearDAL().FiscalYearInsert(Details,connVM);
        }

        public List<FiscalYearVM> DropDownAll()
        {
            return new FiscalYearDAL().DropDownAll(connVM);
        }

        public List<FiscalYearVM> DropDown()
        {
            try
            {
                return new FiscalYearDAL().DropDown(connVM);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public int LockChek()
        {
            return new FiscalYearDAL().LockChek(connVM);
        }

        public string MaxDate()
        {
            return new FiscalYearDAL().MaxDate(connVM);
        }

        public FiscalYearPeriodVM StartEndPeriod(string year)
        {
            try
            {
                return new FiscalYearDAL().StartEndPeriod(year,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
