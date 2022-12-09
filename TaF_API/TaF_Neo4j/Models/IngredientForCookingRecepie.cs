using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.Models
{
    public class IngredientForCookingRecepie
    {
        #region Fields

        public Guid IngredientId { get; set; }

        public int OrdinalNumberOfIngredient { get; set; }

        public string Ingredient { get; set; }

        #endregion Fields

        #region Constructor

        public IngredientForCookingRecepie()
        {
            this.IngredientId = Guid.NewGuid();
        }

        public IngredientForCookingRecepie(int ordinalNumberOfIngredient, string Ingredient)
        {
            this.IngredientId= Guid.NewGuid();
            this.OrdinalNumberOfIngredient = ordinalNumberOfIngredient;
            this.Ingredient = Ingredient;
        }

        #endregion Constructor
    }
}
