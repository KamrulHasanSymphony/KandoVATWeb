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
    public class TransferRawRepo
    {

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public TransferRawRepo()
        {
               connVM = null;
        }
        public TransferRawRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public TransferRawRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }
        public string[] TransferRawInsert(TransferRawMasterVM Master, List<TransferRawDetailVM> Details)
        {
            try
            {
                return new TransferRawDAL().TransferRawInsert(Master, Details, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] TransferRawUpdate(TransferRawMasterVM Master, List<TransferRawDetailVM> Details)
        {
            try
            {
                return new TransferRawDAL().TransferRawUpdate(Master, Details, connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] TransferRawPost(TransferRawMasterVM Master, List<TransferRawDetailVM> Details)
        {
            try
            {
                return new TransferRawDAL().TransferRawPost(Master, Details, connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public List<TransferRawMasterVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new TransferRawDAL().SelectAllList(Id, conditionFields, conditionValues,null,null,connVM);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public List<TransferRawDetailVM> SelectTransferDetail(string transferId, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new TransferRawDAL().SelectTransferDetail(transferId, conditionFields, conditionValues, null, null, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
