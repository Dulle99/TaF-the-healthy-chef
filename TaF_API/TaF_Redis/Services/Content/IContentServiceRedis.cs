using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Redis.Services.Content
{
    public interface IContentServiceRedis
    {
        public Task RemoveContentFromCache(Types.ContentType contentType, Guid contentId);
    }
}
