using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs.BlogDTO;
using TaF_Neo4j.DTOs.CookingRecepieDTO;

namespace TaF_Redis.Services.Content
{
    public interface IContentServiceRedis
    {
        public Task CacheRecomendedContent();

        public Task<List<CookingRecepiePreviewDTO>> GetCachedRecomendedCookingRecepies();
        public Task<List<BlogPreviewDTO>> GetCachedRecomendedBlogs();

        public Task RemoveContentFromCache(Types.ContentType contentType, Guid contentId);

        public Task UpdateContent(Types.ContentType contentType, Guid contentId);

        public Task<bool> ContentContainBadWord(string content);
    }
}
