using Microsoft.AspNetCore.Http;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs;
using TaF_Neo4j.DTOs.CookingRecepieDTO;
using TaF_Neo4j.Services.User.CommonUserMethods;

namespace TaF_Neo4j.Services.CookingRecepie
{
    public static class CookingRecepieServiceAuxiliaryMethods
    {
        public static async Task<CookingRecepiePreviewDTO> GetCookingRecepiePreview(IGraphClient client, Guid cookingRecepieId)
        {
            return (await client.Cypher.Match("(author:Author)-[:HAS_PUBLISHED]->(cookingRecepie: CookingRecepie)")
                                             .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                                             .Return((cookingRecepie, author) => new
                                             {
                                                 CookingRecepie = cookingRecepie.As<Models.CookingRecepie>(),
                                                 Author = author.As<Models.Author>()
                                             }).ResultsAsync)
                                             .Select(obj => new CookingRecepiePreviewDTO 
                                             {
                                                 AuthorUsername = obj.Author.Username,
                                                 AuthorProfilePicture = CommonUserMethods.ReadImageFromFile(obj.Author.Username, true),
                                                 CookingRecepieId = obj.CookingRecepie.CookingRecepieId,
                                                 CookingRecepieTitle = obj.CookingRecepie.CookingRecepieTitle,
                                                 Description = obj.CookingRecepie.Description.Length < 300
                                                               ? obj.CookingRecepie.Description + "..."
                                                               : 
                                                               obj.CookingRecepie.Description.Substring(0, obj.CookingRecepie.Description.Length > 350 ? 350 : obj.CookingRecepie.Description.Length) + "...",
                                                 PreparationTime = obj.CookingRecepie.PreparationTime,
                                                 PublicationDate = obj.CookingRecepie.PublicationDate,
                                                 TypeOfMeal = obj.CookingRecepie.TypeOfMeal,
                                                 AverageRate = obj.CookingRecepie.AverageRate,
                                                 CookingRecepiePicture = MutalMethodsForContent
                                                                        .MutalMethodsForContent
                                                                        .ReadImageFromFile(obj.CookingRecepie.CookingRecepieId, true)
                                                          
                                             }).FirstOrDefault();
        }



        public static async Task<List<Guid>> FilterCookingRecepiesByCriteria(IGraphClient client,string cookingRecepieType, string criteria, int numberOfCookingRecepiesToGet)
        {
            int numberOfIdsToGet = numberOfCookingRecepiesToGet;
            if (numberOfIdsToGet == 0)
                numberOfIdsToGet = 5; //default


            return (await client.Cypher.Match("(cookingRecepie: CookingRecepie)")
                                              .Where((Models.CookingRecepie cookingRecepie)=> cookingRecepie.TypeOfMeal == cookingRecepieType)
                                              .Return((cookingRecepie) => new
                                              {
                                                  CookingRecepie= cookingRecepie.As<Models.CookingRecepie>()
                                              })
                                              .OrderBy(criteria).Limit(numberOfIdsToGet)
                                              .ResultsAsync)
                                              .Select(obj => obj.CookingRecepie.CookingRecepieId).ToList();

        }

        public static async Task<List<IngredientForCookingRecepieDTO>> GetIngredientsForFoodPreparation(IGraphClient client, Guid cookingRecepieId)
        {
            return (await client.Cypher.Match("(recepie: CookingRecepie)-[:NEED]->(ingredient: Ingredient)")
                                              .Where((Models.CookingRecepie recepie) => recepie.CookingRecepieId == cookingRecepieId)
                                              .Return((recepie, ingredient) => new
                                              {
                                                  Ingredient = ingredient.As<Models.IngredientForCookingRecepie>(),

                                              }).ResultsAsync)
                                              .Select((obj) => new IngredientForCookingRecepieDTO
                                              {
                                                  Ingredient = obj.Ingredient.Ingredient,
                                                  OrdinalNumberOfIngredient = obj.Ingredient.OrdinalNumberOfIngredient
                                              }).ToList();
        }

        public static async Task<List<StepInFoodPreparationDTO>> GetStepsInFoodPreparation(IGraphClient client, Guid cookingRecepieId)
        {
            return (await client.Cypher.Match("(recepie: CookingRecepie)-[:HAS]->(step: StepInFoodPreparation)")
                                              .Where((Models.CookingRecepie recepie) => recepie.CookingRecepieId == cookingRecepieId)
                                              .Return((recepie, step) => new
                                              {
                                                  Step = step.As<Models.StepInFoodPreparation>(),

                                              }).ResultsAsync)
                                              .Select((obj) => new StepInFoodPreparationDTO
                                              {
                                                  StepDescription = obj.Step.StepDescription,
                                                  OrdinalNumberOfStep = obj.Step.OrdinalNumberOfStep
                                              }).ToList();
        }

        public static async Task AttachIngredientsToCookingRecepie(IGraphClient client, Guid cookingRecepieId, List<Models.IngredientForCookingRecepie> ingredients)
        {
            foreach (var ingredientObj in ingredients)
            {
                await client.Cypher.Create("(ingredient: Ingredient $newIngredient)")
                                    .WithParam("newIngredient", ingredientObj)
                                    .ExecuteWithoutResultsAsync();

                await client.Cypher.Match("(recepie: CookingRecepie )", "(ingredient: Ingredient)")
                                    .Where((Models.CookingRecepie recepie) => recepie.CookingRecepieId == cookingRecepieId)
                                    .AndWhere((Models.IngredientForCookingRecepie ingredient) => ingredient.IngredientId == ingredientObj.IngredientId)
                                    .Create("(recepie)-[:NEED]->(ingredient)")
                                    .ExecuteWithoutResultsAsync();
            };
        }

        public static async Task AttachStepsInFoodPreparationToCookingRecepie(IGraphClient client, Guid cookingRecepieId, List<Models.StepInFoodPreparation> stepsInFoodPreparation)
        {
            foreach (var stepInFoodPreparationObj in stepsInFoodPreparation)
            {
                await client.Cypher.Create("(step: StepInFoodPreparation $newStep)")
                                    .WithParam("newStep", stepInFoodPreparationObj)
                                    .ExecuteWithoutResultsAsync();

                await client.Cypher.Match("(recepie: CookingRecepie )", "(step:StepInFoodPreparation)")
                                    .Where((Models.CookingRecepie recepie) => recepie.CookingRecepieId == cookingRecepieId)
                                    .AndWhere((Models.StepInFoodPreparation step) => step.StepInFoodPreparatinId == stepInFoodPreparationObj.StepInFoodPreparatinId)
                                    .Create("(recepie)-[:HAS]->(step)")
                                    .ExecuteWithoutResultsAsync();
            };
        }

        public static async Task DetachAndDeleteIngredientsForCookingRecepie(IGraphClient client, Guid cookingRecepieId)
        {
            await client.Cypher.Match("(cookingRecepie: CookingRecepie) -[relIngredient: NEED]->(ingredient: Ingredient)")
                                .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                                .Delete("relIngredient")
                                .Delete("ingredient")
                                .ExecuteWithoutResultsAsync();
        }

        public static async Task DetachAndDeleteStepsInFoodPreparation(IGraphClient client, Guid cookingRecepieId)
        {
            var stepsInFoodPreparationIds = await GetIdsOfStepsInFoodPreparation(client, cookingRecepieId);
            foreach (var stepId in stepsInFoodPreparationIds)
            {
                await client.Cypher.Match("(cookingRecepie: CookingRecepie)-[r:HAS]->(step: StepInFoodPreparation)")
                                         .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                                         .AndWhere((Models.StepInFoodPreparation step) => step.StepInFoodPreparatinId == stepId)
                                         .Delete("r")
                                         .Delete("step")
                                         .ExecuteWithoutResultsAsync();
            }
        }



        public static async Task<List<Guid>> GetIdsOfStepsInFoodPreparation(IGraphClient client, Guid cookingRecepieId)
        {
            return (await client.Cypher.Match("(cookingRecepie: CookingRecepie)-[r:HAS]->(step: StepInFoodPreparation)")
                                              .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                                              .Return((step) => new
                                              {
                                                  Step = step.As<Models.StepInFoodPreparation>()
                                              }).ResultsAsync)
                                              .Select(obj => obj.Step.StepInFoodPreparatinId).ToList();
        }

        public static async Task DetachAndDeleteCommentsOfCookingRecepie(IGraphClient client, Guid cookingRecepieId)
        {
            await client.Cypher.Match("(cookingRecepie: CookingRecepie) -[relComment:HAS_COMMENTS]->(comment: Comment)-[relAuthor:WRITES]->(author: Author)")
                                .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                                .Delete("relComment")
                                .Delete("relAuthor")
                                .Delete("comment")
                                .ExecuteWithoutResultsAsync();

            await client.Cypher.Match("(cookingRecepie: CookingRecepie) -[relComment:HAS_COMMENTS]->(comment: Comment)-[relReader:WRITES]->(reader: Reader)")
                                .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                                .Delete("relComment")
                                .Delete("relReader")
                                .Delete("comment")
                                .ExecuteWithoutResultsAsync();
        }

        public static async Task DetachAndDeleteRatesOfCookingRecepie(IGraphClient client, Guid cookingRecepieId)
        {
            await client.Cypher.Match("(cookingRecepie: CookingRecepie) -[relRate:HAS_RATES]->(rate: Rate)-[relAuthor:RATES]->(author: Author)")
                               .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                               .Delete("relRate")
                               .Delete("relAuthor")
                               .Delete("rate")
                               .ExecuteWithoutResultsAsync();

            await client.Cypher.Match("(cookingRecepie: CookingRecepie) -[relRate:HAS_RATES]->(rate: Rate)-[relReader:RATES]->(reader: Reader)")
                               .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                               .Delete("relRate")
                               .Delete("relReader")
                               .Delete("rate")
                               .ExecuteWithoutResultsAsync();
        }

        public static async Task DetachCookingRecepieForReadLater(IGraphClient client, Guid cookingRecepieId)
        {
            await client.Cypher.Match("(a: Author)-[r:SAVED_TO_READ_LATER]->(cookingRecepie : CookingRecepie)")
                               .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                               .Delete("r")
                               .ExecuteWithoutResultsAsync();

            await client.Cypher.Match("(reader: Reader)-[r:SAVED_TO_READ_LATER]->(cookingRecepie : CookingRecepie)")
                               .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                               .Delete("r")
                               .ExecuteWithoutResultsAsync();
        }

        public static async Task<float> UpdateAverageRateOfCookingRecepie(IGraphClient client, Guid cookingRecepieId)
        {
            var newAverageRate = CalculateAverageRate(client, cookingRecepieId);
            await client.Cypher.Match("(cookingRecepie: CookingRecepie)")
                                     .Where((Models.CookingRecepie cookingRecepie) => cookingRecepie.CookingRecepieId == cookingRecepieId)
                                     .Set("cookingRecepie.AverageRate = $newAverageRate")
                                     .WithParam("newAverageRate", newAverageRate.Result)
                                     .ExecuteWithoutResultsAsync();
            return newAverageRate.Result;
        }

        public static async Task<float> CalculateAverageRate(IGraphClient client, Guid cookingRecepieId)
        {
            var rates = (await client.Cypher.Match("(c: CookingRecepie)-[:HAS_RATES]->(rate: Rate)")
                                                  .Where((Models.CookingRecepie c) => c.CookingRecepieId == cookingRecepieId)
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

 
    }
}
