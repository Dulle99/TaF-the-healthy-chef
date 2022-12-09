using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs.CookingRecepieDTO
{
    public class CookingRecepieDTO
    {
        #region Fields
        public Guid CookingRecepieId { get; set; }

        public string AuthorUsername { get; set; }

        public byte[] AuthorProfilePicture { get; set; }

        public string CookingRecepieTitle { get; set; }

        public string Description { get; set; }

        public List<StepInFoodPreparationDTO> StepsInFoodPreparation { get; set; }

        public List<IngredientForCookingRecepieDTO> Ingredients { get; set; }

        public string TypeOfMeal { get; set; }

        public DateTime PublicationDate { get; set; }

        public int PreparationTime { get; set; }

        public float AverageRate { get; set; }

        public byte[] CookingRecepiePicture { get; set; }

        #endregion Fields

        #region Constructor

        public CookingRecepieDTO() { this.StepsInFoodPreparation = new List<StepInFoodPreparationDTO>(); }

        public CookingRecepieDTO(string authorUsername, byte[] profilePicture, string cookingRecepieTitle, string description, string typeOfMeal,
                                        DateTime publicatioDate, int preparationTime, byte[] cookingRecepiePicture)
        {
            this.AuthorUsername = authorUsername;
            this.AuthorProfilePicture = profilePicture;
            this.CookingRecepieTitle = cookingRecepieTitle;
            this.Description = description;
            this.TypeOfMeal = typeOfMeal;
            this.PublicationDate = publicatioDate;
            this.PreparationTime = preparationTime;
            this.CookingRecepiePicture = cookingRecepiePicture;
        }

        #endregion Constructor
    }
}
