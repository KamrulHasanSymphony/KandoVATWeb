using SymServices.Sage;
using SymViewModel.Sage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Sage
{
    public class GLEmailRepo
    {
        public void EmailPassword(GLUserVM vm)
        {
            try
            {
                new GLEmailDAL().EmailPassword(vm);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
