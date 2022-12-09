using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaF_Neo4j.Models;

namespace TaF_Neo4j.DTOs.UserDTO
{
    public class AuthorDTO
    {
        #region Fields

        public string Username { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Lastname { get; set; }

        public string Biography { get; set; }

        public List<AwardDTO> Awards { get; set; }
        

    public int YearsOfExpiriance { get; set; }


        #endregion Fields

        #region Constructor
        public AuthorDTO() { Awards = new List<AwardDTO>(); }

        
        #endregion Constructor
    }
}
