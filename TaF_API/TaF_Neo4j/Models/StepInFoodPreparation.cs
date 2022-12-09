using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.Models
{
    public class StepInFoodPreparation
    {
        #region Fields

        public Guid StepInFoodPreparatinId { get; set; }

        public int OrdinalNumberOfStep { get; set; }

        public string StepDescription { get; set; }

        #endregion Fields

        #region Constructor

        public StepInFoodPreparation()
        {
            this.StepInFoodPreparatinId = Guid.NewGuid();
        }

        public StepInFoodPreparation(int ordinalNumberOfStep, string stepDescription)
        {
            this.StepInFoodPreparatinId = Guid.NewGuid();
            this.OrdinalNumberOfStep = ordinalNumberOfStep;
            this.StepDescription = stepDescription;
        }

        #endregion Constructor
    }
}
