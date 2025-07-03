using System;
using Library.DataAccess.Domain;
using Library.DataAccess.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Repositories;

public class DALPosts
{
    #region CRUD

    public static async Task<int> CreatePostAsync(Posts posts)
    {
        int result = 0;
        using (var dbContext = new DBContext())
        {   
            dbContext.Add(posts);
            result = await dbContext.SaveChangesAsync();
        }
        return result;
    }

    public static async Task<int> UpdatePostAsync(Posts posts)
    {
        int result = 0;
        using (var dbContext = new DBContext())
        {
            Posts post = await dbContext.Posts.FirstOrDefaultAsync(p => p.ID == posts.ID);
            if(post == null)
            {
                return 0;
            }
            post.TITLE = posts.TITLE;
            post.CONTENT = posts.CONTENT;
            post.CATEGORYID = posts.CATEGORYID;

            dbContext.Update(post);
            result = await dbContext.SaveChangesAsync();
        }
        return result;
    }

    public static async Task<int> DeletePostAsync(Posts posts)
    {
        int result = 0;
        using (var dbContext = new DBContext())
        {
            Posts post = await dbContext.Posts.FirstOrDefaultAsync(p => p.ID == posts.ID);
            if(post == null){ return 0;}

            dbContext.Posts.Remove(post);
            result = await dbContext.SaveChangesAsync();
        }
        return result;
    }

    public static async Task<List<Posts>> GetAllPostsAsync()
    {
        var posts = new List<Posts>();
        using (var dbContext = new DBContext())
        {
            posts = await dbContext.Posts
            .Include(p => p.IMAGES)
            .Include(p=>p.CATEGORY)
            .Include(p=>p.DOCS)
            .OrderByDescending(p=>p.CREATED_AT)
            .ToListAsync();
        }
        return posts;
    }

    public static async Task<List<Posts>> GetPinnedPosts()
    {
        var posts = new List<Posts>();
        var pinnedPosts = new List<PinnedPosts>();
        using (var dbContext = new DBContext())
        {
            pinnedPosts = await dbContext.Pinned_Posts
            .OrderByDescending(p=>p.CREATED_AT)
            .ToListAsync();

            foreach (var pinnedPost in pinnedPosts)
            {
                posts.Add(await GetPostByIdAsync(new Posts { ID = pinnedPost.POSTID }));
            }
        }
        return posts;
    }

    public static async Task<List<Posts>> GetLastPosts()
    {
        var posts = new List<Posts>();
        using (var dbContext = new DBContext())
        {
            posts = await dbContext.Posts
            .Include(p => p.IMAGES)
            .Include(p=>p.CATEGORY)
            .Include(p=>p.DOCS)
            .OrderByDescending(p=>p.CREATED_AT)
            .Take(8)
            .ToListAsync();
        }
        return posts;
    }

    public static async Task<List<Posts>> GetSearchedManagePosts(Posts pPosts)
    {
        var posts = new List<Posts>();
        using (var dbContext = new DBContext())
        {
            var select = dbContext.Posts.AsQueryable();
            select = QuerySearch(select, pPosts);
            posts = await select.ToListAsync();
        }
        return posts;
    }

    public static async Task<Posts> GetPostByIdAsync(Posts posts)
    {
        var post = new Posts();
        using (var dbContext = new DBContext())
        {
            post = await dbContext.Posts
            .Include(p => p.IMAGES)
            .Include(p=>p.CATEGORY)
            .Include(p=>p.DOCS)
            .Where(p=>p.ID == posts.ID)
            .FirstOrDefaultAsync(p => p.ID == posts.ID);
        }
        return post;
    }

    internal static IQueryable<Posts> QuerySearch(IQueryable<Posts> pQuery, Posts pPosts)
    {
        pQuery = pQuery.Include(p => p.IMAGES);
        pQuery = pQuery.Include(p => p.CATEGORY);
        pQuery = pQuery.Include(p => p.DOCS);

        if (!string.IsNullOrWhiteSpace(pPosts.CONTENT) || !string.IsNullOrWhiteSpace(pPosts.TITLE))
        {
            var searchTerm = pPosts.CONTENT ?? pPosts.TITLE;
            pQuery = pQuery.Where(s => s.CONTENT.Contains(searchTerm) || s.TITLE.Contains(searchTerm));
        }

        if (pPosts.CATEGORYID > 0)
            pQuery = pQuery.Where(s => s.CATEGORYID == pPosts.CATEGORYID);
    
        if (pPosts.CREATED_AT != DateTime.MinValue)
        {
            pQuery = pQuery.Where(s => s.CREATED_AT.Date == pPosts.CREATED_AT.Date);
        }

        pQuery = pQuery.OrderByDescending(s => s.CREATED_AT).AsQueryable();
        return pQuery;
    }

    internal static IQueryable<Posts> QuerySelect(IQueryable<Posts> pQuery, Posts pPosts)
    {
        pQuery = pQuery.Include(p => p.IMAGES);
        pQuery = pQuery.Include(p => p.CATEGORY);
        pQuery = pQuery.Include(p => p.DOCS);

        if (pPosts.ID > 0)
            pQuery = pQuery.Where(s => s.ID == pPosts.ID);

        if (!string.IsNullOrWhiteSpace(pPosts.TITLE))
            pQuery = pQuery.Where(s => s.TITLE.Contains(pPosts.TITLE));

        if (!string.IsNullOrWhiteSpace(pPosts.CONTENT))
            pQuery = pQuery.Where(s => s.CONTENT.Contains(pPosts.CONTENT));

        if(pPosts.CATEGORY != null)
        {
            if(pPosts.CATEGORY != null && !string.IsNullOrWhiteSpace(pPosts.CATEGORY.Name))
                pQuery = pQuery.Where(s => s.CATEGORY.Name.Contains(pPosts.CATEGORY.Name));

        }
        
        pQuery = pQuery.OrderByDescending(s => s.CREATED_AT).AsQueryable();
        return pQuery;
    }

    public static async Task<List<Posts>> GetPostsAsync(Posts pPosts)
    {
        var posts = new List<Posts>();
        using (var dbContext = new DBContext())
        {
            var select = dbContext.Posts.AsQueryable();
            select = QuerySelect(select, pPosts).AsQueryable();
            posts = await select.ToListAsync();
        }
        return posts;
    }



    #endregion
}