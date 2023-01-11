using Neo4jClient;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.Services.Blog;
using TaF_Neo4j.Services.CookingRecepie;
using TaF_Redis.KeyScheme;

namespace TaF_Redis.Services.Content
{
    public class ContentServiceRedis : IContentServiceRedis
    {
        private CookingRecepieService _cookingRecepieService;
        private BlogService _blogServiceRedis;
        private IGraphClient _neo4jClient;
        private IDatabase _redis;

        public ContentServiceRedis(IConnectionMultiplexer connectionMultiplexer, IGraphClient _client)
        {
            this._redis = connectionMultiplexer.GetDatabase();
            this._neo4jClient = _client;
        }

        public async Task RemoveContentFromCache(Types.ContentType contentType, Guid contentId)
        { 
            try
            {
                var contentKey = KeyGenerator.CreateKeyForContent(contentType, contentId);
                await this._redis.KeyDeleteAsync(contentKey);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
