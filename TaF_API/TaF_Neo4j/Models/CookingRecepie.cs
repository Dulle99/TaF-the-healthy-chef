using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.Models
{
    public class CookingRecepie
    {
        #region Fields

        public Guid CookingRecepieId { get; set; }

        public string CookingRecepieTitle { get; set; }

        public string Description { get; set; }

        public string TypeOfMeal { get; set; }

        public DateTime PublicationDate { get; set; }

        public int PreparationTime { get; set; }

        public string CookingRecepiePicturePath { get; set; }

        public float AverageRate { get; set; }

        public List<StepInFoodPreparation> StepsInFoodPreparation { get; set; }

        public List<IngredientForCookingRecepie> Ingredients { get; set; }

        #endregion Fields

        #region Constructor

        public CookingRecepie()
        {
            this.CookingRecepieId = Guid.NewGuid();
            this.PreparationTime = 1;
            this.AverageRate = 0;
        }

        #endregion Constructor
    }
}
