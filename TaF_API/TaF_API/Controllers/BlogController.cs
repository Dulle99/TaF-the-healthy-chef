using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaF_Neo4j.Services.Blog;
using TaF_Neo4j.DTOs;
using TaF_Neo4j.DTOs.Comment;
using TaF_Neo4j.DTOs.Rate;
using Microsoft.AspNetCore.Authorization;

namespace TaF_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private IBlogService _blogService;
        public BlogController(IGraphClient client)
        {
            _blogService = new BlogService(client);
        }

        [HttpPost]
        [Authorize(Roles = "Author")]
        [Route("CreateBlog/{authorUsername}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBlog([FromForm] BasicBlogDTO blogDTO, string authorUsername)
        {
            if (await this._blogService.CreateBlog(authorUsername, blogDTO))
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost]
        [Authorize]
        [Route("AddCommentToTheBlog")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCommentToTheBlog([FromBody] BasicCommentDTO commentDTO)
        {
            if (await this._blogService.AddCommentToTheBlog(commentDTO))
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost]
        [Authorize]
        [Route("AddRateToTheBlog")]
        public async Task<IActionResult> AddRateToTheBlog([FromBody] BasicRateDTO rateDTO)
        {
            return new JsonResult(await this._blogService.AddRateToTheBlog(rateDTO));
        }

        [HttpGet]
        [Route("GetBlogsByPublicationDate/{pageNumber}/{latestFirst}")]
        public async Task<IActionResult> GetPreviewBlogsByDatePublication(int pageNumber, bool latestFirst)
        {
            return new JsonResult (await this._blogService.GetPreviewBlogsByDatePublication(pageNumber, latestFirst));
        }

        [HttpGet]
        [Route("GetBlogsByPopularity/{pageNumber}")]
        public async Task<IActionResult> GetPreviewBlogsByPopularity(int pageNumber)
        {
            return new JsonResult(await this._blogService.GetPreviewBlogsByPopularity(pageNumber));
        }

        [HttpGet]
        [Route("GetRecommendedBlogs")]
        public async Task<IActionResult> GetRecommendedPreviewBlogs()
        {
            return new JsonResult(await this._blogService.GetRecommendedBlogs());
        }

        [HttpGet]
        [Authorize]
        [Route("GetBlogsByAuthor/{authorUsername}/{numberOfBlogsToGet}")]
        public async Task<IActionResult> GetPreviewBlogsByAuthor(string authorUsername, int numberOfBlogsToGet)
        {
            return new JsonResult(await this._blogService.GetPreviewBlogsByAuthor(authorUsername, numberOfBlogsToGet));
        }

        [HttpGet]
        [Route("GetBlog/{blogId}")]
        public async Task<IActionResult> GetBlog(Guid blogId)
        {
            return new JsonResult(await this._blogService.GetBlog(blogId));
        }

        [HttpGet]
        [Route("GetCommentsOfBlog/{blogId}/{numberOfCommentsToGet}")]
        public async Task<IActionResult> GetCommentsOfBlog(Guid blogId, int numberOfCommentsToGet)
        {
            return new JsonResult(await this._blogService.GetCommentsByBlog(blogId, numberOfCommentsToGet));
        }

        [HttpPut]
        [Authorize(Roles = "Author")]
        [Route("UpdateBlog/{blogId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateBlog([FromForm] BasicBlogDTO blog, Guid blogId)
        {
            if (await this._blogService.UpdateBlog(blogId, blog))
                return Ok();
            else
                return BadRequest();
        }

        [HttpDelete]
        [Authorize(Roles = "Author")]
        [Route("Delete/{blogId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteBlog(Guid blogId)
        {
            if (await this._blogService.DeleteBlog(blogId))
                return Ok();
            else
                return BadRequest();
        }
    }
}
