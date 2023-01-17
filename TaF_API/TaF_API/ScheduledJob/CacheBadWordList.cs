using Neo4jClient;
using Quartz;
using StackExchange.Redis;

namespace TaF_API.ScheduledJob
{
    public class CacheBadWordList : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var data = context.MergedJobDataMap;
            ConnectionMultiplexer multiplexer = (ConnectionMultiplexer)data.Get("redisMultiplexer");
            var redis = multiplexer.GetDatabase();

            var badWordsListPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "BadWords\\badwords.txt");
            var badWords = File.ReadLines(badWordsListPath);

            foreach (var badWord in badWords ) 
            {
                await redis.SetAddAsync("badWords:", badWord);
            }

        }
    }
}
