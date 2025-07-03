using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLAcquisitionTypes
    {
        public async Task<int> CreateAcquisitionTypesAsync(AcquisitionTypes pAcquisitionTypes)
        {
            return await DALAcquisitionTypes.CreateAcquisitionTypesAsync(pAcquisitionTypes);
        }

        public async Task<int> UpdateAcquisitionTypesAsync(AcquisitionTypes pAcquisitionTypes)
        {
            return await DALAcquisitionTypes.UpdateAcquisitionTypesAsync(pAcquisitionTypes);
        }

        public async Task<int> DeleteAcquisitionTypesAsync(AcquisitionTypes pAcquisitionTypes)
        {
            return await DALAcquisitionTypes.DeleteAcquisitionTypesAsync(pAcquisitionTypes);
        }

        public async Task<List<AcquisitionTypes>> GetAllAcquisitionTypesAsync()
        {
            return await DALAcquisitionTypes.GetAllAcquisitionTypesAsync();
        }

        public async Task<AcquisitionTypes> GetAcquisitionTypesByIdAsync(AcquisitionTypes pAcquisitionTypes)
        {
            return await DALAcquisitionTypes.GetAcquisitionTypesByIdAsync(pAcquisitionTypes);
        }

        public async Task<List<AcquisitionTypes>> GetAcquisitionTypesAsync(AcquisitionTypes pAcquisitionTypes)
        {

            return await DALAcquisitionTypes.GetAcquisitionTypesAsync(pAcquisitionTypes);
        }
    }
}
