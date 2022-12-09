using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs
{
    public class StepInFoodPreparationDTO
    {
        #region Fields

        public int OrdinalNumberOfStep { get; set; }

        public string StepDescription { get; set; }

        #endregion Fields

        #region Constructor

        public StepInFoodPreparationDTO() { }

        #endregion Constructor
    }
}
