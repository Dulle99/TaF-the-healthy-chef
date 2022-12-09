using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.Models
{
    public class Award
    {
        #region Fields

        public Guid AwardId { get; set; }

        public string AwardName { get; set; }

        public DateTime DateOfReceivingAward { get; set; }

        #endregion Fields

        #region Constructor

        public Award() 
        {
            this.AwardId = Guid.NewGuid();
        }

        public Award(string awardName, DateTime dateOfRecevingAward)
        {
            this.AwardId = Guid.NewGuid();
            this.AwardName = awardName;
            this.DateOfReceivingAward = dateOfRecevingAward;
        }

        #endregion Constructor
    }
}
