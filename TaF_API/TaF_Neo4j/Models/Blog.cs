using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.Models
{
    public class Blog
    {
        #region Fields

        public Guid BlogId { get; set; }

        public string BlogTitle { get; set; }

        public string BlogContent { get; set; }

        public DateTime PublicationDate { get; set; }

        public int ReadingTime { get; set; }

        public float AverageRate { get; set; }

        public string BlogPicturePath { get; set; }

        #endregion Fields

        #region Constructor

        public Blog()
        {
            this.BlogId = Guid.NewGuid();
            this.ReadingTime = 1;
            this.AverageRate = 0;
        }

        #endregion Constructor
    }
}
