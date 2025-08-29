using System;
using Library.DataAccess.Domain;
using Library.DataAccess.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Repositories
{
    public class DALBooks
    {
        #region CRUD
        public static async Task<int> CreateBooksAsync(Books pBooks)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                // Mapear CoverImagePath a la columna COVER en la base de datos

                pBooks.COVER = pBooks.CoverImagePath;

                dbContext.Add(pBooks);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }


        public static async Task<int> UpdateBooksAsync(Books pBooks)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var books = await dbContext.Books.FirstOrDefaultAsync(s => s.BOOK_ID == pBooks.BOOK_ID);
                books.COVER = pBooks.CoverImagePath;

                books.ID_CATEGORY = pBooks.ID_CATEGORY;
                books.ID_ACQUISITION = pBooks.ID_ACQUISITION;
                books.ID_EDITION = pBooks.ID_EDITION;
                books.ID_AUTHOR = pBooks.ID_AUTHOR;
                books.EJEMPLARS = pBooks.EJEMPLARS;
                books.ID_EDITORIAL = pBooks.ID_EDITORIAL;
                books.ID_COUNTRY = pBooks.ID_COUNTRY;
                books.ID_CATALOG = pBooks.ID_CATALOG;
                books.DEWEY = pBooks.DEWEY;
                books.CUTER = pBooks.CUTER;
                books.TITLE = pBooks.TITLE;
                books.YEAR = pBooks.YEAR;
                books.EXISTENCES = pBooks.EXISTENCES;
                if (pBooks.COVER != null)
                    books.COVER = pBooks.COVER;
                dbContext.Update(books);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }
        public static async Task<int> UpdateExistencesBooksAsync(Books2 pBooks)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var books = await dbContext.Books.FirstOrDefaultAsync(s => s.BOOK_ID == pBooks.BOOK_ID);
                books.EXISTENCES = pBooks.EXISTENCES;
                dbContext.Update(books);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<int> DeleteBooksAsync(Books pBooks)
        {
            int result = 0;
            using (var dbContext = new DBContext())
            {
                var books = await dbContext.Books.FirstOrDefaultAsync(s => s.BOOK_ID == pBooks.BOOK_ID);
                dbContext.Books.Remove(books);
                result = await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public static async Task<List<Books>> GetAllBooksAsync()
        {
            var books = new List<Books>();
            using (var dbContext = new DBContext())
            {
                books = await dbContext.Books.ToListAsync();
            }
            return books;
        }

        public static async Task<Books> GetBooksByIdAsync(Books pBooks)
        {
            var books = new Books();
            using (var dbContext = new DBContext())
            {
                books = await dbContext.Books.FirstOrDefaultAsync(s => s.BOOK_ID == pBooks.BOOK_ID);
            }
            return books;
        }

        internal static IQueryable<Books> QuerySelect(IQueryable<Books> pQuery, Books pBooks)
        {
            if (pBooks.BOOK_ID > 0)
                pQuery = pQuery.Where(s => s.BOOK_ID == pBooks.BOOK_ID);

            if (pBooks.ID_CATEGORY > 0)
                pQuery = pQuery.Where(s => s.ID_CATEGORY == pBooks.ID_CATEGORY);

            if (pBooks.ID_EDITORIAL > 0)
                pQuery = pQuery.Where(s => s.ID_EDITORIAL == pBooks.ID_EDITORIAL);

            if (pBooks.ID_AUTHOR > 0)
                pQuery = pQuery.Where(s => s.ID_AUTHOR == pBooks.ID_AUTHOR);

            if (pBooks.EJEMPLARS > 0)
                pQuery = pQuery.Where(s => s.EJEMPLARS == pBooks.EJEMPLARS);

            if (pBooks.ID_EDITION > 0)
                pQuery = pQuery.Where(s => s.ID_EDITION == pBooks.ID_EDITION);

            if (pBooks.ID_CATALOG > 0)
                pQuery = pQuery.Where(s => s.ID_CATALOG == pBooks.ID_CATALOG);

            if (!string.IsNullOrWhiteSpace(pBooks.TITLE))
                pQuery = pQuery.Where(s => s.TITLE.Contains(pBooks.TITLE));

            pQuery = pQuery.OrderByDescending(s => s.BOOK_ID).AsQueryable();
            if (pBooks.Top_Aux > 0)
                pQuery = pQuery.Take(pBooks.Top_Aux).AsQueryable();
            return pQuery;
        }

        public static async Task<List<Books>> GetBooksAsync(Books pBooks)
        {
            var books = new List<Books>();
            using (var dbContext = new DBContext())
            {
                var select = dbContext.Books.AsQueryable();
                select = QuerySelect(select, pBooks).Include(e => e.Categories).AsQueryable();
                books = await select.ToListAsync();
            }
            return books;
        }

        public static async Task<List<Books>> GetIncludePropertiesAsync(Books pBooks)
        {
            var books = new List<Books>();
            using (var bdContexto = new DBContext())
            {
                var select = bdContexto.Books.AsQueryable();
                select = QuerySelect(select, pBooks).Include(e => e.Categories).AsQueryable();
                select = QuerySelect(select, pBooks).Include(e => e.AcquisitionTypes).AsQueryable();
                select = QuerySelect(select, pBooks).Include(e => e.Editorials).AsQueryable();
                select = QuerySelect(select, pBooks).Include(e => e.Authors).AsQueryable();
                select = QuerySelect(select, pBooks).Include(e => e.Editorials).AsQueryable();
                select = QuerySelect(select, pBooks).Include(e => e.Countries).AsQueryable();
                select = QuerySelect(select, pBooks).Include(e => e.Catalogs).AsQueryable();

                books = await select.ToListAsync();
            }

            return books;

        }
        #endregion

        public static async Task<List<Books>> GetBooksByTitleAsync(string titulo)
        {
            using (var dbContext = new DBContext())
            {
                var libros = await dbContext.Books
                    .Include(b => b.Authors)
                    .Include(b => b.Editorials)
                    .Where(b => !string.IsNullOrEmpty(b.TITLE) && b.TITLE.Contains(titulo))
                    .ToListAsync();

                return libros;
            }
        }

    }
}
