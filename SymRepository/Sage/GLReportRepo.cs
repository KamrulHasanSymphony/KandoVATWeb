using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Data;

namespace SymRepository.Sage
{
    public class GLReportRepo
    {

        GLReportDAL _dal = new GLReportDAL();
        // PettyCashStatement
        public DataSet Report6(string DateFrom = "1900-Jan-01", string DateTo = "2099-Dec-31", int BranchId = 0)
        {
            try
            {
                return _dal.Report6(DateFrom, DateTo, BranchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
          public DataSet Report7(GLReportVM vm)
        {
            try
            {
                return _dal.Report7(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable Report8(GLReportVM vm)
        {
            try
            {
                return _dal.Report8(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report9(GLReportVM vm)
        {
            try
            {
                return _dal.Report9(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report10(GLReportVM vm)
        {
            try
            {
                return _dal.Report10(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report11(GLReportVM vm)
        {
            try
            {
                return _dal.Report11(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report12(GLReportVM vm)
        {
            try
            {
                return _dal.Report12(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report13(GLReportVM vm)
        {
            try
            {
                return _dal.Report13(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report14(GLReportVM vm)
        {
            try
            {
                return _dal.Report14(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report15(GLReportVM vm)
        {
            try
            {
                return _dal.Report15(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report16(GLReportVM vm)
        {
            try
            {
                return _dal.Report16(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report17(GLReportVM vm)
        {
            try
            {
                return _dal.Report17(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report18(GLReportVM vm)
        {
            try
            {
                return _dal.Report18(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report19(GLReportVM vm)
        {
            try
            {
                return _dal.Report19(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report20(GLReportVM vm)
        {
            try
            {
                return _dal.Report20(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report21(GLReportVM vm)
        {
            try
            {
                return _dal.Report21(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report22(GLReportVM vm)
        {
            try
            {
                return _dal.Report22(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report23(GLReportVM vm)
        {
            try
            {
                return _dal.Report23(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
