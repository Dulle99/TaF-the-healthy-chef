using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs;
using TaF_Neo4j.DTOs.Comment;
using TaF_Neo4j.DTOs.CookingRecepieDTO;
using TaF_Neo4j.DTOs.Rate;

namespace TaF_Neo4j.Services.CookingRecepie
{
    public interface ICookingRecepieService
    {
        Task<bool> CreateCookingRecepie(string authorUsername, BasicCookingRecepieDTO cookingRecepieDTO);
        Task<List<CookingRecepiePreviewDTO>> GetPreviewCookingRecepiesByDatePublication(string cookingRecepieType, int numberOfCookingRecepiesToGet, bool latestFirst);
        Task<List<CookingRecepiePreviewDTO>> GetPreviewCookingRecepiesByPopularity(string cookingRecepieType,int numberOfCookingRecepiesToGet);
        Task<List<CookingRecepiePreviewDTO>> GetFastestToCookPreviewCookingRecepies(string cookingRecepieType, int numberOfCookingRecepiesToGet);
        Task<List<CookingRecepiePreviewDTO>> GetRecommendedCookingRecepies();
        Task<List<CookingRecepiePreviewDTO>> GetPreviewCookingRecepiesByAuthor(string authorUsername, int numberOfCookingRecepiesToGet);
        Task<CookingRecepieDTO> GetCookingRecepie(Guid cookingRecepieId);
        Task<bool> UpdateCookingRecepie(Guid cookingRecepieId, CookingRecepieUpdateDTO cookingRecepieDTO);
        Task<bool> UpdateStepsInFoodPreparation(Guid cookingRecepieId, List<StepInFoodPreparationDTO> stepsDTO);
        Task<bool> DeleteCookingRecepie(Guid cookingRecepieId);
        Task<bool> AddCommentToTheCookingRecepie(BasicCommentDTO commentDTO);
        Task<float> AddRateToTheCookingRecepie(BasicRateDTO rateDTO);
        Task<List<CommentPreviewDTO>> GetCommentsByCookinRecepie(Guid cookingRecepieId, int numberOfCommentsToGet);
    }
}
