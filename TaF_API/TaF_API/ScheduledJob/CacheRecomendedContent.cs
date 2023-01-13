using Neo4jClient;
using Quartz;
using StackExchange.Redis;
using TaF_Neo4j.Services.Blog;
using TaF_Redis.Services.Content;

namespace TaF_API.ScheduledJob
{
    public class CacheRecomendedContent : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var data = context.MergedJobDataMap;
            GraphClient client = (GraphClient)data.Get("neo4jClient");
            ConnectionMultiplexer multiplexer = (ConnectionMultiplexer)data.Get("redisMultiplexer");

            var contentService = new ContentServiceRedis(multiplexer, client);
            await contentService.CacheRecomendedContent(); 
        }
    }
}
