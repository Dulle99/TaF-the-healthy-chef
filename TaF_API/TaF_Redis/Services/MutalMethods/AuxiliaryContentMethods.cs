using Microsoft.Extensions.ObjectPool;
using Neo4jClient;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Mime;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs;
using TaF_Neo4j.DTOs.BlogDTO;
using TaF_Neo4j.DTOs.CookingRecepieDTO;
using TaF_Neo4j.Models;
using TaF_Neo4j.Services.Blog;
using TaF_Neo4j.Services.CookingRecepie;
using TaF_Neo4j.Services.User.Author;
using TaF_Neo4j.Services.User.Reader;
using TaF_Redis.KeyScheme;
using TaF_Redis.Types;

namespace TaF_Redis.Services.MutalMethods
{
    public static class AuxiliaryContentMethods
    {

        #region CreareOrUpdateCache

        public async static Task CacheContent(IDatabase _redis, Types.ContentType contentType, object content, Guid contentId, string keyForList, bool list_pushAtTail = true)
        {
            var contentHash_Key = KeyGenerator.CreateKeyForContent(contentType, contentId);
            if (await _redis.KeyExistsAsync(contentHash_Key))
                await IncrementUsageCounterOfContent(_redis, contentHash_Key);
            else
            {
                var contentHash_Entry = HashDataParser.ToHashEntries(content);
                await _redis.HashSetAsync(contentHash_Key, contentHash_Entry);
                await AppendUsageCounterOfContentField(_redis, contentHash_Key);
            }

            await _redis.ListRemoveAsync(keyForList, contentHash_Key);
            if(list_pushAtTail)
                await _redis.ListRightPushAsync(keyForList, contentHash_Key);
            else
                await _redis.ListLeftPushAsync(keyForList, contentHash_Key);
        }

        public async static Task AppendUsageCounterOfContentField(IDatabase _redis, string hashKey)
        {
            await _redis.HashSetAsync(hashKey, new RedisValue("UsageCounter"), new RedisValue("1"));
        }

        public async static Task IncrementUsageCounterOfContent(IDatabase _redis, string hashKey)
        {
            await _redis.HashIncrementAsync(hashKey, new RedisValue("UsageCounter"));
        }

        public async static Task DecrementUsageCounterOfContent(IDatabase _redis, string hashKey)
        {
            var counterValue = await _redis.HashDecrementAsync(hashKey, new RedisValue("UsageCounter"));
            if (counterValue == 0)
                await _redis.KeyDeleteAsync(hashKey);
        }

        #endregion CreareOrUpdateCache

        #region GetCache

        public async static Task<List<T>> GetContentFromHash<T>(IDatabase _redis, string[] keys)
        {
            if (keys.Length > 0 && keys != null )
            {

                List<T> contents = new List<T>();
                foreach (string key in keys)
                {
                    var contentHashEntry = await _redis.HashGetAllAsync(key);
                    if(contentHashEntry.Length > 0)
                        contents.Add(HashDataParser.ConvertFromRedis<T>(contentHashEntry));
                }
                return contents;
            }
            else
                return new List<T>();
        }

        public async static Task<List<CookingRecepiePreviewDTO>> GetUserSavedCookingRecepies(IGraphClient client, string username, Types.UserType typeOfUser, int numberOfRecepiesToGet = 5)
        {
            if(typeOfUser == Types.UserType.Author)
            {
                var authorService = new AuthorService(client);
                return await authorService.GetReadLaterCookingRecepies(username, numberOfRecepiesToGet);
            }
            else
            {
                var readerService = new ReaderService(client);
                return await readerService.GetReadLaterCookingRecepies(username, numberOfRecepiesToGet);
            }
        }

        public async static Task<List<BlogPreviewDTO>> GetUserSavedBlogs(IGraphClient client, string username, Types.UserType typeOfUser, int numberOfBlogsToGet = 5)
        {
            if (typeOfUser == Types.UserType.Author)
            {
                var authorService = new AuthorService(client);
                return await authorService.GetReadLaterBlogs(username, numberOfBlogsToGet);
            }
            else
            {
                var readerService = new ReaderService(client);
                return await readerService.GetReadLaterBlogs(username, numberOfBlogsToGet);
            }
        }

        public async static Task<object> GetContentPreviewFromDatabase(IGraphClient client,Types.ContentType contentType, Guid contentId)
        {
            if(contentType == Types.ContentType.cookingRecepie)
                return await CookingRecepieServiceAuxiliaryMethods.GetCookingRecepiePreview(client, contentId);
            else
                return await BlogServiceAuxiliaryMethods.GetBlogPreview(client, contentId);
        }

        public static async Task<(object content, Guid contentId)> GetTheNewestContentAndIdBasedByContentType(IGraphClient _client, Types.UserType userType, string username, Types.ContentType contentType)
        {
            try
            {
                if(contentType == Types.ContentType.cookingRecepie || contentType == Types.ContentType.savedCookingRecepie)
                {
                    List<CookingRecepiePreviewDTO> cookingRecepiePreview;
                    if (contentType == Types.ContentType.cookingRecepie)
                    {
                        var cookingRecepieService = new CookingRecepieService(_client);
                        cookingRecepiePreview = await cookingRecepieService.GetPreviewCookingRecepiesByAuthor(username, 1);
                    }
                    else
                        cookingRecepiePreview = await GetUserSavedCookingRecepies(_client, username, userType, 1);
                    
                    if(cookingRecepiePreview.Count > 0) 
                        return (cookingRecepiePreview[0], cookingRecepiePreview[0].CookingRecepieId);
                }
                else
                {
                    List<BlogPreviewDTO> blogPreviewDTO;
                    if (contentType == Types.ContentType.blog)
                    {
                        var blogService = new BlogService(_client);
                        blogPreviewDTO = await blogService.GetPreviewBlogsByAuthor(username, 1);
                    }
                    else
                        blogPreviewDTO = await GetUserSavedBlogs(_client, username, userType, 1);

                    if(blogPreviewDTO.Count > 0)
                        return (blogPreviewDTO[0], blogPreviewDTO[0].BlogId);
                }

                return (null, Guid.Empty);
            }
            catch(Exception ex) { return (null, Guid.Empty); }
        }

        public static async Task PopLastAndAppendNewContentInList(IDatabase _redis, string listKey, Types.ContentType contentType, object content, Guid contentId)
        {
            if (content != null && contentId != Guid.Empty)
            {
                var oldContentFromCacheId = await _redis.ListRightPopAsync(listKey);
                await DecrementUsageCounterOfContent(_redis, oldContentFromCacheId);
                await CacheContent(_redis, contentType, content, contentId, listKey, false);
            }
        }

        #endregion GetCache

        #region RemoveCache


        public static async Task RemoveCache(IDatabase _redis, string listKey)
        {
            //var contentHashKeys = await _redis.SetMembersAsync(setKey);
            var contentHashKeys = await _redis.ListRangeAsync(listKey);
            foreach (var content in  contentHashKeys)
            {
               await DecrementUsageCounterOfContent(_redis, content.ToString());
            }

            await _redis.KeyDeleteAsync(listKey);
        }

        #endregion RemoveCache

        #region FilteringHelpMethods

        public static string GetMergedStepsInFoodPrepration(List<StepInFoodPreparationDTO> stepsInFoodPreparation)
        {
                if (stepsInFoodPreparation.Count > 0)
                {
                    var steps = stepsInFoodPreparation[0].StepDescription;
                    for (int i = 1; i <= stepsInFoodPreparation.Count - 1; i++)
                        steps = MergeStrings(steps, stepsInFoodPreparation[i].StepDescription);

                    return steps;
                }
                else
                    return String.Empty;
        }

        public static string GetMergedIngredients(List<IngredientForCookingRecepieDTO> ingredientsOfCookingRecepie)
        {
            if(ingredientsOfCookingRecepie.Count > 0)
            {
                var ingredients = ingredientsOfCookingRecepie[0].Ingredient;
                for(int i = 1; i <= ingredientsOfCookingRecepie.Count - 1; i++)
                    ingredients = MergeStrings(ingredients, ingredientsOfCookingRecepie[i].Ingredient);

                return ingredients;
            }
            else
                return String.Empty;
        }

        public static string[] GetWords(string content)
        {
            var filteredContent = FilterContentFromPunction(content);
            return SplitContentIntoWords(filteredContent);
        }

        public static string FilterContentFromPunction(string content)
        {
            var filteredContentFromMultipleSpaces = Regex.Replace(content, @"\s+", " ").Trim();
            var filteredContentFromInterpunction = Regex.Replace(filteredContentFromMultipleSpaces, @"[^\w\s]", string.Empty);
            return filteredContentFromInterpunction.ToLower();
        }

        public static string[] SplitContentIntoWords(string content)
        {
            return content.Split(' ');
        }

        public static string MergeStrings(string mainString, string additionalString)
        {
            return mainString + " " + additionalString;
        }
        #endregion FilteringHelpMethods
    }
}
