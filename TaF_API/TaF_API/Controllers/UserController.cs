using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs;
using TaF_Neo4j.DTOs.UserDTO;
using TaF_Neo4j.Models;
using TaF_Neo4j.Services.User.Author;
using TaF_Neo4j.Services.User.Reader;
using TaF_Redis.Services.User;
using TaF_Redis.Types;

namespace TaF_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IAuthorService _authorService;
        private IReaderService _readerService;
        private IUserServiceRedis _userServiceRedis;

        public UserController(IGraphClient client, IConnectionMultiplexer redis)
        {
            _authorService = new AuthorService(client);
            _readerService = new ReaderService(client);
            _userServiceRedis = new UserServiceRedis(redis, client);
        }

        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromForm] UserLoginCredentials credentials)
        {
            /*READER*/
            var loggedUser = await this._readerService.Login(credentials);
            if (loggedUser.Username == credentials.Username)
            {
                var token = JwtToken.JWToken.GenerateToken(loggedUser.Username, false);
                loggedUser.Token = token;
                await _userServiceRedis.CacheUsersSavedCookingRecepies(TaF_Redis.Types.UserType.Reader, loggedUser.Username);
                await _userServiceRedis.CacheUsersSavedBlogs(TaF_Redis.Types.UserType.Reader, loggedUser.Username);
                return new JsonResult(loggedUser);
            }
            else if (loggedUser.LoginInformation == "InvalidPassword")
                return BadRequest();
            /*AUTHOR*/
            else
            {
                loggedUser = await this._authorService.Login(credentials);
                if (loggedUser.Username == credentials.Username)
                {
                    var token = JwtToken.JWToken.GenerateToken(loggedUser.Username, true);
                    loggedUser.Token = token;
                    await _userServiceRedis.CacheAuthorCookingRecepies(loggedUser.Username);
                    await _userServiceRedis.CacheAuthorBlogs(loggedUser.Username);
                    await _userServiceRedis.CacheUsersSavedCookingRecepies(TaF_Redis.Types.UserType.Author, loggedUser.Username);
                    await _userServiceRedis.CacheUsersSavedBlogs(TaF_Redis.Types.UserType.Author, loggedUser.Username);
                    return new JsonResult(loggedUser);
                }
                else if (loggedUser.LoginInformation == "InvalidPassword")
                    return BadRequest();
                else
                    return BadRequest(new String("User does not exist"));
            }
        }

        [HttpPost]
        [Route("RegisterAuthor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterAuthor([FromForm] AuthorDTO authorDTO)
        {
            if (await _authorService.CreateAuthor(authorDTO))
            {
                var token = JwtToken.JWToken.GenerateToken(authorDTO.Username, true);
                var createdUserInformation = new LoggedUserInformation
                {
                    Username = authorDTO.Username,
                    TypeOfUser = "Author",
                    Token = token,
                    LoginInformation = "Succes"
                };
                return new JsonResult(createdUserInformation);
            }
                
            else
                return Conflict();
        }
        [HttpPost]
        [Authorize(Roles = "Author")]
        [Route("AddAwards")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult AddAwards([FromBody] AwardDTO awardDTO)
        {
            if (awardDTO.AwardName!= "")
            {
                return Ok();
            }

            else
                return Conflict();
        }

        [HttpPost]
        [Route("RegisterReader")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterReader([FromForm] ReaderDTO readerDTO)
        {
            if (await _readerService.CreateReader(readerDTO))
            {
                var token = JwtToken.JWToken.GenerateToken(readerDTO.Username, false);
                var createdUserInformation = new LoggedUserInformation
                {
                    Username = readerDTO.Username,
                    TypeOfUser = "Reader",
                    Token = token,
                    LoginInformation = "Succes"
                };
                return new JsonResult(createdUserInformation);
            }
            else
                return Conflict();
        }

        [HttpPost]
        [Authorize(Roles = "Author, Reader")]
        [Route("AddBlogToReadLater/{blogId}/{username}/{typeOfUser}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddBlogToReadLater(Guid blogId, string username, string typeOfUser)
        {
            if (typeOfUser == "Author")
            {
                if (await this._authorService.AddBlogToReadLater(username, blogId))
                {
                    await this._userServiceRedis.CacheUsersNewSavedContent(UserType.Author, username, ContentType.savedBlog);
                    return Ok();
                }
                else
                    return BadRequest();
            }
            else
            {
                if (await this._readerService.AddBlogToReadLater(username, blogId))
                {
                    await this._userServiceRedis.CacheUsersNewSavedContent(UserType.Reader, username, ContentType.savedBlog);
                    return Ok();
                }
                else
                    return BadRequest();
            }
                
        }

        [HttpPost]
        [Authorize(Roles = "Author, Reader")]
        [Route("AddCookingRecepieToReadLater/{cookingRecepieId}/{username}/{typeOfUser}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCookingRecepieToReadLater(Guid cookingRecepieId, string username, string typeOfUser)
        {
            if (typeOfUser == "Author")
            {
                if (await this._authorService.AddCookingRecepieToReadLater(username, cookingRecepieId))
                {
                    await this._userServiceRedis.CacheUsersNewSavedContent(UserType.Author, username, ContentType.cookingRecepie);
                    return Ok();
                }
                else
                    return BadRequest();
            }
            else
            {
                if (await this._readerService.AddCookingRecepieToReadLater(username, cookingRecepieId))
                {
                    await this._userServiceRedis.CacheUsersNewSavedContent(UserType.Reader, username, ContentType.cookingRecepie);
                    return Ok();
                }
                else
                    return BadRequest();
            }
        }

        [HttpPut]
        [Authorize(Roles = "Author")]
        [Route("UpdateAuthor/{currentUsername}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAuthorInformation([FromForm] AuthorUpdateDTO authorUpdateDTO,string currentUsername)
        {
            if (await this._authorService.UpdateAuthor(currentUsername, authorUpdateDTO))
                return Ok();
            else
                return BadRequest();
        }

        [HttpPut]
        [Authorize(Roles = "Reader")]
        [Route("UpdateReader/{currentUsername}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateReaderInformation([FromForm] ReaderUpdateDTO readerUpdateDTO, string currentUsername)
        {
            if (await this._readerService.UpdateReader(currentUsername, readerUpdateDTO))
                return Ok();
            else
                return BadRequest();
        }

        [HttpPut]
        [Authorize(Roles = "Author, Reader")]
        [Route("UpdateProfilePicture")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] UserUpdateProfilePicture profilePictureDTO)
        {
            if(profilePictureDTO.TypeOfUser == "Author")
            {
                await this._authorService.UpdateProfilePicture(profilePictureDTO);
                return Ok();
            }
            else
            {
                await this._readerService.UpdateProfilePicture(profilePictureDTO);
                return Ok();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Author, Reader")]
        [Route("RemoveBlogForReadLater/{blogId}/{username}/{typeOfUser}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveBlogForReadLater(Guid blogId, string username, string typeOfUser)
        {
            if (typeOfUser == "Author")
            {
                if (await this._authorService.RemoveBlogToReadLater(username, blogId))
                {
                    await this._userServiceRedis.RemoveUserSavedContent(username, TaF_Redis.Types.ContentType.savedBlog, blogId);
                    return Ok();
                }
                else
                    return BadRequest();
            }
            else
            {
                if (await this._readerService.RemoveBlogToReadLater(username, blogId))
                {
                    await this._userServiceRedis.RemoveUserSavedContent(username, TaF_Redis.Types.ContentType.savedBlog, blogId);
                    return Ok();
                }
                else
                    return BadRequest();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Author, Reader")]
        [Route("RemoveCookingRecepieForReadLater/{cookingRecepieId}/{username}/{typeOfUser}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveCookingRecepieForReadLater(Guid cookingRecepieId, string username, string typeOfUser)
        {
            if (typeOfUser == "Author")
            {
                if (await this._authorService.RemoveCookingRecepieToReadLater(username, cookingRecepieId))
                {
                    await this._userServiceRedis.RemoveUserSavedContent(username, TaF_Redis.Types.ContentType.savedCookingRecepie, cookingRecepieId);
                    return Ok();
                }
                else
                    return BadRequest();
            }
            else
            {
                if (await this._readerService.RemoveCookingRecepieToReadLater(username, cookingRecepieId))
                {
                    await this._userServiceRedis.RemoveUserSavedContent(username, TaF_Redis.Types.ContentType.savedCookingRecepie, cookingRecepieId);
                    return Ok();
                }
                else
                    return BadRequest();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Author, Reader")]
        [Route("Logout/{username}/{typeOfUser}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task Logout(string username, string typeOfUser)
        {
            if (typeOfUser == "Author")
                await this._userServiceRedis.RemoveUserCachedData(username, TaF_Redis.Types.UserType.Author);
            else
                await this._userServiceRedis.RemoveUserCachedData(username, TaF_Redis.Types.UserType.Reader);
        }


        [HttpGet]
        [Authorize(Roles = "Author, Reader")]
        [Route("GetUserPreview/{username}/{typeOfUser}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserPreview(string username, string typeOfUser)
        {
            if (typeOfUser == "Author")
                return new JsonResult(await this._authorService.GetAuthorPreview(username));
            else
                return new JsonResult(await this._readerService.GetReaderPreview(username));
        }

        [HttpGet]
        [Authorize(Roles = "Author, Reader")]
        [Route("GetSavedBlogs/{username}/{typeOfUser}/{numberOfReadBlogsRecepiesToGet}")]
        public async Task<IActionResult> GetReadLaterBlogs(string username, string typeOfUser, int numberOfReadBlogsRecepiesToGet)
        {
            if (numberOfReadBlogsRecepiesToGet <= 5)
            {
                return new JsonResult(await this._userServiceRedis.GetCached_ReadLaterBlogs(username));
            }
            else
            {
                if (typeOfUser == "Author")
                {
                    return new JsonResult(await this._authorService.GetReadLaterBlogs(username, numberOfReadBlogsRecepiesToGet));
                }
                else
                {
                    return new JsonResult(await this._readerService.GetReadLaterBlogs(username, numberOfReadBlogsRecepiesToGet));
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Author, Reader")]
        [Route("GetSavedCookingRecepie/{username}/{typeOfUser}/{numberOfReadLaterCookingRecepiesToGet}")]
        public async Task<IActionResult> GetReadLaterCookingRecepie(string username, string typeOfUser, int numberOfReadLaterCookingRecepiesToGet)
        {
            if ( numberOfReadLaterCookingRecepiesToGet <=5)
            {
                return new JsonResult(await this._userServiceRedis.GetCached_ReadLaterCookingRecepies(username));
            }
            else
            {
                if (typeOfUser == "Author")
                {
                    return new JsonResult(await this._authorService.GetReadLaterCookingRecepies(username, numberOfReadLaterCookingRecepiesToGet));
                }
                else
                {
                    return new JsonResult(await this._readerService.GetReadLaterCookingRecepies(username, numberOfReadLaterCookingRecepiesToGet));
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Author, Reader")]
        [Route("CheckIfCookingRecepieIsSavedToReadLater/{username}/{typeOfUser}/{cookingRecepieId}")]
        public async Task<ActionResult<bool>> CheckIfCookingRecepieSavedToReadLater(string username, string typeOfUser, Guid cookingRecepieId)
        {
            if(typeOfUser == "Author")
            {
               return await _authorService.CheckIfCookingRecepieSavedToReadLater(username, cookingRecepieId);
            }
            else
            {
                return await _readerService.CheckIfCookingRecepieSavedToReadLater(username, cookingRecepieId);
            }
            
        }

        [HttpGet]
        [Authorize(Roles = "Author, Reader")]
        [Route("CheckBlogIsSavedToReadLater/{username}/{typeOfUser}/{cookingRecepieId}")]
        public async Task<ActionResult<bool>> CheckIfBlogSavedToReadLater(string username, string typeOfUser, Guid cookingRecepieId)
        {
            if (typeOfUser == "Author")
            {
                return await _authorService.CheckIfBlogSavedToReadLater(username, cookingRecepieId);
            }
            else
            {
                return await _readerService.CheckIfBlogSavedToReadLater(username, cookingRecepieId);
            }

        }

        [HttpGet]
        [Authorize(Roles = "Author, Reader")]
        [Route("GetUserProfilePicture/{username}/{typeOfUser}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserProfilePicture(string username, string typeOfUser)
        {
            if (typeOfUser == "Author")
                return new JsonResult(this._authorService.GetAuthorProfilePicture(username));
            else
                return new JsonResult(this._readerService.GetReaderProfilePicture(username));
        }

    }
}
