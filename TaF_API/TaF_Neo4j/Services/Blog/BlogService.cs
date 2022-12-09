using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using TaF_Neo4j.DTOs;
using TaF_Neo4j.DTOs.BlogDTO;
using TaF_Neo4j.DTOs.Comment;
using TaF_Neo4j.DTOs.Rate;
using TaF_Neo4j.Services.User.Author;
using TaF_Neo4j.Services.User.CommonUserMethods;
using TaF_Neo4j.Services.User.Reader;

namespace TaF_Neo4j.Services.Blog
{
    public class BlogService : IBlogService
    {
        private IGraphClient _client;
        private IAuthorService _authorService;
        private IReaderService _readerService;
        
        public BlogService(IGraphClient client)
        {
            this._client = client;
            this._authorService = new AuthorService(client);
            this._readerService = new ReaderService(client);
        }

        public async Task<bool> CreateBlog(string authorUsername, BasicBlogDTO blogDTO)
        {
            try
            {
                var _blog = new Models.Blog
                {
                    BlogTitle = blogDTO.BlogTitle,
                    BlogContent = blogDTO.BlogContent,
                    PublicationDate = DateTime.Now,
                    ReadingTime = blogDTO.ReadingTime
                };

                string filePath = MutalMethodsForContent.MutalMethodsForContent.WriteImageToFolder(blogDTO.BlogPicture, _blog.BlogId, false);
                _blog.BlogPicturePath = filePath;
                await this._client.Cypher.Create("(b: Blog $_blog)")
                                         .WithParam("_blog", _blog)
                                         .ExecuteWithoutResultsAsync();

                await this._client.Cypher.Match("(author: Author)", "(blog: Blog)")
                                         .Where((Models.Author author) => author.Username == authorUsername)
                                         .AndWhere((Models.Blog blog) => blog.BlogId == _blog.BlogId)
                                         .Create("(author)-[:HAS_PUBLISHED]->(blog)")
                                         .ExecuteWithoutResultsAsync();
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }


        public async Task<List<BlogPreviewDTO>> GetPreviewBlogsByDatePublication(int pageNumber, bool latestFirst)
        {
            string orderByCriteriaQuery;
            if (latestFirst)
                orderByCriteriaQuery = "blog.PublicationDate DESC"; 
            else
                orderByCriteriaQuery = "blog.PublicationDate";

            List<Guid> listOfFilteredBlogsGuidIds = await BlogServiceAuxiliaryMethods.FilterBlogsByCriteria(this._client, orderByCriteriaQuery, pageNumber);

            List<BlogPreviewDTO> filteredBlogs = new List<BlogPreviewDTO>();

            foreach( var blogId in listOfFilteredBlogsGuidIds)
            {
                
                filteredBlogs.Add(await BlogServiceAuxiliaryMethods.GetBlogPreview(this._client, blogId));
            }
            return filteredBlogs;
                                              
        }

        public async Task<List<BlogPreviewDTO>> GetPreviewBlogsByPopularity(int pageNumber)
        {
            string orderByCriteriaQuery = "blog.AverageRate";
            List<Guid> listOfFilteredBlogsGuidIds = await BlogServiceAuxiliaryMethods.FilterBlogsByCriteria(this._client, orderByCriteriaQuery,pageNumber);

            List<BlogPreviewDTO> filteredBlogs = new List<BlogPreviewDTO>();

            foreach (var blogId in listOfFilteredBlogsGuidIds)
            {

                filteredBlogs.Add(await BlogServiceAuxiliaryMethods.GetBlogPreview(this._client, blogId));
            }
            return filteredBlogs.OrderByDescending(blog => blog.AverageRate).ToList();
        }

        public async Task<List<BlogPreviewDTO>> GetRecommendedBlogs()
        {
            var latestBlogs = await this.GetPreviewBlogsByDatePublication(3, true);
            var sortedBlogsByAverageRate = latestBlogs.OrderByDescending(blog => blog.AverageRate).ToList();
            return sortedBlogsByAverageRate.Take(3).ToList();
        }

        public async Task<List<BlogPreviewDTO>> GetPreviewBlogsByAuthor(string authorUsername, int numberOfBlogsToGet)
        {
            return (await this._client.Cypher.Match("(author:Author)-[:HAS_PUBLISHED]->(blog: Blog)")
                                             .Where((Models.Author author) => author.Username == authorUsername)
                                             .Return((author, blog) => new
                                             {
                                                 Author = author.As<Models.Author>(),
                                                 Blog = blog.As<Models.Blog>()
                                             }).Limit(numberOfBlogsToGet).ResultsAsync)
                                             .Select(obj => new BlogPreviewDTO(obj.Author.Username, CommonUserMethods.ReadImageFromFile(obj.Author.Username, true),
                                                                               obj.Blog.BlogId, obj.Blog.BlogTitle,
                                                                               obj.Blog.BlogContent, obj.Blog.PublicationDate,
                                                                               obj.Blog.ReadingTime, obj.Blog.AverageRate,
                                                                               MutalMethodsForContent.
                                                                               MutalMethodsForContent.
                                                                               ReadImageFromFile(obj.Blog.BlogId,false))).ToList();
        }

        public async Task<BlogDTO> GetBlog(Guid blogId)
        {
            return (await this._client.Cypher.Match("(author:Author)-[:HAS_PUBLISHED]->(blog: Blog)")
                                            .Where((Models.Blog blog) => blog.BlogId == blogId)
                                            .Return((author, blog) => new
                                            {
                                                Author = author.As<Models.Author>(),
                                                Blog = blog.As<Models.Blog>()
                                            }).ResultsAsync)
                                            .Select(obj => new BlogDTO{
                                                AuthorUsername = obj.Author.Username,
                                                AuthorProfilePicture = CommonUserMethods.ReadImageFromFile(obj.Author.Username, true),
                                                BlogId = obj.Blog.BlogId,
                                                BlogTitle = obj.Blog.BlogTitle,
                                                Content = obj.Blog.BlogContent,
                                                PublicationDate = DateTime.Now,
                                                ReadingTime = obj.Blog.ReadingTime,
                                                BlogPicture = MutalMethodsForContent.
                                                              MutalMethodsForContent.
                                                              ReadImageFromFile(obj.Blog.BlogId, false),
                                                AverageRate = obj.Blog.AverageRate
                                            }).FirstOrDefault();
        }

        public async Task<bool> UpdateBlog(Guid blogId, BasicBlogDTO blogDTO)
        {
            var blog = this.GetBlog(blogId);
            if (blog != null)
            {
                if (blogDTO.BlogPicture != null)
                {
                    MutalMethodsForContent.MutalMethodsForContent.DeleteImageFromFolder(blogId, false);
                    var newImageFilePath = MutalMethodsForContent.MutalMethodsForContent.WriteImageToFolder(blogDTO.BlogPicture, blogId, false);

                    await this._client.Cypher.Match("(blog: Blog)")
                                             .Where((Models.Blog blog) => blog.BlogId == blogId)
                                             .Set("blog.BlogTitle = $newTitle")
                                             .WithParam("newTitle", blogDTO.BlogTitle)
                                             .Set("blog.BlogContent = $newBlogContent")
                                             .WithParam("newBlogContent", blogDTO.BlogContent)
                                             .Set("blog.PublicationDate = $newPublicationDate")
                                             .WithParam("newPublicationDate", DateTime.UtcNow)
                                             .Set("blog.ReadingTime = $newReadingTime")
                                             .WithParam("newReadingTime", blogDTO.ReadingTime)
                                             .Set("blog.BlogPicturePath = $newImageFilePath")
                                             .WithParam("newImageFilePath", newImageFilePath).ExecuteWithoutResultsAsync();
                }
                else
                {
                    await this._client.Cypher.Match("(blog: Blog)")
                                             .Where((Models.Blog blog) => blog.BlogId == blogId)
                                             .Set("blog.BlogTitle = $newTitle")
                                             .WithParam("newTitle", blogDTO.BlogTitle)
                                             .Set("blog.BlogContent = $newBlogContent")
                                             .WithParam("newBlogContent", blogDTO.BlogContent)
                                             .Set("blog.PublicationDate = $newPublicationDate")
                                             .WithParam("newPublicationDate", DateTime.UtcNow)
                                             .Set("blog.ReadingTime = $newReadingTime")
                                             .WithParam("newReadingTime", blogDTO.ReadingTime)
                                             .ExecuteWithoutResultsAsync();
                }
                return true;
            }
            else
                return false;
        }

        public async Task<bool> DeleteBlog(Guid blogId)
        {
            try
            {
                await BlogServiceAuxiliaryMethods.DetachAndDeleteCommentsOfBlog(this._client, blogId);
                await BlogServiceAuxiliaryMethods.DetachAndDeleteRatesOfBlog(this._client, blogId);
                await BlogServiceAuxiliaryMethods.DetachBlogForReadLater(this._client, blogId);

                MutalMethodsForContent.MutalMethodsForContent.DeleteImageFromFolder(blogId, false);

                await this._client.Cypher.Match("(author: Author)-[r:HAS_PUBLISHED]->(b: Blog)")
                                         .Where((Models.Blog b) => b.BlogId == blogId)
                                         .Delete("r")
                                         .Delete("b")
                                         .ExecuteWithoutResultsAsync();
                return true;
            }
            catch(Exception ex) 
            {
                return false;
            }

        }

        public async Task<bool> AddCommentToTheBlog(BasicCommentDTO commentDTO)
        {
            try
            {
                var newComment = new Models.Comment()
                {
                    CommentContent = commentDTO.CommentContent
                };

                if (commentDTO.TypeOfUser == "Author")
                {
                    var author = this._authorService.GetAuthorPreview(commentDTO.Username);
                    if (author.Result == null)
                        return false;
                    else
                    {
                        await this._client.Cypher.Match("(author: Author)", "(blog: Blog)")
                                                 .Where((Models.Author author) => author.Username == commentDTO.Username)
                                                 .AndWhere((Models.Blog blog) => blog.BlogId == commentDTO.ContentId)
                                                 .Create("(comment: Comment $newComment)-[:WRITES]->(author)")
                                                 .WithParam("newComment", newComment)
                                                 .Create("(blog)-[:HAS_COMMENTS]->(comment)")
                                                 .ExecuteWithoutResultsAsync();
                        return true;
                    }
                }
                else
                {
                    var reader = this._readerService.GetReaderPreview(commentDTO.Username);
                    if (reader.Result == null)
                        return false;
                    else
                    {
                        await this._client.Cypher.Match("(reader: Reader)", "(blog: Blog)")
                                                 .Where((Models.Reader reader) => reader.Username == commentDTO.Username)
                                                 .AndWhere((Models.Blog blog) => blog.BlogId == commentDTO.ContentId)
                                                 .Create("(comment: Comment $newComment)-[:WRITES]->(reader)")
                                                 .WithParam("newComment", newComment)
                                                 .Create("(blog)-[:HAS_COMMENTS]->(comment)")
                                                 .ExecuteWithoutResultsAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<float> AddRateToTheBlog(BasicRateDTO rateDTO)
        {
            try
            {
                var newRate = new Models.Rate()
                {
                    RateValue = rateDTO.Value
                };

                if (rateDTO.TypeOfUser == "Author")
                {
                    var author = this._authorService.GetAuthorPreview(rateDTO.Username);
                    if (author.Result == null)
                        return 0;
                    else
                    {
                        var rate = (await this._client.Cypher.Match("(blog: Blog) -[:HAS_RATES]->(rate: Rate)-[:RATES]->(author: Author)")
                                                            .Where((Models.Author author) => author.Username == rateDTO.Username)
                                                            .AndWhere((Models.Blog blog) => blog.BlogId == rateDTO.ContentId)
                                                            .Return((rate) => new
                                                            {
                                                                Rate = rate.As<Models.Rate>()
                                                            }).ResultsAsync)
                                                            .Select(obj => new Models.Rate
                                                            {
                                                                RateId = obj.Rate.RateId,
                                                                RateValue = obj.Rate.RateValue
                                                            }).FirstOrDefault();
                        if (rate != null)
                        {
                            await MutalMethodsForContent.MutalMethodsForContent.UpdateUserRate(this._client, rate.RateId, rateDTO.Value);
                        }
                        else
                        {
                            await this._client.Cypher.Match("(author: Author)", "(blog: Blog)")
                                                     .Where((Models.Author author) => author.Username == rateDTO.Username)
                                                     .AndWhere((Models.Blog blog) => blog.BlogId == rateDTO.ContentId)
                                                     .Create("(rate: Rate $newRate)-[:RATES]->(author)")
                                                     .WithParam("newRate", newRate)
                                                     .Create("(blog)-[:HAS_RATES]->(rate)")
                                                     .ExecuteWithoutResultsAsync();
                        }
                        return await BlogServiceAuxiliaryMethods.UpdateAverageRateOfBlog(this._client, rateDTO.ContentId);
                        
                    }
                }
                else
                {
                    var reader = this._readerService.GetReaderPreview(rateDTO.Username);
                    if (reader.Result == null)
                        return 0;
                    else
                    {
                        var rate = (await this._client.Cypher.Match("(blog: Blog) -[:HAS_RATES]->(rate: Rate)-[:RATES]->(reader: Reader)")
                                                           .Where((Models.Reader reader) => reader.Username == rateDTO.Username)
                                                           .AndWhere((Models.Blog blog) => blog.BlogId == rateDTO.ContentId)
                                                           .Return((rate) => new
                                                           {
                                                               Rate = rate.As<Models.Rate>()
                                                           }).ResultsAsync)
                                                           .Select(obj => new Models.Rate
                                                           {
                                                               RateId = obj.Rate.RateId,
                                                               RateValue = obj.Rate.RateValue
                                                           }).FirstOrDefault();
                        if (rate != null)
                        {
                            await MutalMethodsForContent.MutalMethodsForContent.UpdateUserRate(this._client, rate.RateId, rateDTO.Value);
                        }
                        else
                        {
                            await this._client.Cypher.Match("(reader: Reader)", "(blog: Blog)")
                                                      .Where((Models.Reader reader) => reader.Username == rateDTO.Username)
                                                      .AndWhere((Models.Blog blog) => blog.BlogId == rateDTO.ContentId)
                                                      .Create("(rate: Rate $newRate)-[:RATES]->(reader)")
                                                      .WithParam("newRate", newRate)
                                                      .Create("(blog)-[:HAS_RATES]->(rate)")
                                                      .ExecuteWithoutResultsAsync();
                        }
                        return await BlogServiceAuxiliaryMethods.UpdateAverageRateOfBlog(this._client, rateDTO.ContentId);
                       
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<List<CommentPreviewDTO>> GetCommentsByBlog(Guid blogId, int numberOfCommentsToGet)
        {
            int limit = 10;
            if (numberOfCommentsToGet != 0)
                limit = numberOfCommentsToGet;
            string criteria = "comment.PublicationDate DESC";

            var commentsByAuthors = (await this._client.Cypher.Match("(blog: Blog)-[:HAS_COMMENTS]->(comment: Comment)-[:WRITES]->(author: Author)")
                                              .Where((Models.Blog blog) => blog.BlogId == blogId)
                                              .Return((blog, comment, author) => new
                                              {
                                                  Blog = blog.As<Models.Blog>(),
                                                  Comment = comment.As<Models.Comment>(),
                                                  Author = author.As<Models.Author>()
                                              })
                                              .OrderBy(criteria).Limit(limit / 2).ResultsAsync)
                                              .Select((obj) => new CommentPreviewDTO
                                              {
                                                  AuthorOfComment = obj.Author.Username,
                                                  AuthorOfCommentProfilePicture = CommonUserMethods.ReadImageFromFile(obj.Author.Username, true),
                                                  CommentContent = obj.Comment.CommentContent,
                                                  PublicationDate = obj.Comment.PublicationDate,
                                                  TypeOfUser = "Author"

                                              }).ToList();

            var commentsByReaders = (await this._client.Cypher.Match("(blog: Blog)-[:HAS_COMMENTS]->(comment: Comment)-[:WRITES]->(reader: Reader)")
                                              .Where((Models.Blog blog) => blog.BlogId == blogId)
                                              .Return((blog, comment, reader) => new
                                              {
                                                  Blog = blog.As<Models.Blog>(),
                                                  Comment = comment.As<Models.Comment>(),
                                                  Reader = reader.As<Models.Reader>()
                                              })
                                              .OrderBy(criteria).Limit(limit / 2).ResultsAsync)
                                              .Select((obj) => new CommentPreviewDTO
                                              {
                                                  AuthorOfComment = obj.Reader.Username,
                                                  AuthorOfCommentProfilePicture = CommonUserMethods.ReadImageFromFile(obj.Reader.Username, false),
                                                  CommentContent = obj.Comment.CommentContent,
                                                  PublicationDate = obj.Comment.PublicationDate,
                                                  TypeOfUser = "Reader"

                                              }).ToList();

            List<CommentPreviewDTO> comments = new List<CommentPreviewDTO>();

            foreach (var commentPreview in commentsByAuthors)
                comments.Add(commentPreview);

            foreach (var commentPreview in commentsByReaders)
                comments.Add(commentPreview);

            return comments.OrderByDescending(comment => comment.PublicationDate).ToList();
        }
    }
}
