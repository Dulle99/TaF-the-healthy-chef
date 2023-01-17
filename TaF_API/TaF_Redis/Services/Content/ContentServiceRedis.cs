using Neo4jClient;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs.BlogDTO;
using TaF_Neo4j.DTOs.CookingRecepieDTO;
using TaF_Neo4j.Services.Blog;
using TaF_Neo4j.Services.CookingRecepie;
using TaF_Redis.KeyScheme;
using TaF_Redis.Services.MutalMethods;

namespace TaF_Redis.Services.Content
{
    public class ContentServiceRedis : IContentServiceRedis
    {
        //private CookingRecepieService _cookingRecepieService;
        //private BlogService _blogServiceRedis;
        private IGraphClient _neo4jClient;
        private IDatabase _redis;

        public ContentServiceRedis(IConnectionMultiplexer connectionMultiplexer, IGraphClient _client)
        {
            this._redis = connectionMultiplexer.GetDatabase();
            this._neo4jClient = _client;
        }

        #region CachingContent

        public async Task CacheRecomendedContent()
        {
            try
            {
                await CacheRecomendedCookingRecepies();
                await CacheRecomendedBlogs();
            }
            catch (Exception ex) { }
        }


        private async Task CacheRecomendedCookingRecepies()
        {
            try
            {
                var _cookingRecepieService = new CookingRecepieService(this._neo4jClient);
                var recomendedCookingRecepies = await _cookingRecepieService.GetRecommendedCookingRecepies();
                var recomendedCookingRecepies_SetKey = KeyGenerator.CreateKeyForRecomendedCookingRecepies();

                foreach (var recepie in recomendedCookingRecepies)
                {
                    await AuxiliaryContentMethods.CacheContent(this._redis, Types.ContentType.cookingRecepie,
                                                         recepie, recepie.CookingRecepieId, recomendedCookingRecepies_SetKey);
                }
            }
            catch (Exception ex) { }
        }

        private async Task CacheRecomendedBlogs()
        {
            try
            {
                var _blogServiceRedis = new BlogService(this._neo4jClient);
                var recomendedBlogs = await _blogServiceRedis.GetRecommendedBlogs();
                var reomendedBlogs_SetKey = KeyGenerator.CreateKeyForRecomendedBlogs();

                foreach (var blog in recomendedBlogs)
                {
                    await AuxiliaryContentMethods.CacheContent(this._redis, Types.ContentType.blog, blog, blog.BlogId, reomendedBlogs_SetKey);
                }
            }
            catch (Exception ex) { }
        }

        #endregion CachingContent

        #region GetCachedContent

        public async Task<List<CookingRecepiePreviewDTO>> GetCachedRecomendedCookingRecepies()
        {
            try
            {
                var recomendedCookingRecepies_SetKey = KeyGenerator.CreateKeyForRecomendedCookingRecepies();
                var recomendedCookingRecepies_HashKeys = this._redis.SetMembers(recomendedCookingRecepies_SetKey).ToStringArray();

                return await AuxiliaryContentMethods.GetContentFromHash<CookingRecepiePreviewDTO>(this._redis, recomendedCookingRecepies_HashKeys);
            }
            catch(Exception ex) { return new List<CookingRecepiePreviewDTO>(); }
        }

        public async Task<List<BlogPreviewDTO>> GetCachedRecomendedBlogs()
        {
            try
            {
                var recomendedBlogs_SetKey = KeyGenerator.CreateKeyForRecomendedBlogs();
                var recomendedBlogs_HashKeys =  this._redis.SetMembers(recomendedBlogs_SetKey).ToStringArray();

                return await AuxiliaryContentMethods.GetContentFromHash<BlogPreviewDTO>(this._redis, recomendedBlogs_HashKeys);
            }
            catch (Exception ex) { return new List<BlogPreviewDTO>(); }
        }

        #endregion GetCachedContent

        #region RemoveCachedContent

        public async Task RemoveContentFromCache(Types.ContentType contentType, Guid contentId)
        {
            try
            {
                var contentKey = KeyGenerator.CreateKeyForContent(contentType, contentId);
                await this._redis.KeyDeleteAsync(contentKey);
            }
            catch (Exception ex) { }
        }

        #endregion RemoveCachedContent

        #region UpdateCachedContent

        public async Task UpdateContent(Types.ContentType contentType, Guid contentId)
        {
            try
            {
                var content_HashKey = KeyGenerator.CreateKeyForContent(contentType, contentId);
                var usageCounter = await this._redis.HashGetAsync(content_HashKey, new RedisValue("UsageCounter"));
                if (usageCounter != RedisValue.Null)
                {

                    var contentPreview = await AuxiliaryContentMethods.GetContentPreviewFromDatabase(this._neo4jClient, contentType, contentId);
                    if (contentPreview != null)
                    {
                        var entries = HashDataParser.ToHashEntries(contentPreview);
                        await this._redis.HashSetAsync(content_HashKey, entries);
                        await _redis.HashSetAsync(content_HashKey, new RedisValue("UsageCounter"), usageCounter);
                    }
                }
            }
            catch (Exception ex) { }
        }

        #endregion UpdateCachedContent

        #region FilterContent

        public async Task<bool> ContentContainBadWord(string content)
        {
            try
            {
                var contentTemporaryKey = "temporaryContentKey:" + Guid.NewGuid().ToString();
                var words = GetWords(content);
                foreach(var word in words) 
                {
                    await this._redis.SetAddAsync(contentTemporaryKey, word);
                }
             
                var result = await this._redis.SetCombineAsync(SetOperation.Intersect, "badWords:", contentTemporaryKey);
                await this._redis.KeyDeleteAsync(contentTemporaryKey);

                if (result.Length > 0)
                    return true;
                else
                    return false;
            }
            catch(Exception ex) { return false; }
        }

        public string[] GetWords(string content)
        {
            var filteredContent = FilterContentFromPunction(content);
            return SplitContentIntoWords(filteredContent);
        }

        public string FilterContentFromPunction(string content)
        {
           
            var filteredContentFromInterpunction = Regex.Replace(content, @"[^\w\s]" , string.Empty);
            return filteredContentFromInterpunction.ToLower();
        }

        public string[] SplitContentIntoWords(string content)
        {
            return content.Split(' ');
        }

        #endregion FilterContent

    }
}
