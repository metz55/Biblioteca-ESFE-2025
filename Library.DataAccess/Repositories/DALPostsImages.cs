using System;
using Library.DataAccess.Domain;
using Library.DataAccess.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Repositories;

public class DALPostsImages
{
    #region CRUD

    public static async Task<int> CreatePostImageAsync(PostsImages postsImages)
    {
        int result = 0;
        using (var dbContext = new DBContext())
        {   
            dbContext.Posts_Images.Add(postsImages);
            result = await dbContext.SaveChangesAsync();
        }
        return result;
    }

    public static async Task<int> UpdatePostImageAsync(PostsImages postsImages)
    {
        int result = 0;
        using (var dbContext = new DBContext())
        {
            var postImage = await dbContext.Posts_Images.FirstOrDefaultAsync(p => p.ID == postsImages.ID);
            if(postsImages == null){return 0;}

            postImage.PATH = postsImages.PATH;
            postImage.POSTID = postsImages.POSTID;

            dbContext.Update(postImage);
            result = await dbContext.SaveChangesAsync();
        }
        return result;
    }
    public static async Task<int> UpdatePostImageByIdPostAsync(Posts pPosts, PostsImages postsImages)
    {
        int result = 0;
        using (var dbContext = new DBContext())
        {
            var _postImages = await dbContext.Posts_Images.FirstOrDefaultAsync(p => p.POSTID == pPosts.ID);
            if(_postImages == null){return 0;}

            _postImages.PATH = postsImages.PATH;
            _postImages.POSTID = postsImages.POSTID;

            dbContext.Update(_postImages);
            result = await dbContext.SaveChangesAsync();
        }
        return result;
    }

    public static async Task<int> DeletePostImageAsync(PostsImages postsImages)
    {
        int result = 0;
        using (var dbContext = new DBContext())
        {
            var postImage = await dbContext.Posts_Images.FirstOrDefaultAsync(p => p.ID == postsImages.ID);
            if(postImage == null){ return 0;}

            dbContext.Posts_Images.Remove(postImage);
            result = await dbContext.SaveChangesAsync();
        }
        return result;
    }

    public static async Task<int> DeletePostsImagesByIdPostAsync(Posts pPosts)
    {
        int result = 0;
        using (var dbContext = new DBContext())
        {
            var imagesList = await dbContext.Posts_Images
                            .Where(p => p.POSTID == pPosts.ID)
                            .ToListAsync();
            
            if(!imagesList.Any())
            {
                return 0;
            }

            dbContext.Posts_Images.RemoveRange(imagesList);
            result = await dbContext.SaveChangesAsync();
        }
        return result;
    }

    public static async Task<List<PostsImages>> GetAllPostImagesAsync()
    {
        var postsImages = new List<PostsImages>();
        using (var dbContext = new DBContext())
        {
            postsImages = await dbContext.Posts_Images
            .Include(p => p.Post)
            .ToListAsync();
        }
        return postsImages;
    }

    public static async Task<List<PostsImages>> GetPostImagesByIdAsync(PostsImages postsImages)
    {
        var post = new List<PostsImages>();
        using (var dbContext = new DBContext())
        {
            post = await dbContext.Posts_Images
            .Include(p => p.Post)
            .Where(p => p.ID == postsImages.ID)
            .ToListAsync();
        }
        return post;
    }

    public static async Task<List<PostsImages>> GetPostImagesByIdPostAsync(Posts pPosts)
    {
        var post = new List<PostsImages>();
        using (var dbContext = new DBContext())
        {
            post = await dbContext.Posts_Images
            .Include(p => p.Post)
            .Where(p => p.POSTID == pPosts.ID)
            .ToListAsync();
        }
        return post;
    }

    internal static IQueryable<PostsImages> QuerySelect(IQueryable<PostsImages> pQuery, PostsImages pPosts)
    {
        pQuery = pQuery.Include(p => p.Post);

        if (pPosts.ID > 0)
            pQuery = pQuery.Where(s => s.ID == pPosts.ID);

        if (pPosts.POSTID > 0)
            pQuery = pQuery.Where(s => s.POSTID == pPosts.POSTID);

        pQuery = pQuery.OrderByDescending(s => s.Post.CREATED_AT).AsQueryable();
        return pQuery;
    }

    public static async Task<List<PostsImages>> GetPostImagesAsync(PostsImages pPosts)
    {
        var postsImages = new List<PostsImages>();
        using (var dbContext = new DBContext())
        {
            var select = dbContext.Posts_Images.AsQueryable();
            select = QuerySelect(select, pPosts);
            postsImages = await select.ToListAsync();
        }
        return postsImages;
    }



    #endregion
}