using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
    public class ShiftRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public ShiftRepo()
        {
            connVM = null;
        }

        public ShiftRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public List<ShiftVM> DropDown()
        {
            try
            {
                return new ShiftDAL().DropDown(0,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string[] InsertToShiftNew(ShiftVM vm)
        {
            try

            {
                return new ShiftDAL().InsertToShiftNew(vm,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] UpdateToShiftNew(ShiftVM vm)
        {

            try
            {
                return new ShiftDAL().UpdateToShiftNew(vm,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public List<ShiftVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new ShiftDAL().SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(string a)
        {
            try
            {
                return new ShiftDAL().DeleteShiftNew(a,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
