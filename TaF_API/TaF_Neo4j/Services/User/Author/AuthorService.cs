using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs.BlogDTO;
using TaF_Neo4j.DTOs.CookingRecepieDTO;
using TaF_Neo4j.DTOs.UserDTO;
using TaF_Neo4j.Models;
using TaF_Neo4j.Services.CookingRecepie;

namespace TaF_Neo4j.Services.User.Author
{
    public class AuthorService : IAuthorService
    {
        private IGraphClient _client;
        public AuthorService(IGraphClient client)
        {
            this._client = client;
        }

        public async Task<bool> CreateAuthor(AuthorDTO authorDTO)
        {
            if (await CommonUserMethods.CommonUserMethods.CheckIfUserExists(this._client, authorDTO.Username))
                return false;
            else
            {
                Models.Author author = new Models.Author();
                author.Username = authorDTO.Username;
                author.Password = CommonUserMethods.CommonUserMethods.EncryptPassword(authorDTO.Password, author.PasswordSalt);
                author.Name = authorDTO.Name;
                author.Lastname = authorDTO.Lastname;
                author.Biography = authorDTO.Biography;
                author.YearsOfExpiriance = authorDTO.YearsOfExpiriance;

                await this._client.Cypher.Create("(author: Author $newAuthor)")
                                          .WithParam("newAuthor", author)
                                          .ExecuteWithoutResultsAsync();
                if (authorDTO.Awards != null && authorDTO.Awards.Count > 0)
                {
                    author.Awards = new List<Award>();
                    authorDTO.Awards.ForEach(award =>
                    {
                        author.Awards.Add(new Award(award.AwardName, award.DateOfReceivingAward));
                    });

                    await this.AttachAwardsToAuthor(author.Username, author.Awards);
                } 
                return true;
            }
        }

        public async Task DetachAwardsFromAuthor(string authorUsername)
        {
            await this._client.Cypher.Match("(author: Author)-[r:HAS_AWARD]->(award:Award)")
                                     .Where((Models.Author author) => author.Username == authorUsername)
                                     .Delete("r")
                                     .Delete("award")
                                     .ExecuteWithoutResultsAsync();
        }

        public async Task AttachAwardsToAuthor(string authorUsername, List<Award> awards)
        {
            foreach(var awardObj in awards)
            {
                if (!String.IsNullOrEmpty(awardObj.AwardName))
                {
                    await this._client.Cypher.Create("(award: Award $newAward)")
                                        .WithParam("newAward", awardObj)
                                        .ExecuteWithoutResultsAsync();

                    await this._client.Cypher.Match("(author: Author)", "(award:Award)")
                                        .Where((Models.Author author) => author.Username == authorUsername)
                                        .AndWhere((Models.Award award) => award.AwardId == awardObj.AwardId)
                                        .Create("(author)-[:HAS_AWARD]->(award)")
                                        .ExecuteWithoutResultsAsync();
                }
            };
        }

        public async Task<bool> AddBlogToReadLater(string username, Guid blogId)
        {
            try
            {
                if(await CheckIfBlogSavedToReadLater(username, blogId))
                    return true;

                await this._client.Cypher.Match("(a: Author)", "(b: Blog)")
                                         .Where((Models.Author a) => a.Username == username)
                                         .AndWhere((Models.Blog b) => b.BlogId == blogId)
                                         .Create("(a)-[:SAVED_TO_READ_LATER]->(b)")
                                         .ExecuteWithoutResultsAsync();

                await this._client.Cypher.Match("(a: Author)-[r:SAVED_TO_READ_LATER]->(b: Blog)")
                                         .Where((Models.Author a) => a.Username == username)
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

                await this._client.Cypher.Match("(a: Author)", "(c: CookingRecepie)")
                                         .Where((Models.Author a) => a.Username == username)
                                         .AndWhere((Models.CookingRecepie c) => c.CookingRecepieId == cookingRecepieId)
                                         .Create("(a)-[:SAVED_TO_READ_LATER]->(c)")
                                         .ExecuteWithoutResultsAsync();

                await this._client.Cypher.Match("(a: Author)-[r:SAVED_TO_READ_LATER]->(c: CookingRecepie)")
                                         .Where((Models.Author a) => a.Username == username)
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
                await _client.Cypher.OptionalMatch("(a)-[r:SAVED_TO_READ_LATER]->(b)")
                                    .Where((Models.Author a) => a.Username == username)
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
                await _client.Cypher.OptionalMatch("(a)-[r:SAVED_TO_READ_LATER]->(c)")
                                    .Where((Models.Author a) => a.Username == username)
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
            return (await this._client.Cypher.Match("(a: Author)-[r:SAVED_TO_READ_LATER]->(blog: Blog)")
                                                  .Where((Models.Author a) => a.Username == username)
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
            return (await this._client.Cypher.Match("(a: Author)-[r:SAVED_TO_READ_LATER]->(cookingRecepie : CookingRecepie)")
                                                  .Where((Models.Author a) => a.Username == username)
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

        public async Task<AuthorPreviewDTO> GetAuthorPreview(string username)
        {
            return (await this._client.Cypher.Match("(author: Author)")
                                             .Where((Models.Author author) => author.Username == username)
                                             .OptionalMatch("(author)-[:HAS_AWARD]->(award)")
                                             .Return((author, award) => new
                                             {
                                                 Author = author.As<Models.Author>(),
                                                 Awards = award.CollectAs<Models.Award>()
                                             })
                                             .ResultsAsync)
                                             .Select(obj => new AuthorPreviewDTO
                                             {
                                                 Username = obj.Author.Username,
                                                 Name = obj.Author.Name,
                                                 Lastname = obj.Author.Lastname,
                                                 Biography = obj.Author.Biography,
                                                 YearsOfExpiriance = obj.Author.YearsOfExpiriance,
                                                 ProfilePicture = CommonUserMethods.CommonUserMethods.ReadImageFromFile(obj.Author.Username, true),
                                                 Awards = (obj.Awards != null) ? obj.Awards.ToList() : new List<Award>()
                                             }).FirstOrDefault();
        }

        public async Task<bool> UpdateAuthor(string authorCurrentUsername, AuthorUpdateDTO authorDTO)
        {

            Models.Author author = (await this._client.Cypher.Match("(author: Author)")
                                            .Where((Models.Author author) => author.Username == authorCurrentUsername)
                                            .Return((author) => new { Author = author.As<Models.Author>() })
                                            .ResultsAsync)
                                            .Select(obj => new Models.Author
                                            {
                                                Username = obj.Author.Username,
                                                Password = obj.Author.Password,
                                                PasswordSalt = obj.Author.PasswordSalt
                                            }).FirstOrDefault();

            if (author == null)
                return false;

            if (author.Username == authorCurrentUsername)
            {
                var passwordTest = CommonUserMethods.CommonUserMethods.EncryptPassword(authorDTO.CurrentPassword, author.PasswordSalt);
                if (passwordTest.SequenceEqual(author.Password))
                {
                    if (authorCurrentUsername != authorDTO.Username)
                    {
                        var isNewUsernameTaken = await CommonUserMethods.CommonUserMethods.CheckIfUserExists(this._client, authorDTO.Username);
                        if (isNewUsernameTaken)
                            return false;
                    }

                    if (String.IsNullOrEmpty(authorDTO.NewPassword))
                    {
                        await this._client.Cypher.Match("(author: Author)")
                               .Where((Models.Author author) => author.Username == authorCurrentUsername)
                               .Set("author.Username = $newUsername")
                               .WithParam("newUsername", authorDTO.Username)
                               .Set("author.Name = $newName")
                               .WithParam("newName", authorDTO.Name)
                               .Set("author.Lastname = $newLastname")
                               .WithParam("newLastname", authorDTO.Lastname)
                               .Set("author.Biography = $newBiography")
                               .WithParam("newBiography", authorDTO.Biography)
                               .Set("author.YearsOfExpiriance = $newYearsOfExpiriance")
                               .WithParam("newYearsOfExpiriance", authorDTO.YearsOfExpiriance)
                               .ExecuteWithoutResultsAsync();
                    }
                    else
                    {
                        var newPassword = CommonUserMethods.CommonUserMethods.EncryptPassword(authorDTO.NewPassword, author.PasswordSalt);
                        await this._client.Cypher.Match("(author: Author)")
                               .Where((Models.Author author) => author.Username == authorCurrentUsername)
                               .Set("author.Username = $newUsername")
                               .WithParam("newUsername", authorDTO.Username)
                               .Set("author.Name = $newName")
                               .WithParam("newName", authorDTO.Name)
                               .Set("author.Lastname = $newLastname")
                               .WithParam("newLastname", authorDTO.Lastname)
                               .Set("author.Biography = $newBiography")
                               .WithParam("newBiography", authorDTO.Biography)
                               .Set("author.YearsOfExpiriance = $newYearsOfExpiriance")
                               .WithParam("newYearsOfExpiriance", authorDTO.YearsOfExpiriance)
                               .Set("author.Password = $newPassword")
                               .WithParam("newPassword", newPassword)
                               .ExecuteWithoutResultsAsync();
                    }

                    await DetachAwardsFromAuthor(authorDTO.Username);
                    if (authorDTO.Awards != null && authorDTO.Awards.Count > 0)
                    {
                        var _awards = new List<Award>();
                        authorDTO.Awards.ForEach(award =>
                        {
                            _awards.Add(new Award(award.AwardName, award.DateOfReceivingAward));
                        });

                        await this.AttachAwardsToAuthor(authorDTO.Username, _awards);
                    }
                    return true;


                }
                else
                    return false;
            }
            else
                return false;
        }

        public async Task<bool> UpdateProfilePicture(UserUpdateProfilePicture profilePictureDTO)
        {
            var author = await this.GetAuthorPreview(profilePictureDTO.Username);
            if (!String.IsNullOrEmpty(author.Username))
            {
                CommonUserMethods.CommonUserMethods.DeleteImageFromFolder(profilePictureDTO.Username,  true);
                string profilePictureFilePath = CommonUserMethods.
                                                CommonUserMethods.WriteImageToFolder(profilePictureDTO.ProfilePicture, author.Username, true);
                await this._client.Cypher.Match("(author: Author)")
                                   .Where((Models.Author author) => author.Username == profilePictureDTO.Username)
                                   .Set("author.ProfilePictureFilePath = $ProfilePictureFilePath")
                                   .WithParam("ProfilePictureFilePath", profilePictureFilePath)
                                   .ExecuteWithoutResultsAsync();
                return true;
            }
            else
                return false;
        }

        public async Task<LoggedUserInformation> Login(UserLoginCredentials credentials)
        {
            Models.Author author = (await this._client.Cypher.Match("(author: Author)")
                                            .Where((Models.Author author) => author.Username == credentials.Username)
                                            .Return((author) => new { Author = author.As<Models.Author>() })
                                            .ResultsAsync)
                                            .Select(obj => new Models.Author
                                            {
                                                Username = obj.Author.Username,
                                                Password = obj.Author.Password,
                                                PasswordSalt = obj.Author.PasswordSalt
                                            }).FirstOrDefault();

            if (author == null)
                return new LoggedUserInformation { TypeOfUser = "UserIsNotAuthor" };

            if (author.Username == credentials.Username)
            {
                var passwordTest = CommonUserMethods.CommonUserMethods.EncryptPassword(credentials.Password, author.PasswordSalt);
                if (passwordTest.SequenceEqual(author.Password))
                {
                    return new LoggedUserInformation
                    {
                        Username = author.Username,
                        TypeOfUser = "Author",
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
                    LoginInformation = "UserIsNotAuthor"
                };
            }
        }

        public async Task<bool> CheckIfBlogSavedToReadLater(string username, Guid blogId)
        {
            var authorUsername = (await this._client.Cypher.Match("(a: Author)-[r:SAVED_TO_READ_LATER]->(b:Blog)")
                                              .Where((Models.Author a) => a.Username == username)
                                              .AndWhere((Models.Blog b) => b.BlogId == blogId)
                                              .Return((a) => new { Author = a.As<Models.Author>() }).ResultsAsync)
                                              .Select(obj => obj.Author.Username).FirstOrDefault();
            if (authorUsername == username)
                return true;
            else
                return false;
        }

        public async Task<bool> CheckIfCookingRecepieSavedToReadLater(string username, Guid recepieId)
        {
            var authorUsername = (await this._client.Cypher.Match("(a: Author)-[r:SAVED_TO_READ_LATER]->(c: CookingRecepie)")
                                              .Where((Models.Author a) => a.Username == username)
                                              .AndWhere((Models.CookingRecepie c) => c.CookingRecepieId == recepieId)
                                              .Return((a) => new { Author = a.As<Models.Author>() }).ResultsAsync)
                                              .Select(obj => obj.Author.Username).FirstOrDefault();
            if (authorUsername == username)
                return true;
            else
                return false;
        }

        public byte[] GetAuthorProfilePicture(string username)
        {
            return CommonUserMethods.CommonUserMethods.ReadImageFromFile(username, true);
        }
    }
}
