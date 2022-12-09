using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs;
using TaF_Neo4j.DTOs.Comment;
using TaF_Neo4j.DTOs.CookingRecepieDTO;
using TaF_Neo4j.DTOs.Rate;
using TaF_Neo4j.Services.CookingRecepie;

namespace TaF_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CookingRecepieController : ControllerBase
    {
        private ICookingRecepieService _cookingRecepieService;
        public CookingRecepieController(IGraphClient client)
        {
            _cookingRecepieService = new CookingRecepieService(client);
        }

        [HttpPost]
        [Authorize(Roles = "Author")]
        [Route("CreateCookingRecepie/{authorUsername}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCookigRecepie([FromForm] BasicCookingRecepieDTO cookingRecepieDTO, string authorUsername)
        {
            if (await this._cookingRecepieService.CreateCookingRecepie(authorUsername, cookingRecepieDTO))
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost]
        [Authorize]
        [Route("AddCommentToTheCookingRecepie")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCommentToTheCookingRecepie([FromBody] BasicCommentDTO commentDTO)
        {
            if (await this._cookingRecepieService.AddCommentToTheCookingRecepie(commentDTO))
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost]
        [Authorize]
        [Route("AddRateToTheCookingRecepie")]
        public async Task<IActionResult> AddRateToTheCookingRecepie([FromBody] BasicRateDTO rateDTO)
        {
            return new JsonResult(await this._cookingRecepieService.AddRateToTheCookingRecepie(rateDTO));
        }

        [HttpGet]
        [Route("GetCookingRecepiesByPublicationDate/{cookingRecepieType}/{numberOfCookingRecepiesToGet}/{latestFirst}")]
        public async Task<IActionResult> GetPreviewCookingRecepiesByDatePublication(string cookingRecepieType,int numberOfCookingRecepiesToGet, bool latestFirst)
        {
            return new JsonResult(await this._cookingRecepieService.GetPreviewCookingRecepiesByDatePublication(cookingRecepieType, numberOfCookingRecepiesToGet, latestFirst));
        }

        [HttpGet]
        [Route("GetCookingRecepiesByPopularity/{cookingRecepieType}/{numberOfCookingRecepiesToGet}")]
        public async Task<IActionResult> GetPreviewCookingRecepiesByPopularity(string cookingRecepieType, int numberOfCookingRecepiesToGet)
        {
            return new JsonResult(await this._cookingRecepieService.GetPreviewCookingRecepiesByPopularity(cookingRecepieType, numberOfCookingRecepiesToGet));
        }

        [HttpGet]
        [Route("GetFastestToCookCookingRecepies/{cookingRecepieType}/{numberOfCookingRecepiesToGet}")]
        public async Task<IActionResult> GetFastestToCookPreviewCookingRecepies(string cookingRecepieType, int numberOfCookingRecepiesToGet)
        {
            return new JsonResult(await this._cookingRecepieService.GetFastestToCookPreviewCookingRecepies(cookingRecepieType, numberOfCookingRecepiesToGet));
        }

        [HttpGet]
        [Route("GetRecommendedCookingRecepies")]
        public async Task<IActionResult> GetRecommendedPreviewCookingRecepies()
        {
            return new JsonResult(await this._cookingRecepieService.GetRecommendedCookingRecepies());
        }

        [HttpGet]
        [Authorize]
        [Route("GetCookingRecepiesByAuthor/{authorUsername}/{numberOfCookingRecepiesToGet}")]
        public async Task<IActionResult> GetPreviewCookingRecepiesByAuthor(string authorUsername, int numberOfCookingRecepiesToGet)
        {
            return new JsonResult(await this._cookingRecepieService.GetPreviewCookingRecepiesByAuthor(authorUsername, numberOfCookingRecepiesToGet));
        }

        [HttpGet]
        [Route("GetCookingRecepie/{cookingRecepieId}")]
        public async Task<IActionResult> GetCookingRecepie(Guid cookingRecepieId)
        {
            return new JsonResult(await this._cookingRecepieService.GetCookingRecepie(cookingRecepieId));
        }

        [HttpGet]
        [Route("GetCommentsOfCookingRecepie/{cookingRecepieId}/{numberOfCommentsToGet}")]
        public async Task<IActionResult> GetCommentsOfCookingRecepie(Guid cookingRecepieId, int numberOfCommentsToGet)
        {
            return new JsonResult(await this._cookingRecepieService.GetCommentsByCookinRecepie(cookingRecepieId, numberOfCommentsToGet));
        }

        [HttpPut]
        [Authorize(Roles = "Author")]
        [Route("UpdateCookingRecepie/{cookingRecepieId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCookingRecepie([FromForm] CookingRecepieUpdateDTO cookingRecepieDTO, Guid cookingRecepieId)
        {
            if (await this._cookingRecepieService.UpdateCookingRecepie(cookingRecepieId, cookingRecepieDTO))
                return Ok();
            else
                return BadRequest();
        }

        [HttpPut]
        [Authorize(Roles = "Author")]
        [Route("UpdateStepsInFoodPreparation/{cookingRecepieId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStepsInFoodPreparation(List<StepInFoodPreparationDTO> steps, Guid cookingRecepieId)
        {
            if (await this._cookingRecepieService.UpdateStepsInFoodPreparation(cookingRecepieId, steps))
                return Ok();
            else
                return BadRequest();
        }

        [HttpDelete]
        [Authorize(Roles = "Author")]
        [Route("Delete/{cookingRecepieId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCookingRecepie(Guid cookingRecepieId)
        {
            if (await this._cookingRecepieService.DeleteCookingRecepie(cookingRecepieId))
                return Ok();
            else
                return BadRequest();
        }
    }
}
