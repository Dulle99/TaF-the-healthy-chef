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
        public Task CacheUsersSavedCookingRecepies(UserType userType, string username);
        public Task CacheUsersSavedBlogs(UserType userType, string username);

        public Task<List<CookingRecepiePreviewDTO>> GetCachedCookingRecepiesByAuthor(string authorUsername, int numberOfCookingRecepiesToGet = 5);
        public Task<List<BlogPreviewDTO>> GetCachedBlogsByAuthor(string authorUsername, int numberOfBlogsToGet = 5);
        public Task<List<CookingRecepiePreviewDTO>> GetCached_ReadLaterCookingRecepies(string username, int numberOfCookingRecepiesToGet = 5);
        public Task<List<BlogPreviewDTO>> GetCached_ReadLaterBlogs(string username, int numberOfBlogsToGet = 5);

        public Task RemoveUserCachedData(string username, Types.UserType userType);
        public Task RemoveUserSavedContent(string username, Types.ContentType contentType, Guid contentId);
    }
}
