using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.Models;

namespace TaF_Neo4j.DTOs.UserDTO
{
    public class AuthorPreviewDTO
    {
        #region Fields

        public string Username { get; set; }

        public string Name { get; set; }

        public string Lastname { get; set; }

        public string Biography { get; set; }

        public List<Award> Awards { get; set; }

        public int YearsOfExpiriance { get; set; }

        public byte[] ProfilePicture { get; set; }

        #endregion Fields

        #region Constructor
        public AuthorPreviewDTO() { }

        #endregion Constructor
    }
}
