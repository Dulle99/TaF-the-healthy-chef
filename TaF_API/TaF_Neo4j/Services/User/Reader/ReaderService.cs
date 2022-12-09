using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs.BlogDTO;
using TaF_Neo4j.DTOs.CookingRecepieDTO;
using TaF_Neo4j.DTOs.UserDTO;
using TaF_Neo4j.Services.CookingRecepie;
using Microsoft.AspNetCore.Http;

namespace TaF_Neo4j.Services.User.Reader
{
    public class ReaderService : IReaderService
    {
        private IGraphClient _client;
        public ReaderService(IGraphClient client)
        {
            this._client = client;
        }

        public async Task<LoggedUserInformation> Login(UserLoginCredentials credentials)
        {
            Models.Reader reader = (await this._client.Cypher.Match("(reader: Reader)")
                                            .Where((Models.Reader reader) => reader.Username == credentials.Username)
                                            .Return((reader) => new { Reader = reader.As<Models.Reader>() })
                                            .ResultsAsync)
                                            .Select(obj => new Models.Reader
                                            {
                                                Username = obj.Reader.Username,
                                                Password = obj.Reader.Password,
                                                PasswordSalt = obj.Reader.PasswordSalt
                                            }).FirstOrDefault();
            if (reader == null)
                return new LoggedUserInformation { TypeOfUser = "UserIsNotReader" };

            if (reader.Username == credentials.Username)
            {
                var passwordTest = CommonUserMethods.CommonUserMethods.EncryptPassword(credentials.Password, reader.PasswordSalt);
                if (passwordTest.SequenceEqual(reader.Password))
                {
                    return new LoggedUserInformation
                    {
                        Username = reader.Username,
                        TypeOfUser = "Reader",
                        LoginInformation = "Succes"
                    };
                }
                else
                {
                    return new LoggedUserInformation
                    {
                        LoginInformation = "InvalidPassword"
                    };
                }
            }
            else
            {
                return new LoggedUserInformation
                {
                    LoginInformation = "UserIsNotReader"
                };
            }
        }

        public async Task<bool> CreateReader(ReaderDTO readerDTO)
        {
            if (await CommonUserMethods.CommonUserMethods.CheckIfUserExists(this._client, readerDTO.Username))
                return false;
            else
            {
                Models.Reader reader = new Models.Reader();
                reader.Username = readerDTO.Username;
                reader.Password = CommonUserMethods.CommonUserMethods.EncryptPassword(readerDTO.Password, reader.PasswordSalt);
                reader.Name = readerDTO.Name;
                reader.Lastname = readerDTO.Lastname;
                reader.NameOfSchool = readerDTO.NameOfSchool;
                reader.TypeOfStudent = readerDTO.TypeOfStudent;

                await this._client.Cypher.Create("(reader: Reader $newReader)")
                                          .WithParam("newReader", reader)
                                          .ExecuteWithoutResultsAsync();

                return true;
            }
        }

        public async Task<bool> AddBlogToReadLater(string username, Guid blogId)
        {
            try
            {
                if (await CheckIfBlogSavedToReadLater(username, blogId))
                    return true;

                await this._client.Cypher.Match("(r: Reader)", "(b: Blog)")
                                         .Where((Models.Reader r) => r.Username == username)
                                         .AndWhere((Models.Blog b) => b.BlogId == blogId)
                                         .Create("(r)-[:SAVED_TO_READ_LATER]->(b)")
                                         .ExecuteWithoutResultsAsync();

                await this._client.Cypher.Match("(reader: Reader)-[r:SAVED_TO_READ_LATER]->(b: Blog)")
                                         .Where((Models.Reader reader) => reader.Username == username)
                                         .AndWhere((Models.Blog b) => b.BlogId == blogId)
                                         .Set("r.SavedTime = $savedTime")
                                         .WithParam("savedTime", DateTime.Now)
                                         .ExecuteWithoutResultsAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> AddCookingRecepieToReadLater(string username, Guid cookingRecepieId)
        {
            try
            {
                if (await CheckIfCookingRecepieSavedToReadLater(username, cookingRecepieId))
                    return true;

                await this._client.Cypher.Match("(r: Reader)", "(c: CookingRecepie)")
                                         .Where((Models.Reader r) => r.Username == username)
                                         .AndWhere((Models.CookingRecepie c) => c.CookingRecepieId == cookingRecepieId)
                                         .Create("(r)-[:SAVED_TO_READ_LATER]->(c)")
                                         .ExecuteWithoutResultsAsync();

                await this._client.Cypher.Match("(reader: Reader)-[r:SAVED_TO_READ_LATER]->(c: CookingRecepie)")
                                         .Where((Models.Reader reader) => reader.Username == username)
                                         .AndWhere((Models.CookingRecepie c) => c.CookingRecepieId == cookingRecepieId)
                                         .Set("r.SavedTime = $savedTime")
                                         .WithParam("savedTime", DateTime.Now)
                                         .ExecuteWithoutResultsAsync();
            }
            catch (Exception ex)
            { return false; }
            return true;
        }

        public async Task<bool> RemoveBlogToReadLater(string username, Guid blogId)
        {
            try
            {
                await _client.Cypher.OptionalMatch("(reader)-[r:SAVED_TO_READ_LATER]->(b)")
                                    .Where((Models.Reader reader) => reader.Username == username)
                                    .AndWhere((Models.Blog b) => b.BlogId == blogId)
                                    .Delete("r")
                                    .ExecuteWithoutResultsAsync();
            }
            catch (Exception ex)
            { return false; }
            return true;
        }
        public async Task<bool> RemoveCookingRecepieToReadLater(string username, Guid cookingRecepieId)
        {
            try
            {
                await _client.Cypher.OptionalMatch("(reader)-[r:SAVED_TO_READ_LATER]->(c)")
                                    .Where((Models.Reader reader) => reader.Username == username)
                                    .AndWhere((Models.CookingRecepie c) => c.CookingRecepieId == cookingRecepieId)
                                    .Delete("r")
                                    .ExecuteWithoutResultsAsync();
            }
            catch (Exception ex)
            { return false; }
            return true;
        }

        public async Task<List<BlogPreviewDTO>> GetReadLaterBlogs(string username, int numberOfReadLaterBlogsToGet)
        {
            return (await this._client.Cypher.Match("(reader: Reader)-[r:SAVED_TO_READ_LATER]->(blog: Blog)")
                                                  .Where((Models.Reader reader) => reader.Username == username)
                                                  .Match("(authorOfBlog: Author)-[:HAS_PUBLISHED]->(blog)")
                                                  .Return((blog, authorOfBlog) => new 
                                                  { 
                                                      Blog = blog.As<Models.Blog>(),
                                                      AuthorOfBlog = authorOfBlog.As<Models.Author>()
                                                  })
                                                  .OrderBy("r.SavedTime DESC").Limit(numberOfReadLaterBlogsToGet)
                                                  .ResultsAsync)
                                                  .Select(obj => new BlogPreviewDTO(obj.AuthorOfBlog.Username, CommonUserMethods.CommonUserMethods.ReadImageFromFile(obj.AuthorOfBlog.Username, true),
                                                                                    obj.Blog.BlogId, obj.Blog.BlogTitle, obj.Blog.BlogContent,
                                                                                    obj.Blog.PublicationDate, obj.Blog.ReadingTime,
                                                                                    obj.Blog.AverageRate, MutalMethodsForContent.
                                                                                                          MutalMethodsForContent.
                                                                                                          ReadImageFromFile(obj.Blog.BlogId, false)))
                                                  .ToList();
        }

        public async Task<List<CookingRecepiePreviewDTO>> GetReadLaterCookingRecepies(string username, int numberOfReadLaterCookingRecepiesToGet)
        {
            return (await this._client.Cypher.Match("(reader: Reader)-[r:SAVED_TO_READ_LATER]->(cookingRecepie : CookingRecepie)")
                                                  .Where((Models.Reader reader) => reader.Username == username)
                                                  .Match("(authorOfRecepie: Author)-[:HAS_PUBLISHED]->(cookingRecepie)")
                                                  .Return((cookingRecepie, authorOfRecepie) => new 
                                                  { 
                                                      CookingRecepie = cookingRecepie.As<Models.CookingRecepie>(),
                                                      AuthorOfRecepie = authorOfRecepie.As<Models.Author>()
                                                  })
                                                  .OrderBy("r.SavedTime DESC").Limit(numberOfReadLaterCookingRecepiesToGet)
                                                  .ResultsAsync)
                                                  .Select(obj => new CookingRecepiePreviewDTO
                                                  {
                                                      AuthorUsername = obj.AuthorOfRecepie.Username,
                                                      AuthorProfilePicture = CommonUserMethods.CommonUserMethods.ReadImageFromFile(obj.AuthorOfRecepie.Username, true),
                                                      CookingRecepieId = obj.CookingRecepie.CookingRecepieId,
                                                      CookingRecepieTitle = obj.CookingRecepie.CookingRecepieTitle,
                                                      Description = obj.CookingRecepie.Description,
                                                      PreparationTime = obj.CookingRecepie.PreparationTime,
                                                      PublicationDate = obj.CookingRecepie.PublicationDate,
                                                      TypeOfMeal = obj.CookingRecepie.TypeOfMeal,
                                                      CookingRecepiePicture = MutalMethodsForContent
                                                                        .MutalMethodsForContent.ReadImageFromFile(obj.CookingRecepie.CookingRecepieId, true),
                                                      AverageRate = obj.CookingRecepie.AverageRate
                                                  })
                                                  .ToList();
        }



        public async Task<ReaderPreviewDTO> GetReaderPreview(string username)
        {
            return (await this._client.Cypher.Match("(reader: Reader)")
                                             .Where((Models.Reader reader) => reader.Username == username)
                                             .Return((reader) => new
                                             {
                                                 Reader = reader.As<Models.Reader>()
                                             })
                                             .ResultsAsync)
                                             .Select(obj => new ReaderPreviewDTO
                                             {
                                                 Username = obj.Reader.Username,
                                                 Name = obj.Reader.Name,
                                                 Lastname = obj.Reader.Lastname,
                                                 TypeOfStudent = obj.Reader.TypeOfStudent,
                                                 NameOfSchool = obj.Reader.NameOfSchool,
                                                 ProfilePicture = CommonUserMethods.CommonUserMethods.ReadImageFromFile(obj.Reader.Username, false)
                                             }).FirstOrDefault();
        }
        public async Task<bool> UpdateReader(string readerCurrentUsername, ReaderUpdateDTO readerDTO)
        {
            Models.Reader reader = (await this._client.Cypher.Match("(reader: Reader)")
                                            .Where((Models.Reader reader) => reader.Username == readerCurrentUsername)
                                            .Return((reader) => new { Reader = reader.As<Models.Reader>() })
                                            .ResultsAsync)
                                            .Select(obj => new Models.Reader
                                            {
                                                Username = obj.Reader.Username,
                                                Password = obj.Reader.Password,
                                                PasswordSalt = obj.Reader.PasswordSalt
                                            }).FirstOrDefault();
            if (reader == null)
                return false;

            if (reader.Username == readerCurrentUsername)
            {
                var passwordTest = CommonUserMethods.CommonUserMethods.EncryptPassword(readerDTO.CurrentPassword, reader.PasswordSalt);
                if (passwordTest.SequenceEqual(reader.Password))
                {
                    if (readerCurrentUsername != readerDTO.Username)
                    {
                        var isNewUsernameTaken = await CommonUserMethods.CommonUserMethods.CheckIfUserExists(this._client, readerDTO.Username);
                        if (isNewUsernameTaken)
                            return false;
                    }
                    if(String.IsNullOrEmpty(readerDTO.NewPassword))
                    {
                        await this._client.Cypher.Match("(reader: Reader)")
                               .Where((Models.Reader reader) => reader.Username == readerCurrentUsername)
                               .Set("reader.Username = $newUsername")
                               .WithParam("newUsername", readerDTO.Username)
                               .Set("reader.Name = $newName")
                               .WithParam("newName", readerDTO.Name)
                               .Set("reader.Lastname = $newLastname")
                               .WithParam("newLastname", readerDTO.Lastname)
                               .Set("reader.NameOfSchool = $newNameOfSchool")
                               .WithParam("newNameOfSchool", readerDTO.NameOfSchool)
                               .Set("reader.TypeOfStudent = $newTypeOfStudent")
                               .WithParam("newTypeOfStudent", readerDTO.TypeOfStudent)
                               .ExecuteWithoutResultsAsync();
                        return true;
                    }
                    else
                    {
                        var newPassword = CommonUserMethods.CommonUserMethods.EncryptPassword(readerDTO.NewPassword, reader.PasswordSalt);
                        await this._client.Cypher.Match("(reader: Reader)")
                               .Where((Models.Reader reader) => reader.Username == readerCurrentUsername)
                               .Set("reader.Username = $newUsername")
                               .WithParam("newUsername", readerDTO.Username)
                               .Set("reader.Name = $newName")
                               .WithParam("newName", readerDTO.Name)
                               .Set("reader.Lastname = $newLastname")
                               .WithParam("newLastname", readerDTO.Lastname)
                               .Set("reader.NameOfSchool = $newNameOfSchool")
                               .WithParam("newNameOfSchool", readerDTO.NameOfSchool)
                               .Set("reader.TypeOfStudent = $newTypeOfStudent")
                               .WithParam("newTypeOfStudent", readerDTO.TypeOfStudent)
                               .Set("reader.Password = $newPassword")
                               .WithParam("newPassword", newPassword)
                               .ExecuteWithoutResultsAsync();
                        return true;
                    }
                }
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> UpdateProfilePicture(UserUpdateProfilePicture profilePictureDTO)
        {
            var reader = await this.GetReaderPreview(profilePictureDTO.Username);
            if (!String.IsNullOrEmpty(reader.Username))
            {
                string profilePictureFilePath = CommonUserMethods.
                                                CommonUserMethods.WriteImageToFolder(profilePictureDTO.ProfilePicture, reader.Username, false);
                await this._client.Cypher.Match("(reader: Reader)")
                                   .Where((Models.Reader reader) => reader.Username == profilePictureDTO.Username)
                                   .Set("reader.ProfilePictureFilePath = $ProfilePictureFilePath")
                                   .WithParam("ProfilePictureFilePath", profilePictureFilePath)
                                   .ExecuteWithoutResultsAsync();
                return true;
            }
            else
                return false;
        }

        public async Task<bool> CheckIfBlogSavedToReadLater(string username, Guid blogId)
        {
            var readerUsername = (await this._client.Cypher.Match("(reader: Reader)-[r:SAVED_TO_READ_LATER]->(b:Blog)")
                                              .Where((Models.Reader reader) => reader.Username == username)
                                              .AndWhere((Models.Blog b) => b.BlogId == blogId)
                                              .Return((r) => new { Reader = r.As<Models.Reader>() }).ResultsAsync)
                                              .Select(obj => obj.Reader.Username).FirstOrDefault();
            if (readerUsername == username)
                return true;
            else
                return false;
        }
        public async Task<bool> CheckIfCookingRecepieSavedToReadLater(string username, Guid recepieId)
        {
            var readerUsername = (await this._client.Cypher.Match("(reader: Reader)-[r:SAVED_TO_READ_LATER]->(c:CookingRecepie)")
                                             .Where((Models.Reader reader) => reader.Username == username)
                                             .AndWhere((Models.CookingRecepie c) => c.CookingRecepieId == recepieId)
                                             .Return((r) => new { Reader = r.As<Models.Reader>() }).ResultsAsync)
                                             .Select(obj => obj.Reader.Username).FirstOrDefault();
            if (readerUsername == username)
                return true;
            else
                return false;
        }

        public byte[] GetReaderProfilePicture(string username)
        {
            return CommonUserMethods.CommonUserMethods.ReadImageFromFile(username, false);
        }
    }
}
