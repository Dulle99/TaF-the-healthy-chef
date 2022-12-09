using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs.UserDTO
{
    public class LoggedUserInformation
    {
        #region Fields

        public string Username { get; set; }

        public string TypeOfUser { get; set; }

        public string Token { get; set; }

        public string LoginInformation { get; set; }

        #region Fields

        #endregion Constructor

        public LoggedUserInformation() { }

        #endregion Constructor
    }
}
