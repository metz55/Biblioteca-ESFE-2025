using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLLoanTypes
    {
        public async Task<int> CreateLoanTypesAsync(LoanTypes pLoanTypes)
        {
            return await DALLoanTypes.CreateLoanTypesAsync(pLoanTypes);
        }

        public async Task<int> UpdateLoanTypesAsync(LoanTypes pLoanTypes)
        {
            return await DALLoanTypes.UpdateLoanTypesAsync(pLoanTypes);
        }

        public async Task<int> DeleteLoanTypesAsync(LoanTypes pLoanTypes)
        {
            return await DALLoanTypes.DeleteLoanTypesAsync(pLoanTypes);
        }

        public async Task<List<LoanTypes>> GetAllLoanTypesAsync()
        {
            return await DALLoanTypes.GetAllLoanTypesAsync();
        }

        public async Task<LoanTypes> GetLoanTypesByIdAsync(LoanTypes pLoanTypes)
        {
            return await DALLoanTypes.GetLoanTypesByIdAsync(pLoanTypes);
        }

        public async Task<List<LoanTypes>> GetLoanTypesAsync(LoanTypes pLoanTypes)
        {

            return await DALLoanTypes.GetLoanTypesAsync(pLoanTypes);
        }
    }
}
