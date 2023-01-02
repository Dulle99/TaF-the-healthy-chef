﻿using Neo4jClient;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs.BlogDTO;
using TaF_Neo4j.DTOs.CookingRecepieDTO;
using TaF_Neo4j.Services.Blog;
using TaF_Neo4j.Services.CookingRecepie;
using TaF_Redis.KeyScheme;
using TaF_Redis.Services.MutalMethods;

namespace TaF_Redis.Services.User
{
    public class UserServiceRedis : IUserServiceRedis
    {
        private IDatabase _redis;
        private IGraphClient _client;
        private BlogService? _neo4jBlogService;
        private CookingRecepieService? _neo4jCookingRecepieService;

        public UserServiceRedis(IConnectionMultiplexer connectionMultiplexer, IGraphClient client )
        {
            this._redis = connectionMultiplexer.GetDatabase();
            this._client = client;
        }

        #region CacheData

        public async Task CacheAuthorCookingRecepies(string username, int numberOfCoookingRecepiesToCache = 5)
        {
            try
            {
                _neo4jCookingRecepieService = new CookingRecepieService(_client);
                var cookingRecepiesOfTheAuthor = await _neo4jCookingRecepieService.GetPreviewCookingRecepiesByAuthor(username,numberOfCoookingRecepiesToCache);

                var authorCookingRecepies_SetKey = KeyGenerator.CreateKeyForAuthorPersonalContent(username, Types.ContentType.cookingRecepie);
                foreach(var cookingRecepie in cookingRecepiesOfTheAuthor)
                {
                    await AuxiliaryContentMethods.SaveContentToRedisDatabase(_redis, Types.ContentType.cookingRecepie,cookingRecepie ,
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
                _neo4jBlogService = new BlogService(_client);  
                var blogsOfTheAuthor = await _neo4jBlogService.GetPreviewBlogsByAuthor(username, numberOfBlogsToCache);

                var authorBlogs_SetKey = KeyGenerator.CreateKeyForAuthorPersonalContent(username, Types.ContentType.blog);
                foreach (var blog in blogsOfTheAuthor) 
                {
                    await AuxiliaryContentMethods.SaveContentToRedisDatabase(_redis, Types.ContentType.blog, blog, blog.BlogId, authorBlogs_SetKey);
                }
            }
            catch(Exception ex) 
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
        #endregion GetCachedData

        #region DeleteCachedData

        #endregion DeleteCachedData
    }
}
