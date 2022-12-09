using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.Models
{
    public class Comment
    {
        #region Fields
        public Guid CommentId { get; set; }

        public string CommentContent { get; set; }

        public DateTime PublicationDate { get; set; }
        #endregion Fields

        #region Constructor

        public Comment()
        {
            this.CommentId = Guid.NewGuid();

            this.PublicationDate = DateTime.UtcNow;
        }

        #endregion Constructor
    }
}
