using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs.BlogDTO
{
    public class BlogDTO
    {
        #region Fields

        public string AuthorUsername { get; set; }

        public byte[] AuthorProfilePicture { get; set; }

        public Guid BlogId { get; set; }

        public string BlogTitle { get; set; }

        public string Content { get; set; }

        public DateTime PublicationDate { get; set; }

        public int ReadingTime { get; set; }

        public float AverageRate { get; set; }

        public byte[] BlogPicture { get; set; }

        #endregion Fields

        #region Constructor

        public BlogDTO() { }

        #endregion Constructor
    }
}
