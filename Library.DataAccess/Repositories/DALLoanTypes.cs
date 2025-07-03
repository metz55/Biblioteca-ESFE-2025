using Library.DataAccess.Domain;
using Library.DataAccess.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Repositories
{
    public class DALLoanTypes
    {

        #region CRUD
        public static async Task<int> CreateLoanTypesAsync(LoanTypes pLoanTypes)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                dbContext.Add(pLoanTypes);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> UpdateLoanTypesAsync(LoanTypes pLoanTypes)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var loanTypes = await dbContext.Loan_Types.FirstOrDefaultAsync(s => s.TYPES_ID == pLoanTypes.TYPES_ID);
                loanTypes.TYPES_NAME = pLoanTypes.TYPES_NAME;
                dbContext.Update(loanTypes);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> DeleteLoanTypesAsync(LoanTypes pLoanTypes)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var loanTypes = await dbContext.Loan_Types.FirstOrDefaultAsync(s => s.TYPES_ID == pLoanTypes.TYPES_ID);
                dbContext.Loan_Types.Remove(loanTypes);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<List<LoanTypes>> GetAllLoanTypesAsync()
        {
            var loanTypes = new List<LoanTypes>();
            using (var dbContext = new DBContext())
            {
                loanTypes = await dbContext.Loan_Types.ToListAsync();
            }
            return loanTypes;
        }

        public static async Task<LoanTypes> GetLoanTypesByIdAsync(LoanTypes pLoanTypes)
        {
            var loanTypes = new LoanTypes();
            using (var dbContext = new DBContext())
            {
                loanTypes = await dbContext.Loan_Types.FirstOrDefaultAsync(s => s.TYPES_ID == pLoanTypes.TYPES_ID);
            }
            return loanTypes;
        }

        internal static IQueryable<LoanTypes> QuerySelect(IQueryable<LoanTypes> pQuery, LoanTypes pLoanTypes)
        {
            if (pLoanTypes.TYPES_ID > 0)
                pQuery = pQuery.Where(s => s.TYPES_ID == pLoanTypes.TYPES_ID);
            if (!string.IsNullOrWhiteSpace(pLoanTypes.TYPES_NAME))
                pQuery = pQuery.Where(s => s.TYPES_NAME.Contains(pLoanTypes.TYPES_NAME));
            pQuery = pQuery.OrderByDescending(s => s.TYPES_ID).AsQueryable();
            if (pLoanTypes.Top_Aux > 0)
                pQuery = pQuery.Take(pLoanTypes.Top_Aux).AsQueryable();
            return pQuery;
        }

        public static async Task<List<LoanTypes>> GetLoanTypesAsync(LoanTypes pLoanTypes)
        {
            var loanTypes = new List<LoanTypes>();
            using (var dbContext = new DBContext())
            {
                var select = dbContext.Loan_Types.AsQueryable();
                select = QuerySelect(select, pLoanTypes);
                loanTypes = await select.ToListAsync();
            }
            return loanTypes;
        }
        #endregion
    }
}
