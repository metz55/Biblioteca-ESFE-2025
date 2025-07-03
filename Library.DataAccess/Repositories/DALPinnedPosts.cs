using System;
using Library.DataAccess.Domain;
using Library.DataAccess.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataAccess.Repositories;

public class DALPinnedPosts
{
    #region CRUD

    // <summary>
    /// / Retorna un pinnedPost que contenga el Id de un post
    /// </summary>
    /// <param name="posts">PinnedPost posts</param>
    /// <returns>PinnedPost</returns>
    public static async Task<PinnedPosts> GetPinnedPostByPostId(Posts posts)
    {
        PinnedPosts pinnedPost = new PinnedPosts();
        using (var dbContext = new DBContext())
        {
            pinnedPost = await dbContext.Pinned_Posts.FirstOrDefaultAsync(p => p.POSTID == posts.ID);
        }
        return pinnedPost;
    }

    public static async Task<bool> IsPostPinned(Posts posts)
    {
        PinnedPosts pinnedPost = new PinnedPosts();
        using (var dbContext = new DBContext())
        {
            pinnedPost = await dbContext.Pinned_Posts.FirstOrDefaultAsync(p => p.POSTID == posts.ID);
        }
        return pinnedPost?.ID > 0;
    }

    public static async Task<bool> CreatePinnedPost(PinnedPosts pPosts)
    {   
        bool result = false;
        using(var dbContext = new DBContext())
        {
            
            dbContext.Pinned_Posts.Add(pPosts);
            result = await dbContext.SaveChangesAsync() > 0;
        }

        return result;
    }


    public static async Task<bool> DeletePinnedPost(PinnedPosts pinnedPosts)
    {
        bool result = false;
        using (var dbContext = new DBContext())
        {
            PinnedPosts post = await dbContext.Pinned_Posts.FirstOrDefaultAsync(p => p.POSTID == pinnedPosts.POSTID);
            if(post == null){ return false;}

            dbContext.Pinned_Posts.Remove(post);
            result = await dbContext.SaveChangesAsync() > 0;
        }
        return result;
    }
    #endregion
}