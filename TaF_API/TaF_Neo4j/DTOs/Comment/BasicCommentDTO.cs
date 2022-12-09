using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs.Comment
{
    public class BasicCommentDTO
    {
        #region Fields
        public Guid ContentId { get; set; }

        public string TypeOfUser { get; set; }

        public string Username { get; set; }

        public string CommentContent { get; set; }

        #endregion Fields

        #region Constructor

        public BasicCommentDTO() { }

        #endregion Constructor
    }
}
