using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs.UserDTO
{
    public class AuthorUpdateDTO
    {
        #region Fields

        public string Username { get; set; }

        public string Name { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string Lastname { get; set; }

        public string Biography { get; set; }

        public int YearsOfExpiriance { get; set; }

        public List<AwardDTO> Awards { get; set; }

        #endregion Fields

        #region Constructor
        public AuthorUpdateDTO() { }

        #endregion Constructor
    }
}
