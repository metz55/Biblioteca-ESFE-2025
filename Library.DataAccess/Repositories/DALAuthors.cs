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
    public class DALAuthors
    {
        #region CRUD
        public static async Task<int> CreateAuthorsAsync(Authors pAuthors)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                dbContext.Add(pAuthors);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> UpdateAuthorsAsync(Authors pAuthors)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var authors = await dbContext.Authors.FirstOrDefaultAsync(s => s.AUTHOR_ID == pAuthors.AUTHOR_ID);
                authors.AUTHOR_NAME = pAuthors.AUTHOR_NAME;
                dbContext.Update(authors);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> DeleteAuthorsAsync(Authors pAuthors)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var authors = await dbContext.Authors.FirstOrDefaultAsync(s => s.AUTHOR_ID == pAuthors.AUTHOR_ID);
                dbContext.Authors.Remove(authors);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<List<Authors>> GetAllAuthorsAsync()
        {
            var authors = new List<Authors>();
            using (var dbContext = new DBContext())
            {
                authors = await dbContext.Authors.ToListAsync();
            }
            return authors;
        }

        public static async Task<Authors> GetAuthorsByIdAsync(Authors pAuthors)
        {
            var authors = new Authors();
            using (var dbContext = new DBContext())
            {
                authors = await dbContext.Authors.FirstOrDefaultAsync(s => s.AUTHOR_ID == pAuthors.AUTHOR_ID);
            }
            return authors;
        }

        internal static IQueryable<Authors> QuerySelect(IQueryable<Authors> pQuery, Authors pAuthors)
        {
            if (pAuthors.AUTHOR_ID > 0)
                pQuery = pQuery.Where(s => s.AUTHOR_ID == pAuthors.AUTHOR_ID);
            if (!string.IsNullOrWhiteSpace(pAuthors.AUTHOR_NAME))
                pQuery = pQuery.Where(s => s.AUTHOR_NAME.Contains(pAuthors.AUTHOR_NAME));
            pQuery = pQuery.OrderByDescending(s => s.AUTHOR_ID).AsQueryable();
            if (pAuthors.Top_Aux > 0)
                pQuery = pQuery.Take(pAuthors.Top_Aux).AsQueryable();
            return pQuery;
        }

        public static async Task<List<Authors>> GetAuthorsAsync(Authors pAuthors)
        {
            var authors = new List<Authors>();
            using (var dbContext = new DBContext())
            {
                var select = dbContext.Authors.AsQueryable();
                select = QuerySelect(select, pAuthors);
                authors = await select.ToListAsync();
            }
            return authors;
        }
        #endregion
    }
}
