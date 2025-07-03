using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.DataAccess.Domain;
using Library.DataAccess.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories
{
    public class DALCategories
    {
        #region CRUD
        public static async Task<int> CreateCategoriesAsync(Categories pCategories)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                dbContext.Add(pCategories);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> UpdateCategoriesAsync(Categories pCategories)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var category = await dbContext.Categories.FirstOrDefaultAsync(s => s.CATEGORY_ID == pCategories.CATEGORY_ID);
                category.CATEGORY_NAME = pCategories.CATEGORY_NAME;
                dbContext.Update(category);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> DeleteCategoriesAsync(Categories pCategories)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var category = await dbContext.Categories.FirstOrDefaultAsync(s => s.CATEGORY_ID == pCategories.CATEGORY_ID);
                dbContext.Categories.Remove(category);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<List<Categories>> GetAllCategoriesAsync()
        {
            var categories = new List<Categories>();
            using (var dbContext = new DBContext())
            {
                categories = await dbContext.Categories.ToListAsync();
            }
            return categories;
        }

        public static async Task<Categories> GetCategoriesByIdAsync(Categories pCategories)
        {
            var category = new Categories();
            using (var dbContext = new DBContext())
            {
                category = await dbContext.Categories.FirstOrDefaultAsync(s => s.CATEGORY_ID == pCategories.CATEGORY_ID);
            }
            return category;
        }

        internal static IQueryable<Categories> QuerySelect(IQueryable<Categories> pQuery, Categories pCategories)
        {
            if (pCategories.CATEGORY_ID > 0)
                pQuery = pQuery.Where(s => s.CATEGORY_ID == pCategories.CATEGORY_ID);
            if (!string.IsNullOrWhiteSpace(pCategories.CATEGORY_NAME))
                pQuery = pQuery.Where(s => s.CATEGORY_NAME.Contains(pCategories.CATEGORY_NAME));
            pQuery = pQuery.OrderByDescending(s => s.CATEGORY_ID).AsQueryable();
            if (pCategories.Top_Aux > 0)
                pQuery = pQuery.Take(pCategories.Top_Aux).AsQueryable();
            return pQuery;
        }

        public static async Task<List<Categories>> GetCategoriesAsync(Categories pCategories)
        {
            var categories = new List<Categories>();
            using (var dbContext = new DBContext())
            {
                var select = dbContext.Categories.AsQueryable();
                select = QuerySelect(select, pCategories);
                categories = await select.ToListAsync();
            }
            return categories;
        }
        #endregion
    }
}
