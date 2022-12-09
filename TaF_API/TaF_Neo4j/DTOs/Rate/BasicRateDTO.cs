using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs.Rate
{
    public class BasicRateDTO
    {
        #region Fields

        public Guid ContentId { get; set; }

        public int Value { get; set; }

        public string TypeOfUser { get; set; }

        public string Username { get; set; }

        #endregion Fields

        #region Constructor

        public BasicRateDTO() { }

        #endregion Constructor
    }
}
