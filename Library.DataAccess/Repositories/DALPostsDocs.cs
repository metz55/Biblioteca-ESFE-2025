using System;
using Library.DataAccess.Domain;
using Library.DataAccess.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Repositories;

public class DALPostsDocs
{
    #region CRUD

    public static async Task<int> CreatePostDocAsync(PostsDocs postsDocs)
    {
        int result = 0;
        using (var dbContext = new DBContext())
        {   
            dbContext.Posts_Docs.Add(postsDocs);
            result = await dbContext.SaveChangesAsync();
        }
        return result;
    }

    public static async Task<int> UpdatePostDocAsync(PostsDocs postsDocs)
    {
        int result = 0;
        using (var dbContext = new DBContext())
        {
            var postDoc = await dbContext.Posts_Docs.FirstOrDefaultAsync(p => p.Id == postsDocs.Id);
            if(postsDocs == null){return 0;}

            postDoc.Path = postsDocs.Path;
            postDoc.Name = postsDocs.Name;
            postDoc.PostId = postsDocs.PostId;

            dbContext.Update(postDoc);
            result = await dbContext.SaveChangesAsync();
        }
        return result;
    }

    public static async Task<int> DeletePostDocAsync(PostsDocs postsDocs)
    {
        int result = 0;
        using (var dbContext = new DBContext())
        {
            var postDoc = await dbContext.Posts_Docs.FirstOrDefaultAsync(p => p.Id == postsDocs.Id);
            if(postsDocs == null){ return 0;}

            dbContext.Posts_Docs.Remove(postsDocs);
            result = await dbContext.SaveChangesAsync();
        }
        return result;
    }
    public static async Task<int> DeleteAllPostsDocsByIdPostAsync(Posts pPosts)
    {
        int result = 0;
        using (var dbContext = new DBContext())
        {
            var postsdocs = await dbContext.Posts_Docs
                            .Where(p => p.PostId == pPosts.ID)
                            .ToListAsync();
            
            if(!postsdocs.Any())
            {
                return 0;
            }

            dbContext.Posts_Docs.RemoveRange(postsdocs);
            result = await dbContext.SaveChangesAsync();
        }
        return result;
    }

    public static async Task<List<PostsDocs>> GetAllPostDocsAsync()
    {
        var postsDocs = new List<PostsDocs>();
        using (var dbContext = new DBContext())
        {
            postsDocs = await dbContext.Posts_Docs
            .Include(p => p.Post)
            .ToListAsync();
        }
        return postsDocs;
    }

    public static async Task<PostsDocs> GetPostDocsByIdAsync(PostsDocs postsDocs)
    {
        var post = new PostsDocs();
        using (var dbContext = new DBContext())
        {
            post = await dbContext.Posts_Docs
            .Include(p => p.Post)
            .FirstOrDefaultAsync(p => p.Id == postsDocs.Id);
        }
        return post;
    }

    public static async Task<List<PostsDocs>> GetPostDocsByPostIdAsync(Posts pPosts)
    {
        var post = new List<PostsDocs>();
        using (var dbContext = new DBContext())
        {
            post = await dbContext.Posts_Docs
            .Where(p => p.PostId == pPosts.ID)
            .ToListAsync();
        }
        return post;
    }

    internal static IQueryable<PostsDocs> QuerySelect(IQueryable<PostsDocs> pQuery, PostsDocs pPosts)
    {
        pQuery = pQuery.Include(p => p.Post);

        if (pPosts.Id > 0)
            pQuery = pQuery.Where(s => s.Id == pPosts.Id);

        if (pPosts.PostId > 0)
            pQuery = pQuery.Where(s => s.PostId == pPosts.PostId);

        if (!string.IsNullOrEmpty(pPosts.Name))
            pQuery = pQuery.Where(s => s.Name.Contains(pPosts.Name));

        pQuery = pQuery.OrderByDescending(s => s.Post.CREATED_AT).AsQueryable();
        return pQuery;
    }

    public static async Task<List<PostsDocs>> GetPostDocsAsync(PostsDocs pPosts)
    {
        var postsDocs = new List<PostsDocs>();
        using (var dbContext = new DBContext())
        {
            var select = dbContext.Posts_Docs.AsQueryable();
            select = QuerySelect(select, pPosts);
            postsDocs = await select.ToListAsync();
        }
        return postsDocs;
    }



    #endregion
}