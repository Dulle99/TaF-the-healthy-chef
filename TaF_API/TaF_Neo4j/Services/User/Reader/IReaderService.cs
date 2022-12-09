using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs.BlogDTO;
using TaF_Neo4j.DTOs.CookingRecepieDTO;
using TaF_Neo4j.DTOs.UserDTO;

namespace TaF_Neo4j.Services.User.Reader
{
    public interface IReaderService
    {
        Task<LoggedUserInformation> Login(UserLoginCredentials credentials);
        Task<bool> CreateReader(ReaderDTO readerDTO);
        Task<bool> AddBlogToReadLater(string username, Guid blogId);
        Task<bool> AddCookingRecepieToReadLater(string username, Guid cookingRecepieId);
        Task<bool> RemoveBlogToReadLater(string username, Guid blogId);
        Task<bool> RemoveCookingRecepieToReadLater(string username, Guid cookingRecepieId);
        Task<List<BlogPreviewDTO>> GetReadLaterBlogs(string username, int numberOfReadBlogsRecepiesToGet);
        Task<List<CookingRecepiePreviewDTO>> GetReadLaterCookingRecepies(string username, int numberOfReadLaterCookingRecepiesToGet);
        Task<ReaderPreviewDTO> GetReaderPreview(string username);
        byte[] GetReaderProfilePicture(string username);
        Task<bool> UpdateReader(string readerCurrentUsername, ReaderUpdateDTO readerDTO);
        Task<bool> UpdateProfilePicture(UserUpdateProfilePicture profilePictureDTO);
        Task<bool> CheckIfBlogSavedToReadLater(string username, Guid blogId);
        Task<bool> CheckIfCookingRecepieSavedToReadLater(string username, Guid recepieId);
    }
}
