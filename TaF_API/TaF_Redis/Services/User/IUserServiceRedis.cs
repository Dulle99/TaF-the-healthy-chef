using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs.BlogDTO;
using TaF_Neo4j.DTOs.CookingRecepieDTO;
using TaF_Redis.KeyScheme;
using TaF_Redis.Types;

namespace TaF_Redis.Services.User
{
    public interface IUserServiceRedis
    {
        public Task CacheAuthorCookingRecepies(string username, int numberOfCoookingRecepiesToCache = 5);
        public Task CacheAuthorBlogs(string username, int numberOfBlogsToCache = 5);

        public Task<List<CookingRecepiePreviewDTO>> GetCachedCookingRecepiesByAuthor(string authorUsername, int numberOfCookingRecepiesToGet = 5);
        public Task<List<BlogPreviewDTO>> GetCachedBlogsByAuthor(string authorUsername, int numberOfBlogsToGet = 5);
    }
}
