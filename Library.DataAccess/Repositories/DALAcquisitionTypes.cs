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
    public class DALAcquisitionTypes
    {
        #region CRUD
        public static async Task<int> CreateAcquisitionTypesAsync(AcquisitionTypes pAcquisitionTypes)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                dbContext.Add(pAcquisitionTypes);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> UpdateAcquisitionTypesAsync(AcquisitionTypes pAcquisitionTypes)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var acquisitionTypes = await dbContext.Acquisition_Types.FirstOrDefaultAsync(s => s.ACQUISITION_ID == pAcquisitionTypes.ACQUISITION_ID);
                acquisitionTypes.ACQUISITION_TYPE = pAcquisitionTypes.ACQUISITION_TYPE;
                dbContext.Update(acquisitionTypes);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> DeleteAcquisitionTypesAsync(AcquisitionTypes pAcquisitionTypes)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var acquisitionTypes = await dbContext.Acquisition_Types.FirstOrDefaultAsync(s => s.ACQUISITION_ID == pAcquisitionTypes.ACQUISITION_ID);
                dbContext.Acquisition_Types.Remove(acquisitionTypes);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<List<AcquisitionTypes>> GetAllAcquisitionTypesAsync()
        {
            var acquisitionTypes = new List<AcquisitionTypes>();
            using (var dbContext = new DBContext())
            {
                acquisitionTypes = await dbContext.Acquisition_Types.ToListAsync();
            }
            return acquisitionTypes;
        }

        public static async Task<AcquisitionTypes> GetAcquisitionTypesByIdAsync(AcquisitionTypes pAcquisitionTypes)
        {
            var acquisitionTypes = new AcquisitionTypes();
            using (var dbContext = new DBContext())
            {
                acquisitionTypes = await dbContext.Acquisition_Types.FirstOrDefaultAsync(s => s.ACQUISITION_ID == pAcquisitionTypes.ACQUISITION_ID);
            }
            return acquisitionTypes;
        }

        internal static IQueryable<AcquisitionTypes> QuerySelect(IQueryable<AcquisitionTypes> pQuery, AcquisitionTypes pAcquisitionTypes)
        {
            if (pAcquisitionTypes.ACQUISITION_ID > 0)
                pQuery = pQuery.Where(s => s.ACQUISITION_ID == pAcquisitionTypes.ACQUISITION_ID);
            if (!string.IsNullOrWhiteSpace(pAcquisitionTypes.ACQUISITION_TYPE))
                pQuery = pQuery.Where(s => s.ACQUISITION_TYPE.Contains(pAcquisitionTypes.ACQUISITION_TYPE));
            pQuery = pQuery.OrderByDescending(s => s.ACQUISITION_ID).AsQueryable();
            if (pAcquisitionTypes.Top_Aux > 0)
                pQuery = pQuery.Take(pAcquisitionTypes.Top_Aux).AsQueryable();
            return pQuery;
        }

        public static async Task<List<AcquisitionTypes>> GetAcquisitionTypesAsync(AcquisitionTypes pAcquisitionTypes)
        {
            var acquisitionTypes = new List<AcquisitionTypes>();
            using (var dbContext = new DBContext())
            {
                var select = dbContext.Acquisition_Types.AsQueryable();
                select = QuerySelect(select, pAcquisitionTypes);
                acquisitionTypes = await select.ToListAsync();
            }
            return acquisitionTypes;
        }
        #endregion
    }
}
