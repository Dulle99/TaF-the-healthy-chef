﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaF_Neo4j.DTOs;
using TaF_Neo4j.DTOs.Comment;
using TaF_Neo4j.DTOs.CookingRecepieDTO;
using TaF_Neo4j.DTOs.Rate;
using TaF_Neo4j.Services.CookingRecepie;
using TaF_Redis.Services.Content;
using TaF_Redis.Services.User;
using TaF_Redis.Types;

namespace TaF_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CookingRecepieController : ControllerBase
    {
        private ICookingRecepieService _cookingRecepieService;
        private IUserServiceRedis _userServiceRedis;
        private IContentServiceRedis _contentServiceRedis;

        public CookingRecepieController(IGraphClient client, IConnectionMultiplexer redis)
        {
            _cookingRecepieService = new CookingRecepieService(client);
            _userServiceRedis = new UserServiceRedis(redis, client);
            _contentServiceRedis = new ContentServiceRedis(redis, client);
        }

        [HttpPost]
        [Authorize(Roles = "Author")]
        [Route("CreateCookingRecepie/{authorUsername}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCookigRecepie([FromForm] BasicCookingRecepieDTO cookingRecepieDTO, string authorUsername)
        {
            if (await this._contentServiceRedis.CheckForBadWordsInCookingRecepie(cookingRecepieDTO))
                return BadRequest();

            if (await this._cookingRecepieService.CreateCookingRecepie(authorUsername, cookingRecepieDTO))
            {
                await this._userServiceRedis.CacheAuthorNewContent(authorUsername, ContentType.cookingRecepie);
                return Ok();
            }
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
            return new JsonResult(await this._contentServiceRedis.GetCachedRecomendedCookingRecepies());
        }

        [HttpGet]
        [Authorize]
        [Route("GetCookingRecepiesByAuthor/{authorUsername}/{numberOfCookingRecepiesToGet}")]
        public async Task<IActionResult> GetPreviewCookingRecepiesByAuthor(string authorUsername, int numberOfCookingRecepiesToGet)
        {
            if (numberOfCookingRecepiesToGet > 5)
                return new JsonResult(await this._cookingRecepieService.GetPreviewCookingRecepiesByAuthor(authorUsername, numberOfCookingRecepiesToGet));
            else
                return new JsonResult(await this._userServiceRedis.GetCachedCookingRecepiesByAuthor(authorUsername));
            
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
        public async Task<IActionResult> UpdateCookingRecepie([FromForm] BasicCookingRecepieDTO cookingRecepieDTO, Guid cookingRecepieId)
        {
            if (await this._contentServiceRedis.CheckForBadWordsInCookingRecepie(cookingRecepieDTO))
                return BadRequest();

            if (await this._cookingRecepieService.UpdateCookingRecepie(cookingRecepieId, cookingRecepieDTO))
            {
                await this._contentServiceRedis.UpdateContent(TaF_Redis.Types.ContentType.cookingRecepie, cookingRecepieId);
                return Ok();
            }
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
            {
                await this._contentServiceRedis.RemoveContentFromCache(TaF_Redis.Types.ContentType.cookingRecepie, cookingRecepieId);
                return Ok();
            }
            else
                return BadRequest();
        }
    }
}
