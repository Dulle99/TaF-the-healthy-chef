using Neo4jClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs;
using TaF_Neo4j.DTOs.Comment;
using TaF_Neo4j.DTOs.CookingRecepieDTO;
using TaF_Neo4j.DTOs.Rate;
using TaF_Neo4j.Services.User.Author;
using TaF_Neo4j.Services.User.CommonUserMethods;
using TaF_Neo4j.Services.User.Reader;

namespace TaF_Neo4j.Services.CookingRecepie
{
    public class CookingRecepieService : ICookingRecepieService
    {
        private IGraphClient _client;
        private IAuthorService _authorService;
        private IReaderService _readerService;
        public CookingRecepieService(IGraphClient client)
        {
            this._client = client;
            this._authorService = new AuthorService(client);
            this._readerService = new ReaderService(client);
        }

        public async Task<bool> CreateCookingRecepie(string authorUsername, BasicCookingRecepieDTO cookingRecepieDTO)
        {
            try
            {
                
                var _cookingRecepie = new Models.CookingRecepie
                {
                    CookingRecepieTitle = cookingRecepieDTO.CookingRecepieTitle,
                    Description = cookingRecepieDTO.Description,
                    TypeOfMeal = cookingRecepieDTO.TypeOfMeal,
                    PublicationDate = DateTime.Now,
                    PreparationTime = cookingRecepieDTO.PreparationTime,
                    AverageRate = 0,
                };

                _cookingRecepie.CookingRecepiePicturePath = MutalMethodsForContent
                                                            .MutalMethodsForContent
                                                            .WriteImageToFolder(cookingRecepieDTO.CookingRecepiePicture,
                                                                                _cookingRecepie.CookingRecepieId, true);
                

                await this._client.Cypher.Create("(c: CookingRecepie $_cookingRecepie)")
                                         .WithParam("_cookingRecepie", _cookingRecepie)
                                         .ExecuteWithoutResultsAsync();

                
                if(cookingRecepieDTO.StepsInFoodPreparation.Count > 0)
                {
                    _cookingRecepie.StepsInFoodPreparation = new List<Models.StepInFoodPreparation>();
                    cookingRecepieDTO.StepsInFoodPreparation.ForEach(step =>
                    {
                        _cookingRecepie.StepsInFoodPreparation.Add(new Models.StepInFoodPreparation(step.OrdinalNumberOfStep, step.StepDescription));
                    });
                    await CookingRecepieServiceAuxiliaryMethods.AttachStepsInFoodPreparationToCookingRecepie(this._client, _cookingRecepie.CookingRecepieId, _cookingRecepie.StepsInFoodPreparation);
                }

                if(cookingRecepieDTO.Ingredients.Count > 0)
                {
                    _cookingRecepie.Ingredients = new List<Models.IngredientForCookingRecepie>();
                    cookingRecepieDTO.Ingredients.ForEach(ingredirent =>
                    {
                        _cookingRecepie.Ingredients.Add(new Models.
                                                        IngredientForCookingRecepie(ingredirent.OrdinalNumberOfIngredient,
                                                                                    ingredirent.Ingredient));
                    });

                    await CookingRecepieServiceAuxiliaryMethods
                          .AttachIngredientsToCookingRecepie(this._client,
                                                             _cookingRecepie.CookingRecepieId,
                                                             _cookingRecepie.Ingredients);
                }




                await this._client.Cypher.Match("(author: Author)", "(recepie: CookingRecepie)")
                                         .Where((Models.Author author) => author.Username == authorUsername)
                                         .AndWhere((Models.CookingRecepie recepie) => recepie.CookingRecepieId == _cookingRecepie.CookingRecepieId)
                                         .Create("(author)-[:HAS_PUBLISHED]->(recepie)")
                                         .ExecuteWithoutResultsAsync();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

      
        public async Task<List<CookingRecepiePreviewDTO>> GetPreviewCookingRecepiesByDatePublication(string cookingRecepieType, int numberOfCookingRecepiesToGet, bool latestFirst)
        {
            string orderByQuery;
            if (latestFirst)
                orderByQuery = "cookingRecepie.PublicationDate DESC";
            else
                orderByQuery = "cookingRecepie.PublicationDate";

            List<Guid> cookingRecepiesGuidIds = await CookingRecepieServiceAuxiliaryMethods
                                                     .FilterCookingRecepiesByCriteria(this._client, cookingRecepieType, orderByQuery, numberOfCookingRecepiesToGet);

            List<CookingRecepiePreviewDTO> filteredCookingRecepies = new List<CookingRecepiePreviewDTO>();

            foreach (var cookingRecepieId in cookingRecepiesGuidIds)
            {
                filteredCookingRecepies.Add(await CookingRecepieServiceAuxiliaryMethods
                                                  .GetCookingRecepiePreview(this._client,cookingRecepieId));
            }
            return filteredCookingRecepies;

        }

        public async Task<List<CookingRecepiePreviewDTO>> GetPreviewCookingRecepiesByPopularity(string cookingRecepieType, int numberOfCookingRecepiesToGet)
        {
            string orderByCriteriaQuery = "cookingRecepie.AverageRate";
            List<Guid> listOfFilteredCookingRecepiesGuidIds = await CookingRecepieServiceAuxiliaryMethods
                                                                    .FilterCookingRecepiesByCriteria(this._client, cookingRecepieType, orderByCriteriaQuery, numberOfCookingRecepiesToGet);

            List<CookingRecepiePreviewDTO> filteredCookingRecepies = new List<CookingRecepiePreviewDTO>();

            foreach (var cookingRecepieId in listOfFilteredCookingRecepiesGuidIds)
            {

                filteredCookingRecepies.Add(await CookingRecepieServiceAuxiliaryMethods
                                                  .GetCookingRecepiePreview(this._client, cookingRecepieId));
            }
            return filteredCookingRecepies.OrderByDescending(recepie => recepie.AverageRate).ToList();
        }

        public async Task<List<CookingRecepiePreviewDTO>> GetFastestToCookPreviewCookingRecepies(string cookingRecepieType, int numberOfCookingRecepiesToGet)
        {
            string orderByCriteriaQuery = "cookingRecepie.PreparationTime";
            List<Guid> listOfFilteredCookingRecepiesGuidIds = await CookingRecepieServiceAuxiliaryMethods
                                                                    .FilterCookingRecepiesByCriteria(this._client, cookingRecepieType, orderByCriteriaQuery, numberOfCookingRecepiesToGet);

            List<CookingRecepiePreviewDTO> filteredCookingRecepies = new List<CookingRecepiePreviewDTO>();

            foreach (var cookingRecepieId in listOfFilteredCookingRecepiesGuidIds)
            {

                filteredCookingRecepies.Add(await CookingRecepieServiceAuxiliaryMethods
                                                  .GetCookingRecepiePreview(this._client, cookingRecepieId));
            }
            return filteredCookingRecepies;
        }

        public async Task<List<CookingRecepiePreviewDTO>> GetRecommendedCookingRecepies()
        {
            int fiveRecepies = 5;
            var latestBreakfastCookingRecepies = await this.GetPreviewCookingRecepiesByDatePublication("Dorucak", fiveRecepies, true);
            var latestLunchCookingRecepies = await this.GetPreviewCookingRecepiesByDatePublication("Rucak", fiveRecepies, true);
            var latestDinerCookingRecepies = await this.GetPreviewCookingRecepiesByDatePublication("Vecera", fiveRecepies, true);
            var latestSnacksCookingRecepies = await this.GetPreviewCookingRecepiesByDatePublication("Uzina", fiveRecepies, true);

            latestBreakfastCookingRecepies = latestBreakfastCookingRecepies.OrderByDescending(cookingRecepie => cookingRecepie.AverageRate).ToList();
            latestLunchCookingRecepies = latestLunchCookingRecepies.OrderByDescending(cookingRecepie => cookingRecepie.AverageRate).ToList();
            latestDinerCookingRecepies = latestDinerCookingRecepies.OrderByDescending(cookingRecepie => cookingRecepie.AverageRate).ToList();
            latestSnacksCookingRecepies = latestSnacksCookingRecepies.OrderByDescending(cookingRecepie => cookingRecepie.AverageRate).ToList();

            var combinedCookingRecepies = new List<CookingRecepiePreviewDTO>();
            for(int i=0; i < 2; i++)
            {
                if(latestBreakfastCookingRecepies.Count >= 2)
                    combinedCookingRecepies.Add(latestBreakfastCookingRecepies[i]);

                if (latestLunchCookingRecepies.Count >= 2)
                    combinedCookingRecepies.Add(latestLunchCookingRecepies[i]);

                if (latestDinerCookingRecepies.Count >= 2)
                    combinedCookingRecepies.Add(latestDinerCookingRecepies[i]);

                if (latestSnacksCookingRecepies.Count >= 2)
                    combinedCookingRecepies.Add(latestSnacksCookingRecepies[i]);
            }
            var sortedCookingRecepiesByAverageRate = combinedCookingRecepies.OrderByDescending(cookingRecepie => cookingRecepie.AverageRate).ToList();
            return sortedCookingRecepiesByAverageRate;
        }

        public async Task<List<CookingRecepiePreviewDTO>> GetPreviewCookingRecepiesByAuthor(string authorUsername, int numberOfCookingRecepiesToGet)
        {
            return (await this._client.Cypher.Match("(author:Author)-[:HAS_PUBLISHED]->(cookingRecepie: CookingRecepie)")
                                             .Where((Models.Author author) => author.Username == authorUsername)
                                             .Return((author, cookingRecepie) => new
                                             {
                                                 Author = author.As<Models.Author>(),
                                                 CookingRecepie = cookingRecepie.As<Models.CookingRecepie>()
                                             }).Limit(numberOfCookingRecepiesToGet).ResultsAsync)
                                             .Select(obj => new CookingRecepiePreviewDTO
                                             {
                                                 AuthorUsername = obj.Author.Username,
                                                 AuthorProfilePicture = CommonUserMethods.ReadImageFromFile(obj.Author.Username, true),
                                                 CookingRecepieTitle = obj.CookingRecepie.CookingRecepieTitle,
                                                 CookingRecepieId = obj.CookingRecepie.CookingRecepieId,
                                                 Description = obj.CookingRecepie.Description.Length < 300 
                                                               ? obj.CookingRecepie.Description + "..."
                                                               : obj.CookingRecepie.Description.Substring(0, obj.CookingRecepie.Description.Length / 4) + "...",
                                                 PreparationTime = obj.CookingRecepie.PreparationTime,
                                                 PublicationDate = obj.CookingRecepie.PublicationDate,
                                                 TypeOfMeal = obj.CookingRecepie.TypeOfMeal,
                                                 AverageRate = obj.CookingRecepie.AverageRate,
                                                 CookingRecepiePicture = MutalMethodsForContent
                                                                        .MutalMethodsForContent.ReadImageFromFile(obj.CookingRecepie.CookingRecepieId, true)
                                             }).ToList();
        }

        public async Task<CookingRecepieDTO> GetCookingRecepie(Guid cookingRecepieId)
        {
            var cookingRecepie = (await this._client.Cypher.Match("(author:Author)-[:HAS_PUBLISHED]->(cookingRecepie: CookingRecepie)")
                                            .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                                            .Return((author, cookingRecepie) => new
                                            {
                                                Author = author.As<Models.Author>(),
                                                CookingRecepie = cookingRecepie.As<Models.CookingRecepie>()
                                            }).ResultsAsync)
                                            .Select(obj => new CookingRecepieDTO
                                            {
                                                AuthorUsername = obj.Author.Username,
                                                AuthorProfilePicture = CommonUserMethods.ReadImageFromFile(obj.Author.Username, true),
                                                CookingRecepieTitle = obj.CookingRecepie.CookingRecepieTitle,
                                                Description = obj.CookingRecepie.Description,
                                                PreparationTime = obj.CookingRecepie.PreparationTime,
                                                PublicationDate = obj.CookingRecepie.PublicationDate,
                                                TypeOfMeal = obj.CookingRecepie.TypeOfMeal,
                                                CookingRecepiePicture = MutalMethodsForContent
                                                                        .MutalMethodsForContent.ReadImageFromFile(obj.CookingRecepie.CookingRecepieId, true),
                                                AverageRate = obj.CookingRecepie.AverageRate,
                                                CookingRecepieId = obj.CookingRecepie.CookingRecepieId
                                            }).FirstOrDefault();
            var steps = await CookingRecepieServiceAuxiliaryMethods.GetStepsInFoodPreparation(this._client, cookingRecepieId);
            if (steps.Count > 0)
            {
                cookingRecepie.StepsInFoodPreparation = steps;
                cookingRecepie.StepsInFoodPreparation = cookingRecepie.StepsInFoodPreparation.OrderBy(step => step.OrdinalNumberOfStep).ToList();
            }

            var ingredients = await CookingRecepieServiceAuxiliaryMethods.GetIngredientsForFoodPreparation(this._client, cookingRecepieId);
            if(ingredients.Count > 0)
            {
                cookingRecepie.Ingredients = ingredients.OrderBy(ingredient => ingredient.OrdinalNumberOfIngredient).ToList();
            }

            return cookingRecepie;

        }

        
        public async Task<bool> UpdateCookingRecepie(Guid cookingRecepieId, CookingRecepieUpdateDTO cookingRecepieDTO)
        {
            var cookingRecepie = await this.GetCookingRecepie(cookingRecepieId);
            if (cookingRecepie != null)
            {
                if (cookingRecepieDTO.CookingRecepiePicture != null)
                {
                    MutalMethodsForContent.MutalMethodsForContent.DeleteImageFromFolder(cookingRecepieId, true);
                    var newImageFilePath = MutalMethodsForContent.MutalMethodsForContent.WriteImageToFolder(cookingRecepieDTO.CookingRecepiePicture, cookingRecepieId, true);

                    await this._client.Cypher.Match("(cookingRecepie: CookingRecepie)")
                                             .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                                             .Set("cookingRecepie.CookingRecepieTitle = $newTitle")
                                             .WithParam("newTitle", cookingRecepieDTO.CookingRecepieTitle)
                                             .Set("cookingRecepie.Description = $newDescription")
                                             .WithParam("newDescription", cookingRecepieDTO.Description)
                                             .Set("cookingRecepie.PublicationDate = $newPublicationDate")
                                             .WithParam("newPublicationDate", DateTime.UtcNow)
                                             .Set("cookingRecepie.PreparationTime = $newPreparationTime")
                                             .WithParam("newPreparationTime", cookingRecepieDTO.PreparationTime)
                                             .Set("cookingRecepie.TypeOfMeal = $newTypeOfMeal")
                                             .WithParam("newTypeOfMeal", cookingRecepieDTO.TypeOfMeal)
                                             .Set("cookingRecepie.CookingRecepiePicturePath = $newImageFilePath")
                                             .WithParam("newImageFilePath", newImageFilePath).ExecuteWithoutResultsAsync();

                }
                else
                {
                    await this._client.Cypher.Match("(cookingRecepie: CookingRecepie)")
                                            .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                                            .Set("cookingRecepie.CookingRecepieTitle = $newTitle")
                                            .WithParam("newTitle", cookingRecepieDTO.CookingRecepieTitle)
                                            .Set("cookingRecepie.Description = $newDescription")
                                            .WithParam("newDescription", cookingRecepieDTO.Description)
                                            .Set("cookingRecepie.PublicationDate = $newPublicationDate")
                                            .WithParam("newPublicationDate", DateTime.UtcNow)
                                            .Set("cookingRecepie.PreparationTime = $newPreparationTime")
                                            .WithParam("newPreparationTime", cookingRecepieDTO.PreparationTime)
                                            .Set("cookingRecepie.TypeOfMeal = $newTypeOfMeal")
                                            .WithParam("newTypeOfMeal", cookingRecepieDTO.TypeOfMeal)
                                            .ExecuteWithoutResultsAsync();
                }
                await UpdateIngredientsoodPreparation(cookingRecepieId, cookingRecepieDTO.Ingredients);
                await UpdateStepsInFoodPreparation(cookingRecepieId, cookingRecepieDTO.StepsInFoodPreparation);

                return true;
            }
            else
                return false;
        }

        public async Task<bool> UpdateIngredientsoodPreparation(Guid cookingRecepieId, List<IngredientForCookingRecepieDTO> ingredientsDTO)
        {
            var cookingRecepie = await this.GetCookingRecepie(cookingRecepieId);
            if (cookingRecepie != null)
            {
                if (cookingRecepie.Ingredients.Count > 0)
                    await CookingRecepieServiceAuxiliaryMethods.DetachAndDeleteIngredientsForCookingRecepie(this._client, cookingRecepieId);

                List<Models.IngredientForCookingRecepie> ingredients = new List<Models.IngredientForCookingRecepie>();
                foreach (var ingredient in ingredientsDTO)
                {
                    ingredients.Add(new Models.IngredientForCookingRecepie(ingredient.OrdinalNumberOfIngredient, ingredient.Ingredient));
                }
                await CookingRecepieServiceAuxiliaryMethods.AttachIngredientsToCookingRecepie(this._client, cookingRecepieId, ingredients);
                return true;
            }
            else
                return false;
        }

        public async Task<bool> UpdateStepsInFoodPreparation(Guid cookingRecepieId, List<StepInFoodPreparationDTO> stepsDTO)
        {
            var cookingRecepie = await this.GetCookingRecepie(cookingRecepieId);
            if (cookingRecepie != null)
            {
                if(cookingRecepie.StepsInFoodPreparation.Count> 0)
                    await CookingRecepieServiceAuxiliaryMethods.DetachAndDeleteStepsInFoodPreparation(this._client, cookingRecepieId);

                List<Models.StepInFoodPreparation> stepsInFoodPreparation = new List<Models.StepInFoodPreparation>();
                foreach(var step in stepsDTO)
                {
                    stepsInFoodPreparation.Add(new Models.StepInFoodPreparation(step.OrdinalNumberOfStep, step.StepDescription));
                }
                await CookingRecepieServiceAuxiliaryMethods.AttachStepsInFoodPreparationToCookingRecepie(this._client, cookingRecepieId, stepsInFoodPreparation);
                return true;
            }
            else
                return false;
        }

        public async Task<bool> DeleteCookingRecepie(Guid cookingRecepieId)
        {
            var cookingRecepie = await this.GetCookingRecepie(cookingRecepieId);
            if(cookingRecepie != null)
            {
                if(cookingRecepie.StepsInFoodPreparation.Count > 0)
                    await CookingRecepieServiceAuxiliaryMethods.DetachAndDeleteStepsInFoodPreparation(this._client, cookingRecepieId);
                await CookingRecepieServiceAuxiliaryMethods.DetachAndDeleteCommentsOfCookingRecepie(this._client, cookingRecepieId);
                await CookingRecepieServiceAuxiliaryMethods.DetachAndDeleteRatesOfCookingRecepie(this._client, cookingRecepieId);
                await CookingRecepieServiceAuxiliaryMethods.DetachCookingRecepieForReadLater(this._client, cookingRecepieId);
                await CookingRecepieServiceAuxiliaryMethods.DetachAndDeleteIngredientsForCookingRecepie(this._client, cookingRecepieId);

                MutalMethodsForContent.MutalMethodsForContent.DeleteImageFromFolder(cookingRecepieId, true);

                await this._client.Cypher.Match("(author: Author)-[r:HAS_PUBLISHED]->(cookingRecepie: CookingRecepie)")
                                          .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                                          .Delete("r")
                                          .Delete("cookingRecepie")
                                          .ExecuteWithoutResultsAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> AddCommentToTheCookingRecepie(BasicCommentDTO commentDTO)
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
                        await this._client.Cypher.Match("(author: Author)", "(cookingRecepie: CookingRecepie)")
                                                 .Where((Models.Author author) => author.Username == commentDTO.Username)
                                                 .AndWhere((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == commentDTO.ContentId)
                                                 .Create("(comment: Comment $newComment)-[:WRITES]->(author)")
                                                 .WithParam("newComment", newComment)
                                                 .Create("(cookingRecepie)-[:HAS_COMMENTS]->(comment)")
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
                        await this._client.Cypher.Match("(reader: Reader)", "(cookingRecepie: CookingRecepie)")
                                                 .Where((Models.Reader reader) => reader.Username == commentDTO.Username)
                                                 .AndWhere((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == commentDTO.ContentId)
                                                 .Create("(comment: Comment $newComment)-[:WRITES]->(reader)")
                                                 .WithParam("newComment", newComment)
                                                 .Create("(cookingRecepie)-[:HAS_COMMENTS]->(comment)")
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
        public async Task<float> AddRateToTheCookingRecepie(BasicRateDTO rateDTO)
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
                        var rate = (await this._client.Cypher.Match("(cookingRecepie: CookingRecepie) -[:HAS_RATES]->(rate: Rate)-[:RATES]->(author: Author)")
                                                            .Where((Models.Author author) => author.Username == rateDTO.Username)
                                                            .AndWhere((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == rateDTO.ContentId)
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



                            await this._client.Cypher.Match("(author: Author)", "(cookingRecepie: CookingRecepie)")
                                                     .Where((Models.Author author) => author.Username == rateDTO.Username)
                                                     .AndWhere((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == rateDTO.ContentId)
                                                     .Create("(rate: Rate $newRate)-[:RATES]->(author)")
                                                     .WithParam("newRate", newRate)
                                                     .Create("(cookingRecepie)-[:HAS_RATES]->(rate)")
                                                     .ExecuteWithoutResultsAsync();
                            }
                        return await CookingRecepieServiceAuxiliaryMethods.UpdateAverageRateOfCookingRecepie(this._client, rateDTO.ContentId);
                        
                    }
                }
                else //Reader
                {
                    var reader = this._readerService.GetReaderPreview(rateDTO.Username);
                    if (reader.Result == null)
                        return 0;
                    else
                    {
                        var rate = (await this._client.Cypher.Match("(cookingRecepie: CookingRecepie) -[:HAS_RATES]->(rate: Rate)-[:RATES]->(reader: Reader)")
                                                           .Where((Models.Reader reader) => reader.Username == rateDTO.Username)
                                                           .AndWhere((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == rateDTO.ContentId)
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
                            await this._client.Cypher.Match("(reader: Reader)", "(cookingRecepie: CookingRecepie)")
                                                      .Where((Models.Reader reader) => reader.Username == rateDTO.Username)
                                                      .AndWhere((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == rateDTO.ContentId)
                                                      .Create("(rate: Rate $newRate)-[:RATES]->(reader)")
                                                      .WithParam("newRate", newRate)
                                                      .Create("(cookingRecepie)-[:HAS_RATES]->(rate)")
                                                      .ExecuteWithoutResultsAsync();
                        }
                        return await CookingRecepieServiceAuxiliaryMethods.UpdateAverageRateOfCookingRecepie(this._client, rateDTO.ContentId);
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<List<CommentPreviewDTO>> GetCommentsByCookinRecepie(Guid cookingRecepieId, int numberOfCommentsToGet)
        {
            int limit = 10;
            if (numberOfCommentsToGet != 0)
                limit = numberOfCommentsToGet;

            string criteria = "comment.PublicationDate DESC";
            var commentsByAuthors = (await this._client.Cypher.Match("(cookingRecepie: CookingRecepie)-[:HAS_COMMENTS]->(comment: Comment)-[:WRITES]->(author: Author)")
                                              .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                                              .Return((cookingRecepie, comment, author) => new
                                              {
                                                  CookingRecepie = cookingRecepie.As<Models.CookingRecepie>(),
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

            var commentsByReaders = (await this._client.Cypher.Match("(cookingRecepie: CookingRecepie)-[:HAS_COMMENTS]->(comment: Comment)-[:WRITES]->(reader: Reader)")
                                              .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                                              .Return((cookingRecepie, comment, reader) => new
                                              {
                                                  CookingRecepie = cookingRecepie.As<Models.CookingRecepie>(),
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
