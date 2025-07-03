using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLCountries
    {
        public async Task<int> CreateCountriesAsync(Countries pCountries)
        {
            return await DALCountries.CreateCountriesAsync(pCountries);
        }

        public async Task<int> UpdateCountriesAsync(Countries pCountries)
        {
            return await DALCountries.UpdateCountriesAsync(pCountries);
        }

        public async Task<int> DeleteCountriesAsync(Countries pCountries)
        {
            return await DALCountries.DeleteCountriesAsync(pCountries);
        }

        public async Task<List<Countries>> GetAllCountriesAsync()
        {
            return await DALCountries.GetAllCountriesAsync();
        }

        public async Task<Countries> GetCountriesByIdAsync(Countries pCountries)
        {
            return await DALCountries.GetCountriesByIdAsync(pCountries);
        }

        public async Task<List<Countries>> GetCountriesAsync(Countries pCountries)
        {

            return await DALCountries.GetCountriesAsync(pCountries);
        }
    }
}
