using Microsoft.Extensions.ObjectPool;
using Neo4jClient;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs.BlogDTO;
using TaF_Neo4j.DTOs.CookingRecepieDTO;
using TaF_Neo4j.Models;
using TaF_Neo4j.Services.User.Author;
using TaF_Neo4j.Services.User.Reader;
using TaF_Redis.KeyScheme;

namespace TaF_Redis.Services.MutalMethods
{
    public static class AuxiliaryContentMethods
    {
        public async static Task SaveContentToRedisDatabase(IDatabase _redis, Types.ContentType contentType, object content, Guid contentId, string keyForSet)
        {
            var contentHashEntry = HashDataParser.ToHashEntries(content);
            var contentHashKey = KeyGenerator.CreateKeyForContent(contentType, contentId);
            await _redis.HashSetAsync(contentHashKey, contentHashEntry);
            await _redis.SetAddAsync(keyForSet, contentHashKey);
        }

        public async static Task<List<T>> GetContentFromHash<T>(IDatabase _redis, string[] keys)
        {
            if (keys.Length > 0 && keys != null )
            {

                List<T> contents = new List<T>();
                foreach (string key in keys)
                {
                    var contentHashEntry = await _redis.HashGetAllAsync(key);
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
    }

    
}
