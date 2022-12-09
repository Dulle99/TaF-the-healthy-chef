using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.Models;

namespace TaF_Neo4j.DTOs
{
    public class BasicCookingRecepieDTO
    {
        #region Fields

        public string CookingRecepieTitle { get; set; }

        public string Description { get; set; }

        public string TypeOfMeal { get; set; }

        public int PreparationTime { get; set; }

        public IFormFile CookingRecepiePicture { get; set; }

        public List<StepInFoodPreparationDTO> StepsInFoodPreparation { get; set; }

        public List<IngredientForCookingRecepieDTO> Ingredients { get; set; }

        #endregion Fields

        #region Constructor

        public BasicCookingRecepieDTO()
        { }

        #endregion Constructor
    }
}
