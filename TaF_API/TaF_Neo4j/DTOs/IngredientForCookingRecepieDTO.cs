using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs
{
    public class IngredientForCookingRecepieDTO
    {
        #region Fields

        public int OrdinalNumberOfIngredient { get; set; }

        public string Ingredient { get; set; }

        #endregion Fields

        #region Constructor

        public IngredientForCookingRecepieDTO()
        { }

        public IngredientForCookingRecepieDTO(int ordinalNumberOfIngredient, string Ingredient)
        {
            this.OrdinalNumberOfIngredient = ordinalNumberOfIngredient;
            this.Ingredient = Ingredient;
        }

        #endregion Constructor
    }
}
