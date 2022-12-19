using Neo4jClient;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs.BlogDTO;
using TaF_Neo4j.Services.Blog;
using TaF_Redis.KeyScheme;
using TaF_Redis.Services.MutalMethods;

namespace TaF_Redis.Services.User
{
    public class UserServiceRedis : IUserServiceRedis
    {
        private IDatabase _redis;
        private IGraphClient _client;
        private BlogService _neo4jBlogService;

        public UserServiceRedis(IConnectionMultiplexer connectionMultiplexer, IGraphClient client )
        {
            this._redis = connectionMultiplexer.GetDatabase();
            this._client = client;
        }

        #region CacheData

        public Task CacheAuthorBlogs(string username)
        {
            throw new NotImplementedException();
        }

        public Task CacheAuthorCookingRecipes(string username)
        {
            throw new NotImplementedException();
        }


        #endregion CacheData

        #region GetCachedData

        #endregion GetCachedData

        #region DeleteCachedData

        #endregion DeleteCachedData
    }
}
