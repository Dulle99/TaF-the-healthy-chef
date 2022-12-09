using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.Models
{
    public class Rate
    {
        #region Fields

        public Guid RateId { get; set; }

        public int RateValue { get; set; }

        #endregion Fields

        #region Constructor

        public Rate()
        {
            this.RateId = Guid.NewGuid();
            this.RateValue = 0;
        }
        #endregion Constructor
    }
}
