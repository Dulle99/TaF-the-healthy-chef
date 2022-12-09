using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs;
using TaF_Neo4j.DTOs.BlogDTO;
using TaF_Neo4j.DTOs.Comment;
using TaF_Neo4j.DTOs.Rate;

namespace TaF_Neo4j.Services.Blog
{
    public interface IBlogService
    {
        Task<bool> CreateBlog(string authorUsername, BasicBlogDTO blogDTO);
        Task<List<BlogPreviewDTO>> GetPreviewBlogsByDatePublication(int pageNumber, bool latestFirst);
        Task<List<BlogPreviewDTO>> GetPreviewBlogsByPopularity(int pageNumber);
        Task<List<BlogPreviewDTO>> GetRecommendedBlogs();
        Task<List<BlogPreviewDTO>> GetPreviewBlogsByAuthor(string authorUsername, int numberOfBlogsToGet);
        Task<BlogDTO> GetBlog(Guid blogId);
        Task<bool> UpdateBlog(Guid blogId, BasicBlogDTO blogDTO);
        Task<bool> DeleteBlog(Guid blogId);
        Task<bool> AddCommentToTheBlog(BasicCommentDTO commentDTO);
        Task<float> AddRateToTheBlog(BasicRateDTO rateDTO);
        Task<List<CommentPreviewDTO>> GetCommentsByBlog(Guid blogId, int numberOfCommentsToGet);
        //delete comment
        //update rate
    }
}
