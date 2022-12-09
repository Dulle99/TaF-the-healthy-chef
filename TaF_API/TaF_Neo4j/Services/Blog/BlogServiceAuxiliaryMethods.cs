using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs.BlogDTO;
using TaF_Neo4j.Services.User.CommonUserMethods;

namespace TaF_Neo4j.Services.Blog
{
    public static class BlogServiceAuxiliaryMethods
    {
        public static async Task<List<Guid>> FilterBlogsByCriteria(IGraphClient client, string criteria, int pageNumber)
        {
            int numberOfIdsToGet = pageNumber;
            if (numberOfIdsToGet == 0)
                numberOfIdsToGet = 5; //default


            return (await client.Cypher.Match("(blog: Blog)")
                                              .Return((blog) => new
                                              {
                                                  Blog = blog.As<Models.Blog>()
                                              })
                                              .OrderBy(criteria).Limit(numberOfIdsToGet)
                                              .ResultsAsync)
                                              .Select(obj => obj.Blog.BlogId).ToList();

        }

        public static async Task<BlogPreviewDTO> GetBlogPreview(IGraphClient client, Guid blogId)
        {
            return (await client.Cypher.Match("(author:Author)-[:HAS_PUBLISHED]->(blog: Blog)")
                                             .Where((Models.Blog blog) => blog.BlogId == blogId)
                                             .Return((blog, author) => new
                                             {
                                                 Blog = blog.As<Models.Blog>(),
                                                 Author = author.As<Models.Author>()
                                             }).ResultsAsync)
                                             .Select(obj => new BlogPreviewDTO(obj.Author.Username, CommonUserMethods.ReadImageFromFile(obj.Author.Username, true),
                                                                               obj.Blog.BlogId, obj.Blog.BlogTitle,
                                                                               obj.Blog.BlogContent, obj.Blog.PublicationDate,
                                                                               obj.Blog.ReadingTime, obj.Blog.AverageRate,
                                                                               MutalMethodsForContent.
                                                                               MutalMethodsForContent.
                                                                               ReadImageFromFile(obj.Blog.BlogId, false))).FirstOrDefault();
        }

        public static async Task<float> UpdateAverageRateOfBlog(IGraphClient client, Guid blogId)
        {
            var newAverageRate = CalculateAverageRate(client, blogId);
            await client.Cypher.Match("(blog: Blog)")
                                     .Where((Models.Blog blog) => blog.BlogId == blogId)
                                     .Set("blog.AverageRate = $newAverageRate")
                                     .WithParam("newAverageRate", newAverageRate.Result)
                                     .ExecuteWithoutResultsAsync();
            return newAverageRate.Result;
        }

        public static async Task<float> CalculateAverageRate(IGraphClient client, Guid blogId)
        {
            var rates = (await client.Cypher.Match("(blog: Blog)-[:HAS_RATES]->(rate: Rate)")
                                                  .Where((Models.Blog blog) => blog.BlogId == blogId)
                                                  .Return((rate) => new
                                                  {
                                                      Rates = rate.As<Models.Rate>()
                                                  }).ResultsAsync)
                                                  .Select((obj) => new Models.Rate
                                                  {
                                                      RateId = obj.Rates.RateId,
                                                      RateValue = obj.Rates.RateValue
                                                  }).ToList();
            int totalValueRate = 0;
            foreach (var rate in rates)
            {
                totalValueRate += rate.RateValue;
            }
            return (totalValueRate / rates.Count);
        }

        public static async Task DetachAndDeleteCommentsOfBlog(IGraphClient client, Guid blogId)
        {
            await client.Cypher.Match("(blog: Blog) -[relComment:HAS_COMMENTS]->(comment: Comment)-[relAuthor:WRITES]->(author: Author)")
                                .Where((Models.Blog blog) => blog.BlogId == blogId)
                                .Delete("relComment")
                                .Delete("relAuthor")
                                .Delete("comment")
                                .ExecuteWithoutResultsAsync();

            await client.Cypher.Match("(blog: Blog) -[relComment:HAS_COMMENTS]->(comment: Comment)-[relReader:WRITES]->(reader: Reader)")
                                .Where((Models.Blog blog) => blog.BlogId == blogId)
                                .Delete("relComment")
                                .Delete("relReader")
                                .Delete("comment")
                                .ExecuteWithoutResultsAsync();
        }

        public static async Task DetachAndDeleteRatesOfBlog(IGraphClient client, Guid blogId)
        {
            await client.Cypher.Match("(blog: Blog) -[relRate:HAS_RATES]->(rate: Rate)-[relAuthor:RATES]->(author: Author)")
                               .Where((Models.Blog blog) => blog.BlogId == blogId)
                               .Delete("relRate")
                               .Delete("relAuthor")
                               .Delete("rate")
                               .ExecuteWithoutResultsAsync();

            await client.Cypher.Match("(blog: Blog) -[relRate:HAS_RATES]->(rate: Rate)-[relReader:RATES]->(reader: Reader)")
                               .Where((Models.Blog blog) => blog.BlogId == blogId)
                               .Delete("relRate")
                               .Delete("relReader")
                               .Delete("rate")
                               .ExecuteWithoutResultsAsync();
        }

        public static async Task DetachBlogForReadLater(IGraphClient client, Guid blogId)
        {
            await client.Cypher.Match("(a: Author)-[r:SAVED_TO_READ_LATER]->(blog: Blog)")
                               .Where((Models.Blog blog) => blog.BlogId == blogId)
                               .Delete("r")
                               .ExecuteWithoutResultsAsync();

            await client.Cypher.Match("(reader: Reader)-[r:SAVED_TO_READ_LATER]->(blog:Blog)")
                               .Where((Models.Blog blog) => blog.BlogId == blogId)
                               .Delete("r")
                               .ExecuteWithoutResultsAsync();
        }
    }
}
