using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs
{
    public class BasicBlogDTO
    {
        #region Fields

        public string BlogTitle { get; set; }

        public string BlogContent { get; set; }

        public int ReadingTime { get; set; }

        public IFormFile BlogPicture { get; set; }

        #endregion Fields

        #region Constructor

        public BasicBlogDTO() { }

        #endregion Constructor
    }
}
