using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs.UserDTO
{
    public class ReaderUpdateDTO
    {
        #region Fields

        public string Username { get; set; }

        public string Name { get; set; }

        public string Lastname { get; set; }

        public string NameOfSchool { get; set; }

        public string TypeOfStudent { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        #endregion Fields

        #region Constructor

        public ReaderUpdateDTO() { }

        #endregion Constructor
    }
}
