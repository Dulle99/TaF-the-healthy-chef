using Neo4jClient;
using Newtonsoft.Json;
using StackExchange.Redis;
using TaF_Neo4j.DTOs.BlogDTO;
using TaF_Neo4j.DTOs.CookingRecepieDTO;
using TaF_Neo4j.Services.Blog;
using TaF_Neo4j.Services.CookingRecepie;
using TaF_Redis.KeyScheme;
using TaF_Redis.Services.MutalMethods;
using TaF_Redis.Types;

namespace TaF_Redis.Services.User
{
    public class UserServiceRedis : IUserServiceRedis
    {
        private IDatabase _redis;
        private IGraphClient _neo4jClient;
        private BlogService? _neo4jBlogService;
        private CookingRecepieService? _neo4jCookingRecepieService;

        public UserServiceRedis(IConnectionMultiplexer connectionMultiplexer, IGraphClient client )
        {
            this._redis = connectionMultiplexer.GetDatabase();
            this._neo4jClient = client;
        }

        

        #region CacheData

        public async Task CacheAuthorCookingRecepies(string username, int numberOfCoookingRecepiesToCache = 5)
        {
            try
            {
                _neo4jCookingRecepieService = new CookingRecepieService(_neo4jClient);
                var cookingRecepiesOfTheAuthor = await _neo4jCookingRecepieService.GetPreviewCookingRecepiesByAuthor(username,numberOfCoookingRecepiesToCache);

                var authorCookingRecepies_SetKey = KeyGenerator.CreateKeyForAuthorPersonalContent(username, Types.ContentType.cookingRecepie);
                foreach(var cookingRecepie in cookingRecepiesOfTheAuthor)
                {
                    await AuxiliaryContentMethods.CacheContent(_redis, Types.ContentType.cookingRecepie,cookingRecepie ,
                                                               cookingRecepie.CookingRecepieId, authorCookingRecepies_SetKey);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public async Task CacheAuthorBlogs(string username, int numberOfBlogsToCache = 5)
        {
            try
            {
                _neo4jBlogService = new BlogService(_neo4jClient);  
                var blogsOfTheAuthor = await _neo4jBlogService.GetPreviewBlogsByAuthor(username, numberOfBlogsToCache);

                var authorBlogs_SetKey = KeyGenerator.CreateKeyForAuthorPersonalContent(username, Types.ContentType.blog);
                foreach (var blog in blogsOfTheAuthor) 
                {
                    await AuxiliaryContentMethods.CacheContent(_redis, Types.ContentType.blog, blog, blog.BlogId, authorBlogs_SetKey);
                }
            }
            catch(Exception ex) 
            {

            }
        }

        public async Task CacheUsersSavedCookingRecepies(UserType userType, string username)
        {
            try
            {
                var userSavedCookingRecepies_SetKey = KeyGenerator.CreateKeyForUsersSavedContent(username, ContentType.savedCookingRecepie);
                var savedCookingRecepies = await AuxiliaryContentMethods.GetUserSavedCookingRecepies(this._neo4jClient, username, userType);

                foreach(var cookingRecepie in savedCookingRecepies)
                {
                    await AuxiliaryContentMethods.CacheContent(this._redis, ContentType.cookingRecepie, cookingRecepie, 
                                                                             cookingRecepie.CookingRecepieId, userSavedCookingRecepies_SetKey);
                }

            }
            catch (Exception ex)
            {

            }
        }

        public async Task CacheUsersSavedBlogs(UserType userType, string username)
        {
            try
            {
                var userSavedBlogs_SetKey = KeyGenerator.CreateKeyForUsersSavedContent(username, ContentType.savedBlog);
                var savedBlogs = await AuxiliaryContentMethods.GetUserSavedBlogs(_neo4jClient, username, userType);

                foreach(var blog in savedBlogs)
                {
                    await AuxiliaryContentMethods.CacheContent(_redis, ContentType.blog, blog,blog.BlogId, userSavedBlogs_SetKey);
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion CacheData

        #region GetCachedData

        public async Task<List<CookingRecepiePreviewDTO>> GetCachedCookingRecepiesByAuthor(string authorUsername, int numberOfCookingRecepiesToGet = 5)
        {
            try
            {
                var authorCachedCookingRecepies_SetKey = KeyGenerator.CreateKeyForAuthorPersonalContent(authorUsername, Types.ContentType.cookingRecepie);
                var cachedHashKeys_OfCookingRecepies = _redis.SetMembers(authorCachedCookingRecepies_SetKey).ToStringArray();

                var cookingRecepiesOfTheAuthor = await AuxiliaryContentMethods.GetContentFromHash<CookingRecepiePreviewDTO>(this._redis, cachedHashKeys_OfCookingRecepies);
                return cookingRecepiesOfTheAuthor;
            }
            catch(Exception ex) 
            {
                return new List<CookingRecepiePreviewDTO>();
            }
        }

        public async Task<List<BlogPreviewDTO>> GetCachedBlogsByAuthor(string authorUsername, int numberOfBlogsToGet = 5)
        {
            try
            {
                var authorBlogs_SetKey = KeyGenerator.CreateKeyForAuthorPersonalContent(authorUsername, Types.ContentType.blog);
                var cachedHashKeys_OfBlogs = _redis.SetMembers(authorBlogs_SetKey).ToStringArray();

                var blogsOfTheAuthor = await AuxiliaryContentMethods.GetContentFromHash<BlogPreviewDTO>(this._redis, cachedHashKeys_OfBlogs);
                return blogsOfTheAuthor;
            }
            catch(Exception ex)
            {
                return new List<BlogPreviewDTO>();
            }
        }

        public async Task<List<CookingRecepiePreviewDTO>> GetCached_ReadLaterCookingRecepies(string username, int numberOfCookingRecepiesToGet = 5)
        {
            try
            {
                var userSavedCookingRecepies_SetKey = KeyGenerator.CreateKeyForUsersSavedContent(username, ContentType.savedCookingRecepie);
                var cachedHashKeys_OfSavedCookingRecepies = _redis.SetMembers(userSavedCookingRecepies_SetKey).ToStringArray();

                var savedCookingRecepies = await AuxiliaryContentMethods.GetContentFromHash<CookingRecepiePreviewDTO>(this._redis, cachedHashKeys_OfSavedCookingRecepies);
                return savedCookingRecepies;
            }
            catch(Exception ex) 
            {
                return new List<CookingRecepiePreviewDTO>();
            }
        }

        public async Task<List<BlogPreviewDTO>> GetCached_ReadLaterBlogs(string username, int numberOfBlogsToGet = 5)
        {
            try
            {
                var userSavedBlogs_SetKey = KeyGenerator.CreateKeyForUsersSavedContent(username, ContentType.savedBlog);
                var cachedHashKeys_OfSavedBlogs = _redis.SetMembers(userSavedBlogs_SetKey).ToStringArray();

                var savedBlogs = await AuxiliaryContentMethods.GetContentFromHash<BlogPreviewDTO>(_redis, cachedHashKeys_OfSavedBlogs);
                return savedBlogs;
            }
            catch(Exception ex) 
            {
                return new List<BlogPreviewDTO>();
            }
        }

        #endregion GetCachedData

        #region DeleteCachedData

        public async Task RemoveUserCachedData(string username, Types.UserType userType)
        {
            try
            {
                string savedCookingRecepies_SetKey = KeyGenerator.CreateKeyForUsersSavedContent(username, ContentType.savedCookingRecepie);
                string savedBlogs_SetKey = KeyGenerator.CreateKeyForUsersSavedContent(username, ContentType.savedBlog);
                string personalCookingRecepies_SetKey;
                string personalBlogs_SetKey;

                if (Types.UserType.Author == userType)
                {
                    personalBlogs_SetKey = KeyGenerator.CreateKeyForAuthorPersonalContent(username, ContentType.blog);
                    personalCookingRecepies_SetKey = KeyGenerator.CreateKeyForAuthorPersonalContent(username, ContentType.cookingRecepie);

                    await AuxiliaryContentMethods.RemoveCache(this._redis, personalBlogs_SetKey);
                    await AuxiliaryContentMethods.RemoveCache(this._redis, personalCookingRecepies_SetKey);
                }
                await AuxiliaryContentMethods.RemoveCache(this._redis, savedCookingRecepies_SetKey);
                await AuxiliaryContentMethods.RemoveCache(this._redis, savedBlogs_SetKey);

            }
            catch (Exception ex)
            {

            }
        }

        public async Task RemoveUserSavedContent(string username, Types.ContentType contentType, Guid contentId)
        {
            try
            {
                var contentHashKey = KeyGenerator.CreateKeyForContent(contentType == ContentType.savedCookingRecepie ? ContentType.cookingRecepie : ContentType.blog, contentId);
                var userSavedContentSetKey = KeyGenerator.CreateKeyForUsersSavedContent(username, contentType);
                var res = await _redis.SetRemoveAsync(new RedisKey(userSavedContentSetKey), new RedisValue(contentHashKey));
                if(res)
                    await AuxiliaryContentMethods.DecrementUsageCounterOfContent(_redis, contentHashKey);
            }
            catch(Exception ex)
            {

            }
        }



        #endregion DeleteCachedData
    }
}
