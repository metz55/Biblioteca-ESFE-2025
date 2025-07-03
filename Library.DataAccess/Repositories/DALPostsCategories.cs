using System;
using Library.DataAccess.Domain;
using Library.DataAccess.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Repositories;

public class DALPostsCategories
{
    #region CRUD

    public static async Task<int> CreatePostCategoryAsync(PostsCategories pPostsCategories)
    {
        int result = 0;
        using (var dbContext = new DBContext())
        {   
            dbContext.Add(pPostsCategories);
            result = await dbContext.SaveChangesAsync();
        }
        return result;
    }

    public static async Task<int> UpdatePostCategeoryAsync(PostsCategories pPostsCategories)
    {
        int result = 0;
        using (var dbContext = new DBContext())
        {
            var postCategory = await dbContext.Posts_Categories.FirstOrDefaultAsync(p => p.Id == pPostsCategories.Id);
            if(postCategory == null){return 0;}
            postCategory.Name = pPostsCategories.Name;

            dbContext.Update(postCategory);
            result = await dbContext.SaveChangesAsync();
        }
        return result;
    }

    public static async Task<int> DeletePostCategoryAsync(PostsCategories pPostsCategories)
    {
        int result = 0;
        using (var dbContext = new DBContext())
        {
            var postCategory = await dbContext.Posts_Categories.FirstOrDefaultAsync(p => p.Id == pPostsCategories.Id);
            if(postCategory == null){ return 0;}

            dbContext.Posts_Categories.Remove(postCategory);
            result = await dbContext.SaveChangesAsync();
        }
        return result;
    }

    public static async Task<List<PostsCategories>> GetAllPostCategoriesAsync()
    {
        var postsCategories = new List<PostsCategories>();
        using (var dbContext = new DBContext())
        {
            postsCategories = await dbContext.Posts_Categories
            .ToListAsync();
        }
        return postsCategories;
    }

    public static async Task<PostsCategories> GetPostCategoryByIdAsync(PostsCategories pPostCategories)
    {
        var post = new PostsCategories();
        using (var dbContext = new DBContext())
        {
            post = await dbContext.Posts_Categories
            .FirstOrDefaultAsync(p => p.Id == pPostCategories.Id);
        }
        return post;
    }

    internal static IQueryable<PostsCategories> QuerySelect(IQueryable<PostsCategories> pQuery, PostsCategories pPosts)
    {

        if (pPosts.Id > 0)
            pQuery = pQuery.Where(s => s.Id == pPosts.Id);

        if (!string.IsNullOrEmpty(pPosts.Name))
            pQuery = pQuery.Where(s => s.Name.Contains(pPosts.Name));

        pQuery = pQuery.OrderByDescending(s => s.Id).AsQueryable();
        return pQuery;
    }

    public static async Task<List<PostsCategories>> GetPostCategoryAsync(PostsCategories pPosts)
    {
        var postsCtegories = new List<PostsCategories>();
        using (var dbContext = new DBContext())
        {
            var select = dbContext.Posts_Categories.AsQueryable();
            select = QuerySelect(select, pPosts).AsQueryable();
            postsCtegories = await select.ToListAsync();
        }
        return postsCtegories;
    }



    #endregion
}