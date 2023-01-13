﻿using Microsoft.Extensions.ObjectPool;
using Neo4jClient;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs.BlogDTO;
using TaF_Neo4j.DTOs.CookingRecepieDTO;
using TaF_Neo4j.Models;
using TaF_Neo4j.Services.Blog;
using TaF_Neo4j.Services.CookingRecepie;
using TaF_Neo4j.Services.User.Author;
using TaF_Neo4j.Services.User.Reader;
using TaF_Redis.KeyScheme;

namespace TaF_Redis.Services.MutalMethods
{
    public static class AuxiliaryContentMethods
    {

        #region CreareOrUpdateCache

        public async static Task CacheContent(IDatabase _redis, Types.ContentType contentType, object content, Guid contentId, string keyForSet)
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

            await _redis.SetAddAsync(keyForSet, contentHash_Key);
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

        public async static Task<List<CookingRecepiePreviewDTO>> GetUserSavedCookingRecepies(IGraphClient client, string username, Types.UserType typeOfUser)
        {
            if(typeOfUser == Types.UserType.Author)
            {
                var authorService = new AuthorService(client);
                return await authorService.GetReadLaterCookingRecepies(username, 5);
            }
            else
            {
                var readerService = new ReaderService(client);
                return await readerService.GetReadLaterCookingRecepies(username, 5);
            }
        }

        public async static Task<List<BlogPreviewDTO>> GetUserSavedBlogs(IGraphClient client, string username, Types.UserType typeOfUser)
        {
            if (typeOfUser == Types.UserType.Author)
            {
                var authorService = new AuthorService(client);
                return await authorService.GetReadLaterBlogs(username, 5);
            }
            else
            {
                var readerService = new ReaderService(client);
                return await readerService.GetReadLaterBlogs(username, 5);
            }
        }

        public async static Task<object> GetContentPreviewFromDatabase(IGraphClient client,Types.ContentType contentType, Guid contentId)
        {
            if(contentType == Types.ContentType.cookingRecepie)
                return await CookingRecepieServiceAuxiliaryMethods.GetCookingRecepiePreview(client, contentId);
            else
                return await BlogServiceAuxiliaryMethods.GetBlogPreview(client, contentId);
        }

        #endregion GetCache

        #region RemoveCache


        public static async Task RemoveCache(IDatabase _redis, string setKey)
        {
            var contentHashKeys = await _redis.SetMembersAsync(setKey);
            foreach(var content in  contentHashKeys)
            {
               await DecrementUsageCounterOfContent(_redis, content.ToString());
            }

            await _redis.KeyDeleteAsync(setKey);
        }

        #endregion RemoveCache


    }


}
