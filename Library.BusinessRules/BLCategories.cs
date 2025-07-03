using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Repositories;

namespace Library.BusinessRules
{
    public class BLCategories
    {
        public async Task<int> CreateCategoriesAsync(Categories pCategories)
        {
            return await DALCategories.CreateCategoriesAsync(pCategories);
        }

        public async Task<int> UpdateCategoriesAsync(Categories pCategories)
        {
            return await DALCategories.UpdateCategoriesAsync(pCategories);
        }

        public async Task<int> DeleteCategoriesAsync(Categories pCategories)
        {
            return await DALCategories.DeleteCategoriesAsync(pCategories);
        }

        public async Task<List<Categories>> GetAllCategoriesAsync()
        {
            return await DALCategories.GetAllCategoriesAsync();
        }

        public async Task<Categories> GetCategoriesByIdAsync(Categories pCategories)
        {
            return await DALCategories.GetCategoriesByIdAsync(pCategories);
        }

        public async Task<List<Categories>> GetCategoriesAsync(Categories pCategories)
        {

            return await DALCategories.GetCategoriesAsync(pCategories);
        }
    }
}
