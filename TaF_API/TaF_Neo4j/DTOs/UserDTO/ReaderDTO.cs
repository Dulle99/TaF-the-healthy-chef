using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs.UserDTO
{
    public class ReaderDTO
    {
        #region Fields

        public string Username { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Lastname { get; set; }

        public string NameOfSchool { get; set; }

        public string TypeOfStudent { get; set; }

        #endregion Fields

        #region Constructor

        public ReaderDTO(){ }

        #endregion Constructor
    }
}
