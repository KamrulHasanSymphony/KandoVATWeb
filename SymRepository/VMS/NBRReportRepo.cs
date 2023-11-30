using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;
using VMSAPI;

namespace SymRepository.VMS
{
    public class NBRReportRepo
    {

         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
         public NBRReportRepo()
           {
                connVM = null;
           }
         public NBRReportRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

         public NBRReportRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public DataSet VAT9_1_CompleteSave(VATReturnVM vm)
        {
            try
            {
                return new _9_1_VATReturnDAL().VAT9_1_CompleteSave(vm,connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet VAT9_1_CompleteLoad(VATReturnVM vm)
        {
            try
            {
                return new _9_1_VATReturnDAL().VAT9_1_CompleteLoad(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable SelectAll_VATReturnHeader(VATReturnVM vm)
        {
            try
            {
                return new _9_1_VATReturnDAL().SelectAll_VATReturnHeader(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public VATReturnHeaderVM SelectAll_VATReturnHeader_Model(VATReturnVM vm)
        {
            try
            {
                return new _9_1_VATReturnDAL().SelectAll_VATReturnHeader_Model(vm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResultVM Post(VATReturnVM vm)
        {
            try
            {
                return new _9_1_VATReturnDAL().Post(vm, null, null);
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
