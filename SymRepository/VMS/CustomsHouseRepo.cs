using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
    public class CustomsHouseRepo
    {
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
         public CustomsHouseRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

         public CustomsHouseRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        public List<CustomsHouseVM> SelectAllLst(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new CustomsHouseDAL().SelectAllLst(Id, conditionFields, conditionValues,null,null,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] InsertToCustomsHouse(CustomsHouseVM vm)
        {
            try
            {
                return new CustomsHouseDAL().InsertToCustomsHouse(vm,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] UpdateToCustomsHouse(CustomsHouseVM vm)
        {
            try
            {
                return new CustomsHouseDAL().UpdateToCustomsHouse(vm,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(CustomsHouseVM vm, string[] ids)
        {
            try
            {
                return new CustomsHouseDAL().Delete(vm, ids,null,null,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



    }
}
