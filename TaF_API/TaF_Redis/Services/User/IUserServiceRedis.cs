using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Redis.Services.User
{
    public interface IUserServiceRedis
    {
        public Task CacheAuthorBlogs(string username);
        public Task CacheAuthorCookingRecipes(string username);
    }
}
