using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs
{
    public class AwardDTO
    {
        #region Fields

        public string AwardName { get; set; }

        public DateTime DateOfReceivingAward { get; set; }

        #endregion Fields

        #region Constructor

        public AwardDTO() {  }

        #endregion Constructor
    }
}
