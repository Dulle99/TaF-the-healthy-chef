using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs.Comment
{
    public class CommentPreviewDTO
    {
        #region Fields

        public string AuthorOfComment { get; set; }

        public byte[] AuthorOfCommentProfilePicture { get; set; }

        public DateTime PublicationDate { get; set; }

        public string CommentContent { get; set; }

        public string TypeOfUser { get; set; }

        #endregion Fields

        #region Constructor

        public CommentPreviewDTO() { }
        #endregion Constructor
    }
}
